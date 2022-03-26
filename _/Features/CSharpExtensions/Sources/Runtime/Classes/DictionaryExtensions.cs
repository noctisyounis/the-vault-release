using System.Collections.Generic;
using UnityEngine;

namespace Universe.DebugWatch.Runtime
{
    public static class DictionaryExtensions
    {
        #region Main

        public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> other)
        {
            foreach (var item in other)
            {
                if(!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
                else
                    Debug.LogError($"the key {item.Key} was already in the dictionary and didn't merged the value {item.Value}.");
            }
        }

        #endregion
    }
}