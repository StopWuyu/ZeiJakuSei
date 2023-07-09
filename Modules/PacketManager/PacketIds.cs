namespace ZeiJakuSei.Modules.PacketManager
{
    internal static class PacketIds
    {
        public const int None = 1000;

        // 包Id
        public const int GetConnectionInfoReq = 1001;
        public const int GetConnectionInfoRsp = 1002;
        public const int SendMessageReq = 1003;
        public const int SendMessageRsp = 1004;
        public const int AbortConnectionNotify = 1005;
        public const int PingReq = 1006;
        public const int PingRsp = 1007;
    }
}
