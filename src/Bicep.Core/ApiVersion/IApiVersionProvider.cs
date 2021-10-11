// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.ApiVersion
{
    public interface IApiVersionProvider
    {
        public DateTime? GetRecentApiVersionDate(string fullyQualifiedName, bool useNonPreviewVersionsCache = true);

        public DateTime? ConvertApiVersionToDateTime(string apiVersion);
    }
}
