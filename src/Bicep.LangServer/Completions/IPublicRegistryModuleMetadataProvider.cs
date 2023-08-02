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
    public record PublicRegistryModule(string Name, string? Description, string? DocumentationUri);
    public record PublicRegistryModuleVersion(string Version, string? Description, string? DocumentationUri);

    public interface IPublicRegistryModuleMetadataProvider
    {
        Task<IEnumerable<PublicRegistryModule>> GetModules();

        Task<IEnumerable<PublicRegistryModuleVersion>> GetVersions(string modulePath);
    }
}
