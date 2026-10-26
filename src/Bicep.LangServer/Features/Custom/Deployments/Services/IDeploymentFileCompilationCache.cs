// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol;
using BicepCompilation = Bicep.Core.Semantics.Compilation;

namespace Bicep.LanguageServer.Features.Custom.Deployments.Services
{
    public interface IDeploymentFileCompilationCache
    {
        public void CacheCompilation(DocumentUri documentUri, BicepCompilation compilation);

        public BicepCompilation? FindAndRemoveCompilation(DocumentUri documentUri);
    }
}
