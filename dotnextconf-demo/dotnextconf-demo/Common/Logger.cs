namespace dotnextconf_demo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum LoggerState
    {
        Success = 1,
        Failure = 2,
        Warning = 3,
        Informational = 4
    }

    public static class Logger
    {
        private static void LogPrefix(LoggerState state)
        {
            if (state == LoggerState.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Success] ");
            }
            else if (state == LoggerState.Failure)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[Failure] ");
            }
            else if (state == LoggerState.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[Warning] ");
            }
            else if (state == LoggerState.Informational)
            {
                Console.Write("[Information] ");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Log(string message, LoggerState state = LoggerState.Informational)
        {
            LogPrefix(state);
            Console.WriteLine("{0} \n", message);
        }

        public static void Log(Exception ex, LoggerState state = LoggerState.Failure)
        {
            LogPrefix(state);
            Console.WriteLine("Exception, Type={0}\n", ex.GetType().Name);
        }

    }
}
