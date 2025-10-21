using Fred68.GenDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fred68.Parser
{

	public class Token
	{

		public const char chSuffissoFloat = 'f';
		public const char chSuffissoDouble = 'd';
		// Nota: il carattere dell'esponenziale 'E' (notazione scientifica) è incluso tra gli operatori binari
		
		public enum PromTable
		{
			Std = 0,
			Div,
			Int
		}

		/// <summary>
		/// Tipo di token
		/// </summary>
		public enum TipoTk
		{
			Indefinito = 0,
			Numero,				// 100 0.2
			Esadecimale,		// 0x2AbF
			Binario,			// 0b10100
			Stringa,			// "..."
			Parentesi_Aperta,	// '('
			Parentesi_Chiusa,	// ')'
			Blocco_Aperto,		// '{'
			Blocco_Chiuso,		// '}'
			Fine_Comando,		// ';'
			Separatore,			// ',' separatore di argomenti tra parentesi
			Operatore,			// +, +, *, !...
			Simbolo,			// Stringa simbolica generica (variabile, funzione o parola_chiave)
			Variabile,
			Funzione,
			Parola_chiave
		}
		
		public enum TipoNum		// Mantenere l'ordine: da meno preciso a più preciso
		{
			Indefinito = 0,
			Intero,				// vuoto
			Float,				// f
			Double				// d

		}
		/// <summary>
		/// Stato della macchina a stati
		/// </summary>
		public enum TkStat
		{
			TokenNuovo,			// In attesa di nuovo token
			Numero,				// Numero intero decimale o reale
			NumeroIndef,		// Numero in corso di identificazione (decimale, esadecimale o binario)
			Stringa,			// Testo
			Esadecimale,		// Numero esadecimale intero
			Binario,			// Numero binario intero
			ParentesiAperta,	// (
			ParentesiChiusa,	// )
			BloccoAperto,		// {
			BloccoChiuso,		// }
			FineComando,		// ;
			Separatore,			// ,
			Operatore,			// Operatore
			TokenCompletato,	// Finito token
			Simbolo				// Stringa simbolica generica (variabile, funzione o parola_chiave)
			}

		#region STATIC
		static int _tipoStrLength;			// Lunghezza massima della descrizione, per ToString()
		static int _numPromTab;				// Numeri delle tabelle di promozione
		static TipoNum _tipoDivVM;			// Tipo di dato per operazione di divisione tra interi.

		#warning CREARE più TABELLE DI PROMOZIONE... la divisione tra interi si promuove a virgola mobile
		#warning Le tabelle di promozione devono contenere TipoNum.Indefinito per le operazioni non ammesse (es.: div intera tra float)

		static TipoNum[,,]	_pT;			// Tabella di promozione dei tipi numerici
		
		/// <summary>
		/// Static Ctor
		/// </summary>
		static Token()
		{
			// Imposta la lunghezza massima della descrizione
			int lmax = 0;										
			foreach(TipoTk tp in Enum.GetValues(typeof(TipoTk)))
			{
				if(tp.ToString().Length > lmax)
					lmax = tp.ToString().Length;
			}
			_tipoStrLength = lmax+1;

			// Tipo di dato per divisione tra interi
			_tipoDivVM = TipoNum.Double;

			// Imposta numero di tabelle di promozione
			_numPromTab = Enum.GetValues(typeof(TipoTk)).Length;

			// Crea le tabelle di promozione
			int szpt = Enum.GetNames(typeof(TipoNum)).Length;
			_pT = new TipoNum[_numPromTab,szpt,szpt];
			for(int npt = 0; npt < _numPromTab; npt++)			// Riempie le tabelle di promozione con valori standard
			{
				for(int i = 0; i < szpt; i++)								// Se uno degli operandi numerici è indefinito...
					{
					_pT[0,(int)TipoNum.Indefinito,i] = TipoNum.Indefinito;	// ...il risultato è indefinito
					_pT[0,i,(int)TipoNum.Indefinito] = TipoNum.Indefinito;
					}
				for(int i=1; i < szpt; i++)									// Tra due operandi di precisione diversa... 
					for(int j=1; j < szpt; j++)
						{
							if(npt != (int)PromTable.Int)					// Per quasi tutti i casi...
								_pT[npt,i,j] = (TipoNum)int.Max(i,j);		// ...la precisione del risultato è quella maggiore
							else
								_pT[npt,i,j] = TipoNum.Indefinito;			// Per operazioni intere: sempre indefinito.
						}
			}

			// L'operazione di divisione ha una tabella standard, tranne quella tra interi, che restituisce...
			_pT[(int)PromTable.Div, (int)TipoNum.Intero, (int) TipoNum.Intero] = _tipoDivVM;	// ...float o double
			// La tabella per le operazioni tra interi è definita sono per argomenti interi
			_pT[(int)PromTable.Int, (int)TipoNum.Intero, (int) TipoNum.Intero] = TipoNum.Intero;

			#warning AGGIUNGERE string ShowTabelle()
		}

		/// <summary>
		/// TipoNum restituoto dall'operazione tra due numeri
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public static TipoNum TipoNumRestituito(Token.PromTable tabella, TipoNum? t1, TipoNum? t2)
		{
			return ((t1 == null)||(t2 == null)) ? TipoNum.Indefinito : _pT[(int)tabella, (int)t1,(int)t2];
		}
		#endregion

		TipoTk	_tipo;
		TipoNum _tNum;
		string	_testo = "";
		Dat?	_dat;

		#region PROPRIETA
		public TipoTk Tipo { get { return _tipo; } }
		public string Testo { get { return _testo; } }
		public Dat? Dato
		{
			get { return _dat; }
			set { _dat = value; }
		}
		public TipoNum? TipoNumero { get { return _tNum; } }
		public bool isDatNotNull { get {return (_dat!=null); }}
		
		

		/// <summary>
		/// E' un valore numerico, una stringa o una variabile ?
		/// </summary>
		public bool isValore
		{
			get
				{
				return (	(_tipo==TipoTk.Numero) ||
							(_tipo==TipoTk.Esadecimale) ||
							(_tipo==TipoTk.Binario) ||
							(_tipo==TipoTk.Stringa) ||
							(_tipo==TipoTk.Variabile) 
						);
				}
		}

		/// <summary>
		/// E' un numero ?
		/// </summary>
		public bool isNumero
		{
			get
				{
				return (	(_tipo==TipoTk.Numero) ||
							(_tipo==TipoTk.Esadecimale) ||
							(_tipo==TipoTk.Binario)
						);
				}
		}
		/// <summary>
		/// E' una stringa ?
		/// </summary>
		public bool isStringa
		{
			get
				{
				return (_tipo==TipoTk.Stringa);
				}
		}

		/// <summary>
		/// E' un numero o una stringa ?
		/// </summary>
		public bool isNumeroStringa
		{
			get
				{
				return (	(_tipo==TipoTk.Numero) ||
							(_tipo==TipoTk.Esadecimale) ||
							(_tipo==TipoTk.Binario) ||
							(_tipo==TipoTk.Stringa)
						);
				}
		}
		
		/// <summary>
		/// E' una funzione ?
		/// </summary>
		public bool isFunzione { get {return (_tipo==TipoTk.Funzione);} }

		/// <summary>
		/// E' un operatore
		/// </summary>
		public bool isOperatore { get {return (_tipo==TipoTk.Operatore);} }
		
		public bool isOperatoreFunzione { get {return ((_tipo==TipoTk.Operatore)||(_tipo==TipoTk.Funzione));} }
		#endregion

		/// <summary>
		/// Ctor vuoto
		/// Tipo: indefinito
		/// </summary>
		public Token()
		{
			this.Clear();
		}
		
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="tipo">Tipo</param>
		/// <param name="testo">Contenuto (string)</param>
		public Token(TipoTk tipo, string testo = "")
		{
			_tipo = tipo;
			_tNum = TipoNum.Indefinito;
			_testo = testo;
			_dat = null;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="tipo">Tipo</param>
		/// <param name="tpN">TipoNum</param>
		/// <param name="testo">Contenuto (string)</param>
		public Token(TipoTk tipo, TipoNum tpN, string testo)
		{
			_tipo = tipo;
			_tNum = tpN;
			_testo = testo;
			_dat = null;
		}

		/// <summary>
		/// Cancella tutto
		/// </summary>
		public void Clear()
		{
			_tipo = TipoTk.Indefinito;
			_tNum = TipoNum.Indefinito;
			_testo = "";
			_dat = null;
		}

		/// <summary>
		/// Valuta il testo del token e lo mette in _dat, se non è null
		/// Valuta solo se di tipo: numero, esadecimale, binario, stringa o variabile
		/// </summary>
		/// <param name="bRivaluta">Forza la valutazione anche se _dat non è nullo</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public bool ValutaVal(bool bRivaluta = false)
		{
			bool ok = false;
			if(( _dat == null) || bRivaluta)
			{
				switch(_tipo)
				{
					case TipoTk.Numero:		
					{
						switch(_tNum)
						{
							case TipoNum.Indefinito:
								throw new Exception("TipoNum.Indefinito");
							//break;
							case TipoNum.Intero:
							{
								int x;
								ok = int.TryParse(_testo, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out x);
								if(ok)
								{
									_dat = new Dat(x);
								}
							}
							break;
							case TipoNum.Float:
							{
								float x;
								ok = float.TryParse(_testo, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
								if(ok)
								{
									_dat = new Dat(x);
								}
							}
							break;
							case TipoNum.Double:
							{
								double x;
								ok = double.TryParse(_testo, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
								if(ok)
								{
									_dat = new Dat(x);
								}
							}
							break;
							default:
								throw new NotSupportedException("TipoNum default");
							//break;
						}
					}
					break;

					case TipoTk.Esadecimale:		// Valuta come intero
					{
						int x;
						ok = int.TryParse(_testo, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out x);
						if(ok)
						{
							_dat = new Dat(x);
						}
					}
					break;

					case TipoTk.Binario:			// Valuta come intero
					{
						int x;
						ok = int.TryParse(_testo, System.Globalization.NumberStyles.AllowBinarySpecifier, System.Globalization.CultureInfo.InvariantCulture, out x);
						if(ok)
						{
							_dat = new Dat(x);
						}
					}
					break;

					case TipoTk.Stringa:			// Ricopia la stringa
					{
						_dat = new Dat(_testo);
					}
					break;

					case TipoTk.Variabile:
					{
						throw new NotImplementedException("Valutazione di variabile non ancora implementata.");
					}
					//break;

					default:
					{}
					break;
				}
			}
			
			return ok;
		}

		/// <summary>
		/// Modifica il testo dell'operatore un unario speciale,
		/// anteponendogli un carattere speciale
		/// </summary>
		public void RendiOperatoreSpeciale()
		{
			if( (_tipo == TipoTk.Operatore) && (!_testo.StartsWith(Operators.chUnary)) )
			{
				_testo = Operators.chUnary + _testo;
			}
		}

		/// <summary>
		/// ToString() override
		/// Include carattere per indicare se fload o double
		/// </summary>
		/// <returns></returns>
		public override string ToString() 
		{
			string val;
			string ext = "";
			if(_dat != null)
			{
				val = _dat.ToString();	//_dat.Get().ToString(out val);
			}
			else
			{
				val = "null";	
			}
			
			if(_tipo == TipoTk.Numero)
			{
				switch(_tNum)
				{
					case TipoNum.Float:
						ext = " [f]";
						break;
					case TipoNum.Double:
						ext = " [d]";
						break;
					case TipoNum.Intero:
						ext = " [i]";
						break;
					default:
						ext = " [?]";
						break;
				}
			}

			return $"{_tipo.ToString().Replace('_',' ').PadRight(_tipoStrLength,' ')} {_testo}{ext} {{{val}}}";

		}
	}
}
