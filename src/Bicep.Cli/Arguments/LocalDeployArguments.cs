// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record LocalDeployArguments(
    string ParamsFile,
    bool NoRestore,
    DeploymentOutputFormat? OutputFormat);
