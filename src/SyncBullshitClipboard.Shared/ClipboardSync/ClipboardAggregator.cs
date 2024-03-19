using SyncBullshitClipboard.Shared.ClipboardSync;
using SyncBullshitClipboard.Shared.ClipboardSync.Connector;

namespace SyncBullshitClipboard.Shared.ClipboardSync;

public class ClipboardAggregator(
    IEnumerable<IConnector> connectors,
    IEnumerable<IClipboardSynchronizer> synchronizerList)
    : IDisposable
{
    private readonly List<IConnector> _connectors = connectors.ToList();
    private readonly List<IClipboardSynchronizer> _synchronizerList = synchronizerList.ToList();
    private readonly SemaphoreSlim _lock = new(1);
    private CancellationTokenSource _cts = null!;

    public async ValueTask RunAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        using var _ = _cts;
        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
            foreach (var connector in _connectors)
            {
                connector.OnClipboardDataArrived += ConnectorOnOnClipboardDataArrived;
            }

            foreach (var synchronizer in _synchronizerList)
            {
                synchronizer.OnCopied += SynchronizerOnOnCopied;
            }


            await Task.WhenAll(_connectors.Select(connector => connector.RunAsync(_cts.Token))
                .Concat(_synchronizerList.Select(synchronizer => synchronizer.RunAsync(_cts.Token))));
        }
        finally
        {
            UnsubscribeEvents();
            _lock.Release();
            using var __ = _cts;
        }
    }

    
    private async void SynchronizerOnOnCopied(object? sender, ClipboardData e)
    {
        foreach (var connector in _connectors)
        {
            await connector.PublishAsync(e, _cts.Token);
        }
    }

    private async void ConnectorOnOnClipboardDataArrived(object? sender, ClipboardDataArrivedEvent e)
    {
        foreach (var synchronizer in _synchronizerList)
        {
            await synchronizer.SetToClipboard(e.Sender, e.Data, _cts.Token);
        }
    }

    private void UnsubscribeEvents()
    {
        foreach (var connector in _connectors)
        {
            connector.OnClipboardDataArrived -= ConnectorOnOnClipboardDataArrived;
        }

        foreach (var synchronizer in _synchronizerList)
        {
            synchronizer.OnCopied -= SynchronizerOnOnCopied;
        }

    }
    
    public void Dispose()
    {
        UnsubscribeEvents();
        using var cts = _cts;
        using var @lock = _lock;
    }
}