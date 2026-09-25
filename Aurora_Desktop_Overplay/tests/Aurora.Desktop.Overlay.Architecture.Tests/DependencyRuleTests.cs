using System.Xml.Linq;

namespace Aurora.Desktop.Overlay.Architecture.Tests;

public sealed class DependencyRuleTests
{
    [Fact]
    public void CoreDoesNotReferenceUiOrPlatformProjects()
    {
        var root = FindRepositoryRoot();
        var project = XDocument.Load(Path.Combine(root, "src", "Aurora.Desktop.Overlay.Core", "Aurora.Desktop.Overlay.Core.csproj"));
        var references = project.Descendants("ProjectReference").ToArray();
        Assert.Empty(references);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Aurora.Desktop.Overlay.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root was not found.");
    }
}
