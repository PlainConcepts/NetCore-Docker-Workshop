using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Master.Resilience
{
    public interface IPoliciesFactory
    {
        Policy[] Policies();
    }
}
