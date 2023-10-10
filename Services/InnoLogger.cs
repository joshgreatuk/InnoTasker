using Discord;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services
{
    public class InnoLogger : ILogger
    {
        private static readonly bool s_logDebug = true;
        private static readonly int s_padSource = 20;

        public FileStream? logStream;
        public StreamWriter? streamWriter;

        public Timer logTimer;

        private Dictionary<Type, string> senderCache = new();

        public InnoLogger()
        {
            NewLogFile();
            logTimer = new Timer(NewLogFile, null, DateTime.Today.AddDays(23).AddHours(59).AddSeconds(59).TimeOfDay, TimeSpan.FromHours(24));
        }

        public void Log(LogMessage message)
        { Task.Run(() => LogAsync(message)); }
        public void Log(LogSeverity severity, object sender, string message, Exception? ex = null)
        { Log(new LogMessage(severity, SenderToString(sender), message, ex)); }

        public async Task LogAsync(LogSeverity severity, object sender, string message, Exception? ex = null)
        { await LogAsync(new LogMessage(severity, SenderToString(sender), message, ex)); }
        public async Task LogAsync(LogMessage message)
        {
            if (message.Severity is LogSeverity.Debug or LogSeverity.Verbose && !s_logDebug) return;

            //Log to file and console
            Console.WriteLine(message.ToString(padSource: s_padSource));
            if (streamWriter != null) streamWriter.WriteLine(message.ToString(padSource: s_padSource));
        }

        public void Shutdown(bool dispose = true)
        {
            LogAsync(LogSeverity.Info, this, "Log closed. Bye bye.");
            streamWriter.Close();
            logStream.Close();
            if (dispose) logTimer.Dispose();
        }

        public async void NewLogFile(object? sender = null)
        {
            if (logStream != null || streamWriter != null)
            {
                Shutdown(false);
            }

            string logName = $"{DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss")}.log";
            string logPath = Directory.GetCurrentDirectory() + "/Logs/";
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
            logPath += logName;
            logStream = File.OpenWrite(logPath);
            streamWriter = new(logStream);
            streamWriter.AutoFlush = true;

            await LogAsync(LogSeverity.Info, this, "Log opened! Hi :D");
        }

        public string SenderToString(object sender)
        {
            Type senderType = sender.GetType();
            if (senderCache.TryGetValue(senderType, out string senderString)) return senderString;

            if (!senderType.IsGenericType)
            {
                senderString = senderType.Name;
            }
            else
            {
                senderString = senderType.Name + $"<{String.Join(", ", senderType.GetGenericArguments().Select(x => x.Name))}>";
            }

            senderCache.Add(senderType, senderString);
            return senderString;
        }
    }
}
