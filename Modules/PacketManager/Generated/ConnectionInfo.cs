// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ConnectionInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ZeiJakuSei.Packets {

  /// <summary>Holder for reflection information generated from ConnectionInfo.proto</summary>
  public static partial class ConnectionInfoReflection {

    #region Descriptor
    /// <summary>File descriptor for ConnectionInfo.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ConnectionInfoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRDb25uZWN0aW9uSW5mby5wcm90bxISWmVpSmFrdVNlaS5QYWNrZXRzKi0K",
            "DkNvbm5lY3Rpb25JbmZvEgsKB1N1Y2Nlc3MQABIOCgpEaXNjb25uZWN0EAFi",
            "BnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::ZeiJakuSei.Packets.ConnectionInfo), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum ConnectionInfo {
    [pbr::OriginalName("Success")] Success = 0,
    [pbr::OriginalName("Disconnect")] Disconnect = 1,
  }

  #endregion

}

#endregion Designer generated code
