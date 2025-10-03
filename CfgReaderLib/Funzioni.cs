using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fred68.Parser.Operatori;

namespace Fred68.Parser
{
	public class Funzioni
	{

		/// <summary>
		/// Class Funzione
		/// </summary>
		public class Funzione
		{
			uint _args;

			/// <summary>
			/// Ctor
			/// </summary>
			/// <param name="argomenti">uint > 0, se no errore</param>
			/// <exception cref="Exception"></exception>
			public Funzione(uint argomenti)
			{
				if(!(argomenti > 0))				throw new Exception("[Funzioni] argomenti > 0 in Ctor");
				_args = argomenti;
			}

			/// <summary>
			/// Argomenti
			/// </summary>
			public uint Argomenti {get {return _args;}}

			/// <summary>
			/// ToString() override
			/// </summary>
			/// <returns></returns>
			public override string ToString() {return $"Args= {_args}";}
		}

		Dictionary<string,Funzione> _funcs;		// Dizionario delle funzioni

		/// <summary>
		/// Ctor
		/// </summary>
		public Funzioni()
		{
			_funcs = new Dictionary<string,Funzione>();

			// Funzioni con un argomento
			_funcs.Add("sin".ToUpper(),new Funzione(1));
			_funcs.Add("max".ToUpper(),new Funzione(2));
		}

		/// <summary>
		/// Contains
		/// </summary>
		/// <param name="fnName">funzction name</param>
		/// <returns></returns>
		public bool Contains(string fnName)	{return _funcs.ContainsKey(fnName);}


		/// <summary>
		/// Indice
		/// </summary>
		/// <param name="fnName">Nome della funzione</param>
		/// <returns>Funzione, null se non ha trovato il testo</returns>
		public Funzione? this[string fnName]
			{		
			get
				{
					if(_funcs.ContainsKey(fnName))
					{
						return _funcs[fnName];
					}
					else
					{
						return null;
						//throw new KeyNotFoundException();
					}

				}
			}

		/// <summary>
		/// List of used characters in functions
		/// </summary>
		/// <returns></returns>
		public string UsedCharactes()
		{
			StringBuilder sb = new StringBuilder();
			List<char> chars = new List<char>();
			foreach(string func in _funcs.Keys)
			{
				foreach(char ch in func)
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
