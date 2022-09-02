// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Deploy
{
    public interface IDeploymentFileCompilationCache
    {
        public void CacheCompilation(DocumentUri documentUri, Compilation compilation);

        public Compilation? FindAndRemoveCompilation(DocumentUri documentUri);
    }
}
