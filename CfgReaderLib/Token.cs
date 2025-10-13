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
		
		public enum TipoNum
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
		static int _tipoStrLength;		// Lunghezza massima della descrizione, per ToString()	

		/// <summary>
		/// Static Ctor
		/// </summary>
		static Token()
		{
			int lmax = 0;										// Imposta la lunghezza massima della descrizione
			foreach(TipoTk tp in Enum.GetValues(typeof(TipoTk)))
			{
				if(tp.ToString().Length > lmax)
					lmax = tp.ToString().Length;
			}
			_tipoStrLength = lmax+1;
		}
		#endregion

		TipoTk	_tipo;
		TipoNum _tNum;
		string	_testo = "";
		Dat?	_dat;

		#region PROPRIETA
		public TipoTk Tipo { get { return _tipo; } }
		public string Testo { get { return _testo; } }
		public Dat? Dato { get { return _dat; } }
		public TipoNum? TipoNumero { get { return _tNum; } }
		//Operatori.Operatore? Operatore { get { return _oper; } }
		
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
			//_oper = GetOper(ops, testo);
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
		/// Modifica il testo dell'operatore un unario speciale,
		/// anteponendogli un carattere speciale
		/// </summary>
		public void RendiOperatoreSpeciale()
		{
			if( (_tipo == TipoTk.Operatore) && (!_testo.StartsWith(Operatori.chUnary)) )
			{
				_testo = Operatori.chUnary + _testo;
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
			//string op = "";
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

			//if((_tipo == TipoTk.Operatore) && _oper != null)
			//{
			//	op = _oper.ToString();
			//}

			//return $"{_tipo.ToString().Replace('_',' ').PadRight(_tipoStrLength,' ')} {_testo}{ext} {op} {val}";
			return $"{_tipo.ToString().Replace('_',' ').PadRight(_tipoStrLength,' ')} {_testo}{ext} {val}";

		}
	}
}
