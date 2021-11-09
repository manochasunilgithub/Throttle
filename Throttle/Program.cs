// See https://aka.ms/new-console-template for more information

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Throttle
{
    public static class EnumerableExtensions
    {

        public static async Task ExecuteInParallel<T>(this IEnumerable<T> items, int degreeOfParallelism, Func<T, Task> action)
        {
            List<Task> allTasks = new List<Task>();
            List<Task> activeTasks = new List<Task>();
            foreach( var item in items)
            {
                if(activeTasks.Count>degreeOfParallelism)
                {
                    var completedTask = await Task.WhenAny(activeTasks);
                    activeTasks.Remove(completedTask    );
                    
                }
                var task = action(item);
                allTasks.Add(task);
                activeTasks.Add(task);
            }
            await Task.WhenAll(allTasks);
        }
    }

    class Test
    {

        public static async Task RunMethod(int taskId)
        {
            await Task.Run(() =>
           {
               Console.WriteLine($"Stated Processing TaskId = {taskId}, time = {DateTime.UtcNow}");
               //Random r = new Random();
               Thread.Sleep(2000);
               Console.WriteLine($"Finished Processing Task, TaskId = {taskId}, time = {DateTime.UtcNow}");

           });
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            IEnumerable<int> ids = Enumerable.Range(0, 10);
            int limit = 4;
            Console.WriteLine("Starting to process tasks...");
            Task.Run(async ()=>await ids.ExecuteInParallel(limit, RunMethod));
            Console.Read();

        }
    }
}




