using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using InnoTasker.Data;
using InnoTasker.Services;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InnoTasker
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            Program program = new();
            AppDomain.CurrentDomain.ProcessExit += program.ExitSafely;
            return program.MainAsync();
        }

        private static readonly string s_debugBotToken = "MTE0NTc1NjIwMDEzMTEwNDgyOQ.GzHfFV.VtddJE75PqW5FH6sWuNb373wZ04RPH510BefSI";
        private static readonly string s_releaseBotToken = "";

        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public Program()
        {
            _services = BuildServiceProvider();
            _logger = _services.GetRequiredService<ILogger>();
        }

        public IServiceProvider BuildServiceProvider()
        {
            DiscordSocketConfig discordSocketConfig = new()
            {
                GatewayIntents = GatewayIntents.None
            };

            string basePath = $"{Directory.GetCurrentDirectory()}\\";
            string[] dataDirs = new string[]
            {
                "Data\\",
                "Data\\GuildData\\",
                "Data\\UserEmojis\\"
            };

            foreach (string dir in dataDirs)
            {
                if (!Directory.Exists(basePath + dir)) Directory.CreateDirectory(basePath + dir);
            }

            return new ServiceCollection()
                //Base Services
                .AddSingleton<ILogger, InnoLogger>()
                .AddSingleton(new DiscordSocketClient(discordSocketConfig))
                .AddSingleton<InteractionService>()
                .AddSingleton<InteractionHandler>()

                //Databases
                .AddSingleton(x => new GuildDatabase(x, $"{basePath}\\Data\\GuildData\\"))
                .AddSingleton(x => new UserEmojiDatabase(x, $"{basePath}\\Data\\UserEmojis\\"))

                //Bot Services


                .BuildServiceProvider();
        }

        public async Task MainAsync()
        {
            await _logger.LogAsync(LogSeverity.Info, this, "Initializing InnoTasker");
            InteractionHandler interactionHandler = _services.GetRequiredService<InteractionHandler>(); //Initialize to setup ready event

            DiscordSocketClient client = _services.GetRequiredService<DiscordSocketClient>();
            client.Log += _logger.LogAsync;

            await client.LoginAsync(TokenType.Bot, IsDebug() ? s_debugBotToken : s_releaseBotToken);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public void ExitSafely(object? sender, EventArgs eventArgs)
        {
            //Save anything that needs saving, etc
            List<IDatabase> toSave = new()
            {
                _services.GetRequiredService<GuildDatabase>(),
                _services.GetRequiredService<UserEmojiDatabase>()
            };

            foreach (IDatabase database in toSave) database.Save();

            _logger.LogAsync(LogSeverity.Info, this, "InnoTasker has shutdown successfully <3");
            _logger.Shutdown();
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}