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
    public record PublicRegistryModule(string name, string? description, string? documentationUri);
    public record PublicRegistryModuleVersion(string version, string? description, string? documentationUri);

    public interface IPublicRegistryModuleMetadataProvider
    {
        Task<IEnumerable<PublicRegistryModule>> GetModules();

        Task<IEnumerable<PublicRegistryModuleVersion>> GetVersions(string modulePath);
    }
}
