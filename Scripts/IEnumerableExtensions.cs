using System;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExensions 
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> toApply)
    {
        try
        {
            foreach (var i in collection)
                toApply(i);
        }
        catch(ArgumentNullException exc)
        {
            Debug.LogError(exc.Message);
        }
    }
}
