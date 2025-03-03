// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Utils;

public class Environment : IEnvironment
{
    public string? GetVariable(string variable)
        => System.Environment.GetEnvironmentVariable(variable);

    public IEnumerable<string> GetVariableNames()
        => System.Environment.GetEnvironmentVariables().Keys.OfType<string>();

    public string CurrentDirectory
        => System.Environment.CurrentDirectory;
}
