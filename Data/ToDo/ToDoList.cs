﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.ToDo
{
    public class ToDoList
    {
        public string Name { get; set; }
        public ulong ListChannelID { get; set; }
        public ulong CommandChannelID { get; set; }
        public ulong? ForumChannelID { get; set; }

        public bool IsComplete { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Stages { get; set; } = new();

        public ulong MessageID { get; set; } = new();
        public List<ToDoList> Items { get; set; } = new();

        public Dictionary<ulong, UserEmoji> UserEmojis { get; set; } = new();

    }
}
