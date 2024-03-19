using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace SyncBullshitClipboard.Shared.ClipboardSync;

public static class ClipboardSerializer
{
    public static byte[] Serialize(ClipboardData data)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, data);

        return ms.ToArray();
    }

    public static ClipboardData Deserialize(byte[] data)
    {
        return Serializer.Deserialize<ClipboardData>(new ReadOnlySpan<byte>(data));
    }
}