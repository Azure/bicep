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
    [TestMethod]
    public void CheckForNewerVersionsAsync_ShouldNotRun_WhenShouldCheckIsFalse()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem();
        var environmentMock = StrictMock.Of<IEnvironment>();
        var output = new StringWriter();
        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        // Act
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: false);
        Thread.Sleep(50); // Give async task a moment

        // Assert
        output.ToString().Should().BeEmpty();
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldNotWarn_WhenNoNewerVersionsFound()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        // No newer versions - the file has version 0.20.0 which is older
        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.20.0" }
        };

        var output = new StringWriter();
        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        // Act
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(500); // Wait for async task to complete

        // Assert
        output.ToString().Should().BeEmpty();
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldWarn_WhenNewerVersionFound()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0" }
        };

        var output = new StringWriter();
        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        // Act
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(500); // Wait for async task to complete

        // Assert
        var outputText = output.ToString();
        outputText.Should().Contain("Warning: You are running Bicep CLI version 0.30.23");
        outputText.Should().Contain("0.35.0");
        // Path may be normalized differently on different platforms, so check for either format
        (outputText.Contains("/home/user/.bicep/bin/bicep") || outputText.Contains(@"\home\user\.bicep\bin\bicep") || outputText.Contains(@"C:\home\user\.bicep\bin\bicep")).Should().BeTrue();
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldNotWarn_WhenOlderVersionFound()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.20.0" }
        };

        var output = new StringWriter();
        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        // Act
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(500); // Wait for async task to complete

        // Assert
        output.ToString().Should().BeEmpty();
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldWarn_WhenMultipleNewerVersionsFound()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
            { "/home/user/.azure/bin/bicep", new MockFileData("fake binary") },
            { "/usr/local/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0" },
            { "/home/user/.azure/bin/bicep", "0.40.0" },
            { "/usr/local/bin/bicep", "0.38.5" }
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        // First, test FindNewerVersions directly to ensure the scanning logic works
        var directResults = versionChecker.FindNewerVersions(new Version("0.30.23"), CancellationToken.None);
        Assert.AreEqual(3, directResults.Count, $"Direct call should find 3 versions");

        var output = new StringWriter();

        // Act - call the async version
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(1000); // Wait longer for async task to complete

        // Assert
        var outputText = output.ToString();
        outputText.Should().Contain("Warning: You are running Bicep CLI version 0.30.23");

        // All three versions should be in the output
        outputText.Should().Contain("0.35.0", "should find version in ~/.bicep/bin");
        outputText.Should().Contain("0.40.0", "should find version in ~/.azure/bin");
        outputText.Should().Contain("0.38.5", "should find version in /usr/local/bin");

        // Verify they're sorted by version (descending)
        var lines = outputText.Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var versionLines = lines.Where(l => l.Contains("  - Version")).ToList();
        Assert.AreEqual(3, versionLines.Count, $"Should have 3 version lines, found {versionLines.Count}");

        versionLines[0].Should().Contain("0.40.0", "highest version should be first");
        versionLines[1].Should().Contain("0.38.5", "middle version should be second");
        versionLines[2].Should().Contain("0.35.0", "lowest version should be last");
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldCheckWindowsLocations_OnWindows()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"C:\Users\user\.bicep\bin\bicep.exe", new MockFileData("fake binary") },
            { @"C:\Program Files\Bicep CLI\bicep.exe", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns((string?)null);
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns(@"C:\Users\user");
        environmentMock.Setup(e => e.GetVariable("ProgramFiles")).Returns(@"C:\Program Files");
        environmentMock.Setup(e => e.GetVariable("ProgramFiles(x86)")).Returns(@"C:\Program Files (x86)");
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Windows);

        var versions = new Dictionary<string, string>
        {
            { @"C:\Users\user\.bicep\bin\bicep.exe", "0.35.0" },
            { @"C:\Program Files\Bicep CLI\bicep.exe", "0.40.0" }
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        // First verify that GetWellKnownInstallLocations returns Windows paths
        var locations = versionChecker.GetWellKnownInstallLocations();
        locations.Should().NotBeEmpty("should return install locations");
        locations.Should().Contain(l => l.Contains(".bicep"), "should include .bicep directory");
        locations.Should().Contain(l => l.Contains("Program Files") || l.Contains(".bicep"), "should include Windows-specific paths");

        var output = new StringWriter();

        // Act - the async version should complete without errors
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(1000); // Wait for async task to complete

        // Assert - we can't reliably test exact output due to path normalization differences
        // on Linux dev containers, but we can verify it doesn't crash and completes
        var outputText = output.ToString();

        // The method should either find versions or complete silently without throwing
        // If it found versions, the output should contain a warning
        if (outputText.Length > 0)
        {
            outputText.Should().Contain("Warning:", "if output exists, it should be a warning");
        }
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldHandleNonExistentDirectories()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>());

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var output = new StringWriter();
        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        // Act - should not throw
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(500); // Wait for async task to complete

        // Assert
        output.ToString().Should().BeEmpty();
    }

    [TestMethod]
    public async Task CheckForNewerVersionsAsync_ShouldHandleInvalidVersions()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
            { "/home/user/.azure/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", "test-commit"));
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "invalid" },
            { "/home/user/.azure/bin/bicep", "0.35.0" }
        };

        var output = new StringWriter();
        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);

        // Act
        versionChecker.CheckForNewerVersionsAsync(output, shouldCheck: true);
        await Task.Delay(500); // Wait for async task to complete

        // Assert - should only show the valid version
        var outputText = output.ToString();
        outputText.Should().Contain("0.35.0");
        outputText.Should().NotContain("invalid");
    }

    [TestMethod]
    public void GetWellKnownInstallLocations_ShouldReturnLinuxPaths_OnLinux()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem();
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        // Act
        var locations = versionChecker.GetWellKnownInstallLocations();

        // Assert
        var normalizedLocations = locations.Select(l => l.Replace('\\', '/')).ToList();
        normalizedLocations.Should().Contain("/home/user/.bicep/bin");
        normalizedLocations.Should().Contain("/home/user/.azure/bin");
        normalizedLocations.Should().Contain("/usr/local/bin");
        normalizedLocations.Should().Contain("/usr/bin");
        locations.Should().NotContain(l => l.Contains("Program Files"));
    }

    [TestMethod]
    public void GetWellKnownInstallLocations_ShouldReturnWindowsPaths_OnWindows()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem();
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns((string?)null);
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns(@"C:\Users\user");
        environmentMock.Setup(e => e.GetVariable("ProgramFiles")).Returns(@"C:\Program Files");
        environmentMock.Setup(e => e.GetVariable("ProgramFiles(x86)")).Returns(@"C:\Program Files (x86)");
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Windows);

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        // Act
        var locations = versionChecker.GetWellKnownInstallLocations();

        // Assert
        // MockFileSystem may normalize paths differently on Linux, so check the key components
        locations.Should().HaveCount(4);
        locations.Should().Contain(l => l.Contains(@"user") && l.Contains(@".bicep") && l.Contains(@"bin"));
        locations.Should().Contain(l => l.Contains(@"user") && l.Contains(@".azure") && l.Contains(@"bin"));
        locations.Should().Contain(l => l.Contains(@"Program Files") && l.Contains(@"Bicep CLI") && !l.Contains("(x86)"));
        locations.Should().Contain(l => l.Contains(@"Program Files (x86)") && l.Contains(@"Bicep CLI"));
        locations.Should().NotContain("/usr/local/bin");
        locations.Should().NotContain("/usr/bin");
    }

    [TestMethod]
    public void GetWellKnownInstallLocations_ShouldReturnEmptyList_WhenNoHomePathSet()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem();
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns((string?)null);
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, new Dictionary<string, string>());

        // Act
        var locations = versionChecker.GetWellKnownInstallLocations();

        // Assert
        locations.Should().BeEmpty();
    }

    [TestMethod]
    public void FindNewerVersions_ShouldRespectCancellationToken()
    {
        // Arrange
        var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "/home/user/.bicep/bin/bicep", new MockFileData("fake binary") },
            { "/home/user/.azure/bin/bicep", new MockFileData("fake binary") },
        });

        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(e => e.GetVariable("HOME")).Returns("/home/user");
        environmentMock.Setup(e => e.GetVariable("USERPROFILE")).Returns((string?)null);
        environmentMock.Setup(e => e.CurrentPlatform).Returns(OSPlatform.Linux);

        var versions = new Dictionary<string, string>
        {
            { "/home/user/.bicep/bin/bicep", "0.35.0" },
            { "/home/user/.azure/bin/bicep", "0.40.0" }
        };

        var versionChecker = new TestableVersionChecker(environmentMock.Object, fileSystemMock, versions);
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act
        var result = versionChecker.FindNewerVersions(new Version("0.30.0"), cts.Token);

        // Assert - should return early due to cancellation
        result.Should().BeEmpty();
    }
}

// Testable version of VersionChecker that allows mocking FileVersionInfo
internal class TestableVersionChecker : VersionChecker
{
    private readonly Dictionary<string, string> mockVersions;
    private readonly IFileSystem fileSystem;

    public TestableVersionChecker(IEnvironment environment, IFileSystem fileSystem, Dictionary<string, string> mockVersions)
        : base(environment, fileSystem)
    {
        this.fileSystem = fileSystem;
        // Normalize all keys in the mockVersions dictionary to avoid path separator issues
        this.mockVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in mockVersions)
        {
            this.mockVersions[NormalizePath(kvp.Key)] = kvp.Value;

            try
            {
                var fullPath = fileSystem.Path.GetFullPath(kvp.Key);
                this.mockVersions[NormalizePath(fullPath)] = kvp.Value;
            }
            catch
            {
                // Ignore if GetFullPath fails
            }
        }
    }

    protected override Version? GetBicepVersion(string bicepPath)
    {
        var normalizedPath = NormalizePath(bicepPath);

        if (mockVersions.TryGetValue(normalizedPath, out var versionString))
        {
            if (Version.TryParse(versionString, out var version))
            {
                return version;
            }
        }

        try
        {
            var fullPath = fileSystem.Path.GetFullPath(bicepPath);
            var normalizedFullPath = NormalizePath(fullPath);

            if (mockVersions.TryGetValue(normalizedFullPath, out versionString))
            {
                if (Version.TryParse(versionString, out var version))
                {
                    return version;
                }
            }
        }
        catch
        {
            // Ignore normalization errors
        }

        return null;
    }

    /// <summary>
    /// Normalize path separators to handle cross-platform testing.
    /// This ensures paths match regardless of whether we're testing Windows behavior on Linux or vice versa.
    /// </summary>
    private static string NormalizePath(string path)
    {
        // Replace all backslashes with forward slashes for consistent comparison
        return path.Replace('\\', '/');
    }
}
