using SyncBullshitClipboard.Shared.ClipboardSync.Connector;

namespace SyncBullshitClipboard.Shared.ClipboardSync;

public record struct ClipboardDataArrivedEvent(Client Sender, ClipboardData Data);
