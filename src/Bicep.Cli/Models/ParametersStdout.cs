// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Models;

/// <summary>
/// Both AzCLI and AzPwsh take a dependency on this exact output format.
/// Please be very careful making any changes here to ensure back-compatibility.
/// </summary>
public record BuildParamsStdout(
    string parametersJson,
    string? templateJson,
    string? templateSpecId);