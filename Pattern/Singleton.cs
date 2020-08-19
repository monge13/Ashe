using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ashe
{
    namespace Pattern
    {
        public class Singleton<T> where T : Singleton<T>, new()
        {
            private static T instance;
            public static T I
            {
                get
                {
                    if(instance == null)
                    {
                        instance = new T();
                    }
                    return instance;
                }
            }
        }
    }
}
