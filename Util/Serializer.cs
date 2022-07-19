namespace Util
{
    public class Serializer
    {
        private Queue<Task> queue;
        private bool isProcessing;

        public Serializer()
        {
            this.queue = new Queue<Task>();
            this.isProcessing = false;
        }

        public void Enqueue(Task task)
        {
            this.queue.Enqueue(task);

            // TODO : Thread Safe
            if (!this.isProcessing)
            {
                this.isProcessing = true;
                this.Process();
            }
        }

        private void Process()
        {
            Task task = this.queue.Dequeue();
            if (task == null)
            {
                return;
            }

            task.Start();
            task.ContinueWith((Task t) =>
            {
                if (this.queue.Count == 0)
                {
                    this.isProcessing = false;
                    return;
                }

                this.Process();
            });
        }
    }
}