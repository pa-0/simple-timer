using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Simple_Timer
{
    class Timer : INotifyPropertyChanged
    {
        private System.Timers.Timer t;

        public Timer() : this(Settings.DefaultTime) { }

        public Timer(TimeSpan interval)
        {
            t = new System.Timers.Timer(interval.TotalMilliseconds);
            Time = interval;
            BeginUpdate();
        }

        public string PlayIcon { get { return ">"; } }
        public string PauseIcon { get { return "||"; } }

        public bool IsEnabled { get { return t.Enabled; } set { t.Enabled = value; } }

        public TimeSpan Time { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            t.Start();
        }

        public void Stop()
        {
            t.Stop();
            Console.Beep();
            Time = Settings.DefaultTime;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
        }

        private void BeginUpdate()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (t.Enabled)
                    {
                        Time = Time.Subtract(TimeSpan.FromSeconds(1));
                        if (Time == TimeSpan.Zero)
                            Stop();
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
                    }

                }
            });
        }
    }
}
