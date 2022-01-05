using System;
using System.Collections;

namespace Universe
{
    public abstract class SetBase : UniverseScriptableObject, IEnumerable
    {
        public object this[int index]
        {
            get => List[index];

            set
            {
                List[index] = value;
            }
        }
        
        public int Count => List.Count;
        public bool Contains(object obj) => List.Contains(obj);
        public IEnumerator GetEnumerator() => List.GetEnumerator(); 
        
        public abstract IList List { get; }
        public abstract Type Type { get; }
    }
}