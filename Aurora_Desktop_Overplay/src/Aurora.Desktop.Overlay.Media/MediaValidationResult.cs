namespace Aurora.Desktop.Overlay.Media;

public sealed record MediaValidationResult(bool IsValid, string? ErrorMessage)
{
    public static MediaValidationResult Success { get; } = new(true, null);
    public static MediaValidationResult Failure(string message) => new(false, message);
}
