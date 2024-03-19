using Gdk;
using Gtk;
using SyncBullshitClipboard.Shared.ClipboardSync;

namespace SyncBullshitClipboard.Shared.Gtk.Synchronizer;

public class GtkDataContract(ClipboardData clipboardData)
{
    public bool IsHoldingClipboardOwner { get; private set; } = true;
    
    private TargetEntry[]? _entries = null;

    public TargetEntry[] Entries => _entries ??= clipboardData.Items.Select((item, idx) => new TargetEntry()
    {
        Target = item.MimeType,
        Flags = TargetFlags.App,
        Info = (uint)idx
    }).ToArray();

    public void FillSelection(Clipboard clipboard, SelectionData selection, uint info)
    {
        if (clipboardData.Items.Length < info) return;
        var item = clipboardData.Items[info];
        var type = Atom.Intern(item.MimeType, false);
        selection.Set(type, (int)item.Type, item.Data);
    }

    public void FreeSelection(Clipboard clipboard)
    {
        IsHoldingClipboardOwner = false;
    }
}