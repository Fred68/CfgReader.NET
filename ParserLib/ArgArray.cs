using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fred68.Parser
{
	public class ArgArray<T>
	{
        private T[] _arr;
        private int _nArgs;

        public ArgArray(int size)
        {
            _arr = new T[size];
            _nArgs = 0;
        }

        public T this[int index]
        {
            get => _arr[index];
            set => _arr[index] = value;
        }

        public int nArgs
        {
            get { return _nArgs; }
            set { _nArgs = value; }
        }
        public int Length => _arr.Length;

        public void Resize(int sz)
        {
            Array.Resize(ref _arr, (int)sz);
            _nArgs = 0;
        }
    
	}
}
