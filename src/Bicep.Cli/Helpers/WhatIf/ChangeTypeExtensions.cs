// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Azure.Deployments.Core.Entities;

#nullable disable
namespace Bicep.Cli.Helpers.WhatIf;

public static class ChangeTypeExtensions
{
    private static readonly IReadOnlyDictionary<DeploymentWhatIfChangeType, Color> ColorsByChangeType =
        new Dictionary<DeploymentWhatIfChangeType, Color>
        {
            [DeploymentWhatIfChangeType.NoChange] = Color.Reset,
            [DeploymentWhatIfChangeType.Ignore] = Color.Gray,
            [DeploymentWhatIfChangeType.Deploy] = Color.Blue,
            [DeploymentWhatIfChangeType.Create] = Color.Green,
            [DeploymentWhatIfChangeType.Delete] = Color.Orange,
            [DeploymentWhatIfChangeType.Modify] = Color.Purple,
            [DeploymentWhatIfChangeType.Unsupported] = Color.Gray,
        };

    private static readonly IReadOnlyDictionary<DeploymentWhatIfChangeType, Symbol> SymbolsByChangeType =
        new Dictionary<DeploymentWhatIfChangeType, Symbol>
        {
            [DeploymentWhatIfChangeType.NoChange] = Symbol.Equal,
            [DeploymentWhatIfChangeType.Ignore] = Symbol.Asterisk,
            [DeploymentWhatIfChangeType.Deploy] = Symbol.ExclamationPoint,
            [DeploymentWhatIfChangeType.Create] = Symbol.Plus,
            [DeploymentWhatIfChangeType.Delete] = Symbol.Minus,
            [DeploymentWhatIfChangeType.Modify] = Symbol.Tilde,
            [DeploymentWhatIfChangeType.Unsupported] = Symbol.Cross,
        };

    public static Color ToColor(this DeploymentWhatIfChangeType changeType)
    {
        bool success = ColorsByChangeType.TryGetValue(changeType, out Color colorCode);

        if (!success)
        {
            throw new ArgumentOutOfRangeException(nameof(changeType));
        }

        return colorCode;
    }

    public static Symbol ToSymbol(this DeploymentWhatIfChangeType changeType)
    {
        bool success = SymbolsByChangeType.TryGetValue(changeType, out Symbol symbol);

        if (!success)
        {
            throw new ArgumentOutOfRangeException(nameof(changeType));
        }

        return symbol;
    }
}
