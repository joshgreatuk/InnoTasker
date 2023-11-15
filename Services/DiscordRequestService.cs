using Discord;
using Discord.WebSocket;
using InnoTasker.Services.DiscordRequests;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services
{ 
    public class DiscordRequestService : InnoServiceBase, IDiscordRequestService
    {
        private readonly DiscordSocketClient _client;
        private bool shutdown = false;

        private Thread taskThread;

        private Queue<DiscordRequest> requestQueue = new();

        public DiscordRequestService(ILogger logger, DiscordSocketClient client) : base(logger) 
        {
            _client = client;
            _client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            shutdown = false;
            taskThread = new Thread(async () => await HandleRequestsLoop());
            await _logger.LogAsync(LogSeverity.Info, this, $"Initialized");
        }

        public async Task RequestMessageDeletion(DeleteRequest request)
        {

        }

        public async Task<IUserMessage> RequestMessageModification(ModifyRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<IUserMessage> RequestMessageCreation(CreateRequest request)
        {
            throw new NotImplementedException();
        }

        private async Task HandleRequestsLoop()
        {
            await _logger.LogAsync(LogSeverity.Info, this, $"Request Loop Started");
            while (!shutdown)
            {
                if (requestQueue.Count !> 0)
                {
                    await Task.Yield();
                }

                //Integrate rate limiting
            }
            await _logger.LogAsync(LogSeverity.Info, this, $"Request Loop Shutdown");
        }

        public async Task Shutdown()
        {
            //Shutdown requests thread
            shutdown = true;
            while (taskThread != null && taskThread.IsAlive) await Task.Yield();
            await _logger.LogAsync(LogSeverity.Info, this, $"Shutdown Achieved");
        }
    }
}
