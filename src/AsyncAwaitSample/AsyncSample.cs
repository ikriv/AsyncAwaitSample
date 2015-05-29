using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitSample
{
    class AsyncSample
    {
        private readonly ILogger _logger;

        public AsyncSample(ILogger logger)
        {
            _logger = logger;
        }

        public async void ShowTaskScheduler()
        {
            _logger.Log("UI thread outside any task:");
            PrintTaskScheduler();

            Task.Run(() => MyTask("Task.Run()")).Wait();

            await PrintTaskSchedulerTask();
        }

        private void MyTask(string scheduledBy)
        {
            _logger.Log("Begin MyTask scheduled by " + scheduledBy);
            PrintTaskScheduler();
            _logger.Log("End MyTask scheduled by " + scheduledBy);
            
        }

        private async Task PrintTaskSchedulerTask()
        {
            await Task.Delay(100); // make async
            MyTask("await");
        }

        private void PrintTaskScheduler()
        {
            _logger.Log(String.Format("\tTaskScheduler.Default: {0}", SafeGetTypeName(()=>TaskScheduler.Default)));
            _logger.Log(String.Format("\tTaskScheduler.Current: {0}", SafeGetTypeName(()=>TaskScheduler.Current)));
            _logger.Log(String.Format("\tTaskScheduler.FromCurrentSynchronizationContext(): {0}", SafeGetTypeName(TaskScheduler.FromCurrentSynchronizationContext)));
            _logger.Log(String.Format("\tSynchronziationContext.Current: {0}", SafeGetTypeName(()=>SynchronizationContext.Current)));
        }

        private string SafeGetTypeName(Func<object> func)
        {
            try
            {
                var obj = func();
                if (obj == null) return "null";
                return obj.GetType().Name;
            }
            catch
            {
                return "Exception!";
            }
        }

        public async void AsyncVoid()
        {
            _logger.Log("AsyncVoid() begin");


            _logger.Log("await Task.Delay(3000)");
            await Task.Delay(3000);
            _logger.Log("await returned");

            _logger.Log("AsyncVoid() end");
        }

        public async void AsyncVoidWithSleep()
        {
            _logger.Log("AsyncVoidWithSleep() begin");

            _logger.Log("Thread.Sleep(3000)");
            Thread.Sleep(3000);

            _logger.Log("await Task.Delay(3000)");
            await Task.Delay(3000);
            _logger.Log("await returned");

            _logger.Log("Thread.Sleep(3000)");
            Thread.Sleep(3000);

            _logger.Log("AsyncVoidWithSleep() end");
        }

        public void JustException()
        {
            if (0 < Math.Sqrt(1)) throw new ApplicationException("JustException Error");
        }

        public async void AsyncVoidSyncException()
        {
            _logger.Log("AsyncVoidSyncException() begin: about to throw");
            if (0<Math.Sqrt(1)) throw new ApplicationException("AsyncVoidAsyncException Error");
            await Task.Delay(3000);
            _logger.Log("AsyncVoidSyncException() end");
        }


        public async void AsyncVoidAsyncException()
        {
            _logger.Log("AsyncVoidAsyncException() begin: calling Task.Delay()");
            await Task.Delay(3000);
            if (0 < Math.Sqrt(1)) throw new ApplicationException("AsyncVoidAsyncException Error");
            _logger.Log("AsyncVoidAsyncException() end");
        }


        public async void CreateTaskWithSleep()
        {
            _logger.Log("CreateTask() begin: calling await TaskWithSleep()");
            await TaskWithSleep();
            _logger.Log("CreateTask() end");
        }

        private async Task TaskWithSleep()
        {
            await Task.Delay(100);

            _logger.Log("TaskWithSleep() begin");
            for (int i = 0; i < 5; ++i)
            {
                _logger.Log("TaskWithSleep is hogging UI thread: " + i);
                Thread.Sleep(1000);
            }
            _logger.Log("TaskWithSleep() end");
        }

    }
}
