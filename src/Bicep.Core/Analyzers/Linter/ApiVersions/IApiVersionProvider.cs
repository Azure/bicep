// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public interface IApiVersionProvider
    {
        public IEnumerable<string> GetResourceTypeNames(ResourceScope scope);
        public IEnumerable<ApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceName);
    }
}
