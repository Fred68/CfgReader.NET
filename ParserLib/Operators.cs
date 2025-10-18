using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


		#region CLASSI Operator e Function (+OpBase)
		public class OpBase
		{
			uint _args;
			TipoOp _tOp;

			/// <summary>
			/// Ctor
			/// </summary>
			/// <param name="argomenti"></param>
			/// <exception cref="Exception"></exception>
			protected OpBase(uint argomenti,TipoOp tOp)
			{
				if(!(argomenti > 0))
					throw new Exception("[GenOperator] argomenti > 0 in Ctor");
				_args = argomenti;
				_tOp = tOp;
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
		}

		public class Operator : OpBase
		{
			uint _prec = 0;

			public Operator(uint argomenti, uint precedenza) : base(argomenti,TipoOp.Operatore)
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
			public Function(uint argomenti) : base(argomenti,TipoOp.Funzione)
			{}

			/// <summary>
			/// ToString() override
			/// </summary>
			/// <returns></returns>
			public override string ToString() {return base.ToString();}


		}
		#endregion


		Dictionary<string,OpBase>	_opers;		// Dizionario di operatori e funzioni
		List<string>				_specOp;	// Lista dei nomi degli operatori speciali

		/// <summary>
		/// Ctor
		/// </summary>
		public Operators()
		{
			_opers = new Dictionary<string,OpBase>();
			_specOp = new List<string>();
			

			/////////////////////////////////////////////////////////
			/// Operatori
			/////////////////////////////////////////////////////////

			// Operatore per numeri in notazione esponenziale
			Add(chEsponenziale.ToString(),new Operator(2,100));

			// Operatori unari
			Add("++",new Operator(1,40));
			Add("--",new Operator(1,40));
			
			// Speciali (stesso testo di altri operatori, ma ricodificati come unari)
			AddSpecial("+",new Operator(1,110));
			AddSpecial("-",new Operator(1,110));

			// Operatori binari alta precedenza
			Add("^",new Operator(2,30));
			Add("*",new Operator(2,29));
			Add("/",new Operator(2,28));
			
			// Operatori binari bassa precedenza
			Add("+",new Operator(2,20));
			Add("-",new Operator(2,20));
			
			// Operatore di assegnazione
			Add("=",new Operator(2,10));

			/////////////////////////////////////////////////////////
			/// Funzioni
			/////////////////////////////////////////////////////////

			// Funzioni con un argomento
			Add("sin".ToUpper(),new Function(1));
			Add("max".ToUpper(),new Function(2));

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
