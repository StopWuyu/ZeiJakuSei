using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ZeiJakuSei.Modules.PacketManager
{
    internal class Session
    {
        private UdpClient _client;
        private IPEndPoint _ip;
        public Session(UdpClient client, IPEndPoint endPoint)
        {
            _client = client;
            _ip = endPoint;
        }

        public UdpClient GetStream()
        {
            return _client;
        }

        public IPEndPoint IP()
        {
            return _ip;
        }

        public void Send(BasePacket packet)
        {
            ModBase.Log($"发送包：{packet.GetCmdId()}", "Connecting");
            packet.SendByUdp(_client, _ip);
        }
    }
}
