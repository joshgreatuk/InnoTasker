using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using InnoTasker.Data.Databases;
using InnoTasker.Services;
using InnoTasker.Services.ToDo;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using InnoTasker.Services.Interfaces.Admin;
using InnoTasker.Services.Admin;

namespace InnoTasker
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            Program program = new();
            AppDomain.CurrentDomain.ProcessExit += program.ExitSafely;
            AppDomain.CurrentDomain.UnhandledException += program.ExitSafely;
            Console.CancelKeyPress += program.ExitSafely;
            return program.MainAsync();
        }

        private static readonly string s_debugBotToken = "MTE0NTc1NjIwMDEzMTEwNDgyOQ.GzHfFV.VtddJE75PqW5FH6sWuNb373wZ04RPH510BefSI";
        private static readonly string s_releaseBotToken = "";

        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        private readonly Stopwatch programTimer;

        public Program()
        {
            programTimer = new Stopwatch();
            programTimer.Start();
            _services = BuildServiceProvider();
            _logger = _services.GetRequiredService<ILogger>();
        }

        public IServiceProvider BuildServiceProvider()
        {
            DiscordSocketConfig discordSocketConfig = new()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildScheduledEvents
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
                .AddSingleton<IGuildService, GuildService>()
                .AddSingleton<IToDoListService, ToDoListService>()
                .AddSingleton<IToDoForumService, ToDoForumService>()
                .AddSingleton<IToDoSettingsService, ToDoSettingsService>()
                .AddSingleton<IToDoUpdateService, ToDoUpdateService>()

                //Load AdminServiceData
                .AddSingleton<IAdminService, AdminService>()

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

            Task.WaitAll(
                _services.GetRequiredService<IToDoListService>().InitService(),
                _services.GetRequiredService<IToDoForumService>().InitService(),
                _services.GetRequiredService<IAdminService>().Init()
            );

            programTimer.Stop();
            await _logger.LogAsync(LogSeverity.Info, this, $"InnoTasker Initialized in {programTimer.ElapsedMilliseconds}ms! :)");

            await Task.Delay(-1);
        }

        public void ExitSafely(object? sender, EventArgs eventArgs)
        {
            programTimer.Restart();
            _logger.LogAsync(LogSeverity.Info, this, "Bot shutdown started");

            Task.WaitAll(
                _services.GetRequiredService<IToDoSettingsService>().Shutdown(),
                _services.GetRequiredService<IToDoListService>().Shutdown(),
                _services.GetRequiredService<IToDoForumService>().Shutdown(),
                _services.GetRequiredService<IAdminService>().Shutdown()
            );

            //Save anything that needs saving, etc
            List<IDatabase> toSave = new()
            {
                _services.GetRequiredService<GuildDatabase>(),
                _services.GetRequiredService<UserEmojiDatabase>()
            };
            foreach (IDatabase database in toSave) database.Save();

            //TO-DO: Save AdminService data

            programTimer.Stop();
            _logger.LogAsync(LogSeverity.Info, this, $"InnoTasker has shutdown successfully in {programTimer.ElapsedMilliseconds}ms <3");
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