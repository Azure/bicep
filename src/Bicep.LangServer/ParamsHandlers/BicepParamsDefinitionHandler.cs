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
        private readonly ICompilationManager compilationManager;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepParamsDefinitionHandler(
            ISymbolResolver symbolResolver,
            ICompilationManager compilationManager,
            IFeatureProviderFactory featureProviderFactory) : base()
        {
            this.symbolResolver = symbolResolver;
            this.compilationManager = compilationManager;
            this.featureProviderFactory = featureProviderFactory;
        }
        public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            if (featureProviderFactory.GetFeatureProvider(request.TextDocument.Uri.ToUri()).ParamsFilesEnabled)
            {
                var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
                if (context is null)
                {
                    return Task.FromResult(new LocationOrLocationLinks());
                }

                var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
                if (result is null)
                {
                    return Task.FromResult(new LocationOrLocationLinks());
                }

                if (result.Symbol is ParameterAssignmentSymbol param && param.NameSource is { } nameSyntax)
                {
                    var paramsSemanticModel = context.Compilation.GetEntrypointSemanticModel();
                    if (!paramsSemanticModel.Root.TryGetBicepFileSemanticModelViaUsing(out var bicepSemanticModel, out _))
                    {
                        return Task.FromResult(new LocationOrLocationLinks());
                    }

                    var parameterDeclarations = bicepSemanticModel.Root.Syntax.Children.OfType<ParameterDeclarationSyntax>();
                    var parameterDeclarationSymbol = paramsSemanticModel.TryGetParameterDeclaration(param);

                    if (parameterDeclarationSymbol is null)
                    {
                        return Task.FromResult(new LocationOrLocationLinks());
                    }

                    var range = PositionHelper.GetNameRange(bicepSemanticModel.SourceFile.LineStarts, parameterDeclarationSymbol.DeclaringSyntax);
                    var documentUri = bicepSemanticModel.SourceFile.FileUri;

                    return Task.FromResult(new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
                    {
                        // source of the link. Underline only the symbolic name
                        OriginSelectionRange = nameSyntax.ToRange(context.LineStarts),
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
