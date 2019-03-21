using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using WinForms = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Simple_Timer
{
    public partial class MainWindow : Window
    {
        private bool mouseOverWindow = false;
        private const byte windowHeightFull = 120;
        private const byte windowHeightShort = 60;
        private const int expandDelay = 500;
        private const int contractDelay = 2000;
        private const int mouseDownDelay = 150;
        private WinForms.NotifyIcon notifyIcon = null;
        private WinForms.ContextMenu contextMenu;
        private WinForms.MenuItem exitAppMenuItem;
        private WinForms.MenuItem aboutMenuItem;
        private System.ComponentModel.IContainer components;
        private About AboutWindow;

        readonly Timer timer = new Timer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = timer;
            ShowInTaskbar = false;

            SetupTrayIcon();
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
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            else
                timer.Time = Settings.DefaultTime;
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timer.IsEnabled = timer.IsEnabled ? false : true;
                defocus();
            }
            if (e.Key == Key.Up)
            {   // add time
                timer.Time += Settings.TimeStep;
                Settings.DefaultTime = timer.Time;
            }
            if (e.Key == Key.Down)
            {   // reduce time
                var time = timer.Time - Settings.TimeStep;
                if (time.TotalSeconds >= 0)
                {
                    timer.Time = time;
                    Settings.DefaultTime = timer.Time;
                }
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

        private void defocus()
        {
            FrameworkElement parent = (FrameworkElement)timeInputTxt.Parent;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(timeInputTxt);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
        }

        private void SetupTrayIcon()
        {
            contextMenu = new WinForms.ContextMenu();
            exitAppMenuItem = new WinForms.MenuItem();
            aboutMenuItem = new WinForms.MenuItem();

            this.components = new System.ComponentModel.Container();
            //initialize menu items
            exitAppMenuItem.Index = 0;
            exitAppMenuItem.Text = "Exit";
            exitAppMenuItem.Click += (sender, e) =>
            {
                Application.Current.Shutdown();
            };
            aboutMenuItem.Index = 1;
            aboutMenuItem.Text = "About";
            aboutMenuItem.Click += (sendmer, e) =>
            {
                AboutWindow = new About();
                AboutWindow.Show();
            };
            // initialize context menu
            contextMenu.MenuItems.AddRange(new WinForms.MenuItem[] {
                aboutMenuItem,
                exitAppMenuItem
            });

            notifyIcon = new WinForms.NotifyIcon(this.components);
            notifyIcon.Text = "Simple Timer";
            notifyIcon.Visible = true;
            notifyIcon.Icon = Properties.Resources.timerIcon25m;
            notifyIcon.ContextMenu = contextMenu;
        }
    }
}
