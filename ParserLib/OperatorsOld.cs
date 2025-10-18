using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using static Fred68.Parser.Token;

namespace Fred68.Parser
{

	public class OperatorsOld
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
		public const char chEsponenziale = 'E';
		public const char chUnary = 'u';

		/// <summary>
		/// Class Operatore
		/// </summary>
		public class Operator
		{
			uint _args;
			uint _prec;

			/// <summary>
			/// Ctor
			/// </summary>
			/// <param name="argomenti">uint >0, se no errore</param>
			/// <param name="precedenza">int ma >0, se no errore</param>
			public Operator(uint argomenti, uint precedenza)
			{
				if(!(argomenti > 0))
					throw new Exception("[Operatori] argomenti > 0 in Ctor");
				if(!(precedenza < int.MaxValue))
					throw new Exception("[Operatori] precedenza < int.MaxValue 0 in Ctor");
				_args = argomenti;
				_prec = precedenza;
				//_isSpecial = isSpecial;
			}

			/// <summary>
			/// Numero di argomenti
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

		Dictionary<string,Operator> _opers;		// Dizionario degli operatori
		List<string>				_specOp;	// Lista operatori speciali

		/// <summary>
		/// Ctor
		/// </summary>
		public OperatorsOld()
		{
			_opers = new Dictionary<string,Operator>();
			_specOp = new List<string>();

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
					
		}
		
		/// <summary>
		/// Aggiunge un operatore al dizionario
		/// </summary>
		/// <param name="opName"></param>
		/// <param name="op"></param>
		public void Add(string opName, Operator op)
		{
			_opers.Add(opName,op);
		}

		/// <summary>
		/// Aggiunge un operatore, ma anteponendo un prefisso speciale
		/// </summary>
		/// <param name="opName"></param>
		/// <param name="op"></param>
		public void AddSpecial(string opName, Operator op)
		{
			_opers.Add((string)(chUnary+opName),op);
			_specOp.Add((string)(chUnary+opName));

		}

		/// <summary>
		/// Il dizionario degli operatori ne contiene uno del nome cercato ?
		/// </summary>
		/// <param name="opName">testo dell'operatore</param>
		/// <param name="includiSpeciali">Include nella ricerca i nomi degli operatori speciali (es.: "u+")</param>
		/// <returns></returns>
		public bool Contains(string opName, bool includiSpeciali = false)
		{
			return (includiSpeciali ? _opers.ContainsKey(opName) : (!_specOp.Contains(opName) && _opers.ContainsKey(opName)));
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
		/// Indice
		/// </summary>
		/// <param name="opName">Nome dell'operatore</param>
		/// <returns>Operatore, null se non ha trovato il testo</returns>
		public Operator? this[string opName]
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
					}
				}
			}
		
		/// <summary>
		/// Stringa con tutti i caratteri impiegati nel dizionario degli operatori
		/// </summary>
		/// <returns></returns>
		public string UsedCharactes(bool removeSpecialCh = true)
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
			if(removeSpecialCh)
			{
				chars.Remove(chUnary);
			}
			foreach(char ch in chars)
			{
				sb.Append(ch);
			}
			return sb.ToString();
		}
	}

}
