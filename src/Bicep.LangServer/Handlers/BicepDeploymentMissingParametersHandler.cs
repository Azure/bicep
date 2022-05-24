// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentParam(string name, string? value, bool isMissingParam, bool displayActualDefault);

    public class BicepDeploymentMissingParametersHandler : ExecuteTypedResponseCommandHandlerBase<string, string, List<BicepDeploymentParam>>
    {
        private readonly ICompilationManager compilationManager;

        public BicepDeploymentMissingParametersHandler(ICompilationManager compilationManager, ISerializer serializer)
            : base(LangServerConstants.GetDeployMissingParameters, serializer)
        {
            this.compilationManager = compilationManager;
        }

        public override Task<List<BicepDeploymentParam>> Handle(string documentPath, string parametersFilePath, CancellationToken cancellationToken)
        {
            var documentUri = DocumentUri.FromFileSystemPath(documentPath);
            var compilationContext = compilationManager.GetCompilation(documentUri);

            if (compilationContext is null)
            {
                return Task.FromResult(new List<BicepDeploymentParam>());
            }

            var semanticModel = compilationContext.Compilation.GetEntrypointSemanticModel();
            var paramsFromProvidedParametersFile = GetParametersInfoFromProvidedFile(parametersFilePath);
            var bicepDeploymentParams = new List<BicepDeploymentParam>();

            foreach (var parameterDeclaration in semanticModel.Root.ParameterDeclarations)
            {
                if (parameterDeclaration.DeclaringParameter is ParameterDeclarationSyntax parameterDeclarationSyntax)
                {
                    var parameterName = parameterDeclarationSyntax.Name.IdentifierName;
                    var modifier = parameterDeclarationSyntax.Modifier;
                    var displayActualDefault = true;

                    if (modifier is not null)
                    {
                        if (modifier is ParameterDefaultValueSyntax parameterDefaultValueSyntax &&
                            parameterDefaultValueSyntax.DefaultValue is PropertyAccessSyntax propertyAccessSyntax &&
                            propertyAccessSyntax is not null)
                        {
                            displayActualDefault = false;
                        }

                        var bicepDeploymentParam = new BicepDeploymentParam(parameterName, modifier.ToString(), false, displayActualDefault);
                        bicepDeploymentParams.Add(bicepDeploymentParam);
                    }
                    else
                    {
                        if (paramsFromProvidedParametersFile is null ||
                            !paramsFromProvidedParametersFile.ContainsKey(parameterName))
                        {
                            var bicepDeploymentParam = new BicepDeploymentParam(parameterName, null, true, false);
                            bicepDeploymentParams.Add(bicepDeploymentParam);
                        }
                    }
                }
            }

            return Task.FromResult(bicepDeploymentParams);
        }

        private Dictionary<string, dynamic>? GetParametersInfoFromProvidedFile(string parametersFilePath)
        {
            if (string.IsNullOrWhiteSpace(parametersFilePath))
            {
                return null;
            }

            var parametersFileContents = File.ReadAllText(parametersFilePath);

            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(parametersFileContents);
        }

        public string GetParameterValue(ParameterDeclarationSyntax parameterDeclarationSyntax, string bicepFileContents)
        {
            return string.Empty; 
        }
    }
}
