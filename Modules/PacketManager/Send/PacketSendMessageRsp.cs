using System;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules.PacketManager.Send
{
    internal class PacketSendMessageRsp : BasePacket
    {
        public PacketSendMessageRsp() : base(PacketIds.SendMessageRsp)
        {
            var proto = new SendMessageRsp()
            {
                RecievedTime = DateTime.Now.Ticks
            };

            SetData(proto);
        }
    }
}
