using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
//	using System.Linq;
//	using System.Text;
//	using System.Threading.Tasks;
//	using System.Reflection;

namespace Fred68.GenDictionary
{

	/// <summary>
	/// Tipi di dati trattati
	/// </summary>
	public enum TypeVar
	{
		INT,
		STR,
		BOOL,
		FLOAT,
		DOUBLE,
		DATE,
		COLOR,				// ARGB
		None				// Ultimo 
	}

	/// <summary>
	/// Classe Dat: oggetto generico con associato il tipo di dato.
	/// La classe non è generica, per poter esser contenuta in un unico raccoglitore
	/// </summary>
	public class Dat
	{

		static TypeVar[] _tc;
		static IFormatProvider _frm;

		TypeVar _t;				// Tipo di dato
		object _obj;			// Oggetto
		bool _list;				// true se l'oggetto è una lista

		static Dat()
			{
			_tc = new TypeVar[((int)TypeVar.None)-1];
			_frm = CultureInfo.InvariantCulture;
			}

		public static Type GetEqType(dynamic x)
			{
			return x.GetType();
			
			}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="t"></param>
		/// <param name="_d"></param>
		public Dat(int _d)
			{
			_t = TypeVar.INT;
			_obj = _d;
			_list = false;
			}
		public Dat(List<int> _d)
			{
			_t = TypeVar.INT;
			_obj = _d;
			_list = true;
			}
		public Dat(string _d)
			{
			_t = TypeVar.STR;
			_obj = _d;
			_list = false;
			}
		public Dat(List<string> _d)
			{
			_t = TypeVar.STR;
			_obj = _d;
			_list = true;
			}
		public Dat(bool _d)
			{
			_t = TypeVar.BOOL;
			_obj = _d;
			_list = false;
			}
		public Dat(List<bool> _d)
			{
			_t = TypeVar.BOOL;
			_obj = _d;
			_list = true;
			}		
		public Dat(float _d)
			{
			_t = TypeVar.FLOAT;
			_obj = _d;
			_list = false;
			}
		public Dat(List<float> _d)
			{
			_t = TypeVar.FLOAT;
			_obj = _d;
			_list = true;
			}
		public Dat(double _d)
			{
			_t = TypeVar.DOUBLE;
			_obj = _d;
			_list = false;
			}
		public Dat(List<double> _d)
			{
			_t = TypeVar.DOUBLE;
			_obj = _d;
			_list = true;
			}
		public Dat(DateTime _d)
			{
			_t = TypeVar.DATE;
			_obj = _d;
			_list = false;
			}
		public Dat(List<DateTime> _d)
			{
			_t = TypeVar.DATE;
			_obj = _d;
			_list = true;
			}
		
		/// <summary>
		/// Dat contains a list
		/// </summary>
		public bool IsList
			{
			get { return _list; }
			}

		/// <summary>
		/// Restituisce l'oggetto, riconvertito al tipo di dato originario.
		/// La dichiarazione è dynamic, per avere un'unica funzione Get
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public dynamic Get()
		{
			switch(_t)
			{
				case TypeVar.INT:
				{
					if(_list)
						return (List<int>)_obj;
					else
						return (int)_obj;
				}
					//break;
				case TypeVar.STR:
				{
					if(_list)
						return (List<string>)_obj;
					else
						return (string)_obj;
				}
					//break;
				case TypeVar.BOOL:
				{
					if(_list)
						return (List<bool>)_obj;
					else
						return (bool)_obj;
				}
					//break;
				case TypeVar.FLOAT:
				{
					if(_list)
						return (List<float>)_obj;
					else
						return (float)_obj;
				}
					//break;
				case TypeVar.DOUBLE:
				{
					if(_list)
						return (List<double>)_obj;
					else
						return (double)_obj;
				}
					//break;
				case TypeVar.DATE:
				{
					if(_list)
						return (List<DateTime>)_obj;
					else
						return (DateTime)_obj;
				}
				
				default:
					throw new NotImplementedException("Tipo dato non definito.");
			}
		}

		/// <summary>
		/// Reimposta l'oggetto con un nuovo tipo di dato
		/// </summary>
		/// <param name="_d"></param>
		public void Set(int _d)
			{
			_t = TypeVar.INT;
			_obj = _d;
			_list = false;
			}
		public void Set(List<int> _d)
			{
			_t = TypeVar.INT;
			_obj = _d;
			_list = true;
			}
		public void Set(string _d)
			{
			_t = TypeVar.STR;
			_obj = _d;
			_list = false;
			}
		public void Set(List<string> _d)
			{
			_t = TypeVar.STR;
			_obj = _d;
			_list = true;
			}
		public void Set(bool _d)
			{
			_t = TypeVar.BOOL;
			_obj = _d;
			_list = false;
			}
		public void Set(List<bool> _d)
			{
			_t = TypeVar.BOOL;
			_obj = _d;
			_list = true;
			}		
		public void Set(float _d)
			{
			_t = TypeVar.FLOAT;
			_obj = _d;
			_list = false;
			}
		public void Set(List<float> _d)
			{
			_t = TypeVar.FLOAT;
			_obj = _d;
			_list = true;
			}
		public void Set(double _d)
			{
			_t = TypeVar.DOUBLE;
			_obj = _d;
			_list = false;
			}
		public void Set(List<double> _d)
			{
			_t = TypeVar.DOUBLE;
			_obj = _d;
			_list = true;
			}
		public void Set(DateTime _d)
			{
			_t = TypeVar.DATE;
			_obj = _d;
			_list = false;
			}
		public void Set(List<DateTime> _d)
			{
			_t = TypeVar.DATE;
			_obj = _d;
			_list = true;
			}





		/// <summary>
		/// TypeVar
		/// </summary>
		public TypeVar Type	{ get { return _t; } }

		/// <summary>
		/// Override di ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(false);
		}

		// -> static CultureInfo cultureInfo = System.Globalization.CultureInfo.InvariantCulture;	// Cultura (per la conversione)
		//static CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("it-IT");		// Cultura (per la conversione)
		// -> static DateTimeStyles dtStyles = DateTimeStyles.None;								// Stili di conversione
		/// <summary>
		/// ToString(bool)
		/// </summary>
		/// <param name="expandList">Expand list element into output string</param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public string ToString(bool expandList) 
		{
			StringBuilder sb = new StringBuilder();
			if(_list )
			{
				switch(_t)
				{
				case TypeVar.INT:
					{
						List<int> l = (List<int>)_obj;
						sb.Append($"{_t.ToString()}[{l.Count.ToString()}]");
						if(expandList)
						{	
							sb.Append('{');
							for(int i=0; i<l.Count; i++)
							{
								sb.Append(l[i].ToString(_frm) + ((i < l.Count-1) ? ' ' : '}'));
							}
						}
					}
					break;
				case TypeVar.STR:
					{
						List<string> l = (List<string>)_obj;
						sb.Append($"{_t.ToString()}[{l.Count.ToString()}]");
						if(expandList)
						{	
							sb.Append('{');
							for(int i=0; i<l.Count; i++)
							{
								sb.Append(l[i] + ((i < l.Count-1) ? ' ' : '}'));
							}
							sb.Append('}');
						}
					}
					break;
				case TypeVar.BOOL:
					{
						List<bool> l = (List<bool>)_obj;
						sb.Append($"{_t.ToString()}[{l.Count.ToString()}]");
						if(expandList)
						{	
							sb.Append('{');
							for(int i=0; i<l.Count; i++)
							{
								sb.Append(l[i].ToString(_frm) + ((i < l.Count-1) ? ' ' : '}'));
							}
							sb.Append('}');
						}
					}
					break;
				case TypeVar.FLOAT:
					{
						List<float> l = (List<float>)_obj;
						sb.Append($"{_t.ToString()}[{l.Count.ToString()}]");
						if(expandList)
						{	
							sb.Append('{');
							for(int i=0; i<l.Count; i++)
							{
								sb.Append(l[i].ToString(_frm) + ((i < l.Count-1) ? ' ' : '}'));
							}
							sb.Append('}');
						}
					}
					break;
				case TypeVar.DOUBLE:
					{
						List<double> l = (List<double>)_obj;
						sb.Append($"{_t.ToString()}[{l.Count.ToString()}]");
						if(expandList)
						{	
							sb.Append('{');
							for(int i=0; i<l.Count; i++)
							{
								sb.Append(l[i].ToString(_frm) + ((i < l.Count-1) ? ' ' : '}'));
							}
							sb.Append('}');
						}
					}
					break;
				case TypeVar.DATE:
					{
						List<DateTime> l = (List<DateTime>)_obj;
						sb.Append($"{_t.ToString()}[{l.Count.ToString()}]");
						if(expandList)
						{	
							sb.Append('{');
							for(int i=0; i<l.Count; i++)
							{
								sb.Append(l[i].ToString(_frm) + ((i < l.Count-1) ? ' ' : '}'));
							}
							sb.Append('}');
						}
					}
					break;
				default:
					throw new NotImplementedException("Tipo dato non definito.");
				}

			}
			else
			{
				switch(_t)
				{
				case TypeVar.INT:
					{
						sb.Append(((int)_obj).ToString(_frm));
					}
					break;
				case TypeVar.STR:
					{
						sb.Append((string)_obj);
					}
					break;
				case TypeVar.BOOL:
					{
						sb.Append(((bool)_obj).ToString(_frm));
					}
					break;
				case TypeVar.FLOAT:
					{
						sb.Append(((float)_obj).ToString(_frm));
					}
					break;
				case TypeVar.DOUBLE:
					{
						sb.Append(((double)_obj).ToString(_frm));
					}
					break;
				case TypeVar.DATE:
					{
						sb.Append(((DateTime)_obj).ToString(_frm));
					}
					break;
				default:
					throw new NotImplementedException("Tipo dato non definito.");
				}
			
			}
		return sb.ToString();
		}
	}
}
