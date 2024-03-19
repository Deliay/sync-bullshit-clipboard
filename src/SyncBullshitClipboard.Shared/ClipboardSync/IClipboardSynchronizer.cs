using SyncBullshitClipboard.Shared.ClipboardSync.Connector;

namespace SyncBullshitClipboard.Shared.ClipboardSync;

public interface IClipboardSynchronizer
{
    event EventHandler<ClipboardData> OnCopied;
    
    ValueTask SetToClipboard(Client client, ClipboardData data, CancellationToken cancellationToken);

    Task RunAsync(CancellationToken cancellationToken);
}