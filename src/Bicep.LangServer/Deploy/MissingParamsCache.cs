// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bicep.LanguageServer.Handlers;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Deploy
{
    public class MissingParamsCache : IMissingParamsCache
    {
        private readonly ConcurrentDictionary<DocumentUri, IEnumerable<BicepDeploymentMissingParam>> missingParamsCache = new ConcurrentDictionary<DocumentUri, IEnumerable<BicepDeploymentMissingParam>>();

        public void CacheBicepDeploymentMissingParams(DocumentUri documentUri, IEnumerable<BicepDeploymentMissingParam> missingParams)
        {
            missingParamsCache.AddOrUpdate(documentUri,(documentUri) => missingParams, (documentUri, prevMissingParams) => missingParams);
        }

        public IEnumerable<BicepDeploymentMissingParam> GetBicepDeploymentMissingParams(DocumentUri documentUri)
        {
            missingParamsCache.TryGetValue(documentUri, out IEnumerable<BicepDeploymentMissingParam>? missingParams);

            if (missingParams is null)
            {
                return Enumerable.Empty<BicepDeploymentMissingParam>();
            }

            return missingParams;
        }
    }
}
