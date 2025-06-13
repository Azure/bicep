// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Azure.Deployments.Core.Entities;

#nullable disable
namespace Bicep.Cli.Helpers.WhatIf;

public static class PropertyChangeTypeExtensions
{
    private static readonly IReadOnlyDictionary<DeploymentWhatIfPropertyChangeType, Color> ColorsByPropertyChangeType =
        new Dictionary<DeploymentWhatIfPropertyChangeType, Color>
        {
            [DeploymentWhatIfPropertyChangeType.Create] = Color.Green,
            [DeploymentWhatIfPropertyChangeType.Delete] = Color.Orange,
            [DeploymentWhatIfPropertyChangeType.Modify] = Color.Purple,
            [DeploymentWhatIfPropertyChangeType.Array] = Color.Purple,
            [DeploymentWhatIfPropertyChangeType.NoEffect] = Color.Gray,
        };

    private static readonly IReadOnlyDictionary<DeploymentWhatIfPropertyChangeType, Symbol> SymbolsByPropertyChangeType =
        new Dictionary<DeploymentWhatIfPropertyChangeType, Symbol>
        {
            [DeploymentWhatIfPropertyChangeType.Create] = Symbol.Plus,
            [DeploymentWhatIfPropertyChangeType.Delete] = Symbol.Minus,
            [DeploymentWhatIfPropertyChangeType.Modify] = Symbol.Tilde,
            [DeploymentWhatIfPropertyChangeType.Array] = Symbol.Tilde,
            [DeploymentWhatIfPropertyChangeType.NoEffect] = Symbol.Cross,
        };

    public static Color ToColor(this DeploymentWhatIfPropertyChangeType propertyChangeType)
    {
        bool success = ColorsByPropertyChangeType.TryGetValue(propertyChangeType, out Color colorCode);

        if (!success)
        {
            throw new ArgumentOutOfRangeException(nameof(propertyChangeType));
        }

        return colorCode;
    }

    public static Symbol ToSymbol(this DeploymentWhatIfPropertyChangeType propertyChangeType)
    {
        bool success = SymbolsByPropertyChangeType.TryGetValue(propertyChangeType, out Symbol symbol);

        if (!success)
        {
            throw new ArgumentOutOfRangeException(nameof(propertyChangeType));
        }

        return symbol;
    }
}
