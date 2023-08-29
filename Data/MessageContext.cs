using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data
{
    public struct MessageContext
    {
        public Embed embed;
        public ComponentBuilder component;

        public MessageContext(Embed embed, ComponentBuilder component)
        {
            this.embed = embed;
            this.component = component;
        }
    }
}
