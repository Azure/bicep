// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public record DeployArguments(
    string InputFile,
    bool NoRestore,
    ImmutableDictionary<string, string> AdditionalArguments,
    DeploymentOutputFormat? OutputFormat) : DeployArgumentsBase(InputFile, NoRestore, AdditionalArguments);