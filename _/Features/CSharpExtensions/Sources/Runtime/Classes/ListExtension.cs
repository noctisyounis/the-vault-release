using System.Collections.Generic;

namespace Universe
{
    public static class ListExtension
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

        #endregion
    }
}