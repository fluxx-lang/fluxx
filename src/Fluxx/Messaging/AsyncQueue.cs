using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Faml.Messaging {

    public class AsyncQueue<T> {
        // Note that the semaphore count matches the number of items in the queue
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public void Enqueue(T item) {
            this._queue.Enqueue(item);
            this._semaphore.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken) {
            while (true) {
                await this._semaphore.WaitAsync(cancellationToken);
                if (this._queue.TryDequeue(out T item))
                {
                    return item;
                }
            }
        }
    }
}
