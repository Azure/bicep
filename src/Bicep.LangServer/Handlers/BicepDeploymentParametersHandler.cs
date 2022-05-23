// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public record BicepUpdatedDeploymentParameter(string name, string? value, bool isMissingParam, bool showDefaultValue);

    public class BicepDeploymentParametersHandler : ExecuteTypedResponseCommandHandlerBase<string, string, string, List<BicepUpdatedDeploymentParameter>>
    {
        private readonly ICompilationManager compilationManager;

        public BicepDeploymentParametersHandler(
            ICompilationManager compilationManager,
            ISerializer serializer)
            : base(LangServerConstants.GetDeploymentScopeCommand, serializer)
        {
            this.compilationManager = compilationManager;
        }
        public override Task<List<BicepUpdatedDeploymentParameter>> Handle(string documentPath, string parametersFilePath, string template, CancellationToken cancellationToken)
        {
            var updatedParams = GetUpdatedParams(documentPath, parametersFilePath, template);
            return Task.FromResult(updatedParams);
        }

        private List<BicepUpdatedDeploymentParameter> GetUpdatedParams(string documentPath, string parametersFilePath, string template)
        {
            var parametersFromProvidedParametersFile = GetParametersInfoFromProvidedFile(parametersFilePath);
            var updatedDeploymentParameters = new List<BicepUpdatedDeploymentParameter>();
            var templateObj = JObject.Parse(template);
            var defaultParametersFromTemplate = templateObj["parameters"];

            foreach (var parameterSymbol in GetParameterSymbols(documentPath))
            {
                var modifier = parameterSymbol.DeclaringParameter.Modifier;
                var parameterName = parameterSymbol.Name;
                var displayActualDefault = true;

                if (modifier is null)
                {
                    if (parametersFromProvidedParametersFile is null || !parametersFromProvidedParametersFile.ContainsKey(parameterName))
                    {
                        var updatedDeploymentParameter = new BicepUpdatedDeploymentParameter(parameterName, null, true, false);
                        updatedDeploymentParameters.Add(updatedDeploymentParameter);
                    }
                }
                else
                {
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

            return updatedDeploymentParameters;
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
    }
}
