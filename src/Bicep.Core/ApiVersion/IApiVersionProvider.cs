// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.ApiVersion
{
    public interface IApiVersionProvider
    {
        // List is sorted at top level by ascending date, and each suffix array is sorted ascending alphabetically using Bicep.Core.ApiVersion;
        //using Bicep.Core.ApiVersion;  public (DateTime date, string[] suffixes)[] GetSortedValidApiVersions(string fullyQualifiedResourceName);

        public IEnumerable<string> GetSortedValidApiVersions(TypeSystem.ResourceScope scope, string fullyQualifiedResourceName);
    }
}
