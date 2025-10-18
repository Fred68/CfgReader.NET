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
using static Fred68.Parser.Operators;

namespace Fred68.Parser
{
	/// <summary>
	/// Parte 02: Riordina una lista di token da notazione infissa a notazione polacca inversa (RPN)
	/// Utilizza l'algoritmo Shunting yard: https://en.wikipedia.org/wiki/Shunting_yard_algorithm
	/// Considera anche gli operatori unari + e -, cha hanno la stessa stringa degli operatori binari
	/// </summary>
	public partial class Parser
	{
	
		/// <summary>
		/// Classe che riordina in RPN una lista di token
		/// <param name="input">Lista di token di un'espressione</param>
		/// <returns>Coda di toker in RPN (notazione polacca inversa)</returns>
		public Queue<Token> ShuntingYardReorder(List<Token> input)
		{
			Queue<Token> _out = new Queue<Token>();		// Coda (di uscita)
			Stack<Token> _ops = new Stack<Token>();		// Pila (di operatori)

			int iCicli = 0;									// Contatore e token precedente...
			Token tkPrec = new Token(Token.TipoTk.Numero);	// ...usati per identificare operatori prefissi unari (es.: +5, -2)

			foreach(Token t in input)			// Percorre i token in ordine inverso rispetto a come sono stati inseriti nella coda
			{
				// --> Se è un valore numerico...
				if(t.isValore)					// Lo accoda
				{
					_out.Enqueue(t);
					tkPrec = t;
				}
				// --> Se è una funzione...
				else if(t.isFunzione)			// La impila
				{
					_ops.Push(t);
					tkPrec = t;
					
				}
				// --> Se è un operatore...
				else if(t.isOperatore)
				{
					#warning Aggiungere identificazione degli operatori unari postfissi (4++) ??? Forse superfluo...

					if(operatori.IsSpecial(t.Testo))	// Se il testo dell'operatore è unario speciale
					{
						
						if(	(iCicli == 0)					// Se è il primo token (es.: "-4+3") oppure se...
							||								// ...il token precedente non è né un numero né una stringa...
							(
								(!tkPrec.isNumeroStringa)	// per esempio: 1 + 2  oppure  "A" + "B"
								&&						
								(tkPrec.Tipo!=Token.TipoTk.Parentesi_Chiusa)	// ...e non è una parentesi chiusa...
							)								// per esempio: ) - 2  oppure  ) + "A"	oppure  1.2 E - 2
						   )
						{
							//Token tmp = t;
							t.RendiOperatoreSpeciale();		// ...allora modifica il testo in operatore unario speciale
							//tmp = t;
						}									// Le eventuali incompatibilità verranno analizzate dopo.


						
					}

					// Finché non trova una parentesi aperta...								
					while(	(_ops.Count > 0) &&	(_ops.Peek().Tipo != Token.TipoTk.Parentesi_Aperta) )
					{									
						if(_ops.Peek().isOperatore)		// ...e c'é un operatore...
						{
							Operators.Operator? opAttuale = (Operators.Operator?)operatori[t.Testo,Operators.TipoOp.Operatore];	// Cerca l'operatore attuale...
							Operators.Operator? opStack = (Operators.Operator?)operatori[_ops.Peek().Testo,Operators.TipoOp.Operatore];	// ... e quello sullo stack


							if( (opAttuale != null) && (opStack != null) )
							{
								if(opStack.Precedenza >= opAttuale.Precedenza)		// Confronta le precedenze
								{
									_out.Enqueue(_ops.Pop());						// Accoda i token tra le parentesi
									
								}
								else
								{
									break;	// Finisce il ciclo while
								}
							}
						}
					} // Fine while
					_ops.Push(t);				// Impila il token 
					tkPrec = t;
				}
				// --> Se è un separatore...
				else if(t.Tipo == Token.TipoTk.Separatore)
				{											// Finché non trova una parentesi aperta...	
					while(	(_ops.Count > 0) && (_ops.Peek().Tipo != Token.TipoTk.Parentesi_Aperta) )
					{
						_out.Enqueue(_ops.Pop());			// Accoda i token prendendolo dalla pila
						tkPrec = t;
					}
				}
				// --> Parentesi aperta... la impila
				else if(t.Tipo == Token.TipoTk.Parentesi_Aperta)	
				{
					_ops.Push(t);
					tkPrec = t;
					
				}
				// --> Parentesi chiusa... 
				else if(t.Tipo == Token.TipoTk.Parentesi_Chiusa)
				{
					if(_ops.Count == 0)
					{
						throw new Exception("[RiordinaSY] Parentesi chiusa inaspettata");
					}
					while(	(_ops.Count > 0) &&	(_ops.Peek().Tipo != Token.TipoTk.Parentesi_Aperta) )
					{
						_out.Enqueue(_ops.Pop());			// Accoda i token prendendolo dalla pila
						
					}

					if(_ops.Count == 0 )
					{
						throw new Exception("[RiordinaSY] Parentesi aperta mancante");	
					}
															// Scarta parentesi aperta rimasta sulla pila
					if( (_ops.Count > 0) && (_ops.Peek().Tipo == Token.TipoTk.Parentesi_Aperta) )
					{
						_ops.Pop();	
					}
					tkPrec = t;
				}
				iCicli++;
			}
			while(	_ops.Count > 0 )
				{
					if(_ops.Peek().Tipo == Token.TipoTk.Parentesi_Aperta)	
					{
						throw new Exception("[RiordinaSY] C'è una parentesi aperta di troppo");
					}
					_out.Enqueue(_ops.Pop());				// Accoda il resto della pila.
				}

			return _out;
		}

	}
}
