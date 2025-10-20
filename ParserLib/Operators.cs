using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fred68.GenDictionary;			// Per Dat


namespace Fred68.Parser
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

		public delegate Token pSolver(Token[] argArray);	// Delegate

		/*******************************************************************************/
		#region CLASSI Operator e Function (+OpBase)
		public class OpBase
		{
			uint _args;
			TipoOp _tOp;
			pSolver _pSolve;

			/// <summary>
			/// Ctor
			/// </summary>
			/// <param name="argomenti"></param>
			/// <exception cref="Exception"></exception>
			protected OpBase(uint argomenti,TipoOp tOp,pSolver pSolve)
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
			public uint Argomenti {get {return _args;}}

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
			public Token Solve(Token[] argArray)
			{
				return _pSolve(argArray);
			}
		}

		public class Operator : OpBase
		{
			uint _prec = 0;

			public Operator(uint argomenti, uint precedenza, pSolver pSolve) : base(argomenti,TipoOp.Operatore,pSolve)
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
			public Function(uint argomenti, pSolver pSolve) : base(argomenti,TipoOp.Funzione,pSolve)
			{}

			/// <summary>
			/// ToString() override
			/// </summary>
			/// <returns></returns>
			public override string ToString() {return base.ToString();}


		}
		#endregion
		/*******************************************************************************/

		Dictionary<string,OpBase>	_opers;		// Dizionario di operatori e funzioni
		List<string>				_specOp;	// Lista dei nomi degli operatori speciali

		/// <summary>
		/// Ctor
		/// </summary>
		public Operators()
		{
			_opers = new Dictionary<string,OpBase>();
			_specOp = new List<string>();
			FillOpFuncDictionary();
		}

		/*******************************************************************************/
		#region FUNZIONI di calcolo degli operatori Token _func(Token[] argArray)

		Token _notImplemented(Token[] argArray)
		{
			throw new NotImplementedException("Operatore o funzione non implementato, al momento...");
			//return (Token) null;
		}

		/// <summary>
		/// Esegue sottrazione
		/// Gli argomento sono in ordine inverso: Array[0] è l'ultimo, in notazione infissa
		/// </summary>
		/// <param name="argArray">Array dei parametri</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <exception cref="Exception"></exception>
		Token _sottrazione(Token[] argArray)
		{
			Token _out = new Token();
			Token.TipoTk tp;
			Token.TipoNum tn;

			#warning Aggiungere l'indice della tabella di promozione da usare
			if(CreateTkFromTipoNum(argArray,2, out tp, out tn))
			{
				switch(tp)
				{
					case Token.TipoTk.Numero:
					{
						_out = new Token(tp,tn,"");			// Crea il token
						switch(tn)
						{
							case Token.TipoNum.Intero:
							{
								int x = argArray[1].Dato.Get()-argArray[0].Dato.Get();
								_out.Dato = new Dat(x);
							}
							break;
							case Token.TipoNum.Float:
								{
								float x = argArray[1].Dato.Get()-argArray[0].Dato.Get();
								_out.Dato = new Dat(x);
								}
							break;
							case Token.TipoNum.Double:
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
						_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Indefinito,"");			// Crea il token
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
		Token _somma(Token[] argArray)
		{
			Token _out = new Token();
			Token.TipoTk tp;
			Token.TipoNum tn;

			#warning Aggiungere l'indice della tabella di promozione da usare
			if(CreateTkFromTipoNum(argArray,2, out tp, out tn))
			{
				switch(tp)
				{
					case Token.TipoTk.Numero:
					{
						_out = new Token(tp,tn,"");			// Crea il token
						switch(tn)
						{
							case Token.TipoNum.Intero:
							{
								int x = argArray[1].Dato.Get()+argArray[0].Dato.Get();
								_out.Dato = new Dat(x);
							}
							break;
							case Token.TipoNum.Float:
								{
								float x = argArray[1].Dato.Get()+argArray[0].Dato.Get();
								_out.Dato = new Dat(x);
								}
							break;
							case Token.TipoNum.Double:
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
						_out = new Token(Token.TipoTk.Stringa,Token.TipoNum.Indefinito,"");			// Crea il token
						string s = argArray[1].Dato.Get() + argArray[0].Dato.Get();
						_out.Dato = new Dat(s);
						}
					break;
					default:
						throw new Exception("Operatore su tipo di token errato");
					break;


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


		/// <summary>
		/// Verifica i token dell'array (uno o due argomenti).
		/// Stabilisce il tipo di token prodotto (numero o stringa) e il tipo di numero (con promozione)
		/// </summary>
		/// <param name="argArray"></param>
		/// <param name="nargs"></param>
		/// <param name="tp">out Token.TipoTk</param>
		/// <param name="tn">out Token.TipoNum</param>
		/// <returns>true se tipi corretti</returns>
		/// <exception cref="Exception"></exception>
		private bool CreateTkFromTipoNum(Token[] argArray, int nargs, out Token.TipoTk tp, out Token.TipoNum tn)
		{
			#warning AGGIUNGERE, tra gli argomenti, l'INDICE della tabella di promozione da usare
			bool ok = false;
			int numOk = nargs;				// Numero di argomenti (1 o 2), messo a 0 negli altri casi
			Token? a1, a2;					// Token 1° e 2° argomento
			a1 = a2 = null;
			tp = Token.TipoTk.Indefinito;		// Tipo di token in output
			tn = Token.TipoNum.Indefinito;
			
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
						tn = Token.ResTipoNum(a1.TipoNumero, a2.TipoNumero);	// Tabella di promozione
						if(tn != Token.TipoNum.Indefinito)
							ok = true;
					}
					else if((a1.isStringa)&&(a2.isStringa))
					{
						tp = Token.TipoTk.Stringa;
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
		/// Crea tutti gli operatori e le funzioni
		/// </summary>
		private void FillOpFuncDictionary()
		{
			/////////////////////////////////////////////////////////
			/// Operatori
			/////////////////////////////////////////////////////////

			// Operatore per numeri in notazione esponenziale
			// TipoTk: Num, TipoNum: I,F,D. Promozione.
			// Verifica segni ? Eccezione standard durante il calcolo
			Add(chEsponenziale.ToString(),new Operator(2,100,_notImplemented));

			// Operatori unari
			// TipoTk: Num, TipoNum: I,F,D.
			Add("++",new Operator(1,40,_notImplemented));
			Add("--",new Operator(1,40,_notImplemented));
			
			// Speciali (stesso testo di altri operatori, ma ricodificati come unari)
			// TipoTk: Num, TipoNum: I,F,D.
			AddSpecial("+",new Operator(1,110,_notImplemented));
			AddSpecial("-",new Operator(1,110,_notImplemented));

			// Operatori binari alta precedenza
			// TipoTk: Num, TipoNum: I,F,D. Promozione.
			Add("^",new Operator(2,30,_notImplemented));
			Add("*",new Operator(2,29,_notImplemented));
			Add("/",new Operator(2,28,_notImplemented));
			
			#warning Aggiungere operatore '\' divisione intera (senza resto).
			#warning Aggiungere operatore '%' resto intero.

			// Operatori binari bassa precedenza
			// TipoTk: Num, TipoNum: I,F,D. Promozione.
			// TipoTk.Stringa (+ concatena, - toglie i caratteri)
			Add("+",new Operator(2,20,_somma));	
			Add("-",new Operator(2,20,_sottrazione));
			
			// Operatore di assegnazione
			// TipoTk: Num, TipoNum: I,F,D. Promozione + conversione 
			// TipoTk.Stringa
			Add("=",new Operator(2,10,_notImplemented));

			/////////////////////////////////////////////////////////
			/// Funzioni
			/////////////////////////////////////////////////////////

			// Funzioni con un argomento
			Add("sin".ToUpper(),new Function(1,_notImplemented));
			Add("max".ToUpper(),new Function(2,_notImplemented));
		}

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
					#warning TryGetValue è più veloce
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
