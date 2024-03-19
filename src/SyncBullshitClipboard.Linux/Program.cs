using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SyncBullshitClipboard.Shared.ClipboardSync;
using SyncBullshitClipboard.Shared.DependencyInjection;
using SyncBullshitClipboard.Shared.Gtk.Synchronizer;


var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(cfg => cfg.AddSimpleConsole().SetMinimumLevel(LogLevel.Trace));
await serviceCollection.ConfigureClipboardSync(args);
serviceCollection.AddGtkSynchronizer();

var services = serviceCollection.BuildServiceProvider();

using var clipboardAggregator = services.GetRequiredService<ClipboardAggregator>();
using var cts = new CancellationTokenSource();

await clipboardAggregator.RunAsync(cts.Token);
