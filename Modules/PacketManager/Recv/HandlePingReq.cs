using ZeiJakuSei.Modules.PacketManager.Send;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.PingReq)]
    internal class HandlePingReq : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            var proto = PingReq.Parser.ParseFrom(data);
            ModBase.RemotePrivateKey = proto.RsaPrivateKey;
            session.Send(new PacketPingRsp());
        }
    }
}
