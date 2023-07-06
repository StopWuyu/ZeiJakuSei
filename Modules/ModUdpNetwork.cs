using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ZeiJakuSei.Modules
{
    internal class MessageEventArgs : EventArgs
    {
        public string Message;
        public MessageEventArgs(string Message) 
        {
            this.Message = Message;
        }
    }
    internal class ModUdpNetwork
    {
        private UdpClient udpClient;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<MessageEventArgs> MessageSent;

        public ModUdpNetwork()
        {
            udpClient = new UdpClient();
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length);
                Console.WriteLine("发送信息: " + message);
                
                MessageSent.Invoke(this, new MessageEventArgs(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送信息失败: " + ex.Message);
            }
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                udpClient.Connect(ipAddress, port);
                Console.WriteLine("连接成功！");
            } catch (Exception ex)
            {
                Console.WriteLine("发送信息失败: " + ex.Message);
            }
        }

        public void StartListening(int port)
        {
            try
            {
                udpClient = new UdpClient(port);
                Console.WriteLine("监听本地端口： " + port);

                while (true)
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine("接收信息: " + message);
                    MessageReceived.Invoke(this, new MessageEventArgs(message));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误: " + ex.Message);
            }
            finally
            {
                udpClient.Close();
            }
        }
    }
}
