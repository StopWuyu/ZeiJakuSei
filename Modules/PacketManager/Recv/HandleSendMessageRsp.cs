using System;

namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.SendMessageRsp)]
    internal class HandleSendMessageRsp : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            ModBase.LastRecieved = DateTime.Now.Ticks;
        }
    }
}
