using InnoTasker.Data;
using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Settings
{
    public class ToDoListMenuBuilder : ISettingsPageBuilder
    {
        private readonly IGuildService _guildService;

        public ToDoListMenuBuilder(IGuildService guildService)
        {
            _guildService = guildService;
        }

        public MessageContext BuildPage()
        {
            throw new NotImplementedException();
        }
    }
}
