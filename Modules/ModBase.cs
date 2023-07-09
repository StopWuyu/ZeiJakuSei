using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using ZeiJakuSei.Controls;
using ZeiJakuSei.Modules.PacketManager;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules
{
    static class ModBase
    {
        /// <summary>
        /// 关闭程序
        /// </summary>
        public static void Close()
        {
            Log("程序正在关闭...", "System");
            ConnectingUdpNetwork.SendPacket(new BasePacket(PacketIds.AbortConnectionNotify, new AbortConnectionNotify()));
            Environment.Exit(0);
        }

        public const string Version = "Alpha-1.0";

        private static bool RefreshLogFile = false;

        /// <summary>
        /// 程序工作路径
        /// </summary>
        public static string WorkingPath = Directory.GetCurrentDirectory() + "\\";

        /// <summary>
        /// 日志保存路径
        /// </summary>
        public static string LogPath = WorkingPath + "ZJS\\";

        public static string RsaPublicKey = "";
        public static string RsaPrivateKey = "";

        public static string RemotePrivateKey = "";


        public static ModUdpNetwork ListeningUdpNetwork = new();
        public static ModUdpNetwork ConnectingUdpNetwork = new();

        public static long LastRecieved = 0;

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns>yyyy-MM-dd HH:mm:ss</returns>
        public static string GetTime()
        {
            return GetTime("yyyy-MM-dd HH:mm:ss");
        }
        
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <param name="format">格式化模板</param>
        /// <returns></returns>
        public static string GetTime(string format)
        {
            return DateTime.Now.ToString(format);
            
        }

        /// <summary>
        /// 生成提示框
        /// </summary>
        /// <param name="message">提示信息</param>
        public static void Hint(string message)
        {
            RunInUI(() =>
            {
                HintBar bar = new()
                {
                    Text = message,
                    IsHitTestVisible = false
                };

                var info = GetMainWindow().InfoBar;
                if (info.Children.Count >= 4)
                {
                    info.Children.RemoveAt(3);
                }
                if (info.Children.Count >= 3)
                {
                    ((HintBar)info.Children[2]).onLeave();
                }
                info.Children.Insert(0, bar);
                bar.onLoad();
            });
        }

        /// <summary>
        /// 获取主窗口
        /// </summary>
        public static MainWindow GetMainWindow()
        {
            return (MainWindow)Application.Current.MainWindow;
        }

        /// <summary>
        /// 生成一对rsa密钥
        /// </summary>
        /// <returns>List[0]为公钥 List[1]为私钥</returns>
        public static ArrayList GenerateRsaKey()
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            try
            {
                // 获取私钥和公钥。
                var publicKey = rsa.ToXmlString(false)!;
                var privateKey = rsa.ToXmlString(true)!;
                return new ArrayList { publicKey, privateKey };
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

        /// <summary>
        /// 在新线程中运行
        /// </summary>
        /// <param name="threadStart">线程方法</param>
        /// <returns></returns>
        public static Thread RunInNewThread(ThreadStart threadStart)
        {
            var thread = new Thread(threadStart);
            thread.Start();
            return thread;
        }

        /// <summary>
        /// 在UI线程运行方法
        /// </summary>
        /// <param name="action"></param>
        public static void RunInUI(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static string GetLocalizedText(string key, string defaultText = "Error")
        {
            try {
                var text = App.GetResourceDictionary()[key].ToString();
                return text ?? defaultText;
            } catch
            {
                return defaultText;
            }
        }

        /// <summary>
        /// 生成一条日志并写入Log.txt
        /// </summary>
        /// <param name="msg">自带格式</param>
        public static void Log(string msg, string task = "Main")
        {
            // 判断 LogPath 是否存在
            DirectoryInfo logDirectory = new(LogPath);
            if (!logDirectory.Exists)
            {
                logDirectory.Create();
            }
            FileInfo logFile = new(LogPath + "Log.txt");
            Log($"[{GetTime()}] [{task}] {msg}", logFile);
        }

        private static readonly object WritingLog = 0;

        /// <summary>
        /// 写入指定文件一条日志
        /// </summary>
        /// <param name="msg">不自带格式</param>
        /// <param name="logFile"></param>
        public static void Log(string msg, FileInfo logFile)
        {
            lock (WritingLog)
            {
                if (!RefreshLogFile && logFile.Exists)
                {
                    File.Move(logFile.FullName, LogPath + logFile.Name + ".bak", true);

                    logFile.Create().Close();
                }
                RefreshLogFile = true;
                var stream = logFile.AppendText();
                stream.WriteLine(msg);
                stream.Flush();
                stream.Close();
            }
        }
    }
}
