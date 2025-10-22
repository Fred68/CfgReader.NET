
#define _CFG
#undef _CFG

#define _ANALZ
// #undef _ANALZ

// See https://aka.ms/new-console-template for more information

using System.Text;
using Fred68.CfgReader;
using Fred68.GenDictionary;
using Fred68.Parser;

string filename = "esempio.txt";
Console.WriteLine("Avvio programma.");


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
bool ok = cf2.GetNames(true);						// Imposta le variabili (da classe base a derivata), eliminandole dal dizionario
Console.WriteLine(cf2.ToString());					// Visualizza i messaggi (da classe base)
Console.WriteLine(cf2.Dump());						// Stampa le variabili (funzione della classe derivata)
Console.WriteLine(cf2.DumpEntries());				// Stampa il contenuto del dizionario
cf2.Clear();										// Cancella tutti i dati letti (della classe base)
#endif

#if _ANALZ
Console.WriteLine(new string('-',20));
Console.WriteLine("Analizzatore");


//Console.WriteLine("Prova ciclo su List<obj>");
//Dat iDat = new Dat(new List<int>{10,20,30,40,50});
//Dat dDat = new Dat(new List<double>{1.1,2.2,3.3,4.4,5.5});
//Console.WriteLine(iDat.ToString());
//Console.WriteLine(dDat.ToString(true));
//Console.WriteLine($"char.MinValue= {(int)char.MinValue}\tchar.MaxValue= {(int)char.MaxValue}");

List<string> formule = new List<string>();

//formule.Add("");
//formule.Add("2");
//formule.Add("2++");
//formule.Add("\"Pippo\"");
//formule.Add("2+1");

//formule.Add("3+4*2 / ( 1 − 5 ) ^ 2 ^ 3");
//formule.Add("3+4*2");
//formule.Add("(2+3^2)*(5+1)");
//formule.Add("{2.1+3.2^0.1}");
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
	//formule.Add("0xFF");
	//formule.Add("0b010+100");


	// formule.Add("(2+3^2))*(5+1)");
	// formule.Add("{2+3^2}*{(5+1)");

	Parser analiz = new Fred68.Parser.Parser();
	analiz.FloatStd = Token.TipoNum.Double;

	foreach(string f in formule)
	{
		List<Token> lt = new List<Token>();
		Queue<Token> qt = new Queue<Token>();
		Token res = new Token();

		Console.WriteLine(new string('-',20));
	
		bool ok = true;
		if(ok)
		{
			try
			{
				lt = analiz.Parse(f);
			}
			catch(Exception ex)
			{
				ok = false;
				Console.WriteLine(ex.Message);
			}
		}

		if(ok)
		{
			try
			{
				qt = analiz.ShuntingYardReorder(lt);
			}
			catch (Exception ex)
			{
				ok = false;
				Console.WriteLine(ex.Message);
			}
		}

		if(ok)
		{
			try
			{
				res = analiz.EvaluateRPN(qt);
			}
			catch (Exception ex)
			{
				ok = false;
				Console.WriteLine(ex.Message);
			}
		}
	
		Console.WriteLine($"Formula: {f}");
		Console.WriteLine($"Token (infix):");
		foreach(Token tk in lt)
		{
			Console.WriteLine(tk.ToString());
		}
		Console.WriteLine(new string('-',10));
		Console.WriteLine($"Token (rpn):");
		foreach(Token tk in qt)
		{
			Console.WriteLine(tk.ToString());
		}
		Console.WriteLine(new string('-',10));
		Console.WriteLine($"Risultato:");
		Console.WriteLine(res);
	}
	#endif

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

	//Console.WriteLine(uint.MaxValue);
	//Console.WriteLine(int.MaxValue);
} while(ripeti);

Console.WriteLine("2*1.1-- = " + 2*(1.1-1));
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

