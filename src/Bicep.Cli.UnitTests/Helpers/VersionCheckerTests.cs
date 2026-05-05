// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using Bicep.Cli.Helpers;
using Bicep.Core.Utils;
using Bicep.TextFixtures.Mocks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.UnitTests.Helpers;

[TestClass]
public class VersionCheckerTests
{
    private const string CurrentCommitSha = "1111111111111111111111111111111111111111";
    private const string NewerCommitSha = "2222222222222222222222222222222222222222";

    [TestMethod]
    public void FindNewerVersions_ShouldReturnNewerVersionsWithGitCommitSha()
    {
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
            { "/home/user/.azure/bin/bicep", new MockFileData("fake binary") },
            { "/usr/local/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = CreateLinuxEnvironment();
        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0+" + NewerCommitSha },
            { "/home/user/.azure/bin/bicep", "0.20.0" },
            { "/usr/local/bin/bicep", "invalid" },
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        var result = versionChecker.FindNewerVersions(new Version("0.30.23"), CancellationToken.None);

        result.Should().BeEquivalentTo([
            new BicepInstallationVersion(new Version("0.35.0"), NewerCommitSha, "/home/user/.bicep/bin/bicep"),
        ]);
    }

    [TestMethod]
    public void FindNewerVersions_ShouldReturnVersionsSortedDescending()
    {
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
            { "/home/user/.azure/bin/bicep", new MockFileData("fake binary") },
            { "/usr/local/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = CreateLinuxEnvironment();
        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0" },
            { "/home/user/.azure/bin/bicep", "0.40.0" },
            { "/usr/local/bin/bicep", "0.38.5" },
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        var result = versionChecker.FindNewerVersions(new Version("0.30.23"), CancellationToken.None);

        result.Select(version => version.Version.ToString()).Should().Equal("0.40.0", "0.38.5", "0.35.0");
    }

    [TestMethod]
    public void FindNewerVersions_ShouldUseCurrentEnvironmentVersion()
    {
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = CreateLinuxEnvironment();
        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0" },
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        var result = versionChecker.FindNewerVersions();

        result.Should().ContainSingle(version => version.Version == new Version("0.35.0"));
    }

    [TestMethod]
    public void FindNewerVersions_ShouldTreatTrailingZeroRevisionAsSameVersion()
    {
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = CreateLinuxEnvironment();
        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.30.23.0" },
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        var result = versionChecker.FindNewerVersions(new Version("0.30.23"), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void FindNewerVersions_ShouldRespectCancellationToken()
    {
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
            { "/home/user/.azure/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = CreateLinuxEnvironment();
        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0" },
            { "/home/user/.azure/bin/bicep", "0.40.0" },
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var result = versionChecker.FindNewerVersions(new Version("0.30.0"), cancellationTokenSource.Token);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void GetWellKnownInstallLocations_ShouldReturnLinuxPaths_OnLinux()
    {
        var fileSystemMock = new MockFileSystem();
        var environmentMock = CreateLinuxEnvironment();

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        var locations = versionChecker.GetWellKnownInstallLocations();

        var normalizedLocations = locations.Select(location => location.Replace('\\', '/')).ToList();
        normalizedLocations.Should().Contain("/home/user/.bicep/bin");
        normalizedLocations.Should().Contain("/home/user/.azure/bin");
        normalizedLocations.Should().Contain("/usr/local/bin");
        normalizedLocations.Should().Contain("/usr/bin");
        locations.Should().NotContain(location => location.Contains("Program Files"));
    }

    [TestMethod]
    public void GetWellKnownInstallLocations_ShouldReturnWindowsPaths_OnWindows()
    {
        var fileSystemMock = new MockFileSystem();
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(environment => environment.GetVariable("HOME")).Returns((string?)null);
        environmentMock.Setup(environment => environment.GetVariable("USERPROFILE")).Returns(@"C:\Users\user");
        environmentMock.Setup(environment => environment.GetVariable("ProgramFiles")).Returns(@"C:\Program Files");
        environmentMock.Setup(environment => environment.GetVariable("ProgramFiles(x86)")).Returns(@"C:\Program Files (x86)");
        environmentMock.Setup(environment => environment.GetVariable("PATH")).Returns((string?)null);
        environmentMock.Setup(environment => environment.CurrentPlatform).Returns(OSPlatform.Windows);

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        var locations = versionChecker.GetWellKnownInstallLocations();

        locations.Should().HaveCount(4);
        locations.Should().Contain(location => location.Contains(@"user") && location.Contains(@".bicep") && location.Contains(@"bin"));
        locations.Should().Contain(location => location.Contains(@"user") && location.Contains(@".azure") && location.Contains(@"bin"));
        locations.Should().Contain(location => location.Contains(@"Program Files") && location.Contains(@"Bicep CLI") && !location.Contains("(x86)"));
        locations.Should().Contain(location => location.Contains(@"Program Files (x86)") && location.Contains(@"Bicep CLI"));
        locations.Should().NotContain("/usr/local/bin");
        locations.Should().NotContain("/usr/bin");
    }

    [TestMethod]
    public void GetWellKnownInstallLocations_ShouldIncludePathEntries()
    {
        var fileSystemMock = new MockFileSystem();
        var environmentMock = CreateLinuxEnvironment(path: "/custom/bin:/other/bin");

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        versionChecker.GetWellKnownInstallLocations().Should().Contain(["/custom/bin", "/other/bin"]);
    }

    [TestMethod]
    public void TryGetGitCommitSha_ShouldExtractFullShaFromNerdbankGitVersioningMetadata()
    {
        VersionChecker.TryGetGitCommitSha("Branch.main.Sha." + CurrentCommitSha).Should().Be(CurrentCommitSha);
    }

    private static Mock<IEnvironment> CreateLinuxEnvironment(string? path = null)
    {
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(environment => environment.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", CurrentCommitSha));
        environmentMock.Setup(environment => environment.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(environment => environment.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(environment => environment.GetVariable("PATH")).Returns(path);
        environmentMock.Setup(environment => environment.CurrentPlatform).Returns(OSPlatform.Linux);

        return environmentMock;
    }
}

internal class TestableVersionChecker : VersionChecker
{
    private readonly Dictionary<string, string> mockVersions;
    private readonly IFileSystem fileSystem;

    public TestableVersionChecker(IEnvironment environment, IFileSystem fileSystem, Dictionary<string, string> mockVersions)
        : base(environment, fileSystem)
    {
        this.fileSystem = fileSystem;
        this.mockVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var mockVersion in mockVersions)
        {
            this.mockVersions[NormalizePath(mockVersion.Key)] = mockVersion.Value;

            try
            {
                var fullPath = fileSystem.Path.GetFullPath(mockVersion.Key);
                this.mockVersions[NormalizePath(fullPath)] = mockVersion.Value;
            }
            catch
            {
                // Ignore path normalization failures in the mock file system.
            }
        }
    }

    protected override BicepInstallationVersion? GetBicepVersion(string bicepPath)
    {
        var normalizedPath = NormalizePath(bicepPath);

        if (mockVersions.TryGetValue(normalizedPath, out var versionString))
        {
            return TryParseVersionInfo(bicepPath, versionString);
        }

        try
        {
            var fullPath = fileSystem.Path.GetFullPath(bicepPath);
            var normalizedFullPath = NormalizePath(fullPath);

            if (mockVersions.TryGetValue(normalizedFullPath, out versionString))
            {
                return TryParseVersionInfo(bicepPath, versionString);
            }
        }
        catch
        {
            // Ignore path normalization failures in the mock file system.
        }

        return null;
    }

    private static string NormalizePath(string path)
        => path.Replace('\\', '/');
}
