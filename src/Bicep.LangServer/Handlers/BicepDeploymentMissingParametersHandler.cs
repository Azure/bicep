// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDeploymentMissingParametersHandler : ExecuteTypedResponseCommandHandlerBase<string, string, List<BicepDeploymentMissingParam>>
    {
        private readonly ICompilationManager compilationManager;
        private readonly IMissingParamsCache missingParamsCache;

        public BicepDeploymentMissingParametersHandler(ICompilationManager compilationManager, IMissingParamsCache missingParamsCache, ISerializer serializer)
            : base(LangServerConstants.GetDeployMissingParameters, serializer)
        {
            this.compilationManager = compilationManager;
            this.missingParamsCache = missingParamsCache;
        }

        public override Task<List<BicepDeploymentMissingParam>> Handle(string documentPath, string parametersFilePath, CancellationToken cancellationToken)
        {
            var documentUri = DocumentUri.FromFileSystemPath(documentPath);
            var compilationContext = compilationManager.GetCompilation(documentUri);

            if (compilationContext is null)
            {
                return Task.FromResult(new List<BicepDeploymentMissingParam>());
            }

            var semanticModel = compilationContext.Compilation.GetEntrypointSemanticModel();
            var parameterDeclarations = semanticModel.Root.ParameterDeclarations;
            var paramsWithoutModifiers = new List<ParameterDeclarationSyntax>();

            foreach (var parameterDeclaration in semanticModel.Root.ParameterDeclarations)
            {
                if (parameterDeclaration.DeclaringParameter is ParameterDeclarationSyntax parameterDeclarationSyntax &&
                    parameterDeclarationSyntax.Modifier is null)
                {
                    paramsWithoutModifiers.Add(parameterDeclarationSyntax);
                }
            }

            if (!string.IsNullOrEmpty(parametersFilePath))
            {
                var parametersFileContents = File.ReadAllText(parametersFilePath);
                Dictionary<string, dynamic>? dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(parametersFileContents);

                if (dict is not null)
                {
                    var missingParameters = paramsWithoutModifiers
                        .Where(x => !dict.ContainsKey(x.Name.IdentifierName))
                        .Select(x => x.Name.IdentifierName);
                    var missingParams = GetMissingParams(documentUri, missingParameters);

                    return Task.FromResult(missingParams);
                }
            }

            var missingParamsUsingSemanticModel = GetMissingParams(
                documentUri,
                paramsWithoutModifiers.Select(x => x.Name.IdentifierName));
            return Task.FromResult(missingParamsUsingSemanticModel);
        }

        private List<BicepDeploymentMissingParam> GetMissingParams(DocumentUri documentUri,IEnumerable<string> missingParams)
        {
            var missingParamsFromCache = missingParamsCache.GetBicepDeploymentMissingParams(documentUri);
            List<BicepDeploymentMissingParam> result = new List<BicepDeploymentMissingParam>();

            foreach (var missingParam in missingParams)
            {
                var previouslyUsedBicepDeploymentMissingParamWithValue = missingParamsFromCache.FirstOrDefault(x => x.name == missingParam);
                BicepDeploymentMissingParam bicepDeploymentMissingParam;

                if (previouslyUsedBicepDeploymentMissingParamWithValue is null ||
                    string.IsNullOrEmpty(previouslyUsedBicepDeploymentMissingParamWithValue.value))
                {
                    bicepDeploymentMissingParam = new BicepDeploymentMissingParam(missingParam, string.Empty);
                }
                else
                {
                    bicepDeploymentMissingParam = new BicepDeploymentMissingParam(missingParam, previouslyUsedBicepDeploymentMissingParamWithValue.value);
                }

                result.Add(bicepDeploymentMissingParam);
            }

            return result;
        }
    }
}
