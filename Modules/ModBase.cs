using System;
using System.Threading;
using System.Windows;
using ZeiJakuSei.Controls;

namespace ZeiJakuSei.Modules
{
    static class ModBase
    {
        /// <summary>
        /// 关闭程序
        /// </summary>
        public static void Close()
        {
            Environment.Exit(0);
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
                return text == null ? defaultText : text;
            } catch
            {
                return defaultText;
            }
        }
    }
}
