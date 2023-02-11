using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using Object = System.Object;

namespace educa.core
{
    /// <summary>
    /// Service will contructor when application Initialize 
    /// </summary>
    public sealed class Service : Attribute
    {
#if false
        public static T GetGlobal<T>()
        {
            var u = ServiceController.GetServiceByType<T>();
            Debug.Log(u);
            return (T)u;
#endif

        internal static object GetGlobal(Type type)
        {
            Debug.Log(type);
            var u = ServiceController.GetServiceByType(type);
            Debug.Log(u);
            return u;
        }
    }
}
