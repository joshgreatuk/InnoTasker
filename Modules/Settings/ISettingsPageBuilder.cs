using InnoTasker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Settings
{
    public interface ISettingsPageBuilder
    {
        public MessageContext BuildPage();
    }
}
