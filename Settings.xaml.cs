using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Simple_Timer
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        // the default time for the timer
        public static TimeSpan DefaultTime
        {
            get { return (TimeSpan)Properties.Settings.Default.defaultTime; }
            set { Properties.Settings.Default.defaultTime = value; }
        }

        public static TimeSpan TimeStep { get { return TimeSpan.FromMinutes(5); } }

        public static bool previewAlwaysOnTop = AlwaysOnTop;
        public static bool AlwaysOnTop
        {
            get { return (bool)Properties.Settings.Default["alwaysOnTop"]; }
            set { Properties.Settings.Default["alwaysOnTop"] = value; }
        }

        public static bool previewStartOnStartup = StartOnStartup;
        public static bool StartOnStartup
        {
            get { return (bool)Properties.Settings.Default["startOnStartup"]; }
            set { Properties.Settings.Default["startOnStartup"] = value; }
        }

        public static bool previewAutosaveTime = AutosaveTime;

        public event Action<string> SettingChanged;

        public static bool AutosaveTime
        {
            get { return (bool)Properties.Settings.Default["autosaveTime"]; }
            set { Properties.Settings.Default["autosaveTime"] = value; }
        }

        public Settings()
        {
            InitializeComponent();
            onTop.IsChecked = AlwaysOnTop;
            startOnStartup.IsChecked = StartOnStartup;
            autosaveTime.IsChecked = AutosaveTime;
        }

        private void onTop_Checked(object sender, RoutedEventArgs e)
        {
            previewAlwaysOnTop = true;
        }

        private void onTop_Unchecked(object sender, RoutedEventArgs e)
        {
            previewAlwaysOnTop = false;
        }

        private void startOnStartup_Checked(object sender, RoutedEventArgs e)
        {
            previewStartOnStartup = true;
        }

        private void startOnStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            previewStartOnStartup = false;
        }

        private void autosaveTime_Checked(object sender, RoutedEventArgs e)
        {
            previewAutosaveTime = true;
        }

        private void autosaveTime_Unchecked(object sender, RoutedEventArgs e)
        {
            previewAutosaveTime = false;
        }

        private void okbtn_Click(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop = previewAlwaysOnTop;
            SettingChanged?.Invoke("Topmost");
            StartOnStartup = previewStartOnStartup;
            AutosaveTime = previewAutosaveTime;
            Close();
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
