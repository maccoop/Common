using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace educa.core
{
    public class CoreMonoBehaviour : MonoBehaviour
    {
        public virtual void Awake()
        {
            var properties = GetType().GetFields();
            foreach (var e in properties)
            {
                if (Attribute.IsDefined(e, typeof(Autowire)))
                {
                    e.SetValue(this,Service.GetGlobal(e.FieldType));
                }
            }
        }
    }
}
