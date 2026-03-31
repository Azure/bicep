// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Cli.Arguments;

public abstract record DeployArgumentsBase(
    string InputFile,
    bool NoRestore,
    ImmutableDictionary<string, string> AdditionalArguments) : IInputArguments;
