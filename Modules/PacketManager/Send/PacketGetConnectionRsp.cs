using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeiJakuSei.Packets;

namespace ZeiJakuSei.Modules.PacketManager.Send
{
    internal class PacketGetConnectionRsp : BasePacket
    {
        public PacketGetConnectionRsp() : base(PacketIds.GetConnectionInfoRsp)
        {
            var proto = new GetConnectionInfoRsp()
            {
                ResponseTime = DateTime.Now.Ticks,
                Info = ConnectionInfo.Success
            };

            SetData(proto);
        }
    }
}
