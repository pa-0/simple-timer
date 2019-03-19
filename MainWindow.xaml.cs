using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;

namespace Simple_Timer
{
    public partial class MainWindow : Window
    {
        bool mouseOverWindow = false;
        const byte windowHeightFull = 120;
        const byte windowHeightShort = 60;
        const int expandDelay = 500;
        const int contractDelay = 2000;
        const int mouseDownDelay = 150;

        readonly Timer timer = new Timer();

        public MainWindow()
        {
            InitializeComponent();

            //timeInputTxt.Text = Settings.DefaultTime.ToString("mm\\:ss");
            DataContext = timer;
        }

        private void timeInputTxt_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            DragMove();
        }

        private void timeInputTxt_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            timeInputTxt.Focus();
        }

        private void playPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.IsEnabled = timer.IsEnabled ? false : true;
            SwitchBetweenPlayPauseIcon();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                SwitchBetweenPlayPauseIcon();
            }
        }

        private void SwitchBetweenPlayPauseIcon()
        {
            if (playPauseBtn.Content.ToString() == timer.PlayIcon)
                playPauseBtn.Content = timer.PauseIcon;
            else
                playPauseBtn.Content = timer.PlayIcon;
        }
        // Window handlers
        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timer.IsEnabled = timer.IsEnabled ? false : true;
                SwitchBetweenPlayPauseIcon();
                defocus();
            }
            if (e.Key == Key.Up)
            {   // add time
                //timer.SetTime(timer.Time + Settings.TimeStep);
            }
            if (e.Key == Key.Down)
            {   // reduce time
                //var time = timer.Time - Settings.TimeStep;
                //if (time.TotalSeconds >= 0) // should not go below zero
                //    timer.SetTime(time);
            }
        }

        private void window_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseOverWindow = true;
            Task.Run(() =>
            {
                Thread.Sleep(expandDelay);
                Dispatcher.Invoke(() =>
                {
                    if (mouseOverWindow)
                    {
                        controlsGrid.Visibility = Visibility.Visible;
                        //Height = windowHeightFull;
                    }
                });
            });
        }

        private void window_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseOverWindow = false;
            Task.Run(() =>
            {
                Thread.Sleep(contractDelay);
                Dispatcher.Invoke(() =>
                {
                    if (!mouseOverWindow)
                    {
                        controlsGrid.Visibility = Visibility.Collapsed;
                        //Height = windowHeightShort; // contract window height 50%
                        //SwitchToTimeDisplay();
                    }
                });
            });
        }

        private void timeInputTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (timeInputTxt.IsFocused)
                timer.Time = ParseTimeInput();
        }

        private TimeSpan ParseTimeInput()
        {
            if (int.TryParse(timeInputTxt.Text.Split(':')[0], out int minutes)
            && (int.TryParse(timeInputTxt.Text.Split(':')[1], out int seconds)))
            {
                TimeSpan ts;
                if (minutes >= 60)
                {
                    ts = new TimeSpan(0, 0, 0);
                    MessageBox.Show("Time more than 60 minutes is not supported");
                }
                else
                {
                    ts = new TimeSpan(0, minutes, seconds);
                    Settings.DefaultTime = ts;
                }
                return ts;
            }
            else
            {
                MessageBox.Show("incorrect time format");
                return TimeSpan.Zero;
            }
        }

        private void defocus() {
            FrameworkElement parent = (FrameworkElement)timeInputTxt.Parent;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(timeInputTxt);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        }
    }
}
