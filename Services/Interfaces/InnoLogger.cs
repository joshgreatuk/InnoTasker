using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces
{
    public class InnoLogger : ILogger
    {
        private static readonly int s_padSource = 20;

        public FileStream logStream;
        public StreamWriter streamWriter;

        public Timer logTimer;

        public InnoLogger()
        {
            NewLogFile();
            logTimer = new Timer(NewLogFile, null, DateTime.Today.AddHours(24).TimeOfDay, TimeSpan.FromHours(24));
        }

        public void Log(LogMessage message) 
        { Task.Run(() => LogAsync(message)); }
        public void Log(LogSeverity severity, object sender, string message, Exception? ex = null) 
        { Log(new LogMessage(severity, sender.GetType().Name, message, ex)); }

        public async Task LogAsync(LogSeverity severity, object sender, string message, Exception? ex = null) 
        { await LogAsync(new LogMessage(severity, sender.GetType().Name, message, ex)); }
        public async Task LogAsync(LogMessage message)
        {
            //Log to file and console
            Console.WriteLine(message.ToString(padSource: s_padSource));
            if (streamWriter != null) streamWriter.WriteLine(message.ToString(padSource: s_padSource));
        }

        public void Shutdown(bool dispose=true)
        {
            Log(LogSeverity.Info, this, "Log closed. Bye bye.");
            streamWriter.Close();
            logStream.Close();
            if (dispose) logTimer.Dispose();
        }

        public void NewLogFile(object? sender=null)
        {
            if (logStream != null || streamWriter != null)
            {
                Shutdown(false);
            }

            string logName = $"{DateTime.Now.ToString("dd-MM-yyyy--T")}.log";
            string logPath = Directory.GetCurrentDirectory() + "\\Logs\\";
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
            logPath += logName;
            logStream = File.OpenWrite(logPath);
            streamWriter = new StreamWriter(logStream);
            streamWriter.AutoFlush = true;

            Log(LogSeverity.Info, this, "Log opened! Hi :D");
        }
    }
}
