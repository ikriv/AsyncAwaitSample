using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace AsyncAwaitSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ILogger
    {
        private readonly int _uiThreadId = Environment.CurrentManagedThreadId;

        public ObservableCollection<string> LogRecords { get; private set; }
        public IEnumerable<Action> Actions { get; private set; }
        public ICommand ClickCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        

        public MainWindow() 
        {
            LogRecords = new ObservableCollection<string>();
            var sample = new AsyncSample(this);

            Actions = new Action[]
            {
                sample.ShowTaskScheduler,
                sample.AsyncVoid, 
                sample.AsyncVoidWithSleep,
                sample.AwaitTaskWithSleep, 
                sample.AsyncVoidAsyncException, 
                sample.AsyncVoidSyncException
            };

            ClickCommand = new Command<Action>(OnClick);
            CopyCommand = new Command<Empty>(OnCopy);


            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(LogRecords, LogRecords);
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs eventArgs)
        {
            TimerText.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        public void Log(string text)
        {
            string message = String.Format("{0} [{1}] {2}", 
                DateTime.Now.ToString("HH:mm:ss"), 
                GetThreadString(Environment.CurrentManagedThreadId), 
                text);

            LogRecords.Add(message);
            System.Diagnostics.Trace.WriteLine(message);
        }

        public void ClearLog()
        {
            LogRecords.Clear();
        }

        private string GetThreadString(int id)
        {
            if (id == _uiThreadId) return "UI Thread";
            return "Worker " + id.ToString("D2");
        }

        private void OnClick(Action action)
        {
            ClearLog();
            Log("Click handle begin");
            action();
            Log("Click handle end");

        }

        private void OnCopy(Empty unused)
        {
            var text = String.Join("\r\n", LogRecords);
            Clipboard.SetData(DataFormats.UnicodeText, text);
        }
    }
}
