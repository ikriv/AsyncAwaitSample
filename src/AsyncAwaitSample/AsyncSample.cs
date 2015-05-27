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

        public void ShowTaskScheduler()
        {
            _logger.Log("UI thread outside any task:");
            PrintTaskScheduler();

            Task.Run(() =>
            {
                _logger.Log("Inside a task scheduled by Task.Run()");
                PrintTaskScheduler();
            })
            .ContinueWith(task =>
            {
                _logger.Log("Inside ContinueWith()");
                PrintTaskScheduler();
            });

            Task.Factory.StartNew(() =>
            {
                _logger.Log("Inside a task scheduled on TaskScheduler.FromCurrentSynchronizationContext()");
                PrintTaskScheduler();
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PrintTaskScheduler()
        {
            _logger.Log(String.Format("TaskScheduler.Current: {0}", SafeGetTypeName(TaskScheduler.Current)));
            _logger.Log(String.Format("SynchronziationContext.Current: {0}", SafeGetTypeName(SynchronizationContext.Current)));
        }

        private string SafeGetTypeName(object obj)
        {
            if (obj == null) return "null";
            return obj.GetType().Name;
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


        public async void CreateTask()
        {
            _logger.Log("CreateTask() begin: calling MyTask()");
            await MyTask();
            _logger.Log("CreateTask() end");
        }

        private async Task MyTask()
        {
            var ts1 = TaskScheduler.Current;
            await Task.Delay(100);
            var ts2 = TaskScheduler.Current;
            var ts3 = TaskScheduler.FromCurrentSynchronizationContext();

            _logger.Log("MyTask() begin");
            for (int i = 0; i < 5; ++i)
            {
                _logger.Log("MyTask is hogging UI thread: " + i);
                Thread.Sleep(1000);
            }
            _logger.Log("MyTask() end");
        }

    }
}
