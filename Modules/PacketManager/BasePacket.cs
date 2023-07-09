using Google.Protobuf;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ZeiJakuSei.Modules.PacketManager
{
    internal class BasePacket
    {
        private readonly int cmdId;
        private byte[]? data;
        public BasePacket(int cmdId)
        {
            this.cmdId = cmdId;
        }

        public BasePacket()
        {
            cmdId = PacketIds.None;
        }

        public BasePacket(int cmdId, IMessage message)
        {
            this.cmdId = cmdId;
            data = message.ToByteArray();
        }

        public int GetCmdId() => cmdId;

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        public void SetData(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="message"></param>
        public void SetData(IMessage message)
        {
            data = message.ToByteArray();
        }

        public void SendByUdp(UdpClient client, IPEndPoint endPoint)
        {
            using MemoryStream stream = new();
            using (BinaryWriter writer = new(stream))
            {
                writer.Write((uint)cmdId);
                writer.Write((uint)data!.Length);
                writer.Write((byte)0xA9);
                writer.Write(data!);
            }

            byte[] send = stream.ToArray();
            client.Send(send, send.Length, endPoint);
        }
    }
}
