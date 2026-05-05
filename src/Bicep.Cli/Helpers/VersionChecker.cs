// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Bicep.Core.Utils;

namespace Bicep.Cli.Helpers;

public record BicepInstallationVersion(
    Version Version,
    string? GitCommitSha,
    string Path);

public class VersionChecker(IEnvironment environment, IFileSystem fileSystem)
{
    private static readonly Regex GitCommitShaRegex = new(@"[0-9a-fA-F]{40}", RegexOptions.Compiled);
    private static readonly Regex VersionPrefixRegex = new(@"^\d+(?:\.\d+){1,3}", RegexOptions.Compiled);

    public virtual IReadOnlyList<BicepInstallationVersion> FindNewerVersions(CancellationToken cancellationToken = default)
        => TryParseVersionInfo(string.Empty, environment.CurrentVersion.Version) is { } currentVersion ?
            FindNewerVersions(currentVersion.Version, cancellationToken) :
            [];

    public IReadOnlyList<BicepInstallationVersion> FindNewerVersions(Version currentVersion, CancellationToken cancellationToken)
    {
        var newerVersions = new List<BicepInstallationVersion>();
        var checkedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        currentVersion = NormalizeVersion(currentVersion);

        var currentExePath = GetNormalizedPath(System.Environment.ProcessPath);
        if (currentExePath != null)
        {
            checkedPaths.Add(currentExePath);
        }

        foreach (var location in GetWellKnownInstallLocations())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                if (!fileSystem.Directory.Exists(location))
                {
                    continue;
                }

                var bicepFiles = fileSystem.Directory.GetFiles(
                    location,
                    environment.CurrentPlatform == OSPlatform.Windows ? "bicep.exe" : "bicep",
                    SearchOption.TopDirectoryOnly);

                foreach (var bicepPath in bicepFiles)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var normalizedPath = GetNormalizedPath(bicepPath);
                    if (normalizedPath == null || checkedPaths.Contains(normalizedPath))
                    {
                        continue;
                    }

                    checkedPaths.Add(normalizedPath);

                    var versionInfo = GetBicepVersion(bicepPath);
                    if (versionInfo is { } && versionInfo.Version > currentVersion)
                    {
                        newerVersions.Add(versionInfo);
                    }
                }
            }
            catch
            {
                // Ignore locations that cannot be inspected.
            }
        }

        return newerVersions.OrderByDescending(version => version.Version).ToList();
    }

    public IReadOnlyList<string> GetWellKnownInstallLocations()
    {
        var locations = new List<string>();
        var homePath = environment.GetVariable("HOME") ?? environment.GetVariable("USERPROFILE");

        if (!string.IsNullOrEmpty(homePath))
        {
            locations.Add(fileSystem.Path.Combine(homePath, ".bicep", "bin"));
            locations.Add(fileSystem.Path.Combine(homePath, ".azure", "bin"));
        }

        if (environment.CurrentPlatform == OSPlatform.Windows)
        {
            var programFiles = environment.GetVariable("ProgramFiles");
            if (!string.IsNullOrEmpty(programFiles))
            {
                locations.Add(fileSystem.Path.Combine(programFiles, "Bicep CLI"));
            }

            var programFilesX86 = environment.GetVariable("ProgramFiles(x86)");
            if (!string.IsNullOrEmpty(programFilesX86))
            {
                locations.Add(fileSystem.Path.Combine(programFilesX86, "Bicep CLI"));
            }
        }
        else
        {
            locations.Add("/usr/local/bin");
            locations.Add("/usr/bin");
        }

        AddPathEnvironmentLocations(locations);

        return locations
            .Where(location => !string.IsNullOrWhiteSpace(location))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string? TryGetGitCommitSha(string? commitRef)
    {
        if (string.IsNullOrWhiteSpace(commitRef))
        {
            return null;
        }

        var match = GitCommitShaRegex.Match(commitRef);

        return match.Success ? match.Value : commitRef;
    }

    protected virtual BicepInstallationVersion? GetBicepVersion(string bicepPath)
    {
        try
        {
            if (!fileSystem.File.Exists(bicepPath))
            {
                return null;
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(bicepPath);

            return TryParseVersionInfo(bicepPath, fileVersionInfo.ProductVersion) ??
                TryParseVersionInfo(bicepPath, fileVersionInfo.FileVersion);
        }
        catch
        {
            return null;
        }
    }

    protected static BicepInstallationVersion? TryParseVersionInfo(string bicepPath, string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
        {
            return null;
        }

        var versionParts = versionString.Split('+', 2);
        var versionMatch = VersionPrefixRegex.Match(versionParts[0]);
        if (!versionMatch.Success || !Version.TryParse(versionMatch.Value, out var version))
        {
            return null;
        }

        var gitCommitSha = versionParts.Length > 1 ? TryGetGitCommitSha(versionParts[1]) : null;

        return new(NormalizeVersion(version), gitCommitSha, bicepPath);
    }

    private static Version NormalizeVersion(Version version)
    {
        var build = version.Build >= 0 ? version.Build : 0;

        return version.Revision > 0 ?
            new(version.Major, version.Minor, build, version.Revision) :
            new(version.Major, version.Minor, build);
    }

    private void AddPathEnvironmentLocations(List<string> locations)
    {
        var pathVariable = environment.GetVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathVariable))
        {
            return;
        }

        var pathSeparator = environment.CurrentPlatform == OSPlatform.Windows ? ';' : ':';
        locations.AddRange(pathVariable.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries));
    }

    private string? GetNormalizedPath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        try
        {
            return fileSystem.Path.GetFullPath(path);
        }
        catch
        {
            return path;
        }
    }
}
