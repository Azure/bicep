// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    public interface IAzureContainerRegistryNamesProvider
    {
        Task<IEnumerable<string>> GetRegistryNames(Uri templateUri);
    }
}
