using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    namespace Pattern
    {
        public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
        {
            private static T instance = null;
            public static T I
            {
                get
                {
                    Debug.Assert(instance != null, "Instance is Null");
                    return instance;
                }
            }

            private void Awake()
            {
                if (instance == null)
                {
                    instance = this as T;
                    instance.Init();
                }
            }

            virtual protected void Init()
            {
            }
        }
    }
}