// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Cli.Arguments;

public record TeardownArguments(
    string InputFile,
    bool NoRestore,
    ImmutableDictionary<string, string> AdditionalArguments) : DeployArgumentsBase(InputFile, NoRestore, AdditionalArguments);