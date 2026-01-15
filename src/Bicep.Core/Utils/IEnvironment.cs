// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Runtime.InteropServices;

namespace Bicep.Core.Utils;

public interface IEnvironment
{
    public record BicepVersionInfo(
        string Version,
        string? CommitRef);

    string? GetVariable(string variable);

    IEnumerable<string> GetVariableNames();

    string CurrentDirectory { get; }

    OSPlatform? CurrentPlatform { get; }

    Architecture CurrentArchitecture { get; }

    BicepVersionInfo CurrentVersion { get; }
}
