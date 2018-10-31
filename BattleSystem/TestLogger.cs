using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem
{
    public class TestLogger :ILogger
    {
        public void Log(object message)
        {
            Console.WriteLine(message.ToString());
        }

        public void LogFormat(string format, params object[] args)
        {

            Console.WriteLine(string.Format(format,args));
        }

        public void LogWarning(object message)
        {
            Console.WriteLine(message.ToString());
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        public void LogError(object message)
        {
            Console.WriteLine(message.ToString());
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        public void LogException(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}
