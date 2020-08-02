using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MVCClient.BackgroundHosts
{
    public class BackgroundWSQueue
    {
        private ConcurrentQueue<BinanceConnection> _wsItems =
            new ConcurrentQueue<BinanceConnection>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueWS(BinanceConnection wsItem)
        {
            _wsItems.Enqueue(wsItem);
            _signal.Release();
        }

        public async Task<BinanceConnection> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _wsItems.TryDequeue(out var wsItem);

            return wsItem;
        }
    }
}
