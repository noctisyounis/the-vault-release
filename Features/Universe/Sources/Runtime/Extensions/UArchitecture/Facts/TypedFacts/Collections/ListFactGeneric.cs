using System;
using System.Collections;
using System.Collections.Generic;

namespace Universe
{
	public class ListFactGeneric<T> : FactGeneric<List<T>>, IEnumerable<T>
	{
		#region Main


		public List<T> SafeValue => Value ??= new();
		public int Count => SafeValue.Count;
		public virtual void Sort() => SafeValue.Sort();

		
		public T this[int index]
		{
			get => SafeValue[index];
			set => SafeValue[index] = value;
		}
        
		public IList List => SafeValue;
		public override Type Type => typeof(T);

		public virtual void Add(T obj)
		{
			if (SafeValue.Contains(obj)) return;
            
			SafeValue.Add(obj);
		}
        
		public virtual void Remove(T obj)
		{
			if (!SafeValue.Contains(obj)) return;
            
			SafeValue.Remove(obj);
		}
        
		public void Clear() => SafeValue.Clear();
		public bool Contains(T value) => SafeValue.Contains(value);
		public int IndexOf(T value) => SafeValue.IndexOf(value);
		public void RemoveAt(int index) => SafeValue.RemoveAt(index);
		public void Insert(int index, T value) => SafeValue.Insert(index, value);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#pragma warning disable
		public IEnumerator<T> GetEnumerator() => SafeValue.GetEnumerator();
		public override string ToString() => "Collection<" + typeof(T) + ">(" + Count + ")";
		public T[] ToArray() => SafeValue.ToArray();
		
		#endregion
	}
}
