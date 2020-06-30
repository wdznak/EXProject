using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public interface IStorage
    {
        public Task WriteAsync(string message);
        public void Init(string symbol);
    }
}
