using Core.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class SerializerTest
    {
        private ISerializer lockQueueSerializer = new LockQueueSerializer();

        public void Start()
        {
            Console.WriteLine("===== Serializer Test =====");

            Random random = new Random();

            for (int i=0; i<10; ++i)
            {
                Console.WriteLine("PrintMessage({0}) start", i);
                Task<string> reserveTask = this.PrintMessage(i.ToString(), random.Next(300, 2000));
                reserveTask.ContinueWith((Task<string> taskResult) =>
                {
                    Console.WriteLine("PrintMessage done. result={0}", taskResult.Result);
                });
                Console.WriteLine("PrintMessage({0}) end", i);
            }
        }

        public async Task<string> PrintMessage(string message, int delay)
        {
            Task<string> task = new Task<string>(() =>
            {
                Thread.Sleep(delay);
                Console.WriteLine(message);
                return message;
            });

            this.lockQueueSerializer.Enqueue(task);

            string response = await task;

            return response;
        }
    }
}
