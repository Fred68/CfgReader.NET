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

#warning LOOP INFINITO CON FUNZIONE: sin(...) in Shuntig Yard...: sin(PI)*2 !!!


#warning Aggiungere intestazioni ai file con Nome, Autore e riepilogo

#warning Creare una classe ParseException:Exception oppure semplice per mantenere gli errori senza generare eccezioni

#warning Rendere private le classi interne a Parser (Operatore, Token...), dopo il debug


namespace Fred68.Parser
{	

	/// <summary>
	/// Classe che analizza una stringa

	/// Parte 00: membri statici e costruttore
	/// </summary>
	public partial class Parser
	{
		
		const int ini_arg_array_sz = 3;				// Dimensione (iniziale) dell'array degli argomenti

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
			//#if _LU_TABLES_EXTENSION
			//StringExtension.StringExtension.AddCharLuTable("Spazi","\t \n\r\v\f");
			//StringExtension.StringExtension.AddCharLuTable("Numeri","0123456789");
			//StringExtension.StringExtension.AddCharLuTable("NumeriReali","0123456789.");	// Aggiungere 'e' ed 'E' per notaz. scientifica
			//StringExtension.StringExtension.AddCharLuTable("Operatori","+*-/");				// "!$%^&*+-=#@?|`/\\<>~"
			//StringExtension.StringExtension.AddCharLuTable("Nomi","abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			//StringExtension.StringExtension.AddCharLuTable("Hex","0123456789abcdefABCDEF");
			//StringExtension.StringExtension.AddCharLuTable("Bin","01");
			//#else

			chtSpazi = new CharLuTable("\t \n\r\v\f");
			chtNumeri = new CharLuTable("0123456789");
			chtNumeriReali = new CharLuTable("0123456789.");
			chtHex = new CharLuTable("0123456789abcdefABCDEF");
			chtBin = new CharLuTable("01");
			//chtOperatori = new CharLuTable(operatori.UsedCharactes());	//	"!$%^&*+-=#@?|`/\\<>~"
			chtNomi = new CharLuTable("abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			
			//#endif
			
		}

		Operators operatori;						// Dizionario di operatori e funzioni
		Variabili variabili;						// Dizionario delle variabili
		Token.TipoNum floatStd;						// Tipo float predefinito

		Queue<Token>? _queShow = null;
		Stack<Token>? _stkShow = null;
		Token? _tkShow = null;
		

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
			variabili = new Variabili();
			operatori = new Operators(variabili);				// Le funzioni devono avere acesso alle variabili
			floatStd = Token.TipoNum.Flt;
			
			chtOperatori = new CharLuTable(operatori.UsedCharacters(Operators.TipoOp.Operatore));  //	"!$%^&*+-=#@?|`/\\<>~"

			variabili["PI"] = 3.1415926535d;
			variabili["Pippo"] = 2.4f;
			variabili["Pluto"] = "Pluto!";
			
		}
		
		public string Solve(string formula,bool details = false)
		{
			StringBuilder sb = new StringBuilder();
			Token? res = null;	
			Queue<Token>? lt = null;
			Queue<Token>? qt = null;
			try
			{
				lt = ParseFormula(formula);					// Analizza la formula e crea lista di token in notazione infissa

				if(lt!=null)
				{
					sb.AppendLine($"Analizzati {lt.Count} token");
					if(details)
					{
						sb.AppendLine(new string('-',10));
						sb.AppendLine("Token in notazione infissa:");
						foreach(Parser.Token tk in lt)
						{
							sb.AppendLine(tk.ToString());
						}

					}
					qt = ShuntingYardReorder(lt);		// Riordina i token in notazione polacca inversa

					//throw new Exception("SUPERATO SHUNTING YARD");

					if(qt!=null)
					{
						sb.AppendLine($"Riordinati {qt.Count} token");
						if(details)
						{
							sb.AppendLine(new string('-',10));
							sb.AppendLine("Token in notazione polacca inversa:");
							foreach(Parser.Token tk in qt)
							{
								sb.AppendLine(tk.ToString());
							}
						}
						res = EvaluateRPN(qt);			// Valuta il risultato
					}
					else
					{
						throw new Exception("Riordino in RPN fallito");
					}
				}
				else
				{
					throw new Exception("Lista token nulla dopo Parse()");
				}
			}
			catch (Exception ex)
			{
				sb.AppendLine(ex.Message);	
			}
			if(res!=null)
			{
				if(details)
				{
					sb.AppendLine(new string('-',10));
					sb.AppendLine("Token risultante:");
					sb.AppendLine(res.ToString());
				}

				Dat? dt = res.Dato;
				if(dt!=null)
				{
					sb.AppendLine(new string('-',10));
					sb.AppendLine("Risultato:");
					sb.Append(dt.ToString());
				}
			}

			return sb.ToString();
		}



		public string DumpVariabili()
		{
			StringBuilder sb = new StringBuilder();
			foreach(string key in variabili)
			{
				sb.AppendLine($"{key} = {variabili[key]}");
			}

			return sb.ToString();
		}
	}
}
