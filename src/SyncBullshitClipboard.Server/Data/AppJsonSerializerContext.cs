using System.Text.Json.Serialization;

namespace SyncBullshitClipboard.Server.Data;

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}