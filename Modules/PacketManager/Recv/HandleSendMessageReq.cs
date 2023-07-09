using ZeiJakuSei.Modules.PacketManager.Send;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules.PacketManager.Recv
{
    [Opcode(PacketIds.SendMessageReq)]
    internal class HandleSendMessageReq : PacketHandler
    {
        public override void Handle(Session session, byte[] data)
        {
            var proto = SendMessageReq.Parser.ParseFrom(data);

            // 解密 AES 密钥
            var originalKeyBytes = ModCrypto.DecryptRsaFromBytes(proto.AESKey.ToByteArray(), ModBase.RemotePrivateKey);
            // 解密消息
            var originalMessage = ModCrypto.DecryptAES(proto.SendMessage, originalKeyBytes);
            ModBase.Log($"接收信息：密钥-{ModCrypto.ToHexString(originalKeyBytes)} 原文-{originalMessage}", "Debugging");
            ModBase.ListeningUdpNetwork.MessageRecieveEvent(new MessageEventArgs(originalMessage));

            session.Send(new PacketSendMessageRsp());
        }
    }
}
