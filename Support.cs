using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker
{
    public static class Support
    {
        public static string GenerateSupportID(SupportInfo info)
        {
            string infoString = info.ToString();
            Byte[] infoBytes = Encoding.UTF8.GetBytes(infoString);
            return Convert.ToBase64String(infoBytes);
        }

        public static SupportInfo ParseSupportID(string id)
        {
            Byte[] infoBytes = Convert.FromBase64String(id);
            string infoString = Encoding.UTF8.GetString(infoBytes);
            return SupportInfo.ToInfo(infoString);
        }
    }
}
