using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeiJakuSei.Modules;

namespace ZeiJakuSei.Pages
{
    /// <summary>
    /// PageUdpManager.xaml 的交互逻辑
    /// </summary>
    public partial class PageUdpManager : Page
    {
        bool connected = false;
        bool listening = false;

        public PageUdpManager()
        {
            InitializeComponent();

            ModBase.ListeningUdpNetwork = new();
            ModBase.ConnectingUdpNetwork = new();
        }

        private void InputMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ModBase.ConnectingUdpNetwork.SendMessage(UserName.Text + "：" + InputMessage.Text);
            InputMessage.Text = string.Empty;

            e.Handled = true;
        }

        private void ListeningButton_Click(object sender, RoutedEventArgs e)
        {
            if (!listening)
            {
                if (!int.TryParse(ListeningPort.Text, out int port))
                {
                    ModBase.Log($"端口 {ListeningPort.Text} 不合法！", "Check");
                    ListeningPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                if (port < 1 || port > 65535)
                {
                    ModBase.Log($"端口 {ListeningPort.Text} 不合法！", "Check");
                    ListeningPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                ModBase.ListeningUdpNetwork.MessageReceived += (sender, e) =>
                {
                    ModBase.RunInUI(() =>
                    {
                        bool shouldScrollToEnd = IsTextBoxScrolledToEnd();
                        Log.Text += "接收：" + e.Message + "\r\n";
                        if (shouldScrollToEnd)
                        {
                            Log.ScrollToEnd();
                        }
                    });
                };
                ModBase.RunInNewThread(() =>
                {
                    ModBase.ListeningUdpNetwork.StartListening(port);
                    listening = false;
                });
                listening = true;
                ListeningButton.IsEnabled = false;
            }

            if (listening && connected)
            {
                ListeningButton.Visibility = Visibility.Hidden;
                ConnectingButton.Visibility = Visibility.Hidden;
                ListeningPort.Visibility = Visibility.Hidden;
                ConnectingPort.Visibility = Visibility.Hidden;

                SendButton.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Visible;

                InputMessage.Tag = "请输入要发送的信息";
            }

            e.Handled = true;
        }

        private bool IsTextBoxScrolledToEnd()
        {
            double verticalOffset = Log.VerticalOffset;
            double extentHeight = Log.ExtentHeight;
            double viewportHeight = Log.ViewportHeight;

            // 判断滚动条位置是否在最底部
            return verticalOffset + viewportHeight >= extentHeight;
        }

        private void ConnectingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connected)
            {
                var ip = InputMessage.Text;
                // 检测IP是否合法
                if (!IPAddress.TryParse(ip, out IPAddress? _address))
                {
                    // 不合法
                    // InputMessage.Text = string.Empty;
                    ModBase.Hint(ModBase.GetLocalizedText("InvalidIP"));
                    ModBase.Log(ModBase.GetLocalizedText("InvalidIP"), "Check");
                    return;
                }
                if (!int.TryParse(ConnectingPort.Text, out int port))
                {
                    ModBase.Log($"端口 {ConnectingPort.Text} 不合法！", "Check");
                    ConnectingPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                {
                    ModBase.Log($"端口 {ConnectingPort.Text} 不合法！", "Check");
                    ConnectingPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                var callback = ModBase.ConnectingUdpNetwork.Connect(_address , port);
                if (callback.Success)
                {
                    connected = true;
                    InputMessage.Text = string.Empty;
                    ConnectingButton.IsEnabled = false;
                    ModBase.ConnectingUdpNetwork.MessageSent += ConnectingUdpNetwork_MessageSent;
                    ModBase.ConnectingUdpNetwork.Disconnected += ConnectingUdpNetwork_Disconnected;
                    ModBase.ConnectingUdpNetwork.ConnectingExceptionHandled += ConnectingUdpNetwork_Disconnected;
                    bool shouldScrollToEnd = IsTextBoxScrolledToEnd();
                    Log.Text += "连接成功！ \r\n";
                    // 如果允许自动滚动到最底部，则滚动到最底部
                    if (shouldScrollToEnd)
                    {
                        Log.ScrollToEnd();
                    }
                } 
                else
                {
                    connected = false;
                    bool shouldScrollToEnd = IsTextBoxScrolledToEnd();
                    Log.Text += $"连接失败：{callback.Exception?.Message} \r\n";
                    if (shouldScrollToEnd)
                    {
                        Log.ScrollToEnd();
                    }
                }
            }

            if (listening && connected)
            {
                ListeningButton.Visibility = Visibility.Hidden;
                ConnectingButton.Visibility = Visibility.Hidden;
                ListeningPort.Visibility = Visibility.Hidden;
                ConnectingPort.Visibility = Visibility.Hidden;

                SendButton.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Visible;

                InputMessage.Tag = "请输入要发送的信息";
            }

            e.Handled = true;
        }

        private void ConnectingUdpNetwork_MessageSent(object? sender, MessageEventArgs e)
        {
            ModBase.RunInUI(() =>
            {
                bool shouldScrollToEnd = IsTextBoxScrolledToEnd();
                Log.Text += "发送：" + e.Message + "\r\n";
                if (shouldScrollToEnd)
                {
                    Log.ScrollToEnd();
                }
            });
        }

        private void ConnectingUdpNetwork_Disconnected(object? sender, Modules.ExceptionEventArgs e)
        {
            connected = false;
            ConnectingButton.IsEnabled = true;

            ModBase.ConnectingUdpNetwork.MessageSent -= ConnectingUdpNetwork_MessageSent;
            ModBase.ConnectingUdpNetwork.Disconnected -= ConnectingUdpNetwork_Disconnected;

            ListeningButton.Visibility = Visibility.Visible;
            ConnectingButton.Visibility = Visibility.Visible;
            ListeningPort.Visibility = Visibility.Visible;
            ConnectingPort.Visibility = Visibility.Visible;

            SendButton.Visibility = Visibility.Hidden;
            UserPanel.Visibility = Visibility.Hidden;

            InputMessage.Tag = "请输入连接IP地址";
        }
    }
}
