using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace educa.core
{
    internal class ServiceImp
    {
        internal static void Start(ref Dictionary<Type, object> types)
        {
            string logServiceString = "Services active: ";
            string logServiceErrorString = null;
            for (int i = 0; i < types.Count; i++)
            {
                var e = types.ElementAt(i);
                logServiceString += "\n\t" + e.Key;
                try
                {
                    types[e.Key] = Activator.CreateInstance(e.Key);
                }
                catch (Exception es)
                {
                    Debug.LogError(new MissingMethodException(es.Message));
                    logServiceErrorString += "\n\t" + e.Value;
                }
            }
            Debug.Log(logServiceString);
            if (logServiceErrorString != null)
            {
                logServiceErrorString = "Services error: " + logServiceErrorString;
                Debug.LogError(logServiceErrorString);
            }
        }
    }
}
