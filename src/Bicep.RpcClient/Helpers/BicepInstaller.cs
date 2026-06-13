// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.RpcClient.Helpers;

internal static class BicepInstaller
{
    private const string DownloadBaseUrl = "https://downloads.bicep.azure.com";
    private const string LatestReleaseUrl = $"{DownloadBaseUrl}/releases/latest";
    private const string TagNameProperty = "tag_name";

    // if we're unable to acquire a lock on the install location, we will retry until this timeout is reached
    private static readonly TimeSpan InstallContentionTimeout = TimeSpan.FromSeconds(30);

    // interval at which we will retry acquiring the lock on the install location
    private static readonly TimeSpan InstallContentionRetryInterval = TimeSpan.FromMilliseconds(300);

    public static async Task DownloadAndInstallBicepCliAsync(
        HttpClient httpClient,
        string bicepCliPath,
        OSPlatform targetOSPlatform,
        Architecture targetOSArchitecture,
        string versionTag,
        CancellationToken cancellationToken)
    {
        var downloadUrl = BuildDownloadUrlForTag(
            targetOSPlatform: targetOSPlatform,
            targetOSArchitecture: targetOSArchitecture,
            versionTag: versionTag);

        if (Path.GetDirectoryName(bicepCliPath) is { } parentPath && !Directory.Exists(parentPath))
        {
            Directory.CreateDirectory(parentPath);
        }

        var lockFilePath = bicepCliPath + ".lock";
        var tempFilePath = bicepCliPath + ".tmp";
        await DoWithFileSystemLockAsync(lockFilePath, async () =>
        {
            // Another process may have completed the download while we waited.
            if (File.Exists(bicepCliPath))
            {
                return;
            }

            using var response = await httpClient.GetAsync(downloadUrl, cancellationToken: cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using (var fileStream = File.Create(tempFilePath))
            {
                await response.Content.CopyToAsync(fileStream).ConfigureAwait(false);
            }

            SetExecutablePermissions(tempFilePath, cancellationToken);

            MoveFileAtomically(tempFilePath, bicepCliPath);
        }, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<string> GetLatestBicepVersion(
        HttpClient httpClient,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(LatestReleaseUrl, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var json = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return json.GetProperty(TagNameProperty).GetString()
            ?? throw new InvalidOperationException("Failed to determine the latest Bicep CLI version from the releases API.");
    }

    internal static string BuildDownloadUrlForTag(
        OSPlatform targetOSPlatform,
        Architecture targetOSArchitecture,
        string versionTag)
    {
        var osPlatformArchitectureMap = new Dictionary<(OSPlatform, Architecture), string>
        {
            { (OSPlatform.Windows, Architecture.X64), "bicep-win-x64.exe" },
            { (OSPlatform.Windows, Architecture.Arm64), "bicep-win-arm64.exe" },
            { (OSPlatform.Linux, Architecture.X64), "bicep-linux-x64" },
            { (OSPlatform.Linux, Architecture.Arm64), "bicep-linux-arm64" },
            { (OSPlatform.OSX, Architecture.X64), "bicep-osx-x64" },
            { (OSPlatform.OSX, Architecture.Arm64), "bicep-osx-arm64" }
        };

        if (osPlatformArchitectureMap.TryGetValue((targetOSPlatform, targetOSArchitecture), out var bicepCliArtifactName))
        {
            return $"{DownloadBaseUrl}/{versionTag}/{bicepCliArtifactName}";
        }

        throw new PlatformNotSupportedException(
            $"The Bicep CLI is not available for the specified platform '{targetOSPlatform}' and architecture '{targetOSArchitecture}'. " +
            "Supported combinations include Windows (x64, Arm64), Linux (x64, Arm64), and macOS (x64, Arm64).");
    }

    internal static void SetExecutablePermissions(string filePath, CancellationToken cancellationToken)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // No need to set executable permissions on Windows
            return;
        }

        using var chmod = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"+x \"{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
        };

        cancellationToken.ThrowIfCancellationRequested();

        chmod.Start();
        chmod.WaitForExit();

        if (chmod.ExitCode != 0)
        {
            throw new UnauthorizedAccessException(
                $"Failed to set executable permissions for the file at '{filePath}'. " +
                $"The 'chmod' process exited with code {chmod.ExitCode}. Ensure you have sufficient permissions.");
        }
    }

    public static OSPlatform DetectCurrentOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSPlatform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }
        else
        {
            throw new PlatformNotSupportedException(
                "The current operating system platform is not supported. Supported platforms are Windows, Linux, and macOS.");
        }
    }

    internal static async Task DoWithFileSystemLockAsync(string lockFilePath, Func<Task> action, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < InstallContentionTimeout)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (TryAcquireFileLock(lockFilePath) is { } lockStream)
            {
                using (lockStream)
                {
                    await action().ConfigureAwait(false);
                    return;
                }
            }

            await Task.Delay(InstallContentionRetryInterval, cancellationToken).ConfigureAwait(false);
        }

        throw new TimeoutException($"""Exceeded the timeout of "{InstallContentionTimeout}" to acquire the install lock on file "{lockFilePath}".""");
    }

    internal static FileStream? TryAcquireFileLock(string lockFilePath)
    {
        try
        {
            // FileMode.OpenOrCreate - we don't want Create because it will also execute a truncate operation in some cases, which is unnecessary
            // FileShare.None - we want locking on the file (even if advisory on some platforms)
            // FileOptions.None - DeleteOnClose is NOT ATOMIC on Linux/Mac and causes race conditions
            return new FileStream(lockFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.None);
        }
        catch (IOException exception) when (exception.GetType() == typeof(IOException))
        {
            // when file is locked, an IOException is thrown
            // there are other cases where an exception derived from IOException is thrown, but we want to filter them out
            return null;
        }
    }

    internal static void MoveFileAtomically(string sourcePath, string destinationPath)
    {
        if (File.Exists(destinationPath))
        {
            File.Replace(sourcePath, destinationPath, null);
        }
        else
        {
            File.Move(sourcePath, destinationPath);
        }
    }
}
