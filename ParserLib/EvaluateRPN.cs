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
using static Fred68.Parser.Parser;
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
			ArgArray<Token> _args = new ArgArray<Token>(ini_arg_array_sz);	// Array temporaneo con gli argomenti

			#warning Ammesse solo funzioni a numero fisso di argomenti [per ora].
			#warning Se numero variabile, analizzare il conteggio tra le parentesi e rinominare le funzioni con un numero.

			foreach(Token t in input)
			//while(input.Count > 0) 
			{	
				//Token t = input.Dequeue();
				if(t.isNumeroStringa)
				{
					t.ValutaVal();
					_stack.Push(t);
				}
				else if(t.isOperatoreFunzione)
				{
					if(operatori.Contains(t.Testo,Operators.TipoOp.Indefinito,true))		// Funzione o operatore speciale
					{
						Operators.OpBase? op = operatori[t.Testo,Operators.TipoOp.Indefinito];	// Ottiene operatore o funzione
						if( op != null)
						{
							int nargs = op.Argomenti;					// Numero di argomento dell'operatore
							if(nargs > _args.Length)					// Ridimensiona l'array, se necessario
							{
								_args.Resize(nargs);
							}
							_args.nArgs = nargs;
							for(int i=0; i < nargs;	i++)				// Estrae il numero di argomenti dalla coda di input.
							{
								if(input.Count > 0)
								{
									Token tkq = _stack.Pop();

									// Se l'argomento è stringa o numero già valutato (una variabile viene trasformata in valore)...
									if((tkq.isNumeroStringa)&&(tkq.isDatNotNull)) 
									{
										_args[i] = tkq;					// ...lo inserisce nell'array degli argomenti
									}

									// Se l'operatore è un'assegnazione
									else if(t.Testo == Operators.strAssign)	
									{
										if(tkq.isSimbolo)				// Se l'argomento è un simbolo (non valutato)..
										{
											_args[i] = tkq;				// ...lo inserisce nell'array degli argomenti
										}
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
							_stack.Push(tslv);				// Rimette il risultato nello stack
						}
						else
						{
							throw new Exception("Operatore o funzione non trovato");	
						}
					}
				}
				else if(t.isVariabile)
				{
					bool ok = t.ValutaVar(variabili);
					if(ok)
					{
						_stack.Push(t);	
					}
					else
					{
						throw new Exception("Errore nella valutazione della variabile");
					}
				}
				else if(t.isSimbolo)
				{
					_stack.Push(t);		
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
	
	}
}
