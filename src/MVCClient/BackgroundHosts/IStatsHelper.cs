using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public interface IStatsHelper
    {
        public Task PostMessageAsync(string message);
    }
}
