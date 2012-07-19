using System;
using System.IO;

namespace HeroWars.Tools
{
    static class Log
    {
        public static LoggingLevel LogLevel { get; set; }
        public static string LogFile { get; set; }

        static Log()
      {
#if DEBUG
         LogLevel = LoggingLevel.High;
#else
         LogLevel = LoggingLevel.Low;
#endif
         try
         {
             System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
             LogFile = Path.GetDirectoryName(a.Location) + "\\" + a.FullName.Split(',')[0] + ".log";
         }
         catch (Exception x)
         {
             throw new LogInitializationException("Error while inizializing Log. Can not get executable file name", x);
         }
         try
         {
             StreamWriter file = new StreamWriter(LogFile, true);
             file.WriteLine("\r\n\r\n=== [Starting log: " + DateTime.Now + "] ===");
             file.Close();
         }
         catch (Exception x)
         {
             throw new LogInitializationException("Error while inizializing Log. Can not write to logfile", x);
         }
      }

        public static void WriteDebug(string msg)
        {
            if (LogLevel != LoggingLevel.High) return;

            try
            {
                StreamWriter file = new StreamWriter(LogFile, true);

                file.WriteLine("[" + DateTime.Now + "][DEBUG] " + msg);
                file.Close();
            }
            catch (Exception x)
            {
                throw new WriteToLogException("Error while writing to log.", msg, x);
            }
        }
        public static void WriteError(string msg)
        {
            if (LogLevel == LoggingLevel.Off) return;
            try
            {
                using (StreamWriter file = new StreamWriter(LogFile, true))
                {

                    file.WriteLine("[" + DateTime.Now + "][ERROR] " + msg);
                    file.Close();
                }
            }
            catch (Exception x)
            {
                throw new WriteToLogException("Error while writing to log", msg, x);
            }
        }
        public static void Clear()
        {
            using (StreamWriter file = new StreamWriter(LogFile, false))
            {
                file.Write("");
                file.Close();
            }
        }
    }
    public enum LoggingLevel
    {
        Off,
        Low,
        High
    }

    public class WriteToLogException : Exception
    {
        public WriteToLogException(string errorMessage)
            : base(errorMessage) { }

        public string IncomingMessage { get; set; }
        public WriteToLogException(string errorMessage, string incommingMessage, Exception innerEx)
            : base(errorMessage, innerEx)
        {
            IncomingMessage = incommingMessage;
        }
    }
    public class LogInitializationException : Exception
    {
        public LogInitializationException(string errorMessage)
            : base(errorMessage) { }

        public LogInitializationException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx)
        { }
    }
}
