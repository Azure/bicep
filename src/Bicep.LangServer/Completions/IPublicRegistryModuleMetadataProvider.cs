// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bicep.LanguageServer.Completions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public interface IPublicRegistryModuleMetadataProvider
    {
        Task<IEnumerable<string>> GetModuleNames();

        Task<IEnumerable<string>> GetVersions(string moduleName);
    }
}
