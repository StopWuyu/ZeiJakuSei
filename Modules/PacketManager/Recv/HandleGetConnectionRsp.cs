using System;

namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.GetConnectionInfoRsp)]
    internal class HandleGetConnectionRsp : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            ModBase.LastRecieved = DateTime.Now.Ticks;
        }
    }
}
