using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker
{
    public class SupportInfo
    {
        public ulong guildID;
        public ulong channelID;
        public ulong userID;
        public string listName;

        public SupportInfo(ulong guildID, ulong channelID, ulong userID, string listName)
        {
            this.guildID = guildID;
            this.channelID = channelID;
            this.userID = userID;
            this.listName = listName;
        }

        public override string ToString()
        {
            return $"{guildID}.{channelID}.{listName}.{listName}";
        }

        public static SupportInfo ToInfo(string input)
        {
            string[] info = input.Split(".");
            return new SupportInfo(ulong.Parse(info[0]), ulong.Parse(info[1]), ulong.Parse(info[2]), info[3]);
        }
    }
}
