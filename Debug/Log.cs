using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ashe
{
    namespace Debug
    {
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
                public DateTime time;
                public TYPE type;
                public string message;
            }
            List<LogInfo> logInfos = new List<LogInfo>();
#endif
        }

    }
}
