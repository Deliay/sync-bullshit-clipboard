using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Nethermind.Libp2p.Protocols;
using Nethermind.Libp2p.Stack;
using SyncBullshitClipboard.Shared.ClipboardSync.Connector;
using SyncBullshitClipboard.Shared.ClipboardSync;
using SyncBullshitClipboard.Shared.Data;
using SyncBullshitClipboard.Shared.Data.Common;

namespace SyncBullshitClipboard.Shared.DependencyInjection;

public static class ConfigExtensions
{
    private static readonly JsonSerializerOptions ConfigSerializeOptions = new()
    {
        TypeInfoResolverChain = { ConfigJsonSerializerContext.Default }
    };
    
    public static async ValueTask<IServiceCollection> ConfigureClipboardSync(this IServiceCollection serviceCollection, IEnumerable<string> args)
    {
        var configPath = args.Where((arg) => arg.StartsWith("--config="))
            .Select(arg => arg[9..])
            .FirstOrDefault() ?? "config.json";

        if (!File.Exists(configPath))
        {
            await File.WriteAllTextAsync(configPath,
                JsonSerializer.Serialize(SyncConfig.Default, ConfigSerializeOptions));
        }
        
        await using var configStream = File.OpenRead(configPath);
        var config = await JsonSerializer.DeserializeAsync<SyncConfig>(configStream, ConfigSerializeOptions)
            ?? SyncConfig.Default;
        
        serviceCollection.AddSingleton(config);
        
        serviceCollection.AddLibp2p(builder => builder);
        serviceCollection.AddTransient<MDnsDiscoveryProtocol>();
        
        foreach (var allowMode in config.AllowModes)
        {
            switch (allowMode)
            {
                case WorkingMode.P2P:
                    serviceCollection.AddSingleton<IConnector, P2PConnector>();
                    break;
                case WorkingMode.Http:
                    serviceCollection.AddSingleton<IConnector, HttpConnector>();
                    break;
                default:
                    throw new InvalidDataException($"{nameof(config.AllowModes)} is invalid");
            }
        }

        serviceCollection.AddSingleton<ClipboardAggregator>();
        serviceCollection.AddSingleton(new IdentifyProtocolSettings()
        {
            ProtocolVersion = "ipfs/1.0.0",
            AgentVersion = "sync-bullshit-clipboard/1.0.0"
        });
        return serviceCollection;
    }
}