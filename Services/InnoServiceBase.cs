using InnoTasker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Services
{
    public abstract class InnoServiceBase
    {
        protected readonly ILogger _logger;

        public InnoServiceBase(ILogger logger)
        {
            _logger = logger;
        }
    }
}
