using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using Object = System.Object;

namespace mpz.core
{
    /// <summary>
    /// Service will contructor when application Initialize 
    /// </summary>
    public sealed class Service : Attribute
    {

        private Action onInit;

        public Service(Action OnInit)
        {
            this.onInit = OnInit;
        }

        public static T GetGlobal<T>()
        {
            var u = ServiceController.GetServiceByType<T>();
            Debug.Log(u);
            return (T)u;
        }

        internal static object GetGlobal(Type type)
        {
            Debug.Log(type);
            var u = ServiceController.GetServiceByType(type);
            Debug.Log(u);
            return u;
        }
    }
}
