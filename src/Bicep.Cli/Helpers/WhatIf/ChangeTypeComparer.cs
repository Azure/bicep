// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Azure.Deployments.Core.Entities;

namespace Bicep.Cli.Helpers.WhatIf;

public class ChangeTypeComparer : IComparer<DeploymentWhatIfChangeType>
{
    private static readonly IReadOnlyDictionary<DeploymentWhatIfChangeType, int> WeightsByChangeType =
        new Dictionary<DeploymentWhatIfChangeType, int>
        {
            [DeploymentWhatIfChangeType.Delete] = 0,
            [DeploymentWhatIfChangeType.Create] = 1,
            [DeploymentWhatIfChangeType.Deploy] = 2,
            [DeploymentWhatIfChangeType.Modify] = 3,
            [DeploymentWhatIfChangeType.Unsupported] = 4,
            [DeploymentWhatIfChangeType.NoChange] = 5,
            [DeploymentWhatIfChangeType.Ignore] = 6,
        };

    public int Compare(DeploymentWhatIfChangeType first, DeploymentWhatIfChangeType second)
    {
        return WeightsByChangeType[first] - WeightsByChangeType[second];
    }
}
