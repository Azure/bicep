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
    /// </summary>
    public void CheckForNewerVersions(TextWriter output)
    {
        try
        {
            var currentVersion = environment.CurrentVersion.Version;
            if (!Version.TryParse(currentVersion, out var parsedCurrentVersion))
            {
                return; // Cannot parse current version, skip check
            }

            var newerVersions = FindNewerVersions(parsedCurrentVersion);

            if (newerVersions.Any())
            {
                output.WriteLine($"Warning: You are running Bicep CLI version {currentVersion}, but newer version(s) are installed on this system:");
                foreach (var (version, path) in newerVersions.OrderByDescending(v => v.Version))
                {
                    output.WriteLine($"  - Version {version} at {path}");
                }
                output.WriteLine();
            }
        }
        catch
        {
            // Silently ignore any errors during version checking to avoid disrupting normal CLI operations
        }
    }

    private List<(Version Version, string Path)> FindNewerVersions(Version currentVersion)
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
            try
            {
                if (!fileSystem.Directory.Exists(location))
                {
                    continue;
                }

                var bicepFiles = fileSystem.Directory.GetFiles(
                    location,
                    environment.CurrentPlatform == OSPlatform.Windows ? "bicep.exe" : "bicep",
                    SearchOption.AllDirectories);

                foreach (var bicepPath in bicepFiles)
                {
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

    private List<string> GetWellKnownInstallLocations()
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

            var pathVar = environment.GetVariable("PATH");
            if (!string.IsNullOrEmpty(pathVar))
            {
                foreach (var pathEntry in pathVar.Split(';'))
                {
                    if (!string.IsNullOrWhiteSpace(pathEntry))
                    {
                        locations.Add(pathEntry.Trim());
                    }
                }
            }
        }
        else
        {
            // /usr/local/bin
            locations.Add("/usr/local/bin");

            // /usr/bin
            locations.Add("/usr/bin");

            var pathVar = environment.GetVariable("PATH");
            if (!string.IsNullOrEmpty(pathVar))
            {
                foreach (var pathEntry in pathVar.Split(':'))
                {
                    if (!string.IsNullOrWhiteSpace(pathEntry))
                    {
                        locations.Add(pathEntry.Trim());
                    }
                }
            }
        }

        return locations.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private Version? GetBicepVersion(string bicepPath)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = bicepPath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return null;
            }

            process.WaitForExit(5000);

            if (process.ExitCode != 0)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd();

            // Parse version from output like "Bicep CLI version 0.30.23 (abc123)"
            // Extract the version number between "version " and the next space or parenthesis
            var versionMatch = System.Text.RegularExpressions.Regex.Match(
                output,
                @"version\s+(\d+\.\d+\.\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (versionMatch.Success && Version.TryParse(versionMatch.Groups[1].Value, out var version))
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
