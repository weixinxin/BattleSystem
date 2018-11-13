using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LuaEngine
{
    internal static class Debug
    {
        private static ILogger log = null;

        internal static void InitLogger(ILogger logger)
        {
            log = logger;
        }
        internal static void Log(object message)
        {
            if (log != null)
                log.Log(message);
        }

        internal static void LogFormat(string format, params object[] args)
        {
            if (log != null)
                log.LogFormat(format, args);
        }

        internal static void LogWarning(object message)
        {
            if (log != null)
                log.LogWarning(message);
        }
        internal static void LogWarningFormat(string format, params object[] args)
        {
            if (log != null)
                log.LogWarningFormat(format, args);
        }
        internal static void LogError(object message)
        {
            if (log != null)
                log.LogError(message);
        }

        internal static void LogErrorFormat(string format, params object[] args)
        {
            if (log != null)
                log.LogErrorFormat(format, args);
        }
        internal static void LogException(Exception exception)
        {
            if (log != null)
                log.LogException(exception);
        }
    }
}
