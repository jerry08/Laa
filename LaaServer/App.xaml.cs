using LaaServer.Utils;
using LaaServer.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace LaaServer
{
    public partial class App
    {
        private static Assembly Assembly { get; } = typeof(App).Assembly;

        public static string Name { get; } = Assembly.GetName().Name!;

        public static Version Version { get; } = Assembly.GetName().Version!;

        public static string VersionString { get; } = Version.ToString(3);

        public static string GitHubProjectUrl { get; } = "https://github.com/Code-08/SoundCloudDownloader";
    }

    public partial class App : Application
    {
        public const string AppName = "LaaServer";
        private static Mutex _mutex = null;

        public App()
        {
            //this.Startup += new StartupEventHandler(App_Startup);
            //DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mutex = new Mutex(true, AppName, out bool createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application

                FocusProgram.Set();

                Current.Shutdown();
                return;
            }

            // Set default theme
            // (preferred theme will be chosen later, once the settings are loaded)
            App.SetLightTheme();

            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            IsExit = true;

            MainWindow = new RootView();
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Loaded += (s, e) =>
            {
                if (ProgramOpen.WaitOne(0))
                {
                    FocusProgram.Set();
                    Application.Current.Shutdown();
                }
                ProgramOpen.Set();
            };

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            //_notifyIcon.Icon = BackgroundApplication.Properties.Resources.MyIcon;
            _notifyIcon.Icon = new System.Drawing.Icon(App.GetResourceStream(new Uri("pack://application:,,,/cherry_icon.ico")).Stream);
            _notifyIcon.Visible = true;

            CreateContextMenu();
            ShowMainWindow();
        }

        private static Theme LightTheme { get; } = Theme.Create(
            new MaterialDesignLightTheme(),
            MediaColor.FromHex("#343838"),
            MediaColor.FromHex("#F9A825")
        );

        private static Theme DarkTheme { get; } = Theme.Create(
            new MaterialDesignDarkTheme(),
            MediaColor.FromHex("#E8E8E8"),
            MediaColor.FromHex("#F9A825")
        );

        public static void SetLightTheme()
        {
            var paletteHelper = new PaletteHelper();
            paletteHelper.SetTheme(LightTheme);

            Current.Resources["SuccessBrush"] = new SolidColorBrush(Colors.DarkGreen);
            Current.Resources["CanceledBrush"] = new SolidColorBrush(Colors.DarkOrange);
            Current.Resources["FailedBrush"] = new SolidColorBrush(Colors.DarkRed);
        }

        public static void SetDarkTheme()
        {
            var paletteHelper = new PaletteHelper();
            paletteHelper.SetTheme(DarkTheme);

            Current.Resources["SuccessBrush"] = new SolidColorBrush(Colors.LightGreen);
            Current.Resources["CanceledBrush"] = new SolidColorBrush(Colors.Orange);
            Current.Resources["FailedBrush"] = new SolidColorBrush(Colors.OrangeRed);
        }

        private System.Windows.Forms.NotifyIcon _notifyIcon;
        public static bool IsExit { get; set; }

        EventWaitHandle ProgramOpen = new EventWaitHandle(false, EventResetMode.ManualReset, "ProgramOpen198472");
        EventWaitHandle FocusProgram = new EventWaitHandle(false, EventResetMode.ManualReset, "FocusMyProgram198472");
        private delegate void focusConfirmed();
        Thread FocusCheck;

        private void Focus()
        {
            FocusProgram.WaitOne();
            if (!IsExit)
            {
                App.Current.Dispatcher.Invoke(new focusConfirmed(() =>
                {
                    ShowMainWindow();
                }));
            }
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            IsExit = true;
            FocusProgram.Set();
            ProgramOpen.Reset();
            MainWindow.Close();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!IsExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not

                FocusProgram.Reset();
                FocusCheck = new Thread(Focus);
                FocusCheck.Start();
            }
            else
            {
                FocusProgram.Set();
                ProgramOpen.Reset();
                (App.Current.MainWindow.DataContext as RootViewModel).OnClose();
            }
        }

        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    if (e.ExceptionObject is Exception)
        //        WriteLog(e.ExceptionObject);
        //    else
        //        WriteLog(e);
        //}

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            if (e.Exception is Exception)
                WriteLog(e.Exception);
            else
                WriteLog(e);

            MessageBox.Show(e.Exception.GetBaseException().Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void RestartAsAdmin()
        {
            // Restart and run as admin
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
            startInfo.Verb = "runas";
            startInfo.Arguments = "restart";
            startInfo.UseShellExecute = true;

            try
            {
                Process.Start(startInfo);
            }
            catch
            {

            }

            Current.Shutdown();
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void WriteLog(object exception)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog.txt");

            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.End);
                    byte[] buffer = Encoding.Default.GetBytes(" --------------------------- ----------------------------\r\n ");
                    fs.Write(buffer, 0, buffer.Length);

                    buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + " \r\n ");
                    fs.Write(buffer, 0, buffer.Length);

                    if (exception is Exception ex)
                    {
                        buffer = Encoding.Default.GetBytes(" Member name: " + ex.TargetSite + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes(" The class that caused the exception: " + ex.TargetSite.DeclaringType + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes(" Exception information: " + ex.Message + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes(" The assembly or object that caused the exception: " + ex.Source + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);

                        buffer = Encoding.Default.GetBytes(" Stack: " + ex.StackTrace + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        buffer = Encoding.Default.GetBytes(" Application error: " + exception.ToString() + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch { }
        }
    }
}