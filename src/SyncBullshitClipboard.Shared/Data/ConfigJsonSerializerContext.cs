using System.Text.Json.Serialization;
using SyncBullshitClipboard.Shared.Data.Common;

namespace SyncBullshitClipboard.Shared.Data;

[JsonSerializable(typeof(SyncConfig))]
[JsonSerializable(typeof(P2PConfig))]
[JsonSerializable(typeof(HttpConfig))]
[JsonSerializable(typeof(SecureConfig))]
[JsonSerializable(typeof(EncryptConfig))]
[JsonSerializable(typeof(WorkingMode))]
[JsonSerializable(typeof(AuthorizeMethod))]
public partial class ConfigJsonSerializerContext : JsonSerializerContext
{
    
}