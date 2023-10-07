using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public enum LimitType
    {
        Name,
        Description
    }

    public class Limit
    {
        public string name;
        public int limit;

        public Limit(string name, int limit)
        {
            this.name = name;
            this.limit = limit;
        }
    }

    public static class Limits
    {
        public static Dictionary<LimitType, Limit> limits = new()
        {

        };
    }
}
