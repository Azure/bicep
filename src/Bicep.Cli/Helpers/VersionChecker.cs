// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Bicep.Core.Utils;

namespace Bicep.Cli.Helpers;

public class VersionChecker(IEnvironment environment, IFileSystem fileSystem)
{
    /// <summary>
    /// Checks well-known installation locations for other Bicep CLI versions and warns if a newer version is installed.
    /// Runs asynchronously in the background without blocking CLI startup.
    /// </summary>
    /// <param name="output">Output stream to write warnings to</param>
    /// <param name="shouldCheck">Whether the check should run based on command type and output redirection</param>
    public void CheckForNewerVersionsAsync(TextWriter output, bool shouldCheck)
    {
        if (!shouldCheck)
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                var currentVersion = environment.CurrentVersion.Version;
                if (!Version.TryParse(currentVersion, out var parsedCurrentVersion))
                {
                    return;
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));
                var newerVersions = await Task.Run(() => FindNewerVersions(parsedCurrentVersion, cts.Token), cts.Token);

                if (newerVersions.Any())
                {
                    await output.WriteLineAsync($"Warning: You are running Bicep CLI version {currentVersion}, but newer version(s) are installed on this system:");
                    foreach (var (version, path) in newerVersions.OrderByDescending(v => v.Version))
                    {
                        await output.WriteLineAsync($"  - Version {version} at {path}");
                    }
                    await output.WriteLineAsync();
                }
            }
            catch
            {
                // Silently ignore any errors during version checking to avoid disrupting normal CLI operations
            }
        });
    }

    public List<(Version Version, string Path)> FindNewerVersions(Version currentVersion, CancellationToken cancellationToken)
    {
        var newerVersions = new List<(Version, string)>();
        var checkedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

                    var version = GetBicepVersion(bicepPath);
                    if (version != null && version > currentVersion)
                    {
                        newerVersions.Add((version, bicepPath));
                    }
                }
            }
            catch
            {
                // Skip locations that cannot be accessed
            }
        }

        return newerVersions;
    }

    public List<string> GetWellKnownInstallLocations()
    {
        var locations = new List<string>();
        var homePath = environment.GetVariable("HOME") ?? environment.GetVariable("USERPROFILE");

        if (string.IsNullOrEmpty(homePath))
        {
            return locations;
        }

        // ~/.bicep/bin (default location)
        locations.Add(fileSystem.Path.Combine(homePath, ".bicep", "bin"));

        // ~/.azure/bin (install location from Azure CLI)
        locations.Add(fileSystem.Path.Combine(homePath, ".azure", "bin"));

        if (environment.CurrentPlatform == OSPlatform.Windows)
        {
            // C:\Program Files\Bicep CLI
            var programFiles = environment.GetVariable("ProgramFiles");
            if (!string.IsNullOrEmpty(programFiles))
            {
                locations.Add(fileSystem.Path.Combine(programFiles, "Bicep CLI"));
            }

            // C:\Program Files (x86)\Bicep CLI
            var programFilesX86 = environment.GetVariable("ProgramFiles(x86)");
            if (!string.IsNullOrEmpty(programFilesX86))
            {
                locations.Add(fileSystem.Path.Combine(programFilesX86, "Bicep CLI"));
            }
        }
        else
        {
            // /usr/local/bin
            locations.Add("/usr/local/bin");

            // /usr/bin
            locations.Add("/usr/bin");
        }

        return locations.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    protected virtual Version? GetBicepVersion(string bicepPath)
    {
        try
        {
            if (!fileSystem.File.Exists(bicepPath))
            {
                return null;
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(bicepPath);
            var fileVersion = fileVersionInfo.FileVersion;

            if (string.IsNullOrEmpty(fileVersion))
            {
                return null;
            }

            if (Version.TryParse(fileVersion, out var version))
            {
                return version;
            }

            return null;
        }
        catch
        {
            return null;
        }
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
