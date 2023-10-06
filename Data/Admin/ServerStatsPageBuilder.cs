using Discord;
using Discord.WebSocket;
using InnoTasker.Data.ToDo;
using InnoTasker.Modules.Settings;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Admin
{
    /* Server stats must include:
     * - CPU usage
     * - Memory usage
     * - Disk usage
     */
    public class ServerStatsPageBuilder : ISettingsPageBuilder
    {
        private readonly ILogger _logger;

        private Process process;

        private DriveInfo driveInfo;

        private DateTime lastUpdateTime;
        private TimeSpan lastCpuTime;

        public ServerStatsPageBuilder(ILogger logger)
        {
            _logger = logger;

            process = Process.GetCurrentProcess();
            lastUpdateTime = DateTime.UtcNow;
            lastCpuTime = process.TotalProcessorTime;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                driveInfo = new DriveInfo(Directory.GetCurrentDirectory().Split(":\\").First());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                driveInfo = new DriveInfo(Directory.GetCurrentDirectory());
            }
            else
            {
                logger.LogAsync(LogSeverity.Warning, this, $"OS not supported for disk information");
            }
        }

        public async Task<MessageContext> BuildPage(ToDoSettingsInstance settings)
        {
            DateTime nowTime = DateTime.UtcNow;
            TimeSpan nowCpuTime = process.TotalProcessorTime;

            double cpuUsedMs = (nowCpuTime - lastCpuTime).TotalMilliseconds;
            double totalMsPassed = (nowTime - lastUpdateTime).TotalMilliseconds;
            double cpuUsedTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            float cpuPercentage = MathF.Round((float)cpuUsedTotal * 100, 2);

            lastUpdateTime = DateTime.UtcNow;
            lastCpuTime = process.TotalProcessorTime;

            float takenSpaceGB = 0;
            float totalSpaceGB = 0;
            float takenSpacePercentage = 0;

            if (driveInfo != null) 
            {
                takenSpaceGB = BTGB(driveInfo.TotalSize - driveInfo.AvailableFreeSpace);
                totalSpaceGB = BTGB(driveInfo.TotalSize);
                takenSpacePercentage = MathF.Round((takenSpaceGB / totalSpaceGB) * 100, 2);
            }

            float takenMemoryGB = BTMB(process.PrivateMemorySize64);
            float totalMemoryGB = BTMB(process.WorkingSet64);
            float memoryPercentage = MathF.Round((takenMemoryGB / totalMemoryGB) * 100, 2);

            List<string> fields = new()
            {
                $"**CPU Usage:** {cpuPercentage}%",
                $"**Memory Usage:** {takenMemoryGB}MB / {totalMemoryGB}MB ({memoryPercentage}%)",
                $"**Disk Usage:** {(driveInfo != null ? $"{takenSpaceGB}GB / {totalSpaceGB}GB ({takenSpacePercentage}%)" : "Not Available")}",
                $"**Process Threads:** {process.Threads.Count}",
            };

            Embed embed = new EmbedBuilder()
                .WithTitle("Server Statistics")
                .WithDescription(String.Join("\n", fields))
                .WithCurrentTimestamp()
                .Build();
            return new MessageContext(embed, new ComponentBuilder());
        }

        private float BTMB(long bytes) => MathF.Round((float)bytes / 1024 / 1024, 2);
        private float BTGB(long bytes) => MathF.Round((float)bytes / 1024 / 1024 / 1024, 2);

        public async Task<MessageContext?> HandleInteraction(ToDoSettingsInstance settings, SocketInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanProceed(ToDoSettingsInstance settings) => true;
    }
}
