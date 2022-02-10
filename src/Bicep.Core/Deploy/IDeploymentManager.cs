// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Bicep.Core.Deploy
{
    public interface IDeploymentManager
    {
        Task<string> CreateDeployment(Uri uri, string template, string parameterFilePath, string id, string scope, string location);
    }
}
