using Gdk;
using GLib;
using Gtk;
using Microsoft.Extensions.Logging;
using SyncBullshitClipboard.Shared.ClipboardSync;
using SyncBullshitClipboard.Shared.ClipboardSync.Connector;
using SyncBullshitClipboard.Shared.Data;
using Application = Gtk.Application;
using Task = System.Threading.Tasks.Task;

namespace SyncBullshitClipboard.Shared.Gtk.Synchronizer;

public class GtkSynchronizer : IClipboardSynchronizer, IDisposable
{
    private readonly ILogger<GtkSynchronizer>? _logger;
    private readonly SyncConfig _config;
    private Clipboard _clipboard;
    private readonly Task _gtkTask;
    private GtkDataContract? _contract;

    private Task _gtkThread;
    
    public GtkSynchronizer(ILogger<GtkSynchronizer>? logger, SyncConfig config)
    {
        _logger = logger;
        _config = config;
        Application.Init();
        TaskCompletionSource tcsShutdown = new ();
        _gtkTask = tcsShutdown.Task;
        _gtkThread = Task.Run(() =>
        {
            Application.Run();
            GLib.Application.Default.Shutdown += (_, _) => tcsShutdown.TrySetResult();
        });
    }

    private void ClipboardOnOwnerChange(object o, OwnerChangeArgs args)
    {
        if (o is not Clipboard c) return;
        if (_contract is not null && _contract.IsHoldingClipboardOwner) return;
        
        c.RequestTargets(ClipboardContentReady);
    }

    private unsafe void ClipboardContentReady(Clipboard clipboard, Atom atoms, int n)
    {
        if (atoms.Owned) return;
        
        var span = new ReadOnlySpan<IntPtr>(atoms.Handle.ToPointer(), n);
        List<ClipboardItem> contents = [];
        foreach (var item in span)
        {
            var type = (Atom)Opaque.GetOpaque(item, typeof(Atom), atoms.Owned);
            var data = clipboard.WaitForContents(type);
            if (data is null) continue;
            
            contents.Add(new ClipboardItem()
            {
                MimeType = type.Name,
                Data = data.Data,
                Type = (DataType)data.Format,
            });
        }

        if (contents.Count == 0) return;
        
        _logger?.LogInformation("Load {} items from clipboard", contents.Count);
        OnCopied?.Invoke(this, new ClipboardData()
        {
            Items = contents.ToArray(),
        });
    }

    public event EventHandler<ClipboardData>? OnCopied;
    public ValueTask SetToClipboard(Client client, ClipboardData data, CancellationToken cancellationToken)
    {
        if (client.Name == _config.Identity) return ValueTask.CompletedTask;
        
        _contract = new GtkDataContract(data);
        _clipboard.SetWithData(_contract.Entries, _contract.FillSelection, _contract.FreeSelection);
        
        return ValueTask.CompletedTask;
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        
        _clipboard = Clipboard.Get(Gdk.Selection.Clipboard);
        _clipboard.OwnerChange += ClipboardOnOwnerChange;
        _logger?.LogInformation("GTK synchronizer was listening clipboard changes");
        return Task.WhenAll(_gtkThread, _gtkTask.WaitAsync(cancellationToken));
    }

    public void Dispose()
    {
        Application.Quit();
    }
}