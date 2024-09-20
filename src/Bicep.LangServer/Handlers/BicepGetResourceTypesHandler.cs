// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Bicep.Types.Az;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    // TODO: Update to receive a textDocument to use current document features to find what is available. 
    // [Method("textDocument/getResourceCatalog", Direction.ClientToServer)]
    // public record BicepResourceTypeParams(TextDocumentIdentifier TextDocument) : ITextDocumentIdentifierParams, IRequest<BicepResourceTypes?>;
    [Method("bicep/getResourceCatalog", Direction.ClientToServer)]
    public record BicepResourceTypeParams : IRequest<List<BicepResourceGroup>?>;

    public record BicepResourceType(string resourceType, string? apiVersion);

    public record BicepResourceGroup(string group, IEnumerable<BicepResourceType> resourceTypes);
    public class BicepGetResourceTypesHandler : IJsonRpcRequestHandler<BicepResourceTypeParams, List<BicepResourceGroup>?>
    {
        public async Task<List<BicepResourceGroup>?> Handle(BicepResourceTypeParams request, CancellationToken cancellationToken)
        {
            var resourceTypeProviderLazy = new Lazy<Core.TypeSystem.Providers.IResourceTypeProvider>(() => new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader())));
            var types = resourceTypeProviderLazy.Value.GetAvailableTypes();
            var typeList = types.ToList();
            Dictionary<string, Dictionary<string, string?>> groupedResources = new();
            foreach(var type in typeList)
            {
                var split = type.Type.Split('/');
                var group = split[0];
                var resourceType = split[1];

                if (!groupedResources.ContainsKey(group))
                {
                    groupedResources[group] = new Dictionary<string, string?>();
                }
                if (!(type.ApiVersion != null && type.ApiVersion.Contains("preview")) && (!groupedResources[group].ContainsKey(resourceType) || string.Compare(groupedResources[group][resourceType], type.ApiVersion) < 0))
                {
                    groupedResources[group][resourceType] = type.ApiVersion;
                }
            }

            var result = new List<BicepResourceGroup>();
            foreach (var resourceGroup in groupedResources)
            {
                var resourceTypes = new List<BicepResourceType>();
                foreach(var resourceType in resourceGroup.Value)
                {
                    resourceTypes.Add(new BicepResourceType(resourceType.Key, resourceType.Value));
                }
                ;
                result.Add(new BicepResourceGroup(resourceGroup.Key, [.. resourceTypes.OrderBy(kvp => kvp.resourceType)]));
            }
            result.OrderBy(kvp => kvp.group);
            return await Task.FromResult<List<BicepResourceGroup>?>([.. result.OrderBy(kvp => kvp.group)]);
        }
    }
}

