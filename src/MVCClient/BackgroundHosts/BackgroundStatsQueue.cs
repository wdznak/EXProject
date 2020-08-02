using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public class BackgroundStatsQueue
    {
        private ConcurrentQueue<string> _statsItems = new ConcurrentQueue<string>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueItem(string item)
        {
            _statsItems.Enqueue(item);
            _signal.Release();
        }

        public async Task<string> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _statsItems.TryDequeue(out var item);

            return item;
        }
    }
}
