using System;

namespace ZeiJakuSei.Modules.PacketManager
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class Opcode : Attribute
    {
        readonly int _cmdId;
        public Opcode(int CmdId)
        { 
            _cmdId = CmdId;
        }

        public int CmdId { get { return _cmdId; } }
    }
}
