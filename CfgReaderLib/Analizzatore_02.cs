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
	/// Classe che analizza una stringa
	/// Riconosce operatori, numeri di vario formato, parentesi, funzioni, virgole, stringhe
	/// Riconosce (usando il namespace Fred68.GenDictionary) anche i nomi di variabili da un dizionario generalizzato
	/// </summary>
	public partial class Analizzatore
	{
	
		/// <summary>
		/// Classe che analizza una stringa
		/// Parte 02: Riordina una lista di token da notazione infissa a notazione polacca inversa (RPN)
		/// Utilizza l'algoritmo Shunting yard: https://en.wikipedia.org/wiki/Shunting_yard_algorithm
		/// <param name="input">Lista di token di un'espressione</param>
		/// <returns>Coda di toker in RPN (notazione polacca inversa)</returns>
		public Queue<Token> RiordinaSY(List<Token> input)
		{
			Queue<Token> _out = new Queue<Token>();
			Stack<Token> _ops = new Stack<Token>();

			foreach(Token t in input)
			{
				if(t.isValore)
				{
					_out.Enqueue(t);
				}

				else if(t.isFunzione)
				{
					_ops.Push(t);	
				}

				else if(t.isOperatore)
				{								
					while(	(_ops.Count > 0) &&	(_ops.Peek().Tipo != Token.TipoTk.Parentesi_Aperta) )
					{									
						if(_ops.Peek().isOperatore)		// ...e c'é un operatore...
						{
							Operatori.Operatore? opAttuale = operatori[t.Testo];			// Cerca l'operatore attuale...
							Operatori.Operatore? opStack = operatori[_ops.Peek().Testo];	// ... e quello sullo stack
							
							if( (opAttuale != null) && (opStack != null) )
							{
								if(opStack.Precedenza >= opAttuale.Precedenza)
								{
									_out.Enqueue(_ops.Pop());
								}
								else
								{
									break;	// Finisce il ciclo while
								}
							}
						}
					} // Fine while
					_ops.Push(t);
				}

				else if(t.Tipo == Token.TipoTk.Separatore)
				{
					while(	(_ops.Count > 0) && (_ops.Peek().Tipo != Token.TipoTk.Parentesi_Aperta) )
					{
						_out.Enqueue(_ops.Pop());		
					}
				}

				else if(t.Tipo == Token.TipoTk.Parentesi_Aperta)	
				{
					_ops.Push(t);	
				}

				else if(t.Tipo == Token.TipoTk.Parentesi_Chiusa)
				{
					if(_ops.Count == 0)
					{
						throw new Exception("[RiordinaSY] Parentesi chiusa inaspettata");
					}
					while(	(_ops.Count > 0) &&	(_ops.Peek().Tipo != Token.TipoTk.Parentesi_Aperta) )
					{
						
					}

				}

			}

			return _out;
		}

	}
}
