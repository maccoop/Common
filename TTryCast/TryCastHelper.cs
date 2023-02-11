using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TryCastHelper
{
    public static bool TryCast<T>(this object obj, out T result)
    {
        if (obj is T)
        {
            result = (T)obj;
            return true;
        }

        result = default(T);
        return false;
    }
}
