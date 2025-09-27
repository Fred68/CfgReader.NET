using Fred68.GenDictionary;                 // Dizionario generico
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Fred68.Parser
{
	/// <summary>
	/// String evaluator
	/// Valutazione di una stringa
	/// </summary>
	class StringEvaluator
	{
		/// <summary>
		/// Impostazioni del Parser
		/// </summary>
		public struct Settings
		{
			public GenDictionary.GenDictionary _dict;		// Rif. al dizionario
			public string[] _strTrue;						// Rif. array stringhe "true"...
			public string[] _strFalse;						// ...e "false"
			public CultureInfo _cultureInfo;				// Cultura (per la conversione)
			public DateTimeStyles _dtStyles;				// Stili di conversione

			public Settings(GenDictionary.GenDictionary dict, string[] strTrue,  string[] strFalse,  CultureInfo cultureInfo, DateTimeStyles dtStyles)
			{
				_dict = dict;
				_strTrue = strTrue;
				_strFalse = strFalse;
				_cultureInfo = cultureInfo;	
				_dtStyles = dtStyles;	
			}
		}
		
		Settings _set;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="dict">Rif. al dizionario della variabili</param>
		public StringEvaluator(Settings settings)
		{
			_set = settings;	
		}

		/// <summary>
		/// Semplice conversione di una stringa nel tipo di dato specificato
		/// </summary>
		/// <param name="txt">testo da convertire</param>
		/// <param name="typ">tipo di valore TypeVar</param>
		/// <param name="ok">false se errore</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public dynamic ConvertString(string txt, TypeVar typ, out bool ok)
			{
			switch(typ)
				{
				case TypeVar.INT:
					{
					int x;	// TryParse(..., out int result) imposta result a 0 se errore
					ok = int.TryParse(txt, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out x);
					return x;
					}
				case TypeVar.STR:
					{
					ok = true;		// txt, letto dal file di configurazione, contiene anche i doppi apici ""
					return txt.TrimStart('"').TrimEnd('"');
					}
				case TypeVar.BOOL:
					{
					bool x = false;
					if(_set._strTrue.Contains(txt))
						{
						x = true;
						ok = true;
						}
					else if(_set._strFalse.Contains(txt))
						{
						x = false;
						ok = true;
						}
					else
						{
						ok = false;
						}
					return x;
					}
				case TypeVar.FLOAT:
					{
					float x;
					ok = float.TryParse(txt, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
					return x;
					}
				case TypeVar.DOUBLE:
					{
					double x;
					ok = double.TryParse(txt, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x);
					return x;
					}
				case TypeVar.DATE:
					{
					DateTime x;
					ok = DateTime.TryParse(txt,_set._cultureInfo,_set._dtStyles,out x); 
					return x;
					}
				default:
					{
					ok = false;
					throw new NotImplementedException("Tipo dato non definito");
					}
				}
			}	


	}

}
