using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fred68.GenDictionary;

namespace Fred68.Parser
{
	/*******************************************/
	// Nota:
	// Un token (in una formula) deve avere un nome e l'identificazione di variabile
	// Una variabile deve contenere un valore (numerico, stringa...)
	// Quando il parser legge un token con una variabile:...
	// Se è in lettura, lo sostituisce con un token con il valore (numero, stringa...) ma...
	// ...mantenendo il flag 'variabile', per riconoscerla in caso di nuova assegnazione
	// Se è in scrittura (operatore di assegnazione), legge il valore del token e lo mette nella variabile.
	/*******************************************/
	public partial class Parser
	{
		public class Variabili : GenDictionary.GenDictionary
		{
		
			public Variabili() : base()	{}

		}
	}
}
