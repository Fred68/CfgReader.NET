using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fred68.GenDictionary;			// Per usare Dat



/*************************/
// File con più classi
/*************************/

namespace Fred68.Parser
{

	class Token
	{

		public enum Tipo
		{
			Indefinito,
			Numero,				// 100 0.2 0b1010 0x2A
			Stringa,			// "..."
			ParentesiAperta,	// (
			ParentesiChiusa,	// )
			InizioBlocco,		// {
			FineBlocco,			// }
			FineComando,		// ;
			Separatore,			// ,
			Operatore,			// +, +, *, !...
			Simbolo,			// pippo (es.: variabile, in un dizionario)
			ParolaChiave		// somma (es.: funzione, in un dizionario
		}
		
		public enum TkStat
		{
			NuovoTk,			// In attesa di nuovo token
			Numero,
			String,
			NonTotDef,			// Numero in corso di identificazione
			Esadecimale,
			Binario,
			ParentesiAperta,
			ParentesiChiusa,
			InizioBlocco,
			FineBlocco,		
			FineComando,
			Separatore,
			Operatore,
			Simbolo,
			ParolaChiave,
			FineTk				// Finito token
			}


		static int _tipoStrLen;	
		static Token()
		{
			int lmax = 0;
			foreach(Tipo tp in Enum.GetValues(typeof(Tipo)))
			{
				if(tp.ToString().Length > lmax)
					lmax = tp.ToString().Length;
			}
			_tipoStrLen = lmax+1;
		}

		Tipo	_tipo;
		string	_testo;
		Dat?	_dat;

		

		public Token()
		{
			_tipo = Tipo.Indefinito;
			_testo = "";
			_dat = null;
		}
		
		public override string ToString() 
		{
			string val;
			if(_dat != null)
			{
				val = _dat.ToString();	//_dat.Get().ToString(out val);
			}
			else
			{
				val = string.Empty;	
			}
			
			return $"{_tipo.ToString().PadRight(_tipoStrLen,' ')} {_testo} {val}";
		}
	}

	class Operatori
	{
		/// <summary>
		/// Struct Operatore
		/// </summary>
		public struct Operatore
		{
			uint _args;
			uint _prec;
			
			public Operatore(uint argomenti, uint precedenza)
			{
				_args = argomenti;
				_prec = precedenza;
			}

			public uint Argomenti {get {return _args;}}
			public uint Precedenza {get {return _prec;}}
			public override string ToString() {return $"Args= {_args}, Prec= {_prec}";}
		}

		Dictionary<string,Operatore> _opers;		// Dizionario degli operatori

		/// <summary>
		/// Ctor
		/// </summary>
		public Operatori()
		{
			_opers = new Dictionary<string,Operatore>();

			_opers.Add("+",new Operatore(2,10));
			_opers.Add("-",new Operatore(2,10));
			_opers.Add("*",new Operatore(2,20));
			_opers.Add("/",new Operatore(2,20));
		}

		/// <summary>
		/// Contains
		/// </summary>
		/// <param name="opName">key</param>
		/// <returns>bool</returns>
		public bool Contains(string opName)	{return _opers.ContainsKey(opName);}
		/// <summary>
		/// Indice
		/// </summary>
		/// <param name="opName">key</param>
		/// <returns>Operatore</returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public Operatore this[string opName]
			{		
			get
				{
					if(_opers.ContainsKey(opName))
					{
						return _opers[opName];
					}
					else
					{
						throw new KeyNotFoundException();
					}

				}
			}

	}



	
	/// <summary>
	/// Classe che analizza una stringa
	/// Riconosce operatori, numeri di vario formato, parentesi, funzioni, virgole, stringhe
	/// Riconosce (usando il namespace Fred68.GenDictionary) anche i nomi di variabili da un dizionario generalizzato
	/// </summary>
	class Analizzatore
	{
		Operatori operatori;

		public Analizzatore()
		{
			operatori = new Operatori();	
		}
		
		List<Token> Analizza(string input)
		{
			List<Token> tokens;

			if(input.Length < 1)		throw new Exception("Stringa da analizzare nulla");
			
			tokens = new List<Token>();

			Token.TkStat stAttuale = Token.TkStat.NuovoTk;
			Token.TkStat stFuturo = Token.TkStat.NuovoTk;
			string strTkAttuale = "";
			Token tkAttuale;

			bool bPuntoDecimale = false;
			string strTkNonTotDef = "";

			int nParentesi = 0;
			int nBlocchi = 0;
			
			for(int i=0; i<input.Length;)	// ciclo while con for senza incremento
			{
				switch(stAttuale)
				{
					case Token.TkStat.NuovoTk:
					{

					}
					break;



				}
			}
		return tokens;
		}
		


	}
}
