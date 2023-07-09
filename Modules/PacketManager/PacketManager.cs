using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace ZeiJakuSei.Modules.PacketManager
{
    internal class PacketManager
    {
        private static readonly Dictionary<int, Type> opcodeClassCache = new();

        public static Type? FindClassByOpcode(int cmdid)
        {
            if (opcodeClassCache.ContainsKey(cmdid))
            {
                return opcodeClassCache[cmdid];
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                Opcode? opcodeAttribute = type.GetCustomAttribute<Opcode>();

                if (opcodeAttribute != null && opcodeAttribute.CmdId == cmdid)
                {
                    opcodeClassCache.Add(cmdid, type);
                    return type;
                }
            }

            return null; // 如果未找到对应 cmdid 的类，则返回 null
        }

        /// <summary>
        /// 处理数据包
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="cmdId">包Id</param>
        /// <param name="bytes">数据</param>
        public static void ProcessPacket(UdpClient writer, int cmdId, byte[] bytes, IPEndPoint endPoint)
        {
            Type? classType = FindClassByOpcode(cmdId);
            if (classType == null)
            {
                // 未建立相应的包处理器
                return;
            }
            if (typeof(PacketHandler).IsAssignableFrom(classType))
            {
                PacketHandler? packetHandler = Activator.CreateInstance(classType) as PacketHandler;
                packetHandler?.Handle(new Session(writer, endPoint), bytes);
            }
        }

        /// <summary>
        /// 处理数据包
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="bytes">包含cmdId的原始数据</param>
        public static void ProcessPacket(UdpClient writer, byte[] bytes, IPEndPoint endPoint)
        {
            if (bytes.Length < 8)
            {
                // 数据长度不足，无法解析cmdId和length
                throw new ArgumentException("Invalid packet format");
            }

            int cmdId = BitConverter.ToInt32(bytes, 0); // 获取 cmdId（4 字节）
            int length = BitConverter.ToInt32(bytes, 4); // 获取 length（4 字节）

            if (bytes.Length < 8 + length)
            {
                // 数据长度不足，无法获取完整的数据
                throw new ArgumentException("Invalid packet format");
            }

            byte[] data = new byte[length];
            Array.Copy(bytes, 9, data, 0, length); // 获取数据

            ModBase.Log($"接收包：{cmdId} 长度-{length}", "Listening");
            ProcessPacket(writer, cmdId, data, endPoint);
        }
    }
}
