using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fred68.Parser.Token;

namespace Fred68.Parser
{

	class Operatori
	{
		/// <summary>
		/// Struct Operatore
		/// </summary>
		public struct Operatore
		{
			uint _args;
			uint _prec;
			
			public Operatore(uint argomenti, uint precedenza)
			{
				_args = argomenti;
				_prec = precedenza;
			}

			public uint Argomenti {get {return _args;}}
			public uint Precedenza {get {return _prec;}}
			public override string ToString() {return $"Args= {_args}, Prec= {_prec}";}
		}

		Dictionary<string,Operatore> _opers;		// Dizionario degli operatori

		/// <summary>
		/// Ctor
		/// </summary>
		public Operatori()
		{
			_opers = new Dictionary<string,Operatore>();

			_opers.Add("+",new Operatore(2,10));
			_opers.Add("-",new Operatore(2,10));
			_opers.Add("*",new Operatore(2,20));
			_opers.Add("/",new Operatore(2,20));
			_opers.Add("++",new Operatore(1,20));
			_opers.Add("--",new Operatore(1,20));
			_opers.Add("=",new Operatore(2,20));
			_opers.Add("^",new Operatore(2,20));
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
		/// <param name="opName">key</param>
		/// <returns>Operatore</returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public Operatore this[string opName]
			{		
			get
				{
					if(_opers.ContainsKey(opName))
					{
						return _opers[opName];
					}
					else
					{
						throw new KeyNotFoundException();
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
