using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fred68.GenDictionary;                 // Dizionario generico


namespace Fred68.CfgReader
{
	class ExParser
	{
		GenDictionary.GenDictionary _dict;		// Riferimento al dizionario
		TypeVar x;


		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="dict">Rif. al dizionario della variabili</param>
		public ExParser(GenDictionary.GenDictionary dict)
		{
			_dict = dict;
		}

		


	}
}
