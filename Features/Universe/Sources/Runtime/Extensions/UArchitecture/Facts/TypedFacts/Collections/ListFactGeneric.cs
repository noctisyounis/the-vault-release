using System;
using System.Collections;
using System.Collections.Generic;

namespace Universe
{
	public class ListFactGeneric<T> : FactGeneric<List<T>>, IEnumerable<T>
	{
		#region Main

		public int Count => Value.Count;
		public virtual void Sort() => Value.Sort();

		
		public T this[int index]
		{
			get => Value[index];
			set => Value[index] = value;
		}
        
		public IList List => Value;
		public override Type Type => typeof(T);

		public virtual void Add(T obj)
		{
			if (Value.Contains(obj)) return;
            
			Value.Add(obj);
		}
        
		public virtual void Remove(T obj)
		{
			if (!Value.Contains(obj)) return;
            
			Value.Remove(obj);
		}
        
		public void Clear() => Value.Clear();
		public bool Contains(T value) => Value.Contains(value);
		public int IndexOf(T value) => Value.IndexOf(value);
		public void RemoveAt(int index) => Value.RemoveAt(index);
		public void Insert(int index, T value) => Value.Insert(index, value);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#pragma warning disable
		public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();
		public override string ToString() => "Collection<" + typeof(T) + ">(" + Count + ")";
		public T[] ToArray() => Value.ToArray();
		
		#endregion
	}
}
