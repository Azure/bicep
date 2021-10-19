// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.ApiVersion
{
    public interface IApiVersionProvider
    {
        public (string? date, string? suffix) ParseApiVersion(string apiVersion);

        public string? GetRecentApiVersion(string fullyQualifiedName, string? suffix);
    }
}
