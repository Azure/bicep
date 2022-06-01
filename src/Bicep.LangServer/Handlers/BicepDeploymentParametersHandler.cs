// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentParametersResponse(List<BicepDeploymentParameter> deploymentParameters, bool parametersFileExists, string parametersFileName, string? errorMessage);

    public record BicepDeploymentParameter(string name, string? value, bool isMissingParam, bool isExpression, ParameterType? parameterType);

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
            var parametersFileExists = !string.IsNullOrWhiteSpace(parametersFilePath) && File.Exists(parametersFilePath);
            var parametersFileName = GetParameterFileName(documentPath, parametersFileExists, parametersFilePath);

            try
            {
                var parametersFromProvidedParametersFile = GetParametersInfoFromProvidedFile(parametersFilePath);
                var templateObj = JObject.Parse(template);
                var defaultParametersFromTemplate = templateObj["parameters"];

                var missingArrayOrObjectTypes = new List<string>();
                var updatedDeploymentParameters = new List<BicepDeploymentParameter>();

                foreach (var parameterSymbol in GetParameterSymbols(documentPath))
                {
                    var parameterDeclarationSyntax = parameterSymbol.DeclaringParameter;
                    var modifier = parameterDeclarationSyntax.Modifier;
                    var parameterName = parameterSymbol.Name;
                    var parameterType = GetParameterType(parameterDeclarationSyntax);

                    if (modifier is null)
                    {
                        if (IsOfTypeArrayOrObject(parameterType))
                        {
                            missingArrayOrObjectTypes.Add(parameterName);
                            continue;
                        }

                        if (parametersFromProvidedParametersFile is null || !parametersFromProvidedParametersFile.ContainsKey(parameterName))
                        {
                            var updatedDeploymentParameter = new BicepDeploymentParameter(parameterName, null, true, false, parameterType);
                            updatedDeploymentParameters.Add(updatedDeploymentParameter);
                        }
                    }
                    else
                    {
                        bool isExpression = false;
                        // If param is of type array or object, we don't want to provide an option to override.
                        // We'll simply ignore and continue
                        if (IsOfTypeArrayOrObject(parameterType))
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
                            parameterDefaultValueSyntax.DefaultValue is ExpressionSyntax expressionSyntax &&
                            expressionSyntax is not null &&
                            expressionSyntax is not StringSyntax)
                        {
                            isExpression = true;
                        }

                        if (defaultParametersFromTemplate?[parameterName]?["defaultValue"] is JToken defaultValueObject &&
                            defaultValueObject is not null &&
                            defaultValueObject.ToString() is string defaultValue)
                        {
                            if (isExpression)
                            {
                                defaultValue = defaultValue.TrimStart('[').TrimEnd(']');
                            }
                            var updatedDeploymentParameter = new BicepDeploymentParameter(parameterName, defaultValue.ToString(), false, isExpression, parameterType);
                            updatedDeploymentParameters.Add(updatedDeploymentParameter);
                        }
                    }
                }

                return new BicepDeploymentParametersResponse(
                    updatedDeploymentParameters,
                    parametersFileExists,
                    parametersFileName,
                    GetErrorMessage(missingArrayOrObjectTypes));

            }
            catch (Exception e)
            {
                return new BicepDeploymentParametersResponse(new List<BicepDeploymentParameter>(), parametersFileExists, parametersFileName, e.Message);
            }
        }

        private string GetParameterFileName(string documentPath, bool parametersFileExists, string parametersFilePath)
        {
            if (parametersFileExists)
            {
                return Path.GetFileName(parametersFilePath);
            }

            return Path.GetFileNameWithoutExtension(documentPath) + ".parameters.json";
        }

        private string? GetErrorMessage(List<string> missingArrayOrObjectTypes)
        {
            if (!missingArrayOrObjectTypes.Any())
            {
                return null;
            }

            return string.Format(LangServerResources.MissingParamValueForArrayOrObjectType, string.Join(",", missingArrayOrObjectTypes));
        }

        public Dictionary<string, dynamic>? GetParametersInfoFromProvidedFile(string parametersFilePath)
        {
            if (string.IsNullOrWhiteSpace(parametersFilePath) || !File.Exists(parametersFilePath))
            {
                return null;
            }

            try
            {
                var parametersFileContents = File.ReadAllText(parametersFilePath);
                var jObject = JObject.Parse(parametersFileContents);

                if (jObject.ContainsKey("$schema") && jObject.ContainsKey("contentVersion") && jObject.ContainsKey("parameters"))
                {
                    var parametersObject = jObject["parameters"];

                    if (parametersObject is not null)
                    {
                        parametersFileContents = parametersObject.ToString();
                    }
                }

                return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(parametersFileContents);
            }
            catch(Exception e)
            {
                throw new Exception(string.Format(LangServerResources.InvalidParameterFile, parametersFilePath, e.Message));
            }
        }

        private IEnumerable<ParameterSymbol> GetParameterSymbols(string documentPath)
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

        private bool IsOfTypeArrayOrObject(ParameterType? parameterType)
        {
            return parameterType is not null &&
                (parameterType == ParameterType.Array || parameterType == ParameterType.Object);
        }

        public ParameterType? GetParameterType(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            if (parameterDeclarationSyntax.ParameterType is SimpleTypeSyntax simpleTypeSyntax &&
                simpleTypeSyntax is not null)
            {
                return simpleTypeSyntax.TypeName switch
                {
                    "array" => ParameterType.Array,
                    "bool" => ParameterType.Bool,
                    "int" => ParameterType.Int,
                    "object" => ParameterType.Object,
                    "string" => ParameterType.String,
                    _ => null,
                };
            }

            return null;
        }
    }
}
