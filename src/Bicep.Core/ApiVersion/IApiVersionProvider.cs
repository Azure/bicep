// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.ApiVersion
{
    public interface IApiVersionProvider
    {
        public (string?, string?) GetApiVersionAndPrefix(string apiVersion);

        public string? GetRecentApiVersion(string fullyQualifiedName, string? prefix);
    }
}
