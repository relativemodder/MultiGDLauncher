using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Resources;
using System.Windows.Threading;
using Newtonsoft.Json;
using PEUtils;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;
using System.Reflection;

namespace MultiGDLauncher
{
    internal class Utils
    {
        public delegate void EndedCopyingEventHandler();

        public static EndedCopyingEventHandler EndedCopying;
        public static Notifier SharedNotifier;
        public const int DefaultIconsCount = 7;

        private static NotifyIcon notifyIcon;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static string GetGDVersion(string executablePath)
        {
            PeHeaderReader reader = new PeHeaderReader(executablePath);
            long unixedTimestamp = reader.FileHeader.TimeDateStamp;

            Dictionary<long, string> vers = new Dictionary<long, string>()
            {
                [1419173053] = "1.900", [1419880840] = "1.910", [1421745341] = "1.920",
                [1440638199] = "2.000", [1440643927] = "2.001", [1443053232] = "2.010",
                [1443077847] = "2.011", [1443077847] = "2.020", [1484612867] = "2.100",
                [1484626658] = "2.101", [1484737207] = "2.102", [1510526914] = "2.110",
                [1510538091] = "2.111", [1510619253] = "2.112", [1511220108] = "2.113",
                [1702921605] = "2.200", [1704582672] = "2.201", [1704601266] = "2.202",
                [1704948277] = "2.203", [1705041028] = "2.204"
            };

            if (!vers.ContainsKey(unixedTimestamp))
            {
                return "Unknown";
            }

            return vers[unixedTimestamp];
        }

        public static string GetInstanceDirectory(string instanceName)
        {
            return $"{LauncherSettingsState.StoragePath}\\{instanceName}";
        }

        public static void OnPushClick(object sender, EventArgs e)
        {
            var window = System.Windows.Application.Current.MainWindow;
            if (window.WindowState == WindowState.Minimized)
                window.WindowState = WindowState.Normal;

            // Activate the form.
            window.Activate();
            notifyIcon.Visible = false;
        }

        public static void OnIconClick(object sender, EventArgs e)
        {
            OnPushClick(sender, e);
        }

        public static bool GetIsWindowFocused()
        {
            var myWindowHandle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;
            return myWindowHandle == GetForegroundWindow();
        }

        public static void SendPushNotification(string text, ToolTipIcon icon)
        {
            if (notifyIcon == null)
            {
                notifyIcon = new NotifyIcon();
            }
            notifyIcon.Text = text;

            var iconStream = System.Windows.Application.GetResourceStream(new Uri($"/Assets/notification-icon.ico", UriKind.Relative)).Stream;

            notifyIcon.Icon = new Icon(iconStream);
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipClicked += OnPushClick;
            notifyIcon.Click += OnIconClick;
            notifyIcon.ShowBalloonTip(3000, "MultiGD", text, icon);
        }

        public static MainWindow GetMainWindow()
        {
            return (MainWindow)System.Windows.Application.Current.MainWindow;
        }

        public static void InitDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static List<InstanceInfo> GetInstances()
        {
            var list = new List<InstanceInfo>();

            InitDirectory(LauncherSettingsState.StoragePath);

            foreach (string dirPath in Directory.GetDirectories(LauncherSettingsState.StoragePath))
            {
                var info = GetInstanceInfo(dirPath);
                list.Add(info);
            }

            return list;
        }

        public static void InitNotifier()
        {
            SharedNotifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: System.Windows.Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Dispatcher.CurrentDispatcher;
            });
        }

        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        public static InstanceInfo GetInstanceInfo(string instanceFolderPath)
        {
            string jsonContent = File.ReadAllText($"{instanceFolderPath}\\instance_info.json");
            return JsonConvert.DeserializeObject<InstanceInfo>(jsonContent);
        }

        public static void SetInstanceInfo(string instanceFolderPath, InstanceInfo instanceInfo)
        {
            File.WriteAllText($"{instanceFolderPath}\\instance_info.json", JsonConvert.SerializeObject(instanceInfo));
        }

        public static void CloneGD(string fromFolder, string toFolder, InstanceInfo instanceInfo)
        {
            InitDirectory(LauncherSettingsState.StoragePath);
            InitDirectory(toFolder);

            Task.Run(() =>
            {
                Uri uri = new Uri("/Assets/steam_appid.txt", UriKind.Relative);
                StreamResourceInfo info = System.Windows.Application.GetResourceStream(uri);

                using (FileStream fileStream = File.Create($"{toFolder}\\steam_appid.txt"))
                {
                    info.Stream.CopyTo(fileStream);
                }

                SetInstanceInfo(toFolder, instanceInfo);

                CopyFilesRecursively(fromFolder, toFolder);

                SetInstanceInfo(toFolder, instanceInfo); // for real

                Thread.Sleep(1000);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    EndedCopying?.Invoke();
                });
            });
        }
    }
}
