using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class SetGeneric<T> : SetBase, IEnumerable<T>
    {
        #region Exposed
        
        [SerializeField]
        private List<T> _list = new List<T>();
        
        #endregion
        
        
        #region Main
        
        public new T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        
        public override IList List => _list;
        public override Type Type => typeof(T);

        public void Add(T obj)
        {
            if (_list.Contains(obj)) return;
            
            _list.Add(obj);
        }
        
        public void Remove(T obj)
        {
            if (!_list.Contains(obj)) return;
            
            _list.Remove(obj);
        }
        
        public void Clear() => _list.Clear();
        public bool Contains(T value) => _list.Contains(value);
        public int IndexOf(T value) => _list.IndexOf(value);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Insert(int index, T value) => _list.Insert(index, value);
        public virtual void SortSet() => _list.Sort();
        public virtual void SortSetDescending() => _list.Sort();
        public void SortSet(Comparison<T> comparator) => _list.Sort(comparator);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #pragma warning disable
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        public override string ToString() => "Collection<" + typeof(T) + ">(" + Count + ")";
        public T[] ToArray() => _list.ToArray();
        
        #endregion
    }
}