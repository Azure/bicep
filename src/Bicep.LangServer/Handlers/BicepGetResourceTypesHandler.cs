// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Bicep.Types.Az;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem.Providers;
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
    public record BicepResourceTypeParams : IRequest<ImmutableArray<BicepResourceTypeGroup>?>;

    public record BicepResourceType(string resourceType, string? apiVersion);

    public record BicepResourceTypeGroup(string group, IEnumerable<BicepResourceType> resourceTypes);
    public class BicepGetResourceTypesHandler : IJsonRpcRequestHandler<BicepResourceTypeParams, ImmutableArray<BicepResourceTypeGroup>?>
    {
        private Lazy<IResourceTypeProvider> resourceTypeProviderLazy;

        public BicepGetResourceTypesHandler()
        {
            this.resourceTypeProviderLazy = new Lazy<IResourceTypeProvider>(() => new AzResourceTypeProvider(new AzResourceTypeLoader(new AzTypeLoader())));
        }

        public Task<ImmutableArray<BicepResourceTypeGroup>?> Handle(BicepResourceTypeParams request, CancellationToken cancellationToken)
        {
            var types = resourceTypeProviderLazy.Value.GetAvailableTypes();
            var result = types
                .ToLookup(type => type.Type.Split('/')[0], x => new BicepResourceType(x.Type, x.ApiVersion))
                .OrderBy(grouping => grouping.Key)
                .Select(grouping => new BicepResourceTypeGroup(
                    grouping.Key,
                    grouping
                        .GroupBy(type => type.resourceType)
                        .Select(typeGroup => typeGroup.OrderByDescending(type => type.apiVersion).First()) // get the latest version
                        .OrderBy(type => type.resourceType)))
                .ToImmutableArray();

            return Task.FromResult<ImmutableArray<BicepResourceTypeGroup>?>(result);
        }
    }
}

