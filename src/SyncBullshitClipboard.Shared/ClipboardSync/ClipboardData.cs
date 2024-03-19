using System.Runtime.InteropServices;
using ProtoBuf;

namespace SyncBullshitClipboard.Shared.ClipboardSync;

[ProtoContract]
public readonly struct ClipboardData
{
    [ProtoMember(1)]
    public ClipboardItem[] Items { get; init; }

    public int GetSize() => Items.Sum(item => item.GetSize());
}
