using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpExtensions.Runtime
{
    public class EfficientList<T> : IEnumerable<T> where T : class
    {
        #region Public

        public T[] Items => _items;
        
        #endregion
        
        
        #region Constructor

        public EfficientList(int size)
        {
            _items = new T[size];
            _freeSlots = new Stack<int>(size);
            for (var i = size - 1; i >= 0; i--) _freeSlots.Push(i);
        }

        #endregion
        
        
        #region Enumerable

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i] != null)
                {
                    yield return _items[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        
        
        #region Main Methods

        public bool Add(T item)
        {
            if (_freeSlots.Count > 0)
            {
                _items[_freeSlots.Pop()] = item;
                return true;    
            }
            
            Debug.LogError($"[VAULT] EfficientArray<{typeof(T)}> tried to add an item on an already full EfficientArray");
            return false;
        }

        public bool Remove(T item)
        {
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i] != item) continue;
                
                _freeSlots.Push(i);
                _items[i] = null;
                return true;
            }
           
            Debug.LogError($"[VAULT] EfficientArray<{typeof(T)}> tried to remove an item that is not in the EfficentArray!!!!");
            return false;
        }

        #endregion
        
        
        #region Private And Protected

        private T[] _items;
        private Stack<int> _freeSlots;

        #endregion
    }
}