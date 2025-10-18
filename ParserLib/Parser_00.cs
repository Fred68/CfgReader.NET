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


#warning Creare una classe personalizzata per lanciare ParseException oppure 
#warning oppure usare una classe per mantenere gli errori senza generare eccezioni

namespace Fred68.Parser
{	

	/// <summary>
	/// Classe che analizza una stringa
	/// Parte 01: trasforma un'espressione in una lista di token
	/// Riconosce operatori, numeri di vario formato, parentesi, funzioni, virgole, stringhe
	/// Riconosce (usando il namespace Fred68.GenDictionary) anche i nomi di variabili da un dizionario generalizzato
	/// Un ringraziamento a One Lone Coder https://github.com/OneLoneCoder
	/// Algoritmo copiato da: https://github.com/OneLoneCoder/Javidx9/blob/master/SimplyCode/OneLoneCoder_DIYLanguage_Tokenizer.cpp

	/// Parte 00: membri statici e costruttore
	/// </summary>
	public partial class Parser
	{
		#warning Dopo le prove, impostare a 3
		const int ini_arg_array_sz = 1;				// Dimensione dell'array degli argomenti

		#if !_LU_TABLES_EXTENSION
		
		static CharLuTable chtSpazi;				// Caratteri vuoti da ignorare
		static CharLuTable chtNumeri;				// Caratteri per numeri di vari formati
		static CharLuTable chtNumeriReali;
		static CharLuTable chtHex;
		static CharLuTable chtBin;
		static CharLuTable? chtOperatori;			// Caratteri per gli operatori
		static CharLuTable chtNomi;					// Caratteri per variabili, funzioni e parole chiave

		#endif
		
		/// <summary>
		/// static Ctor
		/// </summary>
		static Parser()
		{
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
			//chtOperatori = new CharLuTable(operatori.UsedCharactes());	//	"!$%^&*+-=#@?|`/\\<>~"
			chtNomi = new CharLuTable("abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			
			#endif
			
		}

		//OperatorsOld operatori;
		//FunctionsOld funzioni;
		Operators operatori;

		Token.TipoNum floatStd;

		/// <summary>
		/// Tipo standard per numero in virgola mobile
		/// </summary>
		public Token.TipoNum FloatStd
		{
			get {return floatStd;}
			set {floatStd = value;}
		}

		/// <summary>
		/// Ctor
		/// </summary>
		public Parser()
		{
			//operatori = new OperatorsOld();
			//funzioni = new FunctionsOld();
			operatori = new Operators();
			floatStd = Token.TipoNum.Float;

			#if !_LU_TABLES_EXTENSION
			chtOperatori = new CharLuTable(operatori.UsedCharactes(Operators.TipoOp.Operatore));  //	"!$%^&*+-=#@?|`/\\<>~"
			#endif
		}
		
	}
}
