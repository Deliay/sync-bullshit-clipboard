using SyncBullshitClipboard.Shared.Data.Common;

namespace SyncBullshitClipboard.Shared.Data;

public record SyncConfig(
    string Topic,
    string Identity,
    List<WorkingMode> AllowModes,
    SecureConfig Secure,
    HttpConfig? Http,
    P2PConfig? P2P)
{
    public static readonly SyncConfig Default = new(
        "bullshit-clipboard", Guid.NewGuid().ToString(),
        [WorkingMode.P2P],
        new SecureConfig(AuthorizeMethod.AllowAnonymous, null, false, null),
        null, null);
}
