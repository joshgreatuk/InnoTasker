using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces
{
    public interface ILogger
    {
        public void Log(LogMessage message);
        public void Log(LogSeverity severity, object source, string message, Exception? ex = null);

        public Task LogAsync(LogMessage message);
        public Task LogAsync(LogSeverity severity, object source, string message, Exception? ex = null);

        public void Shutdown(bool dispose=true);
    }
}
