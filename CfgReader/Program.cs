// See https://aka.ms/new-console-template for more information

using System.Text;
using Fred68.CfgReader;
using Fred68.GenDictionary;

string filename = "esempio.txt";
Console.WriteLine("Avvio programma.");

// UTILIZZO SEMPLICE COME DIZIONARIO

Console.WriteLine("Utilizzo come dizionario...");

dynamic cfgR = new CfgReader();							// Crea l'oggetto della classe base
cfgR.CHR_ListSeparator = @";";
cfgR.ReadConfiguration(filename);						// Legge la configurazione		
Console.WriteLine(cfgR.ToString());						// Visualizza i messaggi
try
	{
	Console.WriteLine("cc = " + cfgR.cc);
	cfgR.cc = 100.3f;
	Console.WriteLine("cc = " + cfgR.cc + " riassegnata");
	}
catch (Exception ex)
	{
	Console.WriteLine(ex.Message);
	}

#if true
Console.WriteLine("\nVariabili importate:");			// Comando DUMP
Console.WriteLine(cfgR.DumpEntries());
Console.WriteLine("Linee:");							// Comando LINES
Console.WriteLine(cfgR.DumpLines());
#endif


// UTILIZZO CON CLASSE DERIVATA

Console.WriteLine("Utilizzo con classe derivata...");
			
CfgR2 cf2 = new CfgR2();							// Crea l'oggetto della classe derivata
cf2.CHR_ListSeparator = @";";	
cf2.ReadConfiguration(filename);					// Legge la configurazione (classe base)
bool ok = cf2.GetNames(true);						// Imposta le variabili (da classe base a derivata), eliminandole dal dizionario
Console.WriteLine(cf2.ToString());					// Visualizza i messaggi (da classe base)
Console.WriteLine(cf2.Dump());						// Stampa le variabili (funzione della classe derivata)
Console.WriteLine(cf2.DumpEntries());				// Stampa il contenuto del dizionario
cf2.Clear();										// Cancella tutti i dati letti (della classe base)


Console.WriteLine(new string('-',20));
Console.WriteLine("Analizzatore");
Console.WriteLine(new string('-',20));

Console.WriteLine("Prova ciclo su List<obj>");
List<int> intL = new List<int>{10,20,30,40,50};
List<double> dblL = new List<double>{1.1,2.2,3.3,4.4,5.5};

Dat iDat = new Dat(intL);
Dat dDat = new Dat(dblL);

Console.WriteLine(iDat.ToString());
Console.WriteLine(dDat.ToString(true));


Console.WriteLine("Fine programma.");
Console.ReadKey();




class CfgR2 : CfgReader
	{
	public int Paperino;
	public float ccc;
	public List<int> ddd;

	public string Dump()
		{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"CfgR2.Dump():");
		sb.AppendLine($"Paperino={Paperino}");
		sb.AppendLine($"ccc={ccc}");
		sb.AppendLine($"ddd={ddd}");

		return sb.ToString();
		}
	}

