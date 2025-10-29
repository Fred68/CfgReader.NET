
#define _CFG
//#undef _CFG

#define _ANALIZ
//#undef _ANALIZ

// See https://aka.ms/new-console-template for more information

using Fred68.CfgReader;
using Fred68.GenDictionary;
using Fred68.Parser;
using System.Text;
using static Fred68.Parser.Parser;

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

Console.ReadKey();
Console.Clear();

Console.WriteLine("2*1.1-- = " + 2*(1.1-1));

#if _ANALIZ
Console.WriteLine(new string('-',20));
Console.WriteLine("Analizzatore");

Parser analiz = new Fred68.Parser.Parser(ShowData);
analiz.FloatStd = Parser.Token.TipoNum.Dbl;

analiz.ShowEnabled = true;

List<string> formule = new List<string>();

Console.WriteLine("Tabelle di promozione\n"+ Parser.Token.TablesToString());

Console.ReadKey();
Console.Clear();

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
		Console.WriteLine(new string('-',40));
	
		Console.WriteLine($"Formula: {f}");
		
		int cursorLeft = Console.CursorLeft;
		int cursorTop = Console.CursorTop;
		string res = analiz.Solve(f,true);
		Console.SetCursorPosition(cursorLeft,cursorTop);

		Console.WriteLine(res);
	}
	
	Console.WriteLine(new string('-',10));
	Console.WriteLine("Variabili:");
	Console.WriteLine(analiz.DumpVariabili());

	ripeti = (formule.Count > 0) ? true : false;

} while(ripeti);

#endif
Console.WriteLine("\nFine programma.");
Console.ReadKey();

void ShowData(Token? tk, Queue<Token>? inpQ, Stack<Token>? stk, Queue<Token>? outQ)
{
	StringBuilder sb = new StringBuilder();
	if(tk != null)
	{
		sb.Clear();
		Console.SetCursorPosition(0,0);
		sb.Append("Token: ");
		sb.Append(tk.ToString());
		sb.Append(new string(' ',Console.WindowWidth-sb.Length-1));
		Console.Write(sb.ToString());
	}
	else
	{
		Console.SetCursorPosition(0,0);
		Console.Write(new string(' ',Console.WindowWidth-1));
	}
	if(inpQ != null)
	{
		sb.Clear();
		Console.SetCursorPosition(0,1);
		sb.Append("InpQ: ");
		foreach(Token t in inpQ)
		{
			sb.Append(t.ToString("s")+" ");
		}
		int x = Console.WindowWidth-sb.Length-1;
		if(x>0) sb.Append(new string(' ',x));
		Console.Write(sb.ToString());
	}
	else
	{
		Console.SetCursorPosition(0,1);
		Console.Write(new string(' ',Console.WindowWidth-1));
	}
	if(stk != null)
	{
		sb.Clear();
		Console.SetCursorPosition(0,2);
		sb.Append("Stk: ");
		foreach(Token t in stk.Reverse())
		{
			sb.Append(t.ToString("s")+" ");
		}
		int x = Console.WindowWidth-sb.Length-1;
		if(x>0) sb.Append(new string(' ',x));
		Console.Write(sb.ToString());
	}
	else
	{
		Console.SetCursorPosition(0,2);
		Console.Write(new string(' ',Console.WindowWidth-1));
	}
	if(outQ != null)
	{
		sb.Clear();
		Console.SetCursorPosition(0,3);
		sb.Append("outQ: ");
		foreach(Token t in outQ)
		{
			sb.Append(t.ToString("s")+" ");
		}
		int x = Console.WindowWidth-sb.Length-1;
		if(x>0) sb.Append(new string(' ',x));
		Console.Write(sb.ToString());
	}
	else
	{
		Console.SetCursorPosition(0,3);
		Console.Write(new string(' ',Console.WindowWidth-1));
	}
	Console.SetCursorPosition(0,4);
	Console.Write(new string('-',Console.WindowWidth-1));
}


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

