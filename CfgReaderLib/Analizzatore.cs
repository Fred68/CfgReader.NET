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


#warning Scrivere 1: Analisi di un'espressione (vd. https://www.youtube.com/watch?v=wrj3iuRdA-M ): numeri, stringhe, operatori, variabili, parentesi
#warning Scrivere 2: Shunting yard per trasformare espressione in RPN (notazione polacca inversa) in coda e stack 
#warning Scrivere 3: Valutazione di un'espressione in RPN inserita in coda e stack


namespace Fred68.Parser
{
	
	/// <summary>
	/// Classe che analizza una stringa
	/// Riconosce operatori, numeri di vario formato, parentesi, funzioni, virgole, stringhe
	/// Riconosce (usando il namespace Fred68.GenDictionary) anche i nomi di variabili da un dizionario generalizzato
	/// </summary>
	public class Analizzatore
	{
		static Operatori operatori;
		
		#if !_LU_TABLES_EXTENSION

		// Caratteri vuoti da ignorare
		static CharLuTable chtSpazi;

		// Caratteri per i numeri
		static CharLuTable chtNumeri;
		static CharLuTable chtNumeriReali;
		static CharLuTable chtHex;
		static CharLuTable chtBin;
		
		// Caratteri per gli operatori (letti dalla classe)
		static CharLuTable chtOperatori;
		
		// Caratteri per variabili, funzioni e parole chiave
		static CharLuTable chtNomi;
		#endif
		
		#warning La notazione scientifica (es.: 2.4E+3) può essere trasfornmata in sequenza di token distinti, da riconoscere successivamente.
		
		/// <summary>
		/// static Ctor
		/// </summary>
		static Analizzatore()
		{
			operatori = new Operatori();

			#if _LU_TABLES_EXTENSION
			StringExtension.StringExtension.AddCharLuTable("Spazi","\t \n\r\v\f");
			StringExtension.StringExtension.AddCharLuTable("Numeri","0123456789");
			StringExtension.StringExtension.AddCharLuTable("NumeriReali","0123456789.");	// Aggiungere 'e' ed 'E' per notaz. scientifica
			StringExtension.StringExtension.AddCharLuTable("Operatori","+*-/");				// "!$%^&*+-=#@?|`/\\<>~"
			StringExtension.StringExtension.AddCharLuTable("Nomi","abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			StringExtension.StringExtension.AddCharLuTable("Hex","0123456789abcdefABCDEF");
			StringExtension.StringExtension.AddCharLuTable("Bin","01");
			#else

			chtSpazi = new CharLuTable("\t \n\r\v\f");
			chtNumeri = new CharLuTable("0123456789");
			chtNumeriReali = new CharLuTable("0123456789.");
			chtHex = new CharLuTable("0123456789abcdefABCDEF");
			chtBin = new CharLuTable("01");
			chtOperatori = new CharLuTable(operatori.UsedCharactes());	//	"!$%^&*+-=#@?|`/\\<>~"
			chtNomi = new CharLuTable("abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
			
			#endif
			
		}

		public Analizzatore()
		{}
		
		//public List<Token> Analizza1(string input)
		//{
		//	List<Token> tokens = new List<Token>();					// Lista dei Token da restituire
						
		//	Token.TkStat statTk = Token.TkStat.TokenNuovo;			// Stato attuale della macchina a stati...
		//	Token.TkStat statTkNew = Token.TkStat.TokenNuovo;		// e nuovo stato per il prossimo carattere
			
		//	Token tkAttuale = new Token();							// Nuovo token, inizializzato per sicurezza 
		//	StringBuilder strTkAttuale = new StringBuilder();		// Testo del token attuale
			
		//	bool bPuntoDecimale = false;							// Flag: ha già trovato il punto decimale
		//	int nParentesi = 0;										// Contatore di parentesi
		//	int nBlocchi = 0;										// Contatore di blocchi
			
		//	if(input.Length < 1)									// Verifica iniziale
		//		throw new Exception("[Analizza] Stringa da analizzare nulla.");

			
		//	for(int i=0; i<input.Length;)							// Percorre tutti i caratteri (come ciclo while)
		//	{
		//		switch(statTk)
		//		{
		//			case Token.TkStat.TokenNuovo:						// Se inizia un nuovo token:...
		//			{
		//				strTkAttuale.Clear();						// Azzera tutto
		//				//tkAttuale = new Token();					// Nuovo token indefinito (restituito così se l'analisi è incompleta).
		//				bPuntoDecimale = false;

		//				if(input[i].isIn(chtSpazi))					// Se il carattere è uno spazio...				
		//				{
		//					i++;										// Passa al carattere successivo e...
		//					statTkNew = Token.TkStat.TokenNuovo;           // ...mantiene lo stato precedente
		//				}
		//				else if(input[i].isIn(chtNumeriReali))		// Se il carattere è di un numero reale...
		//				{
		//					if(input[i] == '0')							// Se inizia per '0': può essere decimale, binario o esadecimale
		//					{
		//						statTkNew = Token.TkStat.NumeroIndef;   // Imposta lo stato non definito (decimale, esadecimale o binario)
		//					}
		//					else
		//					{                                           // Se no...
		//						statTkNew = Token.TkStat.Numero;			// Imposta lo stato in Numero
		//					}
		//				}
		//				else if(input[i].isIn(chtOperatori))		// Operatore...
		//				{
		//					statTkNew= Token.TkStat.Operatore;				
		//				}
		//				else if(input[i] == '(')					// Parentesi / blocchi
		//				{
		//					statTkNew = Token.TkStat.ParentesiAperta;
		//				}
		//				else if(input[i] == ')')
		//				{
		//					statTkNew = Token.TkStat.ParentesiChiusa;
		//				}
		//				else if(input[i] == '{')
		//				{
		//					statTkNew = Token.TkStat.BloccoAperto;
		//				}
		//				else if(input[i] == '}')
		//				{
		//					statTkNew = Token.TkStat.BloccoChiuso;
		//				}
		//				else if(input[i] == ',')					// Separatore / fine comando
		//				{
		//					statTkNew = Token.TkStat.Separatore;
		//				}
		//				else if(input[i] == ';')
		//				{
		//					statTkNew = Token.TkStat.FineComando;
		//				}
		//				else if(input[i] == '\"')					// Stringa (inizio...)
		//				{
		//					i++;										// Passa al carattere successivo e...
		//					statTkNew = Token.TkStat.Stringa;			// ...imposta lo stato
		//				}
		//				else
		//				{											// Non identificato: può essere una stringa simbolica generica
		//					#warning VERIFICARE: linee probabilmente superflue, mantenere solo statTkNew= Token.TkStat.Simbolo;
		//					strTkAttuale.Append(input[i]);				// Lo memorizza e passa al carattere successivo
		//					i++;
		//					statTkNew= Token.TkStat.Simbolo;
		//				}

		//			}
		//			break;

		//			case Token.TkStat.Stringa:						// Se sta leggendo una stringa:...
		//			{
		//				if(input[i] == '\"')							// Se fine stringa: token completato...
		//				{
		//					i++;
		//					statTkNew = Token.TkStat.TokenCompletato;
		//					tkAttuale = new Token(Token.Tipo.Stringa,strTkAttuale.ToString());
		//				}
		//				else
		//				{												// ...se no: memorizza il carattere.
		//					strTkAttuale.Append(input[i]);
		//					i++;
		//				}
		//			}
		//			break;

		//			case Token.TkStat.Numero:						// Se sta leggendo un numero reale
		//			{
		//				if(input[i].isIn(chtNumeriReali))				// Se cifra di un numero decimale...
		//				{
		//					if(input[i] == '.')							// Verifica il punto decimale
		//					{
		//						if(bPuntoDecimale)
		//						{
		//							throw new Exception("[Analizza] Punto decimale doppio.");
		//						}
		//						else
		//						{
		//							bPuntoDecimale = true;
		//						}
		//					}

		//					strTkAttuale.Append(input[i]);				// Memorizza il carattere e prosegue la lettura
		//					i++;
		//					statTkNew= Token.TkStat.Numero;
		//				}
		//				else
		//				{                                               // Se altro carattere...
		//					if(input[i].isIn(chtNomi))                  // Non deve essere un carattere di un nome (lettera o altro...)
		//					{
		//						throw new Exception("[Analizza] Numero reale con carattere errato.");
		//					}
		//					else
		//					{											// Se ok, termina il token, ma non incrementa (i++)
		//						statTkNew = Token.TkStat.TokenCompletato;
		//						tkAttuale = new Token(Token.Tipo.Numero,strTkAttuale.ToString());
		//						//char xxx = input[i];
		//					}
		//				}
		//			}
		//			break;

		//			case Token.TkStat.NumeroIndef:					// Se sta leggendo il numero ancora indefinito (il primo carattere è '0')...
		//			{
		//				if(input[i] == 'x')								// Riconosciuto secondo carattere: formato esadecimale...
		//				{
		//					strTkAttuale.Append((char)input[i]);		// Memorizza
		//					i++;										// Passa al carattere successivo
		//					statTkNew = Token.TkStat.Esadecimale;		// Imposta esadecimale
		//				}
		//				else if(input[i] == 'b')						// Idem con formato binario
		//				{
		//					strTkAttuale.Append((char)input[i]);
		//					i++;
		//					statTkNew = Token.TkStat.Binario;
		//				}
		//				else if(input[i].isIn(chtNumeriReali))			// Se non riconosciuto il secondo carattere speciale:...
		//				{												// ...cambia solo lo stato, ma non incrememta.
		//					statTkNew= Token.TkStat.Numero;				// Al prossimo ciclo for lo analizzerà normalmente
		//				}
		//				else
		//				{
		//					throw new Exception("[Analizza] Carattere errato dopo lo zero.");	
		//				}
		//			}
		//			break;

		//			case Token.TkStat.Esadecimale:					// Se sta leggendo un numero esadecimale
		//			{
		//				if(input[i].isIn(chtHex))						// Se cifra di un numero esadecimale
		//				{
		//					strTkAttuale.Append(input[i]);				// Memorizza il carattere e prosegue la lettura
		//					i++;
		//					statTkNew = Token.TkStat.Esadecimale;
		//				}
		//				else
		//				{
		//					if(input[i].isIn(chtNomi))					// Non deve essere un carattere di un nome (lettera o altro...)
		//					{
		//						throw new Exception("[Analizza] Numero esadecimale con carattere errato.");
		//					}
		//					else
		//					{											// Se ok, termina il token, ma non incrementa (i++)
		//						statTkNew = Token.TkStat.TokenCompletato;
		//						tkAttuale = new Token(Token.Tipo.Esadecimale,strTkAttuale.ToString());
		//					}	
		//				}
		//			}
		//			break;

		//			case Token.TkStat.Binario:						// Se sta leggendo un numero binario...
		//			{
		//				if(input[i].isIn(chtBin))						// Se cifra di un numero binario
		//				{
		//					strTkAttuale.Append(input[i]);				// Memorizza il carattere e prosegue la lettura
		//					i++;
		//					statTkNew = Token.TkStat.Binario;
		//				}
		//				else
		//				{
		//					if(input[i].isIn(chtNomi))					// Non deve essere un carattere di un nome (lettera o altro...)
		//					{
		//						throw new Exception("[Analizza] Numero binario con carattere errato.");
		//					}
		//					else
		//					{											// Se ok, termina il token, ma non incrementa (i++)
		//						statTkNew = Token.TkStat.TokenCompletato;
		//						tkAttuale = new Token(Token.Tipo.Binario,strTkAttuale.ToString());
		//					}	
		//				}
		//			}
		//			break;

		//			case Token.TkStat.Operatore:					// Se sta leggendo un operatore...
		//			{
		//				if(input[i].isIn(chtOperatori))					// Carattere di operatore:...
		//				{															// La stringa attuale + il carattere operatore...
		//					if(operatori.Contains(strTkAttuale.ToString()+input[i].ToString()))		// ...è ancora un operatore valido ?							{
		//					{																		
		//						strTkAttuale.Append(input[i]);						// Sì: memorizza il carattere
		//						i++;
		//					}
		//					else
		//					{														// Se no, controlla la stringa attuale da sola.
		//						if(operatori.Contains(strTkAttuale.ToString()))		// Se è un operatore, lo memorizza...			
		//						{													// ...ma mantiene il carattere letto per il prossimo ciclo
		//							statTkNew = Token.TkStat.TokenCompletato;
		//							tkAttuale = new Token(Token.Tipo.Operatore,strTkAttuale.ToString());
		//						}
		//						else
		//						{
		//							strTkAttuale.Append(input[i]);					// Se non è un operatore: memorizza il carattere
		//							i++;	
		//						}
		//					}
		//				}
		//				else
		//				{													// Carattere non di operatore:...
		//					if(operatori.Contains(strTkAttuale.ToString()))		// Se la stringa attuale è un operatore, lo memorizza
		//					{
		//						statTkNew = Token.TkStat.TokenCompletato;
		//						tkAttuale = new Token(Token.Tipo.Operatore,strTkAttuale.ToString());
		//					}
		//					else
		//					{													// Se non è riconosciuto: errore
		//						throw new Exception("[Analizza] Operatore non riconosciuto.");	
		//					}
		//				}
		//			}
		//			break;

		//			case Token.TkStat.ParentesiAperta:				// Se sta leggendo una parentesi aperta: crea subito un token
		//			{
		//				strTkAttuale.Append(input[i]);
		//				i++;										// Passa al carattere successivo
		//				nParentesi++;								// Conta le parentesi
		//				statTkNew = Token.TkStat.TokenCompletato;
		//				tkAttuale = new Token(Token.Tipo.Parentesi_Aperta,strTkAttuale.ToString());

		//			}
		//			break;

		//			case Token.TkStat.ParentesiChiusa:				// Idem con parentesi chiusa
		//			{
		//				strTkAttuale.Append(input[i]);
		//				i++;
		//				nParentesi--;
		//				statTkNew = Token.TkStat.TokenCompletato;
		//				tkAttuale = new Token(Token.Tipo.Parentesi_Chiusa,strTkAttuale.ToString());

		//			}
		//			break;

		//			case Token.TkStat.BloccoAperto:					// Idem con inizio blocco
		//			{
		//				strTkAttuale.Append(input[i]);
		//				i++;										
		//				nBlocchi++;								
		//				statTkNew = Token.TkStat.TokenCompletato;
		//				tkAttuale = new Token(Token.Tipo.Blocco_Aperto,strTkAttuale.ToString());

		//			}
		//			break;

		//			case Token.TkStat.BloccoChiuso:					// Idem con fine blocco
		//			{
		//				strTkAttuale.Append(input[i]);
		//				i++;
		//				nBlocchi--;
		//				statTkNew = Token.TkStat.TokenCompletato;
		//				tkAttuale = new Token(Token.Tipo.Blocco_Chiuso,strTkAttuale.ToString());
		//			}
		//			break;

		//			case Token.TkStat.Separatore:					// Idem con separatore
		//			{
		//				strTkAttuale.Append(input[i]);
		//				i++;
		//				statTkNew = Token.TkStat.TokenCompletato;
		//				tkAttuale = new Token(Token.Tipo.Separatore,strTkAttuale.ToString());
		//			}
		//			break;

		//			case Token.TkStat.FineComando:					// Idem con fine comando
		//			{
		//				strTkAttuale.Append(input[i]);
		//				i++;
		//				statTkNew = Token.TkStat.TokenCompletato;
		//				tkAttuale = new Token(Token.Tipo.Fine_Comando,strTkAttuale.ToString());
		//			}
		//			break;

		//			case Token.TkStat.Simbolo:
		//			{
		//				if(input[i].isIn(chtNomi))
		//				{
		//					strTkAttuale.Append(input[i]);
		//					i++;
		//				}
		//				else
		//				{
		//					#warning AGGIUNGERE ricerca in dizionari di variabili, funzioni e parole chiave
		//					tkAttuale = new Token(Token.Tipo.Simbolo,strTkAttuale.ToString());	// Per ora considerato simbolo generico	
							
		//					statTkNew = Token.TkStat.TokenCompletato;
		//				}
		//			}
		//			break;
					
		//			case Token.TkStat.TokenCompletato:			// Token completato: lo aggiunge alla lista e inizia con ricerca di nuovo token
		//			{
		//				tokens.Add(tkAttuale);
		//				statTkNew = Token.TkStat.TokenNuovo;
		//			}
		//			break;

		//			default:
		//			{
		//				throw new Exception("[Analizza] Token non riconosciuto in switch...case.");	
		//			}
		//		} // Fine switch(...)

				

		//		statTk = statTkNew;								// Aggiorna lo stato

		//		if( i == input.Length )							// Ultimo ciclo
		//			{
		//				Console.WriteLine($"Ultimo ciclo\nStat: {statTk.ToString()} Token: {tkAttuale.ToString()}");	
		//				//tokens.Add(tkAttuale);
						
		//			}
		//	} // Fine ciclo for(...)
			
		//	//tokens.Add(tkAttuale);
			
		//	if(nParentesi != 0)		throw new Exception("[Analizza] Parentesi () non bilanciate.");
		//	if(nBlocchi != 0)		throw new Exception("[Analizza] Blocchi {} non bilanciati.");
		//	if(statTk == Token.TkStat.Stringa)	throw new Exception("[Analizza] Manca un fine stringa \".");

		//return tokens;
		//}

		public List<Token> Analizza(string input)
		{
			List<Token> tokens = new List<Token>();					// Lista dei Token da restituire
						
			Token.TkStat statTk = Token.TkStat.TokenNuovo;			// Stato attuale della macchina a stati...
			Token.TkStat statTkNew = Token.TkStat.TokenNuovo;		// e nuovo stato per il prossimo carattere
			
			Token tkAttuale = new Token();							// Nuovo token, inizializzato per sicurezza 
			StringBuilder strTkAttuale = new StringBuilder();		// Testo del token attuale
			
			bool bPuntoDecimale = false;							// Flag: ha già trovato il punto decimale
			int nParentesi = 0;										// Contatore di parentesi
			int nBlocchi = 0;										// Contatore di blocchi
			
			if(input.Length < 1)									// Verifica iniziale
				throw new Exception("[Analizza] Stringa da analizzare nulla.");

			
			for(int i=0; i<input.Length+1;)							// Percorre tutti i caratteri (come ciclo while) ed uno aggiuntivo
			{
				char ch = (i < input.Length) ? input[i] : ' ';		// Legge il carattere, se entro il limite
				switch(statTk)
				{
					case Token.TkStat.TokenNuovo:						// Se inizia un nuovo token:...
					{
						strTkAttuale.Clear();						// Azzera tutto
						//tkAttuale = new Token();					// Nuovo token indefinito (restituito così se l'analisi è incompleta).
						bPuntoDecimale = false;

						if(ch.isIn(chtSpazi))					// Se il carattere è uno spazio...				
						{
							i++;										// Passa al carattere successivo e...
							statTkNew = Token.TkStat.TokenNuovo;           // ...mantiene lo stato precedente
						}
						else if(ch.isIn(chtNumeriReali))			// Se il carattere è di un numero reale...
						{
							if(ch == '0')							// Se inizia per '0': può essere decimale, binario o esadecimale
							{
								statTkNew = Token.TkStat.NumeroIndef;   // Imposta lo stato non definito (decimale, esadecimale o binario)
								strTkAttuale.Append(ch);
								i++;
							}
							else
							{                                           // Se no...
								statTkNew = Token.TkStat.Numero;			// Imposta lo stato in Numero
							}
						}
						else if(ch.isIn(chtOperatori))		// Operatore...
						{
							statTkNew= Token.TkStat.Operatore;				
						}
						else if(ch == '(')					// Parentesi / blocchi
						{
							statTkNew = Token.TkStat.ParentesiAperta;
						}
						else if(ch == ')')
						{
							statTkNew = Token.TkStat.ParentesiChiusa;
						}
						else if(ch == '{')
						{
							statTkNew = Token.TkStat.BloccoAperto;
						}
						else if(ch == '}')
						{
							statTkNew = Token.TkStat.BloccoChiuso;
						}
						else if(ch == ',')					// Separatore / fine comando
						{
							statTkNew = Token.TkStat.Separatore;
						}
						else if(ch == ';')
						{
							statTkNew = Token.TkStat.FineComando;
						}
						else if(ch == '\"')					// Stringa (inizio...)
						{
							i++;										// Passa al carattere successivo e...
							statTkNew = Token.TkStat.Stringa;			// ...imposta lo stato
						}
						else
						{											// Non identificato: può essere una stringa simbolica generica
							#warning VERIFICARE: linee probabilmente superflue, mantenere solo statTkNew= Token.TkStat.Simbolo;
							strTkAttuale.Append(ch);				// Lo memorizza e passa al carattere successivo
							i++;
							statTkNew= Token.TkStat.Simbolo;
						}

					}
					break;

					case Token.TkStat.Stringa:						// Se sta leggendo una stringa:...
					{
						if(ch == '\"')							// Se fine stringa: token completato...
						{
							i++;
							statTkNew = Token.TkStat.TokenCompletato;
							tkAttuale = new Token(Token.Tipo.Stringa,strTkAttuale.ToString());
						}
						else
						{												// ...se no: memorizza il carattere.
							strTkAttuale.Append(ch);
							i++;
						}
					}
					break;
					
					case Token.TkStat.NumeroIndef:					// Se sta leggendo il numero ancora indefinito (il primo carattere è '0')...
					{
						if(ch == 'x')								// Riconosciuto secondo carattere: formato esadecimale...
						{
							strTkAttuale.Append((char)ch);		// Memorizza
							i++;										// Passa al carattere successivo
							statTkNew = Token.TkStat.Esadecimale;		// Imposta esadecimale
						}
						else if(ch == 'b')						// Idem con formato binario
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

					case Token.TkStat.Numero:						// Se sta leggendo un numero reale
					{
						if(ch.isIn(chtNumeriReali))				// Se cifra di un numero decimale...
						{
							if(ch == '.')							// Verifica il punto decimale
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
						else
						{                                               // Se altro carattere...
							if(ch.isIn(chtNomi))                  // Non deve essere un carattere di un nome (lettera o altro...)
							{
								throw new Exception("[Analizza] Numero reale con carattere errato.");
							}
							else
							{											// Se ok, termina il token, ma non incrementa (i++)
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.Tipo.Numero,strTkAttuale.ToString());
								//char xxx = ch;
							}
						}
					}
					break;

					case Token.TkStat.Esadecimale:					// Se sta leggendo un numero esadecimale
					{
						if(ch.isIn(chtHex))						// Se cifra di un numero esadecimale
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
							{											// Se ok, termina il token, ma non incrementa (i++)
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.Tipo.Esadecimale,strTkAttuale.ToString());
							}	
						}
					}
					break;

					case Token.TkStat.Binario:						// Se sta leggendo un numero binario...
					{
						if(ch.isIn(chtBin))						// Se cifra di un numero binario
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
							{											// Se ok, termina il token, ma non incrementa (i++)
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.Tipo.Binario,strTkAttuale.ToString());
							}	
						}
					}
					break;

					case Token.TkStat.Operatore:					// Se sta leggendo un operatore...
					{
						if(ch.isIn(chtOperatori))					// Carattere di operatore:...
						{															// La stringa attuale + il carattere operatore...
							if(operatori.Contains(strTkAttuale.ToString()+ch.ToString()))		// ...è ancora un operatore valido ?							{
							{																		
								strTkAttuale.Append(ch);						// Sì: memorizza il carattere
								i++;
							}
							else
							{														// Se no, controlla la stringa attuale da sola.
								if(operatori.Contains(strTkAttuale.ToString()))		// Se è un operatore, lo memorizza...			
								{													// ...ma mantiene il carattere letto per il prossimo ciclo
									statTkNew = Token.TkStat.TokenCompletato;
									tkAttuale = new Token(Token.Tipo.Operatore,strTkAttuale.ToString());
								}
								else
								{
									strTkAttuale.Append(ch);					// Se non è un operatore: memorizza il carattere
									i++;	
								}
							}
						}
						else
						{													// Carattere non di operatore:...
							if(operatori.Contains(strTkAttuale.ToString()))		// Se la stringa attuale è un operatore, lo memorizza
							{
								statTkNew = Token.TkStat.TokenCompletato;
								tkAttuale = new Token(Token.Tipo.Operatore,strTkAttuale.ToString());
							}
							else
							{													// Se non è riconosciuto: errore
								throw new Exception("[Analizza] Operatore non riconosciuto.");	
							}
						}
					}
					break;

					case Token.TkStat.ParentesiAperta:				// Se sta leggendo una parentesi aperta: crea subito un token
					{
						strTkAttuale.Append(ch);
						i++;										// Passa al carattere successivo
						nParentesi++;								// Conta le parentesi
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.Tipo.Parentesi_Aperta,strTkAttuale.ToString());

					}
					break;

					case Token.TkStat.ParentesiChiusa:				// Idem con parentesi chiusa
					{
						strTkAttuale.Append(ch);
						i++;
						nParentesi--;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.Tipo.Parentesi_Chiusa,strTkAttuale.ToString());

					}
					break;

					case Token.TkStat.BloccoAperto:					// Idem con inizio blocco
					{
						strTkAttuale.Append(ch);
						i++;										
						nBlocchi++;								
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.Tipo.Blocco_Aperto,strTkAttuale.ToString());

					}
					break;

					case Token.TkStat.BloccoChiuso:					// Idem con fine blocco
					{
						strTkAttuale.Append(ch);
						i++;
						nBlocchi--;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.Tipo.Blocco_Chiuso,strTkAttuale.ToString());
					}
					break;

					case Token.TkStat.Separatore:					// Idem con separatore
					{
						strTkAttuale.Append(ch);
						i++;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.Tipo.Separatore,strTkAttuale.ToString());
					}
					break;

					case Token.TkStat.FineComando:					// Idem con fine comando
					{
						strTkAttuale.Append(ch);
						i++;
						statTkNew = Token.TkStat.TokenCompletato;
						tkAttuale = new Token(Token.Tipo.Fine_Comando,strTkAttuale.ToString());
					}
					break;

					case Token.TkStat.Simbolo:
					{
						if(ch.isIn(chtNomi))
						{
							strTkAttuale.Append(ch);
							i++;
						}
						else
						{
							#warning AGGIUNGERE ricerca in dizionari di variabili, funzioni e parole chiave
							tkAttuale = new Token(Token.Tipo.Simbolo,strTkAttuale.ToString());	// Per ora considerato simbolo generico	
							
							statTkNew = Token.TkStat.TokenCompletato;
						}
					}
					break;
					
					case Token.TkStat.TokenCompletato:			// Token completato: lo aggiunge alla lista e inizia con ricerca di nuovo token
					{
						tokens.Add(tkAttuale);
						statTkNew = Token.TkStat.TokenNuovo;
					}
					break;

					default:
					{
						throw new Exception("[Analizza] Token non riconosciuto in switch...case.");	
					}
				} // Fine switch(...)

				statTk = statTkNew;								// Aggiorna lo stato

			} // Fine ciclo for(...)  
			
			//tokens.Add(tkAttuale);
			
			if(nParentesi != 0)		throw new Exception("[Analizza] Parentesi () non bilanciate.");
			if(nBlocchi != 0)		throw new Exception("[Analizza] Blocchi {} non bilanciati.");
			if(statTk == Token.TkStat.Stringa)	throw new Exception("[Analizza] Manca un fine stringa \".");

		return tokens;
		}

	}
}
