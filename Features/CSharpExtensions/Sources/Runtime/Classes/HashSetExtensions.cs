using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class HashSetExtensions
{
    #region Main API

    public static T[] ToArray<T>(this HashSet<T> source)
    {
        var count = source.Count;
        var destination = new T[count];
        var i = 0;
        
        foreach (var entry in source)
        {
            destination[i] = entry;
            
            i++;
        }

        return destination;
    }

    public static void IntoArray<T>(this HashSet<T> source, ref T[] destination)
    {
        var count = source.Count;
        var i = 0;

        if (destination == null || destination.Length != count) 
            destination = new T[count];

        foreach (var entry in source)
        {
            destination[i] = entry;

            i++;
        }
    }
    
    #endregion
}
