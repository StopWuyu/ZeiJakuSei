namespace ZeiJakuSei.Modules.PacketManager
{
    [Opcode(PacketIds.None)]
    internal abstract class PacketHandler
    {
        abstract public void Handle(Session session, byte[] data);
    }
}
