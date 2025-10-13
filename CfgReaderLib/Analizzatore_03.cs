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



namespace Fred68.Parser
{
	/// <summary>
	/// Parte 03: wlabora una coda di Token in RPN e calcola il risultato
	/// </summary>

	public partial class Analizzatore
	{
			
		/// <summary>
		/// Elabora una coda di Token in RPN e calcola il risultato
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public Token ElaboraRPN(Queue<Token> input)
		{
			Token _out = new Token();							// Token finale con il risultato
			Stack<Token> _stack = new Stack<Token>();			// Stack temporanei per i token letti dalla coda di input
			Token[] _args = new Token[ini_arg_array_sz];		// Array temporaneo con gli argomenti

			#warning Ammesse solo funzioni a numero fisso di argomenti [per ora].
			#warning Se numero variabile, analizzare il conteggio tra le parentesi e rinominare le funzioni con un numero.
			#warning Usato array perché più semplice e rapida l'indicizzazione.
			#warning Usare delegate per fare i calcoli

			/*
				Ciclo per tutti i token della coda di input

				Legge il token

				Se è un numero:
					se dat è null, lo valuta (se è intero, float, binario ecc...).
					lo inserisce nello stack

				Se è un operatore:
					legge il testo
					risale all'operatore
					legge il numero di operandi
					se è superiore alle dimensioni dell'array, lo ridimensiona (può azzerarlo).
					ciclo su 'i' da 0 a numero di operandi -1
						estrae un token dallo stack (se lo stack è vuoto: errore)
						controlla che sia un numero o una stringa [in base all'operatore] e che dat non sia nullo
						lo mette nell'array alla posizione 'i'
					al termine chiama il puntatore a funzione che accetta un array di token, che restituisce un nuov token
					mette il token nello stack

				Se è una funzione: idem


			*/
			foreach(Token t in input)
			{
				if(t.isNumeroStringa)
				{
					#warning Elabora il contenuto (valuta il testo e lo mette in Dat)
					_stack.Push(t);
				}
				else if(t.isOperatore)
				{
					if(operatori.Contains(t.Testo))
					{

					}

				}
			}


			return _out;
		}

	}
}
