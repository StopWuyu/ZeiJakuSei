using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ZeiJakuSei.Modules.PacketManager;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules
{
    internal class MessageEventArgs : EventArgs
    {
        public string Message;

        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }
    internal class ExceptionEventArgs : EventArgs
    {
        public Exception exception;

        public ExceptionEventArgs(Exception ex)
        {
            exception = ex;
        }
    }

    internal class ConnectCallback
    {
        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 连接失败的异常类
        /// </summary>
        public Exception? Exception { get; set; }
        public ConnectCallback(bool success, Exception? ex)
        {
            Success = success;
            Exception = ex;
        }
    }

    internal class UdpConfig
    {
        public int Port { get; set; }

        public IPAddress Address { get; set; }

        public UdpConfig(IPAddress address, int port)
        {
            Address = address;
            Port = port;
        }
    }

    internal class ModUdpNetwork
    {
        private UdpClient? udpClient;
        private bool isConnected = false;
        private UdpConfig config = new(IPAddress.None, 8254);

        public event EventHandler<MessageEventArgs>? MessageReceived;
        public event EventHandler<MessageEventArgs>? MessageSent;
        public event EventHandler<ExceptionEventArgs>? Disconnected;
        public event EventHandler<ExceptionEventArgs>? ConnectingExceptionHandled;

        private readonly List<Thread> threadPool = new();

        //public ModUdpNetwork()
        //{
        //}

        public bool IsConnected
        {
            get { return isConnected; }
        }

        public ConnectCallback Connect(IPAddress ipAddress, int port)
        {
            try
            {
                udpClient = ipAddress.AddressFamily switch
                {
                    AddressFamily.InterNetwork => new UdpClient(),
                    AddressFamily.InterNetworkV6 => new UdpClient(AddressFamily.InterNetworkV6),
                    _ => throw new Exception("Invalid ip!")
                };

                config = new UdpConfig(ipAddress, port);
                // udpClient.Connect(ipAddress, port);
                ModBase.Log($"尝试连接： {ipAddress}:{port}", "Connecting");
                isConnected = true;
                //if (!SendMessage(PacketTestConnective))
                //{
                //    ModBase.Log($"连接失败： {ipAddress}:{port}", "Connecting");
                //    return new ConnectCallback(false, null);
                //}

                SendPacket(new BasePacket(PacketIds.GetConnectionInfoReq, new GetConnectionInfoReq()
                {
                    RequestTime = DateTime.Now.Ticks,
                    RsaPrivateKey = ModBase.RsaPrivateKey
                }));
                ModBase.LastRecieved = DateTime.Now.Ticks;
                threadPool.Add(ModBase.RunInNewThread(() =>
                {
                    while (true)
                    {
                        // 检测连接
                        if (ModBase.LastRecieved + 300000000 < DateTime.Now.Ticks)
                        {
                            ModBase.Log("断开连接！", "Connecting");
                            udpClient.Close();
                            isConnected = false;
                            ModBase.RunInUI(() =>
                            {
                                Disconnected?.Invoke(this, new ExceptionEventArgs(new Exception()));
                            });
                            StopAllThread();
                            break;
                        }
                    }
                }));
                threadPool.Add(ModBase.RunInNewThread(() =>
                {
                    while (isConnected)
                    {
                        try
                        {
                            SendPacket(new BasePacket(PacketIds.PingReq, new PingReq()
                            {
                                RequestTime = DateTime.Now.Ticks,
                                RsaPrivateKey = ModBase.RsaPrivateKey
                            }));
                            Thread.Sleep(10000);
                        } catch
                        {
                            isConnected = false;
                            StopAllThread();
                            break;
                        }
                    }
                }));
                return new ConnectCallback(true, null);
            }
            catch (Exception ex)
            {
                ModBase.Log("连接失败: " + ex.Message + "\r\n" + ex.StackTrace, "Connecting");
                isConnected = false;
                udpClient?.Close();
                return new ConnectCallback(false, ex);
            }
        }

        public void Disconnect()
        {
            udpClient?.Close();
            isConnected = false;
            StopAllThread();
            ModBase.RunInUI(() =>
            {
                Disconnected?.Invoke(this, new ExceptionEventArgs(new Exception()));
            });
        }

        public void StopAllThread()
        {
            if (threadPool != null)
            {
                foreach (var thread in threadPool)
                {
                    thread.Interrupt();
                }
                threadPool.Clear();
            }
        }

        public bool SendMessage(string message)
        {
            try
            {
                if (IsConnected)
                {
                    var key = ModCrypto.GenerateAesKey();
                    byte[] convertedKey = ModCrypto.EncryptRsaToBytes(key, ModBase.RsaPublicKey);
                    var convertedMessage = ModCrypto.EncryptAES(message, key);
                    SendPacket(new BasePacket(PacketIds.SendMessageReq, new SendMessageReq()
                    {
                        SendMessage = convertedMessage,
                        SentTime = DateTime.Now.Ticks,
                        AESKey = ByteString.CopyFrom(convertedKey)
                    }));
                    ModBase.Log($"发送信息：" + message, "Connecting");
                    MessageSent?.Invoke(this, new MessageEventArgs(message));
                    return true;
                }
                else
                {
                    ModBase.Log("发送信息失败: 未连接到目标主机", "Connecting");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModBase.Log("发送信息失败: " + ex.Message + "\r\n" + ex.StackTrace, "Connecting");
                MessageSent?.Invoke(this, new MessageEventArgs("发送信息失败: " + ex.Message));
                ConnectingExceptionHandled?.Invoke(this, new ExceptionEventArgs(ex));
                return false;
            }
        }

        public bool SendPacket(BasePacket packet)
        {
            if (udpClient == null)
            {
                return false;
            }
            new Session(udpClient, new IPEndPoint(config.Address, config.Port)).Send(packet);
            return true;
        }

        public void StartListening(int port, int retryCount = 0)
        {
            try
            {
                bool isIPv6 = Socket.OSSupportsIPv6;

                udpClient = isIPv6 ? new UdpClient(AddressFamily.InterNetworkV6) : new UdpClient();

                if (isIPv6)
                {
                    udpClient.Client.DualMode = true; // 允许同时监听 IPv4 和 IPv6 地址
                }

                udpClient.Client.Bind(new IPEndPoint(isIPv6 ? IPAddress.IPv6Any : IPAddress.Any, port));

                string logMessage = $"第{retryCount + 1}次监听本地端口：{port} ({(isIPv6 ? "IPv6/IPv4" : "IPv4")})";
                ModBase.Log(logMessage, "Listening");
                MessageReceived?.Invoke(this, new MessageEventArgs(logMessage));

                while (true)
                {
                    IPEndPoint remoteEndPoint = new IPEndPoint(isIPv6 ? IPAddress.IPv6Any : IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref remoteEndPoint);

                    // 处理接收到的数据包
                    PacketManager.PacketManager.ProcessPacket(udpClient, data, new IPEndPoint(config.Address, config.Port));
                }
            }
            catch (SocketException ex)
            {
                // 检查是否是不兼容的协议错误
                if (ex.SocketErrorCode == SocketError.ProtocolFamilyNotSupported)
                {
                    // 尝试使用另一种地址族再次启动监听器
                    if (retryCount < 1)
                    {
                        StartListening(port, retryCount + 1);
                        return;
                    }
                }

                string errorMessage = $"发生错误: {ex.Message}\r\n{ex.StackTrace}";
                ModBase.Log(errorMessage, "Listening");
                MessageReceived?.Invoke(this, new MessageEventArgs(errorMessage));
            }
            catch (Exception ex)
            {
                string errorMessage = $"发生错误: {ex.Message}\r\n{ex.StackTrace}";
                ModBase.Log(errorMessage, "Listening");
                MessageReceived?.Invoke(this, new MessageEventArgs(errorMessage));
            }
            finally
            {
                udpClient?.Close();
                udpClient = null;
            }
        }


        public void MessageRecieveEvent(MessageEventArgs args)
        {
            MessageReceived?.Invoke(this, args);
        }
    }
}
