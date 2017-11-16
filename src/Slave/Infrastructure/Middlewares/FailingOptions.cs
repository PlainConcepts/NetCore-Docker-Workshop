using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slave.Infrastructure.Middlewares
{
    public class FailingOptions
    {
        public string ConfigPath = "/Failing";
        public List<string> EndpointPaths { get; set; } = new List<string>();
    }
}
