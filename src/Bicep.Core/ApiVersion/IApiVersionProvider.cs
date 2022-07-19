// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.ApiVersion
{
    public interface IApiVersionProvider
    {
        // No guaranteed order asdfg
        public IEnumerable<string> GetResourceTypeNames(ResourceScope scope);

        // No guaranteed order asdfg
        public IEnumerable<string> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceName);
    }
}
