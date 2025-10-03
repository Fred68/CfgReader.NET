using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fred68.GenDictionary;			// Per usare Dat
using StringExtension;


#warning Scrivere 3: Valutazione di un'espressione in RPN inserita in coda e stack
#warning La notazione scientifica (es.: 2.4E+3) può essere trasfornmata in sequenza di token distinti, da riconoscere successivamente.


namespace Fred68.Parser
{
	

	/// <summary>
	/// Classe che analizza una stringa
	/// Un ringraziamento a One Lone Coder, algoritmo ispirato da https://github.com/OneLoneCoder
	/// Riconosce operatori, numeri di vario formato, parentesi, funzioni, virgole, stringhe
	/// Riconosce (usando il namespace Fred68.GenDictionary) anche i nomi di variabili da un dizionario generalizzato
	/// </summary>
	public partial class Analizzatore
	{
		static Operatori operatori;
		
		#if !_LU_TABLES_EXTENSION

		// Caratteri vuoti da ignorare
		static CharLuTable chtSpazi;

		// Caratteri per i numeri
		static CharLuTable chtNumeri;
		static CharLuTable chtNumeriReali;
		static CharLuTable chtHex;
		static CharLuTable chtBin;
		
		// Caratteri per gli operatori (letti dalla classe)
		static CharLuTable chtOperatori;
		
		// Caratteri per variabili, funzioni e parole chiave
		static CharLuTable chtNomi;
		#endif
		
		
		/// <summary>
		/// static Ctor
		/// </summary>
		static Analizzatore()
		{
			operatori = new Operatori();

			#if _LU_TABLES_EXTENSION
			StringExtension.StringExtension.AddCharLuTable("Spazi","\t \n\r\v\f");
			StringExtension.StringExtension.AddCharLuTable("Numeri","0123456789");
			StringExtension.StringExtension.AddCharLuTable("NumeriReali","0123456789.");	// Aggiungere 'e' ed 'E' per notaz. scientifica
			StringExtension.StringExtension.AddCharLuTable("Operatori","+*-/");				// "!$%^&*+-=#@?|`/\\<>~"
			StringExtension.StringExtension.AddCharLuTable("Nomi","abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			StringExtension.StringExtension.AddCharLuTable("Hex","0123456789abcdefABCDEF");
			StringExtension.StringExtension.AddCharLuTable("Bin","01");
			#else

			chtSpazi = new CharLuTable("\t \n\r\v\f");
			chtNumeri = new CharLuTable("0123456789");
			chtNumeriReali = new CharLuTable("0123456789.");
			chtHex = new CharLuTable("0123456789abcdefABCDEF");
			chtBin = new CharLuTable("01");
			chtOperatori = new CharLuTable(operatori.UsedCharactes());	//	"!$%^&*+-=#@?|`/\\<>~"
			chtNomi = new CharLuTable("abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			
			#endif
			
		}

		/// <summary>
		/// Ctor
		/// </summary>
		public Analizzatore()
		{}
		
	}
}
