using System;
using System.Threading;

namespace ThreadPoolExercises.Core
{
    public class ThreadingHelpers
    {
        public static void ExecuteOnThread(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            // * Create a thread and execute there `action` given number of `repeats` - waiting for the execution!
            //   HINT: you may use `Join` to wait until created Thread finishes
            // * In a loop, check whether `token` is not cancelled
            // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)
            var thread = new Thread((repeats) =>
            {
                try
                {
                    if (repeats is null || (int)repeats is 0) throw new ArgumentNullException("Number of repeats argument is null or 0");
                    int numberOfRepeats = (int)repeats;

                    while (numberOfRepeats > 0)
                    {
                        if (token.IsCancellationRequested)
                        {
                            if (errorAction != null)
                                errorAction(new OperationCanceledException());
                            else
                                token.ThrowIfCancellationRequested();

                            numberOfRepeats = 0;
                        }

                        action();
                        numberOfRepeats--;
                    }
                }
                catch (Exception ex)
                {

                }
            })
            {
                IsBackground = true
            };
            thread.Start(repeats);
            thread.Join();

        }

        public static void ExecuteOnThreadPool(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            // * Queue work item to a thread pool that executes `action` given number of `repeats` - waiting for the execution!
            //   HINT: you may use `AutoResetEvent` to wait until the queued work item finishes
            // * In a loop, check whether `token` is not cancelled
            // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

            //Fire and forget without AutoResetEvent
            //ThreadPool.QueueUserWorkItem(
            //    (repeats) =>
            //    {
            //        if (repeats is 0) throw new ArgumentNullException("Number of repeats argument is null or 0");

            //        while (repeats > 0)
            //        {
            //            if (token.IsCancellationRequested)
            //            {
            //                if (errorAction != null)
            //                    errorAction(new OperationCanceledException());
            //                else
            //                    token.ThrowIfCancellationRequested();
            //            }

            //            action();
            //            repeats--;
            //        }

            //    }, repeats, true);

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem((_) => 
            {
                try
                {
                    while (repeats > 0)
                    {
                        if (token.IsCancellationRequested)
                        {
                            if (errorAction != null)
                                errorAction(new OperationCanceledException());
                            else
                                token.ThrowIfCancellationRequested();
                            
                            repeats = 0;
                        }

                        action();
                        repeats--;
                    }

                    autoResetEvent.Set();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            });
            
            autoResetEvent.WaitOne();
            autoResetEvent.Close();
        }

        public static void ExecuteAfterWait<T>(int sleep, Action<T> action, T arg)
        {
            AutoResetEvent evt = new AutoResetEvent(false);
            RegisteredWaitHandle? handle = null;
            handle = ThreadPool.RegisterWaitForSingleObject(evt, (state, timedOut) =>
            {
                handle!.Unregister(evt);
                action(arg);
            }, null, sleep, true);
        }

    }
}
