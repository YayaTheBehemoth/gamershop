using System.Collections.Concurrent;
using System.Threading;

namespace gamershop.Server.Services
{
    public class SimpleMessageQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly AutoResetEvent _signal = new AutoResetEvent(false);

        public void Enqueue(T item)
        {
            Console.WriteLine("enqued!");
            Console.WriteLine(item);
            _queue.Enqueue(item);
            _signal.Set(); // Signal that there's a new item in the queue
        }

    public T Dequeue()
{
    Console.WriteLine("dequed!");
    _signal.WaitOne(); // Wait for a signal indicating there's an item in the queue
    _queue.TryDequeue(out var item);
    
    // Reset the AutoResetEvent if the queue is empty
    if (_queue.IsEmpty)
    {
        _signal.Reset();
    }
    
    return item;
}

    }
}
