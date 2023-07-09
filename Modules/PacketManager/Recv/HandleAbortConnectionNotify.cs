namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.AbortConnectionNotify)]
    internal class HandleAbortConnectionNotify : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            ModBase.ConnectingUdpNetwork.Disconnect();
        }
    }
}
