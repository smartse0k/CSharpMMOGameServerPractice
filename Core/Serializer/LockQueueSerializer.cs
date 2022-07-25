namespace Core.Serializer
{
    /**
     * 일반 Queue를 사용한 Serializer 구현체.
     * 
     * Queue<T>가 Thread Safe하지 않다.
     * 그렇기 때문에 Queue에 접근할때는 lock 문을 통해 잠근다.
     * https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.queue-1?view=net-6.0
     * 
     * 처리 상태를 구분하는 bool 변수 또한 Thread Safe하기 위해 lock을 통해 잠근다.
     * 
     * 데드락 방어를 위해 잠금 순서를 준수한다.
     * 1. Queue 잠금
     * 2. Boolean 잠금
     */

    public class LockQueueSerializer : ISerializer
    {
        private Queue<Task> queue;
        private bool isProcessing;

        private object mutexQueue;
        private object mutexIsProcessing;

        public LockQueueSerializer()
        {
            queue = new Queue<Task>();
            isProcessing = false;

            mutexQueue = new object();
            mutexIsProcessing = new object();
        }

        void ISerializer.Enqueue(Task task)
        {
            lock (mutexQueue)
            {
                queue.Enqueue(task);
            }
            
            lock (mutexIsProcessing)
            {
                if (isProcessing)
                {
                    return;
                }

                isProcessing = true;
            }

            Process();
        }

        private void Process()
        {
            Task? task = null;

            lock (mutexQueue)
            {
                task = queue.Dequeue();
            }

            if (task == null)
            {
                return;
            }

            task.Start();

            task.ContinueWith(OnProceeded);
        }

        private void OnProceeded(Task proceededTask)
        {
            lock (mutexQueue)
            {
                if (queue.Count == 0)
                {
                    lock (mutexIsProcessing)
                    {
                        isProcessing = false;
                        return;
                    }
                }
            }

            Process();
        }
    }
}