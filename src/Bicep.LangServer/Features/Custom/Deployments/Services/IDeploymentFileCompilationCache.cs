// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Deployments.Services
{
    public interface IDeploymentFileCompilationCache
    {
        public void CacheCompilation(DocumentUri documentUri, global::Bicep.Core.Semantics.Compilation compilation);

        public global::Bicep.Core.Semantics.Compilation? FindAndRemoveCompilation(DocumentUri documentUri);
    }
}
