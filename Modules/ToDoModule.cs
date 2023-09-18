﻿using Discord.Interactions;
using InnoTasker.Services.Interfaces.ToDo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules
{
    [Group("to-do", "Commands relating to a to-do list")]
    public class ToDoModule
    {
        private readonly IToDoUpdateService _updateService;

        public ToDoModule(IToDoUpdateService updateService)
        {
            _updateService = updateService;
        }
    }
}
