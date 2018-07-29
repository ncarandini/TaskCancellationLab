using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCancellationLab1
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            var task = Task.Factory.StartNew(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation requested before string task.");
                    throw new OperationCanceledException();
                }
                else
                {
                    Console.WriteLine("Starting task...");
                }               

                bool moreToDo = true;
                while (moreToDo)
                {
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        Console.WriteLine("Cancellation requested during task execution.");
                        throw new OperationCanceledException();
                    }
                }
            }, cts.Token); // Pass cancellation token to StartNew.

            task.Wait(1000);
            cts.Cancel();

            // Just continue on this thread, or Wait/WaitAll with try-catch:
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(e.Message + " " + v.Message);
            }
            finally
            {
                cts.Dispose();
            }

            Console.Write("Hit a key to terminate...");
            Console.ReadKey();
        }
    }
}