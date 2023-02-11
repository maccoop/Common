using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Object = System.Object;

namespace educa.core
{
    /// <summary>
    /// Auto ref to service was contructor
    /// </summary>
    public sealed class Autowire : Attribute
    {
        public Autowire()
        {

        }
    }
}
