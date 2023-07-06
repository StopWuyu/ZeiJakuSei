using System.Windows;

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
        }

        public static ResourceDictionary GetResourceDictionary()
        {
            return resourceDictionary!;
        }
    }
}
