namespace SyncBullshitClipboard.Shared.Data.Common;

public record SecureConfig(
    AuthorizeMethod AuthorizeMethod, string? Password, 
    bool EnabledCertEncrypt, EncryptConfig? Encrypt);
