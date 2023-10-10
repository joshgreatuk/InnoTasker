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
    public class InnoTasker
    {
        public static Task Main(string[] args)
        {
            InnoTasker program = new();
            AppDomain.CurrentDomain.UnhandledException += program.ExitSafely;
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; program.ExitSafely(s, e); Environment.Exit(0); };
            return program.MainAsync();
        }

        private static readonly string s_debugBotToken = "MTE0NTc1NjIwMDEzMTEwNDgyOQ.GzHfFV.VtddJE75PqW5FH6sWuNb373wZ04RPH510BefSI";
        private static readonly string s_releaseBotToken = "";

        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        private readonly Stopwatch programTimer;

        public InnoTasker()
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

            string basePath = $"{Directory.GetCurrentDirectory()}/";
            string[] dataDirs = new string[]
            {
                "Data/",
                "Data/GuildData/",
                "Data/UserEmojis/"
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
                .AddSingleton(x => new GuildDatabase(x, $"{basePath}/Data/GuildData/"))
                .AddSingleton(x => new UserEmojiDatabase(x, $"{basePath}/Data/UserEmojis/"))
                .AddSingleton(x => new AdminPostDatabase(x, $"{basePath}/Data/AdminPosts/"))

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
            client.Ready += ClientReady;

            await client.LoginAsync(TokenType.Bot, IsDebug() ? s_debugBotToken : s_releaseBotToken);
            await client.StartAsync();

            programTimer.Stop();

            await Task.Delay(-1);
        }

        public async Task ClientReady()
        {
            programTimer.Start();

            Task.WaitAll(
                _services.GetRequiredService<GuildDatabase>().Init(),
                _services.GetRequiredService<UserEmojiDatabase>().Init(),
                _services.GetRequiredService<AdminPostDatabase>().Init()
             );

            Task.WaitAll(
                _services.GetRequiredService<IToDoListService>().InitService(),
                _services.GetRequiredService<IToDoForumService>().InitService(),
                _services.GetRequiredService<IAdminService>().Init()
            );

            programTimer.Stop();
            await _logger.LogAsync(LogSeverity.Info, this, $"InnoTasker Initialized in {programTimer.ElapsedMilliseconds}ms! :)");
        }

        public void ExitSafely(object? sender, EventArgs eventArgs)
        {
            programTimer.Restart();
            _logger.LogAsync(LogSeverity.Info, this, "Bot shutdown started");

            Task.WaitAll(
                TryTask(_services.GetRequiredService<IToDoSettingsService>().Shutdown()),
                TryTask(_services.GetRequiredService<IToDoListService>().Shutdown()),
                TryTask(_services.GetRequiredService<IToDoForumService>().Shutdown()),
                TryTask(_services.GetRequiredService<IAdminService>().Shutdown())
            );

            //Save anything that needs saving, etc
            List<IDatabase> toSave = new()
            {
                _services.GetRequiredService<GuildDatabase>(),
                _services.GetRequiredService<UserEmojiDatabase>(),
                _services.GetRequiredService<AdminPostDatabase>()
            };
            foreach (IDatabase database in toSave)
            {
                try
                {
                    database.Save();
                }
                catch (Exception ex)
                {
                    _logger.LogAsync(LogSeverity.Error, this, $"There was a problem saving database '{database.GetType().Name}'", ex);
                }
            }

            programTimer.Stop();
            _logger.LogAsync(LogSeverity.Info, this, $"InnoTasker has shutdown successfully in {programTimer.ElapsedMilliseconds}ms <3");
            _logger.Shutdown();
        }

        public async Task TryTask(Task action)
        {
            try
            {
                await action;
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, this, $"There was a problem trying a task", ex);
            }
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