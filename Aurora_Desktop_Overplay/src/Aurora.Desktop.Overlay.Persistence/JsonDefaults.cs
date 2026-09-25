using System.Text.Json;

namespace Aurora.Desktop.Overlay.Persistence;

public static class JsonDefaults
{
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web) { WriteIndented = true };
}
