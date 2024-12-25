using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using Fred68.GenDictionary;					// Dizionario generico
using System.Dynamic;

namespace Fred68.CfgReader
	{

	public partial class CfgReader : DynamicObject
		{

		public static class MSG
			{
			public const string LetturaConfigurazioneOK = @"Lettura della configurazione completata.";
			public const string LetturaConfigurazioneERR = @"Errore nella lettura della configurazione.";
			public const string LineeValide = @"Linee valide: {0}.";
			public const string FileNotFound = "Errore: file '{0}' non trovato."; 
			public const string ErroreNellaRiga = @"Errore nella riga {0}: [{1}].";
			public const string SezioneNonRiconosciuta = @"Errore: sezione '{0}' non riconosciuta.";
			public const string ErroreSintassiSezione = @"Errore di sintassi: '{0}={1}' non riconosciuta, usare [Sez]={2}/{3}.";
			public const string ErroreSintassiTipoNonRiconosciuto = @"Errore di sintassi: tipo '{0}' non riconosciuto.";
			public const string ErroreSintassiTipoNonSpecificato = @"Errore di sintassi: tipo non specificato.";
			public const string ErroreSintassiTipoNonImplementato = @"Errore: tipo {0} non implementato";
			}

		#region PROPRIETA'
		/// <summary>
		/// Message
		/// </summary>
		public string Message
			{
			get {return _msg.ToString();}
			}
		/// <summary>
		/// Ok if true, error if false
		/// </summary>
		public bool IsOk
			{
			get {return ok;}
			}	
		/// <summary>
		/// Comment prefix
		/// </summary>
		public string CHR_Commento {get; set;} = @"#";
		/// <summary>
		/// Caratteri ammessi (Regex)
		/// Lettere e numeri, interpunzioni, parentesi, oltre ai caratteri speciali
		/// </summary>
		public string CHR_Ammessi {get; } = "[^-A-Za-z0-9 .,:;_/?!#%=$\"\\[]]";	
		/// <summary>
		/// Inizio nome identificativo di sezione
		/// </summary>
		public string CHR_SezioneOpenBracket {get; set;} = @"[";
		/// <summary>
		/// Fine nome identificativo di sezione
		/// </summary>
		public string CHR_SezioneClosedBracket {get; set;} = @"]";
		/// <summary>
		/// Carattere fine sezione
		/// </summary>
		public string CHR_SezioneEnd {get; set;} = @".";
		/// <summary>
		/// Assegnazione
		/// </summary>
		public string CHR_Assign {get; } = @"=";
		/// <summary>
		/// Delimitatore di stringa (Regex non usato)
		/// </summary>
		public string CHR_StringDelimiter {get; set;} = "\"";	// public string CHR_StringDelimiterRgx {get; } = "(?<=\").*?(?=\")";	
		/// <summary>
		/// Separatore di lista
		/// </summary>
		public string CHR_ListSeparator {get; set; } = @",";
		/// <summary>
		/// Fine linea (assegnazioni multilinea)
		/// </summary>
		public string CHR_MergeNextLine {get; set; } = @"_";
		/// <summary>
		/// Separatore tra tipo e nome variabile (spazio)
		/// </summary>
		public string CHR_TypeArgSeparator {get; set; } = @" ";
		/// <summary>
		/// Allineamento numeri di linea
		/// </summary>
		public int PADlines {get; set; } = 6;
		/// <summary>
		/// On string
		/// </summary>
		public string STR_On {get; set;} = @"ON";
		/// <summary>
		/// Off string
		/// </summary>
		public string STR_Off {get; set;} = @"OFF";
		/// <summary>
		/// Separatore tra prefisso di sezione e nome di variabile
		/// </summary>
		public string CHR_SectionPrefixSeparator = @"_";
		///// <summary>
		///// Inizio linea per terminare la lettura
		///// </summary>
		//public string STR_Errore {get; set;} = @"ERROR";
		#endregion

		/// <summary>
		/// Strings for true
		/// </summary>
		public string[] strTrue		{get;} = {"true", "TRUE", "1"};
		/// <summary>
		/// Strings for false
		/// </summary>
		public string[] strFalse	{get;} = {"false", "FALSE", "0"};

		void CreateCommands()
			{
			_cmds["MSG"]	=new Func<string, bool>( arg => {_msg.AppendLine(arg); return true;});		// Genera un messaggio e continua
			_cmds["STOP"]	=new Func<string, bool>( arg => {_msg.AppendLine(arg); return false;});		// Esce dal ciclo con un messaggio
			_cmds["DUMP"]	=new Func<string, bool>( arg => {_msg.AppendLine(DumpEntries()); return true;});	// Variabili
			_cmds["LINES"]	=new Func<string, bool>( arg => {_msg.AppendLine(DumpLines()); return true;});		// Linee lette
			_cmds["SECTIONPREFIXON"]	=new Func<string, bool>( arg => {_useSectPrefix = true; _msg.AppendLine("SECTION PREFIX ON"); return true;});
			_cmds["SECTIONPREFIXOFF"]	=new Func<string, bool>( arg => {_useSectPrefix = true; _msg.AppendLine("SECTION PREFIX OFF"); return true;});
			}
		}
	}
