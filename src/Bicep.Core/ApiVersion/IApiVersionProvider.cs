// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.ApiVersion
{
    public interface IApiVersionProvider
    {
        public string? GetRecentApiVersion(string fullyQualifiedName, bool useNonApiVersionCache = true);
    }
}
