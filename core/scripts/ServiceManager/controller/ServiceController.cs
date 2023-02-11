using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace educa.core
{
    public static class ServiceController
    {
        internal static Assembly[] assemblies;
        private static Dictionary<Type, object> objects;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            InitService();
            InitAutowire();

            #region local medthod
            void GetObjects()
            {
                for (int i = 0; i < assemblies.Length; i++)
                {
                    Assembly e = assemblies[i];
                    var ctypes = GetTypesWithServiceAttribute(e);
                    foreach (var ee in ctypes)
                    {
                        try
                        {
                            objects.Add(ee, null);
                        }
                        catch (Exception es)
                        {
                            Debug.LogError("[Error]: " + ee);
                        }
                    }
                }
            }

            void InitService()
            {
                objects = new Dictionary<Type, object>();
                GetObjects();
                ServiceImp.Start(ref objects);
            }

            void InitAutowire(){
            }

            void GetFieldInfo()
            {
                List<FieldInfo> results = new List<FieldInfo>();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    var list = GetTypesWithAutowireAttribute(assemblies[i]);
                }
            }

            #endregion
        }

        internal static object GetServiceByType(Type type)
        {
            try
            {
                return objects[type];
            }
            catch
            {
                return null;
            }
        }

        internal static object GetServiceByType<T>()
        {
            try
            {
                return objects[typeof(T)];
            }
            catch
            {
                return null;
            }
        }

        internal static List<Type> GetTypesWithServiceAttribute(Assembly assembly)
        {
            List<Type> types = new List<Type>();

            foreach (Type e in assembly.GetTypes())
            {
                if (e.GetCustomAttributes(typeof(Service), true).Length > 0)
                    types.Add(e);
            }

            return types;
        } 
        
        internal static List<FieldInfo> GetTypesWithAutowireAttribute(Assembly assembly)
        {
            List<FieldInfo> results = new List<FieldInfo>();

            foreach (Type e in assembly.GetTypes())
            {
                var properties = e.GetFields();
                foreach (var ee in properties)
                {
                    if (Attribute.IsDefined(e, typeof(Autowire)))
                    {
                        results.Add(ee);
                    }
                }
            }

            return results;
        }

    }

}
