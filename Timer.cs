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
        private TimeSpan time;
        private const string playIcon = ">";
        private const string pauseIcon = "||";
        private string playBtnIcon = playIcon;

        public Timer() : this(Settings.DefaultTime) { }

        public Timer(TimeSpan interval)
        {
            t = new System.Timers.Timer(interval.TotalMilliseconds);
            Time = interval;
            BeginUpdate();
        }

        public string PlayBtnIcon
        {
            get
            {
                return playBtnIcon;
            }
            set
            {
                playBtnIcon = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayBtnIcon"));
            }
        }

        public bool IsEnabled
        {
            get
            {
                return t.Enabled;
            }
            set
            {
                if (value == true)
                    playBtnIcon = pauseIcon;
                else
                    playBtnIcon = playIcon;
                t.Enabled = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PlayBtnIcon"));
            }
        }

        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Start()
        {
            t.Start();
            playBtnIcon = pauseIcon;
        }

        public void Stop()
        {
            t.Stop();
            PlayBtnIcon = playIcon;
            Time = Settings.DefaultTime;
        }

        public override string ToString()
        {
            return Time.ToString("mm\\:ss");
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
                        {
                            Console.Beep();
                            Stop();
                        }
                    }

                }
            });
        }
    }
}
