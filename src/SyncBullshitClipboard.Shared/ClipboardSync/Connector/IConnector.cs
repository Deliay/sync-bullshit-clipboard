namespace SyncBullshitClipboard.Shared.ClipboardSync.Connector;

public interface IConnector : IAsyncDisposable
{
    event EventHandler<ClipboardDataArrivedEvent> OnClipboardDataArrived;

    ValueTask PublishAsync(ClipboardData data, CancellationToken cancellationToken = default);

    Task RunAsync(CancellationToken cancellationToken);
}
