namespace SyncBullshitClipboard.Shared.ClipboardSync.Connector;

public class HttpConnector : IConnector
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public event EventHandler<ClipboardDataArrivedEvent>? OnClipboardDataArrived;
    
    public ValueTask PublishAsync(ClipboardData data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}