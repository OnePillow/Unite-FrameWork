using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unite.Framework
{
    public interface IDataCollection<T> : IList<T> {}
    public interface IUIDataCollection<T> : IDataCollection<T>
    {
        public enum DataStructType : int
        {
            
        }
        public List<T> Data { get; }
        public Transform dataParent { get; set; }
        public void UpdateData(object[] data, int index);
    }

    public class DataEnumerator<ElementType, DataType> : IEnumerator<DataType>
    {
        public delegate DataType GetCurrentDelegate(in IList<ElementType> list, in int index);
        private GetCurrentDelegate GetCurrent;

        public DataEnumerator(IList<ElementType> list, GetCurrentDelegate GetCurrent)
        {
            this.list = list;
            this.GetCurrent = GetCurrent;
        }

        protected IList<ElementType> list;
        protected int current = -1;
        public DataType Current
        {
            get
            {
                if (current < 0 || current > list.Count) return default(DataType);

                return GetCurrent.Invoke(list, current);
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            return ++current < list.Count;
        }

        public void Reset()
        {
        }

        public void Dispose()
        {
        }
    }
}
