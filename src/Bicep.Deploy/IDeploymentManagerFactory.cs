// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Deploy;

public interface IDeploymentManagerFactory
{
    IDeploymentManager CreateDeploymentManager(RootConfiguration rootConfiguration);
}
