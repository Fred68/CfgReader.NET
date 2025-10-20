using Fred68.GenDictionary;			// Per usare Dat
using StringExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;



namespace Fred68.Parser
{
	/// <summary>
	/// Parte 03: wlabora una coda di Token in RPN e calcola il risultato
	/// </summary>

	public partial class Parser
	{
			
		/// <summary>
		/// Elabora una coda di Token in RPN e calcola il risultato
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public Token EvaluateRPN(Queue<Token> input)
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
				Lo valuta, se è intero, float, binario ecc...
				Se è un numero:
					lo inserisce nello stack
				Se è un operatore (o una funzione):
					legge il testo
					risale all'operatore
					legge il numero di operandi
					se è superiore alle dimensioni dell'array, lo ridimensiona (può azzerarlo).
					ciclo su 'i' da 0 a numero di operandi -1
						estrae un token dallo stack (se lo stack è vuoto: errore)
						controlla che sia un numero o una stringa [in base all'operatore] e che dat non sia nullo
						lo mette nell'array alla posizione 'i'
					al termine chiama il puntatore a funzione che accetta un array di token, che restituisce un nuovo token
					mette il token nello stack

			*/

			foreach(Token t in input)
			{	
				t.ValutaVal();
				if(t.isNumeroStringa)
				{
					_stack.Push(t);
				}
				else if(t.isOperatoreFunzione)
				{
					if(operatori.Contains(t.Testo,Operators.TipoOp.Indefinito,true))		// Funzione o operatore speciale
					{
						Operators.OpBase? op = operatori[t.Testo,Operators.TipoOp.Indefinito];	// Ottiene operatore o funzione
						if( op != null)
						{
							uint nargs = op.Argomenti;
							if(nargs > _args.Length)					// Ridimensiona l'array, se necessario
							{
								Array.Resize(ref _args, (int)nargs);
							}
							
							for(int i=0; i < nargs;	i++)				// Estrae il numero di argomenti dalla coda di input...
							{
								if(input.Count > 0)
								{
									Token tkq = _stack.Pop();
									if((tkq.isNumeroStringa)&&(tkq.isDatNotNull))
									{
										_args[i] = tkq;					// ...e li mette nell'array degli argomenti
									}
									else
									{
										if(!tkq.isNumeroStringa)
											throw new Exception("Il token sullo stack non è un valore: errato numero di argomenti");
										else if(!tkq.isDatNotNull)
											throw new Exception("Il token sullo stack non è stato valutato");
									}
								}
								else
								{
									throw new Exception("Wrong argument numbers");
								}

							}

							Token tslv = op.Solve(_args);	// Esegue in calcolo dell'operatore

							_stack.Push(tslv);
						}
						else
						{
							throw new Exception("Operatore o funzione non trovato");	
						}
					}
					

				}
			}

			if(_stack.Count != 1)
			{
				throw new Exception("Errore nel calcolo della RPN");
			}
			else
			{
				_out = _stack.Pop();
			}
			return _out;
		}
	
		
		public Token EvaluateRPN_backup(Queue<Token> input)
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
				Lo valuta, se è intero, float, binario ecc...
				Se è un numero:
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
					al termine chiama il puntatore a funzione che accetta un array di token, che restituisce un nuovo token
					mette il token nello stack

				Se è una funzione: idem
				
				
			
			*/

			
			foreach(Token t in input)
			{	
				t.ValutaVal();
				if(t.isNumeroStringa)
				{
					_stack.Push(t);
				}
				else if(t.isOperatore)
				{
					if(operatori.Contains(t.Testo,Operators.TipoOp.Operatore,true))		// Anche operatori speciali, es.: u+
					{
						Operators.OpBase? op = operatori[t.Testo,Operators.TipoOp.Operatore];
						if( op != null)
						{
							uint nargs = op.Argomenti;
							if(nargs > _args.Length)			// Ridimensiona l'array, se necessario
							{
								Array.Resize(ref _args, (int)nargs);
							}
							for(int i=0; i < nargs;	i++)
							{
								if(input.Count > 0)
								{
									Token tkq = _stack.Pop();
									if((tkq.isNumeroStringa)&&(tkq.isDatNotNull))
									{
										_args[i] = tkq;
									}
									else
									{
										if(!tkq.isNumeroStringa)
											throw new Exception("Il token sullo stack non è un valore: errano numero di argomenti");
										else if(!tkq.isDatNotNull)
											throw new Exception("Il token sullo stack non è stato valutato");
									}
								}
								else
								{
									throw new Exception("Wrong argument numbers");
								}

							}
						}
						else
						{
							throw new Exception("Operatore non trovato");	
						}
					}
					else if(operatori.Contains(t.Testo,Operators.TipoOp.Funzione))
					{
						throw new NotImplementedException("Funzioni non ancora disponibili");
					}

				}
			}


			return _out;
		}
	
	}
}
