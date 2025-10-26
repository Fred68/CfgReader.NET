
#define _CFG
//#undef _CFG

#define _ANALIZ
//#undef _ANALIZ

// See https://aka.ms/new-console-template for more information

using System.Text;
using Fred68.CfgReader;
using Fred68.GenDictionary;
using Fred68.Parser;

string filename = "esempio.txt";
Console.WriteLine("Avvio programma.");

bool ok = true;

#if _CFG
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
ok = cf2.GetNames(true);							// Imposta le variabili (da classe base a derivata), eliminandole dal dizionario
Console.WriteLine(cf2.ToString());					// Visualizza i messaggi (da classe base)
Console.WriteLine(cf2.Dump());						// Stampa le variabili (funzione della classe derivata)
Console.WriteLine(cf2.DumpEntries());				// Stampa il contenuto del dizionario
cf2.Clear();										// Cancella tutti i dati letti (della classe base)
#endif

Console.WriteLine("2*1.1-- = " + 2*(1.1-1));

#if _ANALIZ
Console.WriteLine(new string('-',20));
Console.WriteLine("Analizzatore");

Parser analiz = new Fred68.Parser.Parser();
analiz.FloatStd = Parser.Token.TipoNum.Dbl;

List<string> formule = new List<string>();

Console.WriteLine("Tabelle di promozione\n"+ Parser.Token.TablesToString());

bool ripeti = true;
do
{
	bool bCont = false;
	formule.Clear();
	do
	{
		bCont = false;
		Console.Write("Inserire formula:");
		string? inpf = Console.ReadLine();
		if(inpf != null)
		{
			if(inpf.Length > 0)
			{

				formule.Add(inpf);
				bCont = true;
			}
		}
	
	} while(bCont);

	foreach(string f in formule)
	{
		Console.WriteLine(new string('-',20));
	
		Console.WriteLine($"Formula: {f}");
		string res = analiz.Solve(f,true);
		Console.WriteLine(res);
	}
	
	Console.WriteLine(new string('-',10));
	Console.WriteLine("Variabili:");
	Console.WriteLine(analiz.DumpVariabili());

	ripeti = false;
	Console.Write("\nNuove formule ?");
	string? inpf2 = Console.ReadLine();
	if(inpf2 != null)
		{
			if((inpf2.ToUpper()!="N") && (inpf2.ToUpper()!="n"))
			{
				ripeti = true;
			}
		}
} while(ripeti);
#endif
Console.WriteLine("\nFine programma.");
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

