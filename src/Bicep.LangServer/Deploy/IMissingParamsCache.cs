// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.LanguageServer.Handlers;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Deploy
{
    public interface IMissingParamsCache
    {
        public void CacheBicepDeploymentMissingParams(DocumentUri documentUri, IEnumerable<BicepDeploymentMissingParam> missingParams);

        public IEnumerable<BicepDeploymentMissingParam> GetBicepDeploymentMissingParams(DocumentUri documentUri);
    }
}
