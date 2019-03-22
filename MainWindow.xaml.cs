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
        private bool customTime = false;
        private const byte windowHeightFull = 120;
        private const byte windowHeightShort = 60;
        private const int expandDelay = 500;
        private const int contractDelay = 2000;
        private const int mouseDownDelay = 150;
        private WinForms.NotifyIcon notifyIcon = null;
        private WinForms.ContextMenu contextMenu;
        private WinForms.MenuItem exitAppMenuItem;
        private WinForms.MenuItem aboutMenuItem;
        private WinForms.MenuItem settingsMenuItem;
        private System.ComponentModel.IContainer components;
        private About aboutWindow;
        private Settings settingsWindow;

        readonly Timer timer = new Timer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = timer;
            ShowInTaskbar = false;
            Topmost = Settings.AlwaysOnTop;
            SetupTrayIcon();
            if (Settings.StartOnStartup)
                timer.Start();
        }

        private void timeInputTxt_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            DragMove();
        }

        private void timeInputTxt_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            customTime = true;
        }

        private void playPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (customTime)
            {
                timer.Time = ParseTimeInput();
                customTime = false;
            }
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (customTime)
                {
                    timer.Time = ParseTimeInput();
                    customTime = false;
                }
                timer.IsEnabled = timer.IsEnabled ? false : true;
                defocus();
            }
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {   // add time
                timer.Time += Settings.TimeStep;
                if (Settings.AutosaveTime)
                    Settings.DefaultTime = timer.Time;
                customTime = true;
            }
            if (e.Key == Key.Down)
            {   // reduce time
                var time = timer.Time - Settings.TimeStep;
                if (time.TotalSeconds >= 0)
                {
                    timer.Time = time;
                    if (Settings.AutosaveTime)
                        Settings.DefaultTime = timer.Time;
                    customTime = true;
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
                    }
                });
            });
            if (timeInputTxt.IsFocused)
                defocus();
        }

        private TimeSpan ParseTimeInput()
        {
            if (timeInputTxt.Text.Contains(":"))
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
                        if (Settings.AutosaveTime)
                            Settings.DefaultTime = ts;
                    }
                    return ts;
                }
                return TimeSpan.Zero;
            }
            else if (int.TryParse(timeInputTxt.Text, out int min))
            {
                TimeSpan ts;
                if (min >= 60)
                {
                    ts = new TimeSpan(0, 0, 0);
                    MessageBox.Show("Time more than 60 minutes is not supported");
                }
                else
                {
                    ts = new TimeSpan(0, min, 0);
                    if (Settings.AutosaveTime)
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

        private void SettingChanged(string settingName)
        {
            switch (settingName)
            {
                case "Topmost":
                    Topmost = Settings.AlwaysOnTop;
                    break;
                default:
                    break;
            }
        }

        private void SetupTrayIcon()
        {
            contextMenu = new WinForms.ContextMenu();
            exitAppMenuItem = new WinForms.MenuItem();
            aboutMenuItem = new WinForms.MenuItem();
            settingsMenuItem = new WinForms.MenuItem();

            this.components = new System.ComponentModel.Container();
            //initialize menu items
            aboutMenuItem.Text = "About";
            aboutMenuItem.Click += (sendmer, e) =>
            {
                aboutWindow = new About();
                aboutWindow.Show();
            };
            settingsMenuItem.Text = "Settings";
            settingsMenuItem.Click += (sender, e) =>
            {
                settingsWindow = new Settings();
                settingsWindow.SettingChanged += SettingChanged;
                settingsWindow.Show();
            };
            Thread.Sleep(50);
            exitAppMenuItem.Text = "Exit";
            exitAppMenuItem.Click += (sender, e) =>
            {
                Properties.Settings.Default.Save();
                Application.Current.Shutdown();
            };
            // initialize context menu
            contextMenu.MenuItems.AddRange(new WinForms.MenuItem[] {
                aboutMenuItem,
                exitAppMenuItem,
                settingsMenuItem
            });

            notifyIcon = new WinForms.NotifyIcon(this.components);
            notifyIcon.Text = "Simple Timer";
            notifyIcon.Visible = true;
            notifyIcon.Icon = Properties.Resources.timerIcon25m;
            notifyIcon.ContextMenu = contextMenu;
        }
    }
}
