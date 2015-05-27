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
                _logger.Log("Begin task scheduled by Task.Run()");
                PrintTaskScheduler();
                _logger.Log("End task scheduled by Task.Run()");
            })
            .ContinueWith(task =>
            {
                _logger.Log("Begin task scheduled by Task.Run().ContinueWith()");
                PrintTaskScheduler();
                _logger.Log("End task scheduled by Task.Run().ContinueWith()");
            })
            .Wait();

            Task.Factory.StartNew(() =>
            {
                _logger.Log("Begin task scheduled by TaskScheduler.FromCurrentSynchronizationContext()");
                PrintTaskScheduler();
                _logger.Log("End task scheduled by TaskScheduler.FromCurrentSynchronizationContext()");
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void PrintTaskScheduler()
        {
            _logger.Log(String.Format("\tTaskScheduler.Current: {0}", SafeGetTypeName(TaskScheduler.Current)));
            _logger.Log(String.Format("\tSynchronziationContext.Current: {0}", SafeGetTypeName(SynchronizationContext.Current)));
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
