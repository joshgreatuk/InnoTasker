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

            return new ServiceCollection()
                .AddSingleton<ILogger, InnoLogger>()
                .AddSingleton(new DiscordSocketClient(discordSocketConfig))
                .AddSingleton<InteractionService>()
                .AddSingleton<InteractionHandler>()
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
            _logger.Log(LogSeverity.Info, this, "InnoTasker has shutdown successfully <3");
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