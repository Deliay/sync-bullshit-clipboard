using Microsoft.Extensions.DependencyInjection;
using SyncBullshitClipboard.Shared.ClipboardSync;

namespace SyncBullshitClipboard.Shared.Gtk.Synchronizer;

public static class GtkSynchronizerExtensions
{
    public static IServiceCollection AddGtkSynchronizer(this IServiceCollection serviceCollection)
    {

        serviceCollection.AddSingleton<IClipboardSynchronizer, GtkSynchronizer>();
        return serviceCollection;
    }
}