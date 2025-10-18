//#define _LU_TABLES_EXTENSION
//#undef _LU_TABLES_EXTENSION


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;			// Regex

namespace StringExtension
{
	public static class StringExtension
	{

		//#region LOOK UP TABLES

		//#if _LU_TABLES_EXTENSION
		///// <summary>
		///// ASCII character lookup table
		///// </summary>
		//protected class CharLuTable
		//{
		//	const int chMin = 32;
		//	const int chMax = 127;
		//	bool[] _chars = new bool[chMax-chMin];
		
		//	/// <summary>
		//	/// Ctor
		//	/// </summary>
		//	/// <param name="chList">Lista caratteri ASCII accettati</param>
		//	public CharLuTable(string chList)
		//	{
		//		foreach(char ch in chList)
		//		{
		//			if( (ch >= chMin) && (ch<chMax))	_chars[ch] = true;
		//		}
		//	}
		
		//	/// <summary>
		//	/// Is ASCII character ch accepted ?
		//	/// </summary>
		//	/// <param name="ch"></param>
		//	/// <returns>bool</returns>
		//	public bool isIn(char ch)
		//	{
		//		if( (ch >= chMin) && (ch<chMax))
		//			return _chars[ch-chMin];
		//		else
		//			return false;
		//	}
		//}

		//// Static members
		//static Dictionary<string, CharLuTable> _luTables;			// CharLuTables dictionary

		///// <summary>
		///// Static CTOR
		///// </summary>
		//static StringExtension()
		//{
		//	_luTables = new Dictionary<string, CharLuTable>();
		//}

		///// <summary>
		///// Add new CharLuTable
		///// </summary>
		///// <param name="name"></param>
		///// <param name="chars"></param>
		//public static void AddCharLuTable(string name, string chars)
		//{
		//	_luTables.Add(name, new CharLuTable(chars));
		//}

		///// <summary>
		///// Cleat CharLuTables dictionary
		///// </summary>
		//public static void ClearCharLuTables()
		//{
		//	_luTables.Clear();
		//}

		///// <summary>
		///// Check if a char is in a CharLuTable
		///// </summary>
		///// <param name="ch">this char</param>
		///// <param name="charLuTableName">CharLuTable name</param>
		///// <returns></returns>
		//public static bool isIn(this char ch, string charLuTableName)
		//{
		//	return _luTables[charLuTableName].isIn(ch);
		//}
		//#endif
		//#endregion


		//#region STRING ANALYSIS

		/// <summary>
		/// Find the indexes of the string txt
		/// only i not enclosed by the delimiters
		/// Example:
		/// string: list = "testo; " ; " testo 2" ; ";"
		///         012345678901234567890123456789012345
		/// Indexes              no  17           30 no
		/// </summary>
		/// <param name="line">this string</param>
		/// <param name="txt"></param>
		/// <param name="delimiterStart"></param>
		/// <param name="delimiterEnd"></param>
		/// <returns></returns>
		public static List<int> IndexOfOutside(this string line, string txt, string delimiterStart, string delimiterEnd)
		{
			List<int> lst = new List<int>();

			List<Tuple<int,int>> excluded = new List<Tuple<int,int>>();			// Lista degli intervalli interni ai delimitatori, da non considerare
			string pattern = $"(?<={delimiterStart}).*?(?={delimiterEnd})";		// Prepara la stringa per Regex
			try
			{
				MatchCollection mc = Regex.Matches(line, pattern);		// Trova le corrispondenze
				int id = 0;												// Contatore
				foreach(Match m in mc)
				{
					if(m.Success)
				{
						if( (delimiterStart != delimiterEnd) || (id%2 == 0) )		// Se stesso delimitatore, contano solo le occorrenze pari (base 0)
							excluded.Add(new Tuple<int,int>(m.Index, m.Index+m.Length));
						id++;
					}
				}
			}
			catch {} 													// In caso di errore non fa nulla (corrispondenza non trovata)

			int i, start = 0;
			bool repeat = true;
			while(repeat)
			{
				i = line.IndexOf(txt, start);							// Cerca la prima occorrenza da start in poi
				if( i != -1)											// Se l'ha trovata...
				{
					start = i + txt.Length;								// ...sposta in avanti il punto di partenza
					bool inside = false;
					foreach(Tuple<int,int> exc in excluded)				// Poi verifica se l'indice dell'occorrenza è compreso tra una coppia di indici
					{
						if( (i >= exc.Item1) && (i <= exc.Item2) )
						{
							inside = true;
							break;
						}
					}
					if(!inside)											// Se non è compreso in nessuna coppia di indici...
					{
						lst.Add(i);										// Lo aggiunge alla lista
					}
				}
				else													// Se non ha trovato l'occorrenza, ferma la ricerca
				{
					repeat = false;
				}
			}

			return lst;
		}

		#if false  // Usare Regex, più flessibile                                 
		public static string Between(this string text, string start, string end)
			{
			string str = string.Empty;
			if(text.Length > 0)
				{
				int i1,i2;
				i1 = text.IndexOf(start);
				i2 = text.LastIndexOf(end);
				if( (i1 >= 0) && (i2 >=0) && (i2 > i1) )
					{
					str = text.Substring(i1 + start.Length, i2 - i1 -1);
					}
				}
			return str;
			}
		#endif
		//#endregion

	}
}

