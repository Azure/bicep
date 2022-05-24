// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentParametersResponse(List<BicepUpdatedDeploymentParameter> bicepUpdatedDeploymentParameters, string? errorMessage);

    public record BicepUpdatedDeploymentParameter(string name, string? value, bool isMissingParam, bool showDefaultValue);

    public class BicepDeploymentParametersHandler : ExecuteTypedResponseCommandHandlerBase<string, string, string, BicepDeploymentParametersResponse>
    {
        private readonly ICompilationManager compilationManager;

        public BicepDeploymentParametersHandler(
            ICompilationManager compilationManager,
            ISerializer serializer)
            : base(LangServerConstants.GetDeploymentParametersCommand, serializer)
        {
            this.compilationManager = compilationManager;
        }
        public override Task<BicepDeploymentParametersResponse> Handle(string documentPath, string parametersFilePath, string template, CancellationToken cancellationToken)
        {
            var updatedParams = GetUpdatedParams(documentPath, parametersFilePath, template);
            return Task.FromResult(updatedParams);
        }

        private BicepDeploymentParametersResponse GetUpdatedParams(string documentPath, string parametersFilePath, string template)
        {
            var parametersFromProvidedParametersFile = GetParametersInfoFromProvidedFile(parametersFilePath);
            var updatedDeploymentParameters = new List<BicepUpdatedDeploymentParameter>();
            var templateObj = JObject.Parse(template);
            var defaultParametersFromTemplate = templateObj["parameters"];
            var missingArrayOrObjectTypes = new List<string>();

            foreach (var parameterSymbol in GetParameterSymbols(documentPath))
            {
                var parameterDeclarationSyntax = parameterSymbol.DeclaringParameter;
                var modifier = parameterDeclarationSyntax.Modifier;
                var parameterName = parameterSymbol.Name;
                var displayActualDefault = true;

                if (modifier is null)
                {
                    if (IsOfTypeArrayOrObject(parameterDeclarationSyntax))
                    {
                        missingArrayOrObjectTypes.Add(parameterName);
                        continue;
                    }

                    if (parametersFromProvidedParametersFile is null || !parametersFromProvidedParametersFile.ContainsKey(parameterName))
                    {
                        var updatedDeploymentParameter = new BicepUpdatedDeploymentParameter(parameterName, null, true, false);
                        updatedDeploymentParameters.Add(updatedDeploymentParameter);
                    }
                }
                else
                {
                    // If param is of type array or object, we don't want to provide an option to override.
                    // We'll simply ignore and continue
                    if (IsOfTypeArrayOrObject(parameterDeclarationSyntax))
                    {
                        continue;
                    }

                    // If the parameter:
                    // - contains default value in bicep file
                    // - is also mentioned in parameters file
                    // then the value specified in the parameters file will take precendence.
                    // We will not provide an option to override it in the UI
                    if (parametersFromProvidedParametersFile is not null && parametersFromProvidedParametersFile.ContainsKey(parameterName))
                    {
                        continue;
                    }

                    if (modifier is ParameterDefaultValueSyntax parameterDefaultValueSyntax &&
                        parameterDefaultValueSyntax.DefaultValue is PropertyAccessSyntax propertyAccessSyntax &&
                        propertyAccessSyntax is not null)
                    {
                        displayActualDefault = false;
                    }

                    var defaultValue = defaultParametersFromTemplate?[parameterName]?["defaultValue"];

                    if (defaultValue is not null)
                    {
                        var updatedDeploymentParameter = new BicepUpdatedDeploymentParameter(parameterName, defaultValue.ToString(), false, displayActualDefault);
                        updatedDeploymentParameters.Add(updatedDeploymentParameter);
                    }
                }
            }

            return new BicepDeploymentParametersResponse(updatedDeploymentParameters, GetErrorMessage(missingArrayOrObjectTypes));
        }

        private string? GetErrorMessage(List<string> missingArrayOrObjectTypes)
        {
            if (!missingArrayOrObjectTypes.Any())
            {
                return null;
            }

            return "Parameters of type array or object should either contain a default value or must be specified in parameters.json file. Please update the value for following parameters: " + string.Join(",", missingArrayOrObjectTypes);
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
 

        public IEnumerable<ParameterSymbol> GetParameterSymbols(string documentPath)
        {
            var documentUri = DocumentUri.FromFileSystemPath(documentPath);
            var compilationContext = compilationManager.GetCompilation(documentUri);

            if (compilationContext is null)
            {
                return Enumerable.Empty<ParameterSymbol>();
            }

            var semanticModel = compilationContext.Compilation.GetEntrypointSemanticModel();
            return semanticModel.Root.ParameterDeclarations
                .Where(sym => semanticModel.FindReferences(sym).OfType<VariableAccessSyntax>().Any());
        }

        private bool IsOfTypeArrayOrObject(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            return parameterDeclarationSyntax.ParameterType is SimpleTypeSyntax simpleTypeSyntax &&
                simpleTypeSyntax is not null &&
                (simpleTypeSyntax.TypeName == LanguageConstants.objectType || simpleTypeSyntax.TypeName == LanguageConstants.arrayType);
        }
    }
}
