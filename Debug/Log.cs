using System;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ashe
{
    public class D
    {
        [Conditional("_LOG_ENABLED")]
        public static void Assert(bool flag, string message = "assertion failed")
        {
            if (!flag)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Assert", message, "OK");
#endif
                Log.E(message);
            }
        }

        public class Log
        {
            [Conditional("_LOG_ENABLED")]
            public static void I (string message)
            {
                UnityEngine.Debug.Log(message);
            }

            [Conditional("_LOG_ENABLED")]
            public static void W(string message)
            {
                UnityEngine.Debug.Log(message);
            }

            [Conditional("_LOG_ENABLED")]
            public static void E(string message)
            {
                UnityEngine.Debug.Log(message);
            }

#if _LOG_ENABLED
            class LogInfo
            {
                public enum TYPE
                {
                    INFO,
                    WARNING,
                    ERROR,
                }
                public DateTime time = default;
                public TYPE type = TYPE.INFO;
                public string message = default;
            }
            List<LogInfo> logInfos = new List<LogInfo>();
#endif
        }

    }
}
