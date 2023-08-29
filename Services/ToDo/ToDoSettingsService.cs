using Discord;
using InnoTasker.Data;
using InnoTasker.Modules.Settings;
using InnoTasker.Services.Interfaces;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services.ToDo
{
    public class ToDoSettingsService : InnoServiceBase, IToDoSettingsService
    {
        private readonly IGuildService _guildService;
        private readonly IToDoListService _toDoListService;

        ISettingsPageBuilder toDoListMenuBuilder;
        public List<ISettingsPageBuilder> settingsPages;
        public List<ToDoSettingsInstance> toDoSettingsInstances = new();

        public ToDoSettingsService(ILogger logger, IGuildService guildService, IToDoListService toDoListService) : base(logger)
        {
            _guildService = guildService;
            _toDoListService = toDoListService;

            //Add settings pages
            toDoListMenuBuilder = new ToDoListMenuBuilder(_guildService);
            settingsPages = new()
            {

            };
        }

        public MessageContext GetToDoListPage(ulong interactionID) //Settings page should replace this menu, so should create an instance?
        {
            ToDoSettingsInstance instance = GetSettingsInstance(interactionID);
            return toDoListMenuBuilder.BuildPage();
        }

        public MessageContext GetSettingsPage(ulong interactionID, int index)
        {
            ToDoSettingsInstance instance = GetSettingsInstance(interactionID);
            instance.pageIndex = index;
            return GetSettingsPage(instance);
        }

        public MessageContext GetSettingsPage(ToDoSettingsInstance instance)
        {
            //Add back, close and forward buttons before sending
            ISettingsPageBuilder builder = settingsPages[instance.pageIndex];
            MessageContext page = builder.BuildPage();
            page.component.WithButton("<-", "settings-last")
                .WithButton("Close", "settings-close", ButtonStyle.Danger)
                .WithButton("->", "settings-next");
            return page;
        }

        public MessageContext GetNextSettingsPage(ulong interactionID)
        {
            ToDoSettingsInstance instance = GetSettingsInstance(interactionID);
            if (instance.pageIndex == settingsPages.Count - 1) return GetSettingsPage(instance);
            instance.pageIndex++;
            return GetSettingsPage(instance);
        }

        public MessageContext GetLastSettingsPage(ulong interactionID)
        {
            ToDoSettingsInstance instance = GetSettingsInstance(interactionID);
            if (instance.pageIndex == 0) return GetSettingsPage(instance);
            instance.pageIndex--;
            return GetSettingsPage(instance);
        }

        public ToDoSettingsInstance GetSettingsInstance(ulong interactionID)
        {
            ToDoSettingsInstance? instance = toDoSettingsInstances.FirstOrDefault(x => x.interactionID == interactionID);
            if (instance == null)
            {
                instance = new ToDoSettingsInstance(interactionID);
                toDoSettingsInstances.Add(instance);
            }
            return instance;
        }

        public void Shutdown() 
        { 
            //Close all settings instances and leave a message saying sorry :P

        }
    }
}
