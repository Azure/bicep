// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.ParamsHandlers
{
    public class BicepParamsDefinitionHandler : DefinitionHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;
        private readonly IParamsCompilationManager paramsCompilationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepParamsDefinitionHandler(
            ISymbolResolver symbolResolver,
            IParamsCompilationManager paramsCompilationManager,
            IFeatureProviderFactory featureProviderFactory) : base()
        {
            this.symbolResolver = symbolResolver;
            this.paramsCompilationManager = paramsCompilationManager;
            this.featureProviderFactory = featureProviderFactory;
        }
        public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            if (featureProviderFactory.GetFeatureProvider(request.TextDocument.Uri.ToUri()).ParamsFilesEnabled)
            {
                var paramsContext = this.paramsCompilationManager.GetCompilation(request.TextDocument.Uri);
                if (paramsContext is null)
                {
                    return Task.FromResult(new LocationOrLocationLinks());
                }

                var resolutionResult = this.symbolResolver.ResolveParamsSymbol(request.TextDocument.Uri, request.Position);
                if (resolutionResult is null)
                {
                    return Task.FromResult(new LocationOrLocationLinks());
                }

                if (resolutionResult.Symbol is ParameterAssignmentSymbol param && param.NameSyntax is { } nameSyntax)
                {
                    var bicepCompilation = paramsContext.ParamsSemanticModel.BicepCompilation;

                    if (bicepCompilation is null)
                    {
                        return Task.FromResult(new LocationOrLocationLinks());
                    }
                    var bicepSemanticModel = bicepCompilation.GetEntrypointSemanticModel();

                    var parameterDeclarations = bicepSemanticModel.Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
                    var parameterDeclarationSymbol = paramsContext.ParamsSemanticModel.TryGetParameterDeclaration(param);

                    if (parameterDeclarationSymbol is null)
                    {
                        return Task.FromResult(new LocationOrLocationLinks());
                    }

                    var range = PositionHelper.GetNameRange(bicepCompilation.SourceFileGrouping.EntryPoint.LineStarts, parameterDeclarationSymbol.DeclaringSyntax);
                    var documentUri = bicepCompilation.SourceFileGrouping.EntryPoint.FileUri;

                    return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                    {
                        // source of the link. Underline only the symbolic name
                        OriginSelectionRange = nameSyntax.ToRange(paramsContext.LineStarts),
                        TargetUri = documentUri,

                        // entire span of the declaredSymbol
                        TargetRange = range,
                        TargetSelectionRange = range
                    })));
                }
            }

            return Task.FromResult(new LocationOrLocationLinks());
        }

        protected override DefinitionRegistrationOptions CreateRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.CreateForParamsOnly()
        };
    }
}
