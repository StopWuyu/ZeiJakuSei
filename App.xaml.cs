using System.Windows;
using System.Windows.Threading;
using ZeiJakuSei.Modules;

namespace ZeiJakuSei
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static ResourceDictionary? resourceDictionary;
        public void AppStartup(object sender, StartupEventArgs e)
        {
            // 获取Application对象
            var application = (Application)sender;

            // 获取App.xaml中定义的资源字典
            resourceDictionary = application.Resources;

            var keys = ModBase.GenerateRsaKey();
            ModBase.RsaPublicKey = (string)keys[0]!;
            ModBase.RsaPrivateKey = (string)keys[1]!;
            ModBase.Log("程序已启动！", "System");
            ModBase.Log("版本：" + ModBase.Version, "System");
        }

        public static ResourceDictionary GetResourceDictionary()
        {
            return resourceDictionary!;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ModBase.Log($"发生了未经处理的异常：{e.Exception.Message}\r\n{e.Exception.StackTrace}", "Exception");
            ModBase.Hint("发生了未经处理的异常！");

            e.Handled = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ModBase.Close();
        }
    }
}
