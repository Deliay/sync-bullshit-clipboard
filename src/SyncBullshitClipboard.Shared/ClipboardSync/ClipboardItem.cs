using System.Runtime.InteropServices;
using System.Text;
using ProtoBuf;

namespace SyncBullshitClipboard.Shared.ClipboardSync;

[ProtoContract]
public readonly struct ClipboardItem
{
    [ProtoMember(1)]
    public DataType Type { get; init; }
    [ProtoMember(2)]
    public string MimeType { get; init; }
    [ProtoMember(3)]
    public byte[] Data { get; init; }

    public int GetSize() => sizeof(DataType) + Encoding.UTF8.GetByteCount(MimeType) + Data.Length;
}
