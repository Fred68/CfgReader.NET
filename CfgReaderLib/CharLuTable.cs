using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fred68.Parser
{
	/// <summary>
	/// Look up table per i caratteri ASCII da 32 (incluso) a 127 (escluso)
	/// </summary>
	public class CharLuTable
	{
		const int chMin = 32;
		const int chMax = 127;
		bool[] _chars;
		
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="chList">Lista caratteri ASCII accettati</param>
		public CharLuTable(string chList)
		{
			_chars = new bool[chMax-chMin];
			foreach(char ch in chList)
			{
				if( (ch >= chMin) && (ch<chMax))	_chars[ch-chMin] = true;
			}
		}
		
		/// <summary>
		/// Is ASCII character ch accepted ?
		/// </summary>
		/// <param name="ch"></param>
		/// <returns>bool</returns>
		public bool Contains(char ch)
		{
			if( (ch >= chMin) && (ch<chMax))
			{
				return _chars[ch-chMin];
			}
			else
			{
				return false;
			}
		}


		
	}

	#if !_LU_TABLES_EXTENSION
	public static class CharExtension
	{
		public static bool isIn(this char ch, CharLuTable charLuTable)
		{
			return charLuTable.Contains(ch);
		}
	}
	#endif
}
