using System;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules.PacketManager.Send
{
    internal class PacketPingRsp : BasePacket
    {
        public PacketPingRsp() : base(PacketIds.PingRsp)
        {
            SetData(new PingRsp()
            {
                ResponseTime = DateTime.Now.Ticks
            });
        }
    }
}
