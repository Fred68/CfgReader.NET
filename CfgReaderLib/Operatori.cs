using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fred68.Parser.Token;

namespace Fred68.Parser
{

	public class Operatori
	{

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

		/// <summary>
		/// Class Operatore (deve esser nullable)
		/// </summary>
		public class Operatore
		{
			uint _args;
			uint _prec;
			
			/// <summary>
			/// Ctor
			/// </summary>
			/// <param name="argomenti">uint >0 se to errore</param>
			/// <param name="precedenza">int ma >0, se no errore</param>
			public Operatore(uint argomenti, uint precedenza)
			{
				if(!(argomenti > 0))				throw new Exception("[Operatori] argomenti > 0 in Ctor");
				if(!(precedenza < int.MaxValue))	throw new Exception("[Operatori] precedenza < int.MaxValue 0 in Ctor");
				_args = argomenti;
				_prec = precedenza;
			}

			/// <summary>
			/// Argomenti (> 0)
			/// </summary>
			public uint Argomenti {get {return _args;}}
			/// <summary>
			/// Precedenza (<  int.MaxValue)
			/// </summary>
			public uint Precedenza {get {return _prec;}}

			/// <summary>
			/// ToString() override
			/// </summary>
			/// <returns></returns>
			public override string ToString() {return $"Args= {_args}, Prec= {_prec}";}
		}

		Dictionary<string,Operatore> _opers;		// Dizionario degli operatori

		/// <summary>
		/// Ctor
		/// </summary>
		public Operatori()
		{
			_opers = new Dictionary<string,Operatore>();

			// Operatori unari
			_opers.Add("++",new Operatore(1,40));
			_opers.Add("--",new Operatore(1,40));

			// Operatori binari alta precedenza
			_opers.Add("^",new Operatore(2,30));
			_opers.Add("*",new Operatore(2,30));
			_opers.Add("/",new Operatore(2,30));
			
			// Operatori binari bassa precedenza
			_opers.Add("+",new Operatore(2,20));
			_opers.Add("-",new Operatore(2,20));
			
			_opers.Add("=",new Operatore(2,10));

			// ERRORE: _opers.Add("xxx",new Operatore(0,2147483648));
			
		}

		/// <summary>
		/// Contains
		/// </summary>
		/// <param name="opName">key</param>
		/// <returns>bool</returns>
		public bool Contains(string opName)	{return _opers.ContainsKey(opName);}

		/// <summary>
		/// Indice
		/// </summary>
		/// <param name="opName">Testo dell'operatore</param>
		/// <returns>Operatore (by value, è una struct). Null se non ha trovato il testo</returns>
		public Operatore? this[string opName]
			{		
			get
				{
					if(_opers.ContainsKey(opName))
					{
						return _opers[opName];
					}
					else
					{
						return null;
						//throw new KeyNotFoundException();
					}

				}
			}
		
		/// <summary>
		/// Stringa con tutti i caratteri impiegati nel dizionario degli operatori
		/// </summary>
		/// <returns></returns>
		public string UsedCharactes()
		{
			StringBuilder sb = new StringBuilder();
			List<char> chars = new List<char>();
			foreach(string oper in _opers.Keys)
			{
				foreach(char ch in oper)
				{
					if(!chars.Contains(ch))
						chars.Add(ch);	
				}
			}
			foreach(char ch in chars)
			{
				sb.Append(ch);
			}
			return sb.ToString();
		}
	}

}
