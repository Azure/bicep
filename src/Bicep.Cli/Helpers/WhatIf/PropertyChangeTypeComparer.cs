// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Azure.Deployments.Core.Entities;

namespace Bicep.Cli.Helpers.WhatIf;

public class PropertyChangeTypeComparer : IComparer<DeploymentWhatIfPropertyChangeType>
{
    private static readonly IReadOnlyDictionary<DeploymentWhatIfPropertyChangeType, int> WeightsByPropertyChangeType =
        new Dictionary<DeploymentWhatIfPropertyChangeType, int>
        {
            [DeploymentWhatIfPropertyChangeType.Delete] = 0,
            [DeploymentWhatIfPropertyChangeType.Create] = 1,
            // Modify and Array are set to have the same weight by intention.
            [DeploymentWhatIfPropertyChangeType.Modify] = 2,
            [DeploymentWhatIfPropertyChangeType.Array] = 2,
            [DeploymentWhatIfPropertyChangeType.NoEffect] = 3,
        };

    public int Compare(DeploymentWhatIfPropertyChangeType first, DeploymentWhatIfPropertyChangeType second)
    {
        return WeightsByPropertyChangeType[first] - WeightsByPropertyChangeType[second];
    }
}
