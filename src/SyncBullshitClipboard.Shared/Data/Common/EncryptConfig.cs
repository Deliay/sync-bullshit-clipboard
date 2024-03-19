namespace SyncBullshitClipboard.Shared.Data.Common;

public record EncryptConfig(string? Base64PamCert, string? Base64PamKey, string? CertPassword);
