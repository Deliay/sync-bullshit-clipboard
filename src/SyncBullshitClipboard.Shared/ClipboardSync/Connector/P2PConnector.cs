using Microsoft.Extensions.Logging;
using Multiformats.Address;
using Nethermind.Libp2p.Core;
using Nethermind.Libp2p.Protocols;
using Nethermind.Libp2p.Protocols.Pubsub;
using SyncBullshitClipboard.Shared.Data;

namespace SyncBullshitClipboard.Shared.ClipboardSync.Connector;

public class P2PConnector : IConnector
{
    private readonly ILogger<P2PConnector>? _logger;
    private readonly SyncConfig _config;
    private readonly PubsubRouter _router;
    private readonly MDnsDiscoveryProtocol _mDnsDiscoveryProtocol;
    private readonly ILocalPeer _localPeer;
    private readonly ITopic _topic;
    private readonly Client _client;
    private readonly Multiaddress _address;

    public P2PConnector(ILogger<P2PConnector>? logger, IPeerFactory peerFactory, SyncConfig config,
        PubsubRouter router, MDnsDiscoveryProtocol mDnsDiscoveryProtocol)
    {
        _logger = logger;
        _config = config;
        _router = router;
        _mDnsDiscoveryProtocol = mDnsDiscoveryProtocol;
        mDnsDiscoveryProtocol.OnAddPeer += addresses =>
        {
            logger?.LogInformation("those addresses discovered: {}", string.Join('\n', addresses.Select(item => item)));
            return true;
        };
        var identity = config.P2P is { KeyType: not null, KeyPam.Length: > 0 }
            ? new Identity(Convert.FromBase64String(config.P2P.KeyPam), config.P2P.KeyType.Value)
            : new Identity();

        _address = Multiaddress.Decode($"/ip4/0.0.0.0/udp/0/http/p2p-circuit/p2p/{identity.PeerId}");
        _localPeer = peerFactory.Create(identity, _address);
        _topic = router.Subscribe(config.Topic);
        _topic.OnMessage += TopicOnOnMessage;
        _client = new Client($"{identity.PeerId}", config.Identity);
    }

    private void TopicOnOnMessage(byte[] obj)
    {
        _logger?.LogInformation("[IN] {} bytes was received from peers", obj.Length);
        var data = ClipboardSerializer.Deserialize(obj);
        OnClipboardDataArrived?.Invoke(this, new ClipboardDataArrivedEvent(_client, data));
    }

    public event EventHandler<ClipboardDataArrivedEvent>? OnClipboardDataArrived;
    
    public ValueTask PublishAsync(ClipboardData data, CancellationToken cancellationToken = default)
    {
        var serializeData = ClipboardSerializer.Serialize(data);
        
        _logger?.LogInformation("[OUT] {} bytes will send to peers", serializeData.Length);
        _topic.Publish(serializeData);
        
        return ValueTask.CompletedTask;
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Starting sync on topic={} in address {}", _config.Topic, _address);
        return _router.RunAsync(_localPeer, _mDnsDiscoveryProtocol, token: cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}