// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public interface IApiVersionProvider
    {
        public IEnumerable<string> GetResourceTypeNames(ResourceScope scope);
        public IEnumerable<AzureResourceApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceName);
    }
}
