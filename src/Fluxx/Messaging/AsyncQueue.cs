using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Fluxx.Messaging
{

    public class AsyncQueue<T>
    {
        // Note that the semaphore count matches the number of items in the queue
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public void Enqueue(T item)
        {
            this.queue.Enqueue(item);
            this.semaphore.Release();
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await this.semaphore.WaitAsync(cancellationToken);
                if (this.queue.TryDequeue(out T item))
                {
                    return item;
                }
            }
        }
    }
}
