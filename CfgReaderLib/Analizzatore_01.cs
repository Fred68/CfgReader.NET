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


// SimplyCode/OneLoneCoder_DIYLanguage_Tokenizer.cpp

namespace Fred68.Parser
{
	
	/// <summary>
	/// Classe che analizza una stringa
	/// Parte 01: trasforma un'espressione in una lista di token
	/// Riconosce operatori, numeri di vario formato, parentesi, funzioni, virgole, stringhe
	/// Riconosce (usando il namespace Fred68.GenDictionary) anche i nomi di variabili da un dizionario generalizzato
	/// Un ringraziamento a One Lone Coder https://github.com/OneLoneCoder
	/// Algoritmo copiato da: https://github.com/OneLoneCoder/Javidx9/blob/master/SimplyCode/OneLoneCoder_DIYLanguage_Tokenizer.cpp
	/// </summary>
	public partial class Analizzatore
	{
		
		/// <summary>
		/// Analizza una stringa con formule
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public List<Token> Analizza(string input)
		{
			List<Token> tokens = new List<Token>();					// Lista dei Token da restituire
						
			Token.TkStat statTk = Token.TkStat.TokenNuovo;			// Stato attuale della macchina a stati...
			Token.TkStat statTkNew = Token.TkStat.TokenNuovo;		// e nuovo stato per il prossimo carattere
			
			Token tkAttuale = new Token();							// Nuovo token, inizializzato per sicurezza 
			StringBuilder strTkAttuale = new StringBuilder();		// Testo del token attuale
			
			bool bPuntoDecimale = false;							// Ha trovato il punto decimale
			Token.TipoNum tpNum = Token.TipoNum.Indefinito;			// Suffisso per il tipo di numero (float o double)
			int nParentesi = 0;										// Contatore di parentesi
			int nBlocchi = 0;										// Contatore di blocchi
			
			if(input.Length < 1)									// Verifica iniziale
				throw new Exception("[Analizza] Stringa da analizzare nulla.");

			
			for(int i=0; i<input.Length+1;)							// Percorre tutti i caratteri (come ciclo while) ed uno aggiuntivo
			{
				char ch = (i < input.Length) ? input[i] : Operatori.chSpazio;	// Legge il carattere, se entro il limite
				switch(statTk)
				{
					/*********************************************************/
					case Token.TkStat.TokenNuovo:					// Se inizia un nuovo token:...
					{
						strTkAttuale.Clear();						// Azzera tutto
						//tkAttuale = new Token();					// Nuovo token indefinito (restituito così se l'analisi è incompleta).
						bPuntoDecimale = false;
						tpNum = Token.TipoNum.Indefinito;

						if(ch.isIn(chtSpazi))						// Se il carattere è uno spazio...				
						{
							i++;									// Passa al carattere successivo e...
							statTkNew = Token.TkStat.TokenNuovo;    // ...mantiene lo stato precedente
						}
						else if(ch.isIn(chtNumeriReali))			// Se il carattere è di un numero reale...
						{
							if(ch == Operatori.chZero)				// Se inizia per '0': può essere decimale, binario o esadecimale
							{
								statTkNew = Token.TkStat.NumeroIndef;   // Imposta lo stato non definito (decimale, esadecimale o binario)
								strTkAttuale.Append(ch);
								i++;
							}
							else
							{										// Se no...
								statTkNew = Token.TkStat.Numero;	// Imposta lo stato in Numero
							}
						}
						else if(ch.isIn(chtOperatori))				// Operatore...
						{
							statTkNew= Token.TkStat.Operatore;				
						}
						else if(ch == Operatori.chParentesiAperta)		// Parentesi / blocchi
						{
							statTkNew = Token.TkStat.ParentesiAperta;
						}
						else if(ch == Operatori.chParentesiChiusa)
						{
							statTkNew = Token.TkStat.ParentesiChiusa;
						}
						else if(ch == Operatori.chGraffaAperta)
						{
							statTkNew = Token.TkStat.BloccoAperto;
						}
						else if(ch == Operatori.chGraffaChiusa)
						{
							statTkNew = Token.TkStat.BloccoChiuso;
						}
						else if(ch == Operatori.chVirgola)				// Separatore
						{
							statTkNew = Token.TkStat.Separatore;
						}
						else if(ch == Operatori.chPuntoVirgola)			// fine comando
						{
							statTkNew = Token.TkStat.FineComando;
						}
						else if(ch == Operatori.chStringaInizio)		// Stringa (inizio...)
						{
							i++;										// Passa al carattere successivo e...
							statTkNew = Token.TkStat.Stringa;			// ...imposta lo stato
						}
						else
						{											// Non identificato: può essere una stringa simbolica generica
							strTkAttuale.Append(ch);				// Lo memorizza e passa al carattere successivo
							i++;
							statTkNew= Token.TkStat.Simbolo;		// Imposta lo stato come simbolo generico
						}

					}
					break;
					/*********************************************************/
					case Token.TkStat.Stringa:						// Se sta leggendo una stringa:...
					{
						if(ch == Operatori.chStringaFine)			// Se fine stringa: token completato...
						{
							i++;
							statTkNew = Token.TkStat.TokenCompletato;
							tkAttuale = new Token(Token.TipoTk.Stringa,strTkAttuale.ToString());
						}
						else
						{											// ...se no: memorizza il carattere.
							strTkAttuale.Append(ch);
							i++;
						}
					}
					break;
					/*********************************************************/					
					case Token.TkStat.NumeroIndef:					// Se sta leggendo il numero ancora indefinito (il primo carattere è '0')...
					{
						if(ch == Operatori.chHex)					// Riconosciuto secondo carattere: formato esadecimale...
						{
							strTkAttuale.Append((char)ch);			// Memorizza
							i++;										// Passa al carattere successivo
							statTkNew = Token.TkStat.Esadecimale;		// Imposta esadecimale
						}
						else if(ch == Operatori.chBin)					// Idem con formato binario
						{
							strTkAttuale.Append((char)ch);
							i++;
							statTkNew = Token.TkStat.Binario;
						}
						else if(ch.isIn(chtNumeriReali))			// Se non riconosciuto il secondo carattere speciale:...
						{												// ...cambia solo lo stato, ma non incrememta.
							statTkNew= Token.TkStat.Numero;				// Al prossimo ciclo for lo analizzerà normalmente
						}
						else
						{
							throw new Exception("[Analizza] Carattere errato dopo lo zero.");	
						}
					}
					break;
					/*********************************************************/
					case Token.TkStat.Numero:						// Se sta leggendo un numero reale
					{
						if(ch.isIn(chtNumeriReali))					// Se cifra di un numero decimale...
						{
							if(ch == Operatori.chPuntoDecimale)		// Verifica il punto decimale
							{
								if(bPuntoDecimale)
								{
									throw new Exception("[Analizza] Punto decimale doppio.");
								}
								else
								{
									bPuntoDecimale = true;
								}
							}

							strTkAttuale.Append(ch);				// Memorizza il carattere e prosegue la lettura
							i++;
							statTkNew= Token.TkStat.Numero;
						}
						else if(									// Se suffisso di un numero in virgola mobile...
								(ch == Token.chSuffissoFloat) ||
								(ch == Token.chSuffissoDouble)
								)	
						{
							if(tpNum != Token.TipoNum.Indefinito)
							{
								throw new Exception("[Analizza] Suffisso di tipo di numero doppio.");
							}
							else
							{
								switch(ch)
								{
									case Token.chSuffissoFloat:
										tpNum = Token.TipoNum.Float;
									break;
									case Token.chSuffissoDouble:
										tpNum= Token.TipoNum.Double;
									break;
								}

							}
							i++;
						}
						else if(ch.isIn(chtNomi))					// Non deve essere un carattere di un nome (lettera o altro...)
						{
							throw new Exception("[Analizza] Numero reale con carattere errato.");
						}
						else
						{											// Se ok, termina il token, ma non incrementa (i++)
							tkAttuale = new Token(Token.TipoTk.Numero,tpNum,strTkAttuale.ToString());
							statTkNew = Token.TkStat.TokenCompletato;
							tpNum = Token.TipoNum.Indefinito;
							//char xxx = ch;
						}
						
					}
					break;
					/*********************************************************/
					case Token.TkStat.Esadecimale:					// Se sta leggendo un numero esadecimale
					{
						if(ch.isIn(chtHex))							// Se cifra di un numero esadecimale
						{
							strTkAttuale.Append(ch);				// Memorizza il carattere e prosegue la lettura
							i++;
							statTkNew = Token.TkStat.Esadecimale;
						}
						else
						{
							if(ch.isIn(chtNomi))					// Non deve essere un carattere di un nome (lettera o altro...)
							{
								throw new Exception("[Analizza] Numero esadecimale con carattere errato.");
							}
							else
							{										// Se ok, termina il token, ma non incrementa (i++)
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.TipoTk.Esadecimale,strTkAttuale.ToString());
							}	
						}
					}
					break;
					/*********************************************************/
					case Token.TkStat.Binario:						// Se sta leggendo un numero binario
					{
						if(ch.isIn(chtBin))							// Se cifra di un numero binario
						{
							strTkAttuale.Append(ch);				// Memorizza il carattere e prosegue la lettura
							i++;
							statTkNew = Token.TkStat.Binario;
						}
						else
						{
							if(ch.isIn(chtNomi))					// Non deve essere un carattere di un nome (lettera o altro...)
							{
								throw new Exception("[Analizza] Numero binario con carattere errato.");
							}
							else
							{										// Se ok, termina il token, ma non incrementa (i++)
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.TipoTk.Binario,strTkAttuale.ToString());
							}	
						}
					}
					break;
					/*********************************************************/
					case Token.TkStat.Operatore:					// Se sta leggendo un operatore...
					{
						if(ch.isIn(chtOperatori))					// Carattere di operatore:...
						{															// La stringa attuale + il carattere operatore...
							if(operatori.Contains(strTkAttuale.ToString()+ch.ToString()))		// ...è ancora un operatore valido ?							{
							{																		
								strTkAttuale.Append(ch);			// Sì: memorizza il carattere
								i++;
							}
							else
							{														// Se no, controlla la stringa attuale da sola.
								if(operatori.Contains(strTkAttuale.ToString()))		// Se è un operatore, lo memorizza...			
								{													// ...ma mantiene il carattere letto per il prossimo ciclo
									statTkNew = Token.TkStat.TokenCompletato;
									tkAttuale = new Token(Token.TipoTk.Operatore,strTkAttuale.ToString());
								}
								else
								{
									strTkAttuale.Append(ch);		// Se non è un operatore: memorizza il carattere
									i++;	
								}
							}
						}
						else
						{											// Carattere non di operatore:...
							if(operatori.Contains(strTkAttuale.ToString()))		// Se la stringa attuale è un operatore, lo memorizza
							{
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.TipoTk.Operatore,strTkAttuale.ToString());
							}
							else
							{													// Se non è riconosciuto: errore
								throw new Exception("[Analizza] Operatore non riconosciuto.");	
							}
						}
					}
					break;
					/*********************************************************/
					case Token.TkStat.ParentesiAperta:				// Se sta leggendo una parentesi aperta:...
					{
						strTkAttuale.Append(ch);					// ... crea subito un token
						i++;										// Passa al carattere successivo
						nParentesi++;								// Conta le parentesi
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.TipoTk.Parentesi_Aperta,strTkAttuale.ToString());

					}
					break;
					/*********************************************************/
					case Token.TkStat.ParentesiChiusa:				// Idem con parentesi chiusa
					{
						strTkAttuale.Append(ch);
						i++;
						nParentesi--;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.TipoTk.Parentesi_Chiusa,strTkAttuale.ToString());

					}
					break;
					/*********************************************************/
					case Token.TkStat.BloccoAperto:					// Idem con inizio blocco
					{
						strTkAttuale.Append(ch);
						i++;										
						nBlocchi++;								
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.TipoTk.Blocco_Aperto,strTkAttuale.ToString());

					}
					break;
					/*********************************************************/
					case Token.TkStat.BloccoChiuso:					// Idem con fine blocco
					{
						strTkAttuale.Append(ch);
						i++;
						nBlocchi--;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.TipoTk.Blocco_Chiuso,strTkAttuale.ToString());
					}
					break;
					/*********************************************************/
					case Token.TkStat.Separatore:					// Idem con separatore
					{
						strTkAttuale.Append(ch);
						i++;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.TipoTk.Separatore,strTkAttuale.ToString());
					}
					break;
					/*********************************************************/
					case Token.TkStat.FineComando:					// Idem con fine comando
					{
						strTkAttuale.Append(ch);
						i++;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.TipoTk.Fine_Comando,strTkAttuale.ToString());
					}
					break;
					/*********************************************************/
					case Token.TkStat.Simbolo:
					{
						if(ch.isIn(chtNomi))
						{
							strTkAttuale.Append(ch);
							i++;
						}
						else
						{
							#warning MANCA ricerca in dizionari di variabili e parole chiave

							if(funzioni.Contains(strTkAttuale.ToString().ToUpper()))
							{
								tkAttuale = new Token(Token.TipoTk.Funzione,strTkAttuale.ToString().ToUpper());
							}
							else
							{	// Se non roconisciuto: classificato come simbolo generico
								tkAttuale = new Token(Token.TipoTk.Simbolo,strTkAttuale.ToString());
							}
							statTkNew = Token.TkStat.TokenCompletato;
						}
					}
					break;
					/*********************************************************/					
					case Token.TkStat.TokenCompletato:			// Token completato: lo aggiunge alla lista e inizia con ricerca di nuovo token
					{
						tokens.Add(tkAttuale);
						statTkNew = Token.TkStat.TokenNuovo;
					}
					break;
					/*********************************************************/
					default:
					{

						throw new Exception("[Analizza] Token non riconosciuto in switch...case.");	
					}
				} // Fine switch(...)

				statTk = statTkNew;								// Aggiorna lo stato

			} // Fine ciclo for(...)  
			
			if(nParentesi != 0)		throw new Exception("[Analizza] Parentesi () non bilanciate.");
			if(nBlocchi != 0)		throw new Exception("[Analizza] Blocchi {} non bilanciati.");
			if(statTk == Token.TkStat.Stringa)	throw new Exception("[Analizza] Manca un fine stringa \".");

		return tokens;
		}

	
	}
}
