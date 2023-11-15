using Discord;
using InnoTasker.Services.DiscordRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.Interfaces
{
    public interface IDiscordRequestService
    {
        public Task RequestMessageDeletion(DeleteRequest request);
        public Task<IUserMessage> RequestMessageModification(ModifyRequest request);
        public Task<IUserMessage> RequestMessageCreation(CreateRequest request);

        public Task Shutdown();
    }
}
