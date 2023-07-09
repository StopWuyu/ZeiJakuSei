using System;

namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.PingRsp)]
    internal class HandlePingRsp : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            ModBase.LastRecieved = DateTime.Now.Ticks;
        }
    }
}
