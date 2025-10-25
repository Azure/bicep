// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Runtime.InteropServices;
using Azure.Core.Pipeline;

namespace Bicep.Core.Utils;

public class Environment : IEnvironment
{
    private static readonly Lazy<OSPlatform?> lazyCurrentOsPlatform = new(() => TryGetCurrentOSPlatform());

    public string? GetVariable(string variable)
        => System.Environment.GetEnvironmentVariable(variable);

    public IEnumerable<string> GetVariableNames()
        => System.Environment.GetEnvironmentVariables().Keys.OfType<string>();

    public string CurrentDirectory
        => System.Environment.CurrentDirectory;

    public Architecture CurrentArchitecture => RuntimeInformation.ProcessArchitecture;

    public OSPlatform? CurrentPlatform => lazyCurrentOsPlatform.Value;

    public IEnvironment.BicepVersionInfo CurrentVersion { get; } = GetVersionInfo();

    private static OSPlatform? TryGetCurrentOSPlatform()
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

        return null;
    }
    
    private static IEnvironment.BicepVersionInfo GetVersionInfo()
    {
        var version = ThisAssembly.AssemblyInformationalVersion.Split('+');

        return new(version[0], version.Length > 1 ? version[1] : null);
    }
}
