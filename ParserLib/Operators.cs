using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fred68.GenDictionary;			// Per Dat e GenDictionary



namespace Fred68.Parser
{
	public partial class Parser
	{
		public class Operators
		{
		
			public enum TipoOp
			{
				Indefinito,
				Operatore,
				Funzione
			}

			public const char chSpazio = ' ';	
			public const char chZero =  '0';
			public const char chParentesiAperta = '(';
			public const char chParentesiChiusa = ')';
			public const char chGraffaAperta = '{';
			public const char chGraffaChiusa = '}';
			public const char chVirgola = ',';
			public const char chPuntoVirgola = ';';
			public const char chStringaInizio = '\"';
			public const char chStringaFine = '\"';
			public const char chHex = 'x';
			public const char chBin = 'b';
			public const char chPuntoDecimale = '.';
			public const char chEsponenziale = 'E';
		
			// Caratteri speciali
			public const char chUnary = 'u';

			// Assegnazione
			public const string strAssign = "=";

			//public delegate Token pSolver(Token[] argArray);	// Delegate
			public delegate Token pSolver(ArgArray<Token> argArray);	// Delegate

			/*******************************************************************************/
			#region CLASSI Operator e Function (+OpBase)
			public class OpBase
			{
				int _args;
				TipoOp _tOp;
				pSolver _pSolve;

				/// <summary>
				/// Ctor
				/// </summary>
				/// <param name="argomenti"></param>
				/// <exception cref="Exception"></exception>
				protected OpBase(int argomenti,TipoOp tOp,pSolver pSolve)
				{
					if(!(argomenti > 0))
						throw new Exception("[GenOperator] argomenti > 0 in Ctor");
					_args = argomenti;
					_tOp = tOp;
					_pSolve = pSolve;
				}

				/// <summary>
				/// Numero di argomenti
				/// </summary>
				public int Argomenti {get {return _args;}}

				public bool IsOperator {get {return _tOp == TipoOp.Operatore;}}
				public bool IsFunction {get {return _tOp == TipoOp.Funzione;}}
				public TipoOp Type {get {return _tOp;} }
			
				/// <summary>
				/// E' un operatore del tipo richiesto ?
				/// Se richiesto indefinito, restituisce true
				/// </summary>
				/// <param name="tOp">TipoOp</param>
				/// <returns></returns>
				public bool IsTypeOf(TipoOp tOp)
				{
					return (tOp == TipoOp.Indefinito) ? true : (_tOp == tOp);
				}

				/// <summary>
				/// ToString() override
				/// </summary>
				/// <returns></returns>
				public override string ToString() {return $"Args={_args}";}
			
				/// <summary>
				/// Solve
				/// </summary>
				/// <param name="argArray"></param>
				/// <returns>Token</returns>
				
				public Token Solve(ArgArray<Token> argArray)		// public Token Solve(Token[] argArray)
				{
					return _pSolve(argArray);
				}
			}

			public class Operator : OpBase
			{
				uint _prec = 0;

				public Operator(int argomenti, uint precedenza, pSolver pSolve) : base(argomenti,TipoOp.Operatore,pSolve)
				{
					if(!(precedenza < int.MaxValue))
						throw new Exception("[Operatori] precedenza < int.MaxValue 0 in Ctor");
					_prec = precedenza;
				}
		
				/// <summary>
				/// Precedenza
				/// </summary>
				public uint Precedenza {get {return _prec;}}

				/// <summary>
				/// ToString() override
				/// </summary>
				/// <returns></returns>
				public override string ToString() {return $"{base.ToString()}, Prec={_prec}";}
			}

			public class Function : OpBase
			{

				/// <summary>
				/// Ctor
				/// </summary>
				/// <param name="argomenti"></param>
				public Function(int argomenti, pSolver pSolve) : base(argomenti,TipoOp.Funzione,pSolve)
				{}

				/// <summary>
				/// ToString() override
				/// </summary>
				/// <returns></returns>
				public override string ToString() {return base.ToString();}


			}
			#endregion
			/*******************************************************************************/

			Dictionary<string,OpBase>		_opers;			// Dizionario di operatori e funzioni
			List<string>					_specOp;		// Lista dei nomi degli operatori speciali (unari)
			Variabili						_vars;

			/// <summary>
			/// Ctor
			/// </summary>
			public Operators(Variabili variabili)
			{
				_opers = new Dictionary<string,OpBase>();
				_specOp = new List<string>();
				_vars = variabili;
				FillOpFuncDictionary();
			}

			/*******************************************************************************/
			// CS8602: possibile deferenziamento di null.
			// CS8629: il tipo valore nullable non nuò nssere null.
			// Ma Token[] argArray contiene valori già controllati
			#pragma warning disable CS8602
			#region FUNZIONI di calcolo degli operatori Token _func(Token[] argArray)

			Token _notImplemented(ArgArray<Token> argArray)
			{
				throw new NotImplementedException("Operatore o funzione non implementato, al momento...");
				//return (Token) null;
			}

			// Esecuzione delle operazioni tra token
			// Gli argomento sono in ordine inverso: Array[0] è l'ultimo, in notazione infissa
			// Ci sono alcune eccezioni aggiuntive, oltre a quelle matematiche
			Token _sottrazione(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[1].Dato.Get()-argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = argArray[1].Dato.Get()-argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[1].Dato.Get()-argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						case Token.TipoTk.Stringa:
							{
							_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Nd,"");			// Crea il token
							string s1 = argArray[1].Dato.Get();
							string s2 = argArray[0].Dato.Get();
							int x = s1.IndexOf(s2);
							string s = (x!=-1) ? s1.Substring(0, x)+s1.Substring(x+s2.Length) : s1;
							_out.Dato = new Dat(s);
							}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _sottrazione_unaria(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*1,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = -argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = -argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = -argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						//case Token.TipoTk.Stringa:
						//	{
						//	_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Indefinito,"");			// Crea il token
						//	string s1 = argArray[1].Dato.Get();
						//	string s2 = argArray[0].Dato.Get();
						//	int x = s1.IndexOf(s2);
						//	string s = (x!=-1) ? s1.Substring(0, x)+s1.Substring(x+s2.Length) : s1;
						//	_out.Dato = new Dat(s);
						//	}
						//break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _decremento(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*1,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[0].Dato.Get()-1;
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = argArray[0].Dato.Get()-1f;
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[0].Dato.Get()-1d;
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						//case Token.TipoTk.Stringa:
						//	{
						//	_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Indefinito,"");			// Crea il token
						//	string s1 = argArray[1].Dato.Get();
						//	string s2 = argArray[0].Dato.Get();
						//	int x = s1.IndexOf(s2);
						//	string s = (x!=-1) ? s1.Substring(0, x)+s1.Substring(x+s2.Length) : s1;
						//	_out.Dato = new Dat(s);
						//	}
						//break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _somma(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[1].Dato.Get()+argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = argArray[1].Dato.Get()+argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[1].Dato.Get()+argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						case Token.TipoTk.Stringa:
							{
							_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Nd,"");			// Crea il token
							string s = argArray[1].Dato.Get() + argArray[0].Dato.Get();
							_out.Dato = new Dat(s);
							}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;


					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}

				return _out;
			}
			Token _somma_unaria(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*1,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							#warning Velocizzare: assegnare direttamente argArray[0] ad _out
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						case Token.TipoTk.Stringa:
							{
							_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Nd,"");			// Crea il token
							string s = argArray[0].Dato.Get();
							_out.Dato = new Dat(s);
							}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;


					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}

				return _out;
			}
			Token _incremento(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*1,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							#warning Velocizzare: assegnare direttamente argArray[0] ad _out
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[0].Dato.Get()+1;
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = argArray[0].Dato.Get()+1f;
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[0].Dato.Get()+1d;
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;


					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}

				return _out;
			}
			Token _prodotto(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Std, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[1].Dato.Get()*argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x = argArray[1].Dato.Get()*argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[1].Dato.Get()*argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
					
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;


					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}

				return _out;
			}
			Token _divisione(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Div, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									throw new Exception("Errore nella tabella di promozione della divisione");
								}
								//break;
								case Token.TipoNum.Flt:
									{
									float x = ((float)argArray[1].Dato.Get())/((float)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = ((double)argArray[1].Dato.Get())/((double)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _divisioneInt(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Int, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									// Integer division
									int x = ((int)argArray[1].Dato.Get())/((int)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									throw new Exception("Errore nella tabella di promozione della divisione");
									}
								//break;
								case Token.TipoNum.Dbl:
									{
									throw new Exception("Errore nella tabella di promozione della divisione");
									}
								//break;
							}
						}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _restoInt(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Int, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									// Integer division
									int x = ((int)argArray[1].Dato.Get())%((int)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									throw new Exception("Errore nella tabella di promozione della divisione");
									}
								//break;
								case Token.TipoNum.Dbl:
									{
									throw new Exception("Errore nella tabella di promozione della divisione");
									}
								//break;
							}
						}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _potenza(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Div, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									throw new Exception("Errore nella tabella di promozione della divisione");
								}
								//break;
								case Token.TipoNum.Flt:
									{
									float x = MathF.Pow((float)argArray[1].Dato.Get(),(float)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = Math.Pow((double)argArray[1].Dato.Get(),(double)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
			Token _esponenziale(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Div, out tp, out tn))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									throw new Exception("Errore nella tabella di promozione della divisione");
								}
								//break;
								case Token.TipoNum.Flt:
									{
									float x = (float)argArray[1].Dato.Get() * MathF.Pow(10f,(float)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = (double)argArray[1].Dato.Get() * Math.Pow(10d,(double)argArray[0].Dato.Get());
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Token incompatibili");	
				}
				return _out;
			}
		
			Token _assegnazione(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				Token.TipoTk tp;
				Token.TipoNum tn;

				// Calcola il token di uscita: l'espressione (a=1) deve restituire 1, dopo aver assegnato 1 ad a
				if(TipoTkFromPromTable(argArray, /*2,*/ Token.PromTable.Std, out tp, out tn,true))
				{
					switch(tp)
					{
						case Token.TipoTk.Numero:
						{
							_out = new Token(tp,tn,"");			// Crea il token
							switch(tn)
							{
								case Token.TipoNum.Int:
								{
									int x = argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
								}
								break;
								case Token.TipoNum.Flt:
									{
									float x =argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
								case Token.TipoNum.Dbl:
									{
									double x = argArray[0].Dato.Get();
									_out.Dato = new Dat(x);
									}
								break;
							}
						}
						break;
						case Token.TipoTk.Stringa:
							{
							_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Nd,"");			// Crea il token
							string s = argArray[0].Dato.Get();
							_out.Dato = new Dat(s);
							}
						break;
						default:
							throw new Exception("Operatore su tipo di token errato");
						//break;
					}

					Token arg = argArray[1];
					if(arg.isVar)						// E' un token di tipo 'variabile' (non ancora valutata) ?
					{
						string nome = arg.Testo;
						_vars[nome] = _out.Dato.Get();
					}
					else if(arg.isSimbolo)
					{
						string nome = arg.Testo;
						_vars[nome] = _out.Dato.Get();

					}
					else
					{
						throw new Exception("L'argomento a sinistra dell'assegnazione deve essere il nome di una variabile");
					}

				}
				else
				{
					throw new Exception("Token incompatibili");	
				}

				return _out;
			}


			#endregion
			/*******************************************************************************/
			#region FUNZIONI di calcolo delle funzioni Token _func(Token[] argArray)
			Token _sin(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				if(CheckNumericArgs(argArray,1))
				{
					
					switch(argArray[0].TipoNumero)
					{
						case Token.TipoNum.Int:
						case Token.TipoNum.Flt:
						{
							_out = new Token(Token.TipoTk.Numero,Token.TipoNum.Flt,"");
							float x = MathF.Sin((float)argArray[0].Dato.Get());
							_out.Dato = new Dat(x);
						}
						break;
						case Token.TipoNum.Dbl:
						{
							_out = new Token(Token.TipoTk.Numero,Token.TipoNum.Dbl,"");
							double x = Math.Sin((double)argArray[0].Dato.Get());
							_out.Dato = new Dat(x);

						}
						break;
						default:
							throw new Exception("Operazione su token numerico errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Operazione su token non numerico");	
				}
				return _out;
			}

			Token _max(ArgArray<Token> argArray)
			{
				Token _out = new Token();
				if(CheckNumericArgs(argArray,1))
				{
					
					switch(argArray[0].TipoNumero)
					{
						case Token.TipoNum.Int:
						case Token.TipoNum.Flt:
						{
							_out = new Token(Token.TipoTk.Numero,Token.TipoNum.Flt,"");
							float x = MathF.Sin((float)argArray[0].Dato.Get());
							_out.Dato = new Dat(x);
						}
						break;
						case Token.TipoNum.Dbl:
						{
							_out = new Token(Token.TipoTk.Numero,Token.TipoNum.Dbl,"");
							double x = Math.Sin((double)argArray[0].Dato.Get());
							_out.Dato = new Dat(x);

						}
						break;
						default:
							throw new Exception("Operazione su token numerico errato");
						//break;
					}
				}
				else
				{
					throw new Exception("Operazione su token non numerico");	
				}
				return _out;
			}

			
			#endregion
			#pragma warning restore CS8602
			/*******************************************************************************/

			#pragma warning disable CS8602
			#pragma warning disable CS8629
			/// <summary>
			/// Verifica i token dell'array (uno o due argomenti).
			/// Stabilisce il tipo di token prodotto (numero o stringa) e il tipo di numero (con promozione)
			/// Usa un indice di tabella (la divisione, per esempio, ha diversa promozione: int, int => float o double
			/// </summary>
			/// <param name="argArray"></param>
			/// <param name="nargs"></param>
			/// <param name="iProm"></param>
			/// <param name="tp">out Token.TipoTk</param>
			/// <param name="tn">out Token.TipoNum</param>
			/// <param name="assign">Assegnazione di un simbolo o di una variabile</param>
			/// <returns>true se tipi corretti</returns>
			/// <exception cref="Exception"></exception>
			private bool TipoTkFromPromTable(ArgArray<Token> argArray, /*int nargs,*/ Token.PromTable iProm, out Token.TipoTk tp, out Token.TipoNum tn, bool assign = false)
			{
				bool ok = false;
				int numOk = argArray.nArgs;		// Numero di argomenti (1 o 2), messo a 0 negli altri casi
				Token? a1, a2;					// Token 1° e 2° argomento
				a1 = a2 = null;
				tp = Token.TipoTk.Indefinito;		// Tipo di token in output
				tn = Token.TipoNum.Nd;
			
				switch(numOk)					// Verifica il numero di argomenti: uno e due soltanto
				{
					case 1:
						a1 = argArray[0];
						if(a1 == null)
						{
							numOk = 0;
							throw new Exception("Argomento null");
						}
						break;
					case 2:
						a1 = argArray[0];
						a2 = argArray[1];
						if((a1 == null)||(a2 == null))
						{
							numOk = 0;
							throw new Exception("Argomento null");
						}
						break;
					default:
						numOk = 0;
						break;
				}
				switch(numOk)					// Verifica i tipi di dato (numero o stringa) e il tipo di numero
				{								// a1 e a2 non sono null
					case 1:
						if(a1.isNumero)
						{
							tp = Token.TipoTk.Numero;
							tn = (Token.TipoNum)a1.TipoNumero;
							ok = true;
						}
						else if(a1.isStringa)
						{
							tp = Token.TipoTk.Stringa;
							ok = true;
						}
						else
						{
							throw new Exception("Operazione unaria su token non numerico né stringa");
						}
						break;
					case 2:
						if((a1.isNumero)&&(a2.isNumero))
						{
							tp = Token.TipoTk.Numero;
							tn = Token.TipoNumRestituito(iProm,a1.TipoNumero, a2.TipoNumero);	// Tabella di promozione
							if(tn != Token.TipoNum.Nd)
								ok = true;
						}
						else if((a1.isStringa)&&(a2.isStringa))
						{
							tp = Token.TipoTk.Stringa;
							ok = true;
						}
						else if(assign && (a2.isSimbolo || a2.isVar))
						{
							tp = a1.Tipo;
							tn = (Token.TipoNum)a1.TipoNumero;
							ok = true;
						}
						else
						{
							throw new Exception("Operazione tra token non numerici né stringa oppure incompatibili");
						}
						break;
					default:
						break;
				}

			
				return ok;
			}

			/// <summary>
			/// Verifica che tutti gli argomenti siano numerici
			/// </summary>
			/// <param name="argArray"></param>
			/// <param name="nargs"></param>
			/// <returns></returns>
			private bool CheckNumericArgs(ArgArray<Token> argArray, int nargs)
			{
				bool ok = true;
				Token tk;
				for(int i=0; i < nargs;	i++)
				{
					tk = argArray[i];
					if(tk != null)
					{
						if(!tk.isNumero)
						{
							ok = false;
							break;
						}
					}
					else
					{
						ok = false;
						throw new Exception("Argomento null");
					}

				}
				return ok;
			}
			#pragma warning restore CS8629
			#pragma warning restore CS8602

			/*******************************************************************************/
			#region RIEMPIE il dizionario degli operatori e delle funzioni
			private void FillOpFuncDictionary()
			{
				/////////////////////////////////////////////////////////
				/// Operatori
				/////////////////////////////////////////////////////////

				// Operatore per numeri in notazione esponenziale
				// TipoTk: Num, TipoNum: I,F,D. Promozione.
				// Verifica segni ? Eccezione standard durante il calcolo
				Add(chEsponenziale.ToString(),new Operator(2,100,_esponenziale));

				// Operatori unari, postfissi, con precedenza elevata
				// TipoTk: Num, TipoNum: I,F,D.
				Add("++",new Operator(1,40,_incremento));
				Add("--",new Operator(1,40,_decremento));
			
				// Speciali (stesso testo di altri operatori, ma ricodificati come unari)
				// TipoTk: Num, TipoNum: I,F,D.
				AddSpecial("+",new Operator(1,110,_somma_unaria));
				AddSpecial("-",new Operator(1,110,_sottrazione_unaria));

				// Operatori binari alta precedenza
				// TipoTk: Num, TipoNum: I,F,D. Promozione.
				Add("^",new Operator(2,30,_potenza));
				Add("*",new Operator(2,29,_prodotto));
				Add("/",new Operator(2,28,_divisione));
				// Operatori binari alta precedenza tra interi
				Add("\\",new Operator(2,28,_divisioneInt));
				Add("%",new Operator(2,28,_restoInt));
				
				#warning Aggiungere operatori tra bit (solo per gli interi)

				// Operatori binari bassa precedenza
				// TipoTk: Num, TipoNum: I,F,D. Promozione.
				// TipoTk.Stringa (+ concatena, - toglie i caratteri)
				Add("+",new Operator(2,20,_somma));	
				Add("-",new Operator(2,20,_sottrazione));
			
				// Operatore di assegnazione
				// TipoTk: Num, TipoNum: I,F,D. Promozione + conversione
				// TipoTk.Stringa
				Add(strAssign,new Operator(2,10,_assegnazione));

				/////////////////////////////////////////////////////////
				/// Funzioni
				/////////////////////////////////////////////////////////

				// Funzioni con un argomento
				Add("sin".ToUpper(),new Function(1,_sin));
				Add("max".ToUpper(),new Function(2,_notImplemented));		// <= DA SCRIVERE !!!
				
				#warning Aggiungere funzioni di conversione INT() FLOAT()...
			}
			#endregion
			/*******************************************************************************/

			/// <summary>
			/// Aggiunge un operatore
			/// </summary>
			/// <param name="opName"></param>
			/// <param name="op"></param>
			public void Add(string opName, Operator op)
			{
				_opers.Add(opName,op);
			}

			/// <summary>
			/// Aggiunge una funzione
			/// </summary>
			/// <param name="opName"></param>
			/// <param name="fn"></param>
			public void Add(string opName, Function fn)
			{
				_opers.Add(opName,fn);
			}

			/// <summary>
			/// Aggiunge un operatore speciale
			/// </summary>
			/// <param name="opName"></param>
			/// <param name="op"></param>
			public void AddSpecial(string opName, Operator op)
			{
				_opers.Add((string)(chUnary+opName),op);
				_specOp.Add((string)(chUnary+opName));

			}

			/// <summary>
			/// Il nome è contenuto nel dizionario di operatori e funzioni ?
			/// </summary>
			/// <param name="opName"></param>
			/// <param name="includiSpeciali">Considera anche i nomi speciali</param>
			/// <returns></returns>
			private bool Contains(string opName, bool includiSpeciali = false)
			{
				bool isIn = _opers.ContainsKey(opName);
				if(!includiSpeciali)	isIn = isIn && (!_specOp.Contains(opName));
				return isIn;

				// La versione precedente è meno efficiente, fa due volte la ricerca nel dizionario:
				// return (includiSpeciali ? _opers.ContainsKey(opName) : (!_specOp.Contains(opName) && _opers.ContainsKey(opName)));
			}

			/// <summary>
			/// Il nome è contenuto nel dizionario di operatori e funzioni...
			/// ...ed è del tipo richiesto (se Indefinito, accetta tutti).
			/// </summary>
			/// <param name="opName">Testo dell'operatore o della funzione</param>
			/// <param name="tOp">TipoOp</param>
			/// <param name="includiSpeciali">Considera anche i nomi speciali</param>
			/// <returns></returns>
			public bool Contains(string opName, TipoOp tOp, bool includiSpeciali = false)
			{
				bool ret = false;
				if(Contains(opName,includiSpeciali))
				{
					ret = _opers[opName].IsTypeOf(tOp);
				}
				return ret;
			}

			/// <summary>
			/// Il nome dell'operatore (con o senza il prefisso speciale) è un operatore speciale ?
			/// </summary>
			/// <param name="opName"></param>
			/// <returns></returns>
			public bool IsSpecial(string opName)
			{
				bool sp = false;
				if(opName.StartsWith(chUnary))
				{
					sp = _specOp.Contains(opName);
				}
				else
				{
					sp = _specOp.Contains(chUnary+opName);
				}
				return sp;
			}

			/// <summary>
			/// ch è un carattere speciale (prefisso unario) ?
			/// </summary>
			/// <param name="ch"></param>
			/// <returns></returns>
			public bool IsSpecialChar(char ch)	{return (ch == chUnary);}

			/// <summary>
			/// Stringa con tutti i caratteri impiegati nel dizionario degli operatori
			/// </summary>
			/// <param name="tipoOp">Ricerca dei caratteri di Operatori, Funzioni o entrambi</param>
			/// <param name="removeOperatorSpecialCh">Non considera il carattere speciale</param>
			/// <returns></returns>
			public string UsedCharacters(TipoOp tipoOp, bool removeOperatorSpecialCh = true)
			{
				StringBuilder sb = new StringBuilder();	
				List<char> chars = new List<char>();

				_opers.AsParallel().ForAll(op =>			// Più veloce di for o di foreach. op è KeyValuePair<string,OpBase>
					{
						if(op.Value.IsTypeOf(tipoOp))		// Controlla se del tipo richiesto (Indefinito è sempre sì)
						{
							foreach(char ch in op.Key)		
							{
								bool add = true;

								// Se richiesta rimozione caratteri speciali, se op è un Operatore e se ch è speciale
								if(removeOperatorSpecialCh && (op.Value.Type == TipoOp.Operatore) && IsSpecialChar(ch))
								{
									add = false;	
								}

								if(add && (!chars.Contains(ch)))	// Lo aggiunge, se non c'é ancora.
								{
									// Corretto contro errore occasionale nella trasformazione in RPN.
									// Con formule simili a: 1+2*(3+4--) talvolta veniva trascurato l'ultimo operatore.
									// Necessario lock su List<char> chars a cui i processi possono accedere contemporaneamente.
									lock (chars)	// Deve usare lock, operazioni in parallelo !!!
									{
										chars.Add(ch);
									}
								}
							}

						}
					});

				foreach(char ch in chars)
				{
					sb.Append(ch);
				}

				return sb.ToString();
			}

			/// <summary>
			/// Ricerca indicizzata per nome e tipo
			/// </summary>
			/// <param name="opName">testo dell'operatore o della funzione</param>
			/// <param name="tOp">TipoOp (se TipoOp.Indefinito accetta tutti)</param>
			/// <returns></returns>
			public OpBase? this[string opName, TipoOp tOp]
				{		
				get
					{
						#warning TryGetValue è più veloce ?
						if(_opers.ContainsKey(opName))
						{
							OpBase opb = _opers[opName];
							if(opb.IsTypeOf(tOp))
							{
								return opb;
							}
							else
							{
								return null;
							}
						}
						else
						{
							return null;
						}
					}
				}

		}
	}
}



