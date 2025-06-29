// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Utils;

public interface IEnvironment
{
    string? GetVariable(string variable);

    IEnumerable<string> GetVariableNames();

    string CurrentDirectory { get; }
}
