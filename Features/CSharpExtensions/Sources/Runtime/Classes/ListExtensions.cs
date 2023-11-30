using System.Collections.Generic;

namespace Universe
{
    public static class ListExtensions
    {
        #region Main

        public static void Merge<T>(this List<T> source, List<T> other, int[] indexes)
        {
            var count = indexes.Length;

            foreach (var index in indexes)
            {
                if(index >= count || index < 0) continue;
                source.Add(other[index]);
            }
        }

        public static bool GreaterThan<T>( this List<T> source, int index )
        {
            var count = source.Count;

            if( index < 0 ) return false;
            if( index >= count ) return false;

            return true;
        }

        #endregion
    }
}