using ZeiJakuSei.Modules.PacketManager.Send;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.GetConnectionInfoReq)]
    internal class HandleGetConnectionReq : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            var proto = GetConnectionInfoReq.Parser.ParseFrom(data);
            ModBase.RemotePrivateKey = proto.RsaPrivateKey;
            session.Send(new PacketGetConnectionRsp());
        }
    }
}
