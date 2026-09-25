using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface IOverlayConfigurationRepository
{
    Task<ConfigurationLoadResult> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(OverlayConfiguration configuration, CancellationToken cancellationToken = default);
}
