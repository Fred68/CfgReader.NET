using Fred68.GenDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fred68.Parser
{
	public partial class Parser
	{
		public class Token
		{

			public const char chSuffissoFloat = 'f';
			public const char chSuffissoDouble = 'd';
		
			#region ENUM
			public enum PromTable
			{
				Std = 0,
				Div,
				Int
			}
			/// <summary>
			/// Tipo di token
			/// Gli operatori includono l'operatore di assegnazione.
			/// Un token di tipo 'Variabile', quando viene valutato, è trasformato nel tipo valore (numero, stringa...)
			/// ...ma mantiene il flag per riconoscerlo per una eventuale riassegnazione.
			/// Un 'Simbolo' è una stringa non ancora rionosciuta (es.: una variabile nuovo non ancora memorizzata).
			/// Una 'Funzione' opera su uno o più argomenti tra parentesi. Può svolgere calcoli, es. MAX(1,2),...
			/// ...conversioni, per es. FLOAT(), oppure altre operazioni, es.: KILL(nomevar), MAKEREADONLY(nomevar)
			/// NOTA: non aggiungere altri tipi di token, per non complicare gli algoritmi.
			/// Le costanti si trattano come variabili (con il flag readonly)
			/// Conversioni, operazioni speciali ecc... come funzioni.
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
				Operatore,			// +, +, *, !... incluso l'operatore di assegnazione.
				Variabile,			// Variabile riconosciuta.
				Simbolo,			// Stringa simbolica generica (può essere variabile, funzione...)
				Funzione			// Funzione
			}
		
			/// <summary>
			/// Tipo di numero
			/// </summary>
			public enum TipoNum		// Mantenere l'ordine: da meno preciso a più preciso
			{
				Nd = 0,				/// Indefinito
				Int,				/// Intero
				Flt,				/// Float
				Dbl					/// Souble
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
			#endregion

			#region STATIC
			static int _tipoStrLength;			// Lunghezza massima della descrizione, per ToString()
			static int _numPromTab;				// Numeri delle tabelle di promozione
			static TipoNum _tipoDivVM;			// Tipo di dato per operazione di divisione tra interi.

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
				_tipoDivVM = TipoNum.Flt;

				// Imposta numero di tabelle di promozione
				_numPromTab = Enum.GetValues(typeof(TipoTk)).Length;

				// Crea le tabelle di promozione
				int szpt = Enum.GetNames(typeof(TipoNum)).Length;
				_pT = new TipoNum[_numPromTab,szpt,szpt];
				for(int npt = 0; npt < _numPromTab; npt++)			// Riempie le tabelle di promozione con valori standard
				{
					for(int i = 0; i < szpt; i++)								// Se uno degli operandi numerici è indefinito...
						{
						_pT[0,(int)TipoNum.Nd,i] = TipoNum.Nd;	// ...il risultato è indefinito
						_pT[0,i,(int)TipoNum.Nd] = TipoNum.Nd;
						}
					for(int i=1; i < szpt; i++)									// Tra due operandi di precisione diversa... 
						for(int j=1; j < szpt; j++)
							{
								if(npt != (int)PromTable.Int)					// Per quasi tutti i casi...
									_pT[npt,i,j] = (TipoNum)int.Max(i,j);		// ...la precisione del risultato è quella maggiore
								else
									_pT[npt,i,j] = TipoNum.Nd;			// Per operazioni intere: sempre indefinito.
							}
				}

				// L'operazione di divisione ha una tabella standard, tranne quella tra interi, che restituisce...
				_pT[(int)PromTable.Div, (int)TipoNum.Int, (int) TipoNum.Int] = _tipoDivVM;	// ...float o double
				// La tabella per le operazioni tra interi è definita sono per argomenti interi
				_pT[(int)PromTable.Int, (int)TipoNum.Int, (int) TipoNum.Int] = TipoNum.Int;

			}

			/// <summary>
			/// Mostra le tabelle di promozione
			/// </summary>
			/// <returns></returns>
			public static string TablesToString()
			{
				StringBuilder sb = new StringBuilder();
				foreach(PromTable pt in Enum.GetValues(typeof(PromTable)))
				{
					sb.AppendLine(pt.ToString());
					foreach(TipoNum c in Enum.GetValues(typeof(TipoNum)))		// Intestazioni di colonna
					{
						sb.Append("\t"+c.ToString());
					}
					sb.AppendLine();

					foreach(TipoNum r in Enum.GetValues(typeof(TipoNum)))		// Righe
					{
						sb.Append(r.ToString());								// Intestazione di riga
						foreach(TipoNum c in Enum.GetValues(typeof(TipoNum)))
						{
							sb.Append('\t'+_pT[(int)pt, (int)c, (int)r].ToString()  );

						}
						sb.AppendLine();
					}
					sb.AppendLine();
				}
				return sb.ToString();
			}

			/// <summary>
			/// TipoNum restituoto dall'operazione tra due numeri
			/// </summary>
			/// <param name="t1"></param>
			/// <param name="t2"></param>
			/// <returns></returns>
			public static TipoNum TipoNumRestituito(Token.PromTable tabella, TipoNum? t1, TipoNum? t2)
			{
				return ((t1 == null)||(t2 == null)) ? TipoNum.Nd : _pT[(int)tabella, (int)t1,(int)t2];
			}
			#endregion

			#warning Al posto del flag _var mettere un enum: valore = 0, variabile, readonly...

			TipoTk	_tipo;			// Tipo di token
			TipoNum _tNum;			// Tipo numerico
			string	_testo = "";	// Testo del token nell'espressione (valore, nome della variabile, testo dell'operatore...)
			Dat?	_dat;			// Dato
			bool	_var;			// E' una variabile oppure il valore della valutazione di una variabile ?

			#region PROPRIETA
			public TipoTk Tipo { get { return _tipo; } }
			public string Testo { get { return _testo; } }
			public Dat? Dato
			{
				get { return _dat; }
				set { _dat = value; }
			}
			public TipoNum? TipoNumero { get { return _tNum; } }
			
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
			/// E' una variabile oppure è valore numerico derivato dalla valutazione di una variabile ?
			/// </summary>
			public bool isVar { get { return _var; } }
			public bool isDatNotNull { get {return (_dat!=null); }}
			/// <summary>
			/// E' un numero (intero, float o double), esadecimale o binario ?
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
			public bool isFunzione { get {return (_tipo==TipoTk.Funzione);} }
			public bool isOperatore { get {return (_tipo==TipoTk.Operatore);} }
			public bool isOperatoreFunzione { get {return ((_tipo==TipoTk.Operatore)||(_tipo==TipoTk.Funzione));} }
			/// <summary>
			/// E' un token di tipo 'Varianile' ?
			/// </summary>
			public bool isVariabile { get {return (_tipo==TipoTk.Variabile);} }
			public bool isSimbolo { get { return (_tipo==TipoTk.Simbolo);} }

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
			public Token(TipoTk tipo, string testo = "", bool simbolo = false)
			{
				_tipo = tipo;
				_tNum = TipoNum.Nd;
				_testo = testo;
				_dat = null;
				_var = simbolo;
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
				_var = false;
			}

			/// <summary>
			/// Cancella tutto
			/// </summary>
			public void Clear()
			{
				_tipo = TipoTk.Indefinito;
				_tNum = TipoNum.Nd;
				_testo = "";
				_dat = null;
				_var = false;
			}

			/// <summary>
			/// Valuta il testo del token e lo mette in _dat, se non è null
			/// Valuta solo se di tipo: numero, esadecimale, binario O stringa
			/// Se è una variabile, va valutata con funzione apposita
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
						case TipoTk.Numero:				// Valuta numero, imposta TipoNum		
						{
							switch(_tNum)
							{
								case TipoNum.Nd:
									throw new Exception("TipoNum.Indefinito");
								//break;
								case TipoNum.Int:
								{
									int x;
									ok = int.TryParse(_testo, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out x);
									if(ok)
									{
										_dat = new Dat(x);
									}
								}
								break;
								case TipoNum.Flt:
								{
									float x;
									ok = float.TryParse(_testo, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
									if(ok)
									{
										_dat = new Dat(x);
									}
								}
								break;
								case TipoNum.Dbl:
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

						default:
						{}
						break;
					}
				}
			
				return ok;
			}

			/// <summary>
			/// Reimposta il token in base alla variabile con il nome del testo del token.
			/// Il token attuale cambia TipoTk da variabile al tipo di valore (numero, stringa...).
			/// Non controlla se il Token è del tipo giusto perché la classe sarà privata e...
			/// ...ValutaVar è chiamato solo da Parser in un case: in cui il token è sempre una variabile
			/// </summary>
			/// <param name="vars">Dizionario delle variabili</param>
			/// <returns>true se l'operazione è completata</returns>
			/// <exception cref="NotImplementedException"></exception>
			/// <exception cref="Exception"></exception>
			public bool ValutaVar(Variabili vars)
			{
				bool ok = false;
				if(_testo.Length > 0)
				{
					if(vars.ContainsKey(_testo))
					{
						Dat dt = vars.GetDat(_testo);	// Invece di vars[_testo], che estrae il valore, qui legge il Dat
						if(!dt.IsList)
						{
							switch(dt.Type)
							{
								case TypeVar.INT:
								{
									_dat = dt;
									_tipo = TipoTk.Numero;
									_tNum = TipoNum.Int;
									ok = true;
								}
								break;
								case TypeVar.FLOAT:
								{
									_dat = dt;
									_tipo = TipoTk.Numero;
									_tNum = TipoNum.Flt;
									ok = true;
								}
								break;
								case TypeVar.DOUBLE:
								{
									_dat = dt;
									_tipo = TipoTk.Numero;
									_tNum = TipoNum.Dbl;
									ok = true;
								}
								break;
								case TypeVar.STR:
								{
									_dat = dt;
									_tipo = TipoTk.Stringa;
									_tNum = TipoNum.Nd;
									ok = true;
								}
								break;
								case TypeVar.BOOL:
								{
									throw new NotImplementedException("BOOL variables are not implemented");
								}
								//break;
								case TypeVar.COLOR:
								{
									throw new NotImplementedException("COLOR variables are not implemented");
								}
								//break;
								case TypeVar.DATE:
								{
									throw new NotImplementedException("DATE variables are not implemented");
								}
								//break;
								default:
								{
									throw new Exception("Variable type not implemented");	
								}
								//break;
							}
						}
						else
						{
							throw new NotImplementedException("List variables are not implemented");
						}
					}
					else
					{
						throw new Exception("Variable name not found");
					}
				}
				else
				{
					throw new Exception("Variable name is empty");
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
				string simb = "";
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
						case TipoNum.Flt:
							ext = " [f]";
							break;
						case TipoNum.Dbl:
							ext = " [d]";
							break;
						case TipoNum.Int:
							ext = " [i]";
							break;
						default:
							ext = " [?]";
							break;
					}
				}

				if(_var)	simb="Var";

				return $"{_tipo.ToString().Replace('_',' ').PadRight(_tipoStrLength,' ')} {_testo}{ext} {{{val}}} {simb}";

			}
		}
	}
}
