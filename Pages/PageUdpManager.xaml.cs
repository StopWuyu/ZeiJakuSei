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
        ModUdpNetwork listeningUdpNetwork;
        ModUdpNetwork connectingUdpNetwork;

        public PageUdpManager()
        {
            InitializeComponent();

            listeningUdpNetwork = new();
            connectingUdpNetwork = new();
        }

        private void InputMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            connectingUdpNetwork.SendMessage(UserName.Text + "：" + InputMessage.Text);
            InputMessage.Text = string.Empty;

            e.Handled = true;
        }

        private void ListeningButton_Click(object sender, RoutedEventArgs e)
        {
            if (!listening)
            {
                if (!int.TryParse(ListeningPort.Text, out int port))
                {
                    ListeningPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                if (port < 1 || port > 65535)
                {
                    ListeningPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                listeningUdpNetwork.MessageReceived += (sender, e) =>
                {
                    ModBase.RunInUI(() =>
                    {
                        Log.Text += "接收：" + e.Message + "\r\n";
                    });
                };
                ModBase.RunInNewThread(() =>
                {
                    listeningUdpNetwork.StartListening(port);
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

        private void ConnectingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connected)
            {
                var ip = InputMessage.Text;
                // 检测IP是否合法
                if (!IPAddress.TryParse(ip, out _))
                {
                    // 不合法
                    InputMessage.Text = string.Empty;
                    ModBase.Hint(ModBase.GetLocalizedText("InvalidIP"));
                    return;
                }
                if (!int.TryParse(ConnectingPort.Text, out int port))
                {
                    ConnectingPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                {
                    ConnectingPort.Text = "8254";
                    ModBase.Hint("端口不合法！");
                    return;
                }
                connectingUdpNetwork.MessageSent += (sender, e) =>
                {
                    ModBase.RunInUI(() =>
                    {
                        Log.Text += "发送：" + e.Message + "\r\n";
                    });
                };
                ModBase.RunInNewThread(() =>
                {
                    connectingUdpNetwork.Connect(ip, port);
                });
                connected = true;
                InputMessage.Text = string.Empty;
                ConnectingButton.IsEnabled = false;
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
    }
}
