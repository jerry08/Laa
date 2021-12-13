using Microsoft.Win32;
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
using System.Windows.Threading;

namespace LaaServer
{
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        public static bool IsExit;

        public const string AppName = "LaaServer";
        private static Mutex _mutex = null;

        EventWaitHandle ProgramOpen = new EventWaitHandle(false, EventResetMode.ManualReset, "ProgramOpen198472");
        EventWaitHandle FocusProgram = new EventWaitHandle(false, EventResetMode.ManualReset, "FocusMyProgram198472");
        private delegate void focusConfirmed();
        Thread FocusCheck;

        private void focus()
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

        public App()
        {
            //this.Startup += new StartupEventHandler(App_Startup);
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mutex = new Mutex(true, AppName, out bool createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application

                //Process currentProcess = Process.GetCurrentProcess();
                //Process process = Process.GetProcesses().Where(x => x.ProcessName == currentProcess.ProcessName && x.Id != currentProcess.Id).FirstOrDefault();
                //
                //var hWnd = process.MainWindowHandle.ToInt32();
                //WindowHelper.ShowWindow(hWnd, WindowHelper.SW_SHOW);
                //WindowHelper.BringProcessToFront(process);

                FocusProgram.Set();

                Application.Current.Shutdown();
                return;
            }

            App.IsExit = true;

            MainWindow = new MainWindow();
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
            _notifyIcon.Icon = new Icon(App.GetResourceStream(new Uri("pack://application:,,,/cherry_icon.ico")).Stream);
            _notifyIcon.Visible = true;

            CreateContextMenu();
            ShowMainWindow();
            //SetSelfStarting(true, "test12.exe");
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
                FocusCheck = new Thread(focus);
                FocusCheck.Start();
            }
            else
            {
                FocusProgram.Set();
                ProgramOpen.Reset();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                WriteLog(e.ExceptionObject);
            else
                WriteLog(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is Exception)
                WriteLog(e.Exception);
            else
                WriteLog(e);
        }

        /*public void SingleInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (Process.GetProcesses().Count(p => p.ProcessName == currentProcess.ProcessName) > 1)
            {
                Process process = Process.GetProcesses().Where(x => x.ProcessName == currentProcess.ProcessName && x.Id != currentProcess.Id).FirstOrDefault();
                List<Process> processes = Process.GetProcesses().Where(x => x.ProcessName == currentProcess.ProcessName && x.Id != currentProcess.Id).ToList();

                try
                {
                    if (IsAdministrator())
                    {
                        processes.ForEach(x => x.Kill());

                        return;
                    }
                }
                catch
                {

                }

                if (Settings.Default.IsProcessClosing)
                {
                    try
                    {
                        process.WaitForExit(15);
                    }
                    catch
                    {
                        WindowHelper.BringProcessToFront(process);
                        currentProcess.Kill();
                    }
                }
                else
                {
                    WindowHelper.BringProcessToFront(process);
                    currentProcess.Kill();
                    return;
                }

                if (processes.Count == 1)
                {
                    currentProcess.Kill();
                    return;
                }

                if (processes.Count > 0)
                {
                    foreach (var proc in processes)
                    {
                        proc.Kill();
                    }
                }
            }
        }*/

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

        /// <summary> 
        ///Automatically start on boot
        /// </summary> 
        /// <param name="started"> Set boot up, or cancel boot up </param> 
        /// <param name="exeName"> the name in the registry </param> 
        /// <returns> whether the activation or deactivation is successful </returns> 
        public bool SetSelfStarting(bool started, string exeName)
        {
            RegistryKey key = null;
            try
            {
                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                RegistryKey HKLM = Registry.CurrentUser;
                key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true); //Open the registry subkey
                key = Registry.CurrentUser.OpenSubKey(" SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run ", true); //Open the registry subkey

                if (key == null) //If the item does not exist, create the sub item
                {
                    key = Registry.LocalMachine.CreateSubKey(" SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run ");
                }

                if (started)
                {
                    try
                    {
                        object ob = key.GetValue(exeName, -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals(" -1 "))
                            {
                                key.DeleteValue(exeName); //Cancel startup
                            }
                            key.SetValue(exeName, exeDir); //Set to boot
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue(exeName); //Cancel startup
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        private void WriteLog(object exception)
        {
            Exception ex = exception as Exception;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog.txt");

            using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.End);
                byte[] buffer = Encoding.Default.GetBytes(" --------------------------- ----------------------------\r\n ");
                fs.Write(buffer, 0, buffer.Length);

                buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + " \r\n ");
                fs.Write(buffer, 0, buffer.Length);

                if (ex != null)
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
    }


    /*public partial class App2 : Application
    {
        Mutex mutex;
        public App2()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, " TestEXEName ", out ret);

            if (!ret)
            {
                MessageBox.Show(" There is already a client running, please end the original client first! ");
                Environment.Exit(0);
            }
            #region Set the program to run automatically when booting (+ registry key)
            try
            {
                SetSelfStarting(true, " TestEXEName.exe ");
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }

            #endregion

        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                WriteLog(e.ExceptionObject);
            else
                WriteLog(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is Exception)
                WriteLog(e.Exception);
            else
                WriteLog(e);
        }

        #region Registry self-startup


        /// <summary> 
        ///Automatically start on boot
        /// </summary> 
        /// <param name="started"> Set boot up, or cancel boot up </param> 
        /// <param name=" exeName"> the name in the registry </param> 
        /// <returns> whether the activation or deactivation is successful </returns> 
        public bool SetSelfStarting(bool started, string exeName)
        {
            RegistryKey key = null;
            try
            {

                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                RegistryKey HKLM = Registry.CurrentUser;
                key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true); //Open the registry subkey
                key = Registry.CurrentUser.OpenSubKey(" SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run ", true); //Open the registry subkey

                if (key == null) //If the item does not exist, create the sub item
                {
                    key = Registry.LocalMachine.CreateSubKey(" SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run ");
                }
                if (started)
                {
                    try
                    {
                        object ob = key.GetValue(exeName, -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals(" -1 "))
                            {
                                key.DeleteValue(exeName); //Cancel startup
                            }
                            key.SetValue(exeName, exeDir); //Set to boot
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue(exeName); //Cancel startup
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                if (key != null)
                {
                    key.Close();
                }
                return false;
            }
        }

        #endregion

        private void WriteLog(object exception)
        {
            Exception ex = exception as Exception;

            using (FileStream fs = File.Open(" .//ErrorLog.txt ", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.End);
                byte[] buffer = Encoding.Default.GetBytes(" --------------------------- ----------------------------\r\n ");
                fs.Write(buffer, 0, buffer.Length);

                buffer = Encoding.Default.GetBytes(DateTime.Now.ToString() + " \r\n ");
                fs.Write(buffer, 0, buffer.Length);

                if (ex != null)
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
    }*/
}
