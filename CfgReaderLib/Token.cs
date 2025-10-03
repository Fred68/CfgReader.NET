using Fred68.GenDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fred68.Parser
{
	public class Token
	{
		/// <summary>
		/// Tipo di token
		/// </summary>
		public enum TipoTk
		{
			Indefinito,
			Numero,				// 100 0.2
			Esadecimale,		// 0x2AbF
			Binario,			// 0b10100
			Stringa,			// "..."
			Parentesi_Aperta,	// '('
			Parentesi_Chiusa,	// ')'
			Blocco_Aperto,		// '{'
			Blocco_Chiuso,		// '}'
			Fine_Comando,		// ';'
			Separatore,			// ',' separatore di argomenti tra parentesi
			Operatore,			// +, +, *, !...
			Simbolo,			// Stringa simbolica generica (variabile, funzione o parola_chiave)
			Variabile,
			Funzione,
			Parola_chiave
		}
		
		/// <summary>
		/// Stato della macchina a stati
		/// </summary>
		public enum TkStat
		{
			TokenNuovo,			// In attesa di nuovo token
			Numero,				// Numero intero decimale o reale
			NumeroIndef,		// Numero in corso di identificazione (decimale, esadecimale o binario)
			Stringa,			// Testo
			Esadecimale,		// Numero esadecimale intero
			Binario,			// Numero binario intero
			ParentesiAperta,	// (
			ParentesiChiusa,	// )
			BloccoAperto,		// {
			BloccoChiuso,		// }
			FineComando,		// ;
			Separatore,			// ,
			Operatore,			// Operatore
			TokenCompletato,	// Finito token
			Simbolo				// Stringa simbolica generica (variabile, funzione o parola_chiave)
			}

		#region STATIC
		static int _tipoStrLength;		// Lunghezza massima della descrizione, per ToString()	

		/// <summary>
		/// Static Ctor
		/// </summary>
		static Token()
		{
			int lmax = 0;										// Imposta la lunghezza massima della descrizione
			foreach(TipoTk tp in Enum.GetValues(typeof(TipoTk)))
			{
				if(tp.ToString().Length > lmax)
					lmax = tp.ToString().Length;
			}
			_tipoStrLength = lmax+1;
		}
		#endregion

		TipoTk	_tipo;
		string	_testo = "";
		Dat?	_dat;

		#region PROPRIETA
		public TipoTk Tipo { get { return _tipo; } }
		public string Testo { get { return _testo; } }
		public Dat? Dato { get { return _dat; } }
		
		/// <summary>
		/// E' un valore numerico, una stringa o una variabile ?
		/// </summary>
		public bool isValore
		{
			get
				{
				return (	(_tipo==TipoTk.Numero) ||
							(_tipo==TipoTk.Esadecimale) ||
							(_tipo==TipoTk.Binario) ||
							(_tipo==TipoTk.Stringa) ||
							(_tipo==TipoTk.Variabile) 
						);
				}
		}

		/// <summary>
		/// E' un numero ?
		/// </summary>
		public bool isNumero
		{
			get
				{
				return (	(_tipo==TipoTk.Numero) ||
							(_tipo==TipoTk.Esadecimale) ||
							(_tipo==TipoTk.Binario)
						);
				}
		}
		
		/// <summary>
		/// E' una funzione ?
		/// </summary>
		public bool isFunzione { get {return (_tipo==TipoTk.Funzione);} }

		/// <summary>
		/// E' un operatore
		/// </summary>
		public bool isOperatore { get {return (_tipo==TipoTk.Operatore);} }
		
		#endregion


		/// <summary>
		/// Ctor vuoto
		/// Tipo: indefinito
		/// </summary>
		public Token()
		{
			this.Clear();
		}
		
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="tipo">Tipo</param>
		/// <param name="testo">Contenuto (string)</param>
		public Token(TipoTk tipo, string testo)
		{
			_tipo = tipo;
			_testo = testo;
			_dat = null;
			#warning AGGIUNGERE _dat, SE CALCOLABILE
		}

		/// <summary>
		/// Cancella tutto
		/// </summary>
		public void Clear()
		{
			_tipo = TipoTk.Indefinito;
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
				val = "";	
			}
			
			return $"{_tipo.ToString().Replace('_',' ').PadRight(_tipoStrLength,' ')} {_testo} {val}";
		}
	}
}
