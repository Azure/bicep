// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Deploy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentParametersResponse(List<BicepDeploymentParameter> deploymentParameters, string parametersFileName, string? errorMessage);

    public record BicepDeploymentParameter(string name, string? value, bool isMissingParam, bool isExpression, bool isSecure, ParameterType? parameterType);

    /// <summary>
    /// Handles getDeploymentParameters LSP request.
    /// The BicepDeploymentParametersHandler returns information about deployment parameters, parameters file name and error message, if any.
    /// List of <see cref="BicepDeploymentParameter"/>, included in the response has informtion about the parameter e.g. name , value, if the parameter has
    /// @secure() decorator, if it's missing default value/is not present in parameters file, is an expression etc
    /// The above information will be used to display appropriate controls in UI.
    /// </summary>
    public class BicepDeploymentParametersHandler : ExecuteTypedResponseCommandHandlerBase<string, string, string, BicepDeploymentParametersResponse>
    {
        private readonly IDeploymentFileCompilationCache deploymentFileCompilationCache;

        public BicepDeploymentParametersHandler(
            IDeploymentFileCompilationCache deploymentFileCompilationCache,
            ISerializer serializer)
            : base(LangServerConstants.GetDeploymentParametersCommand, serializer)
        {
            this.deploymentFileCompilationCache = deploymentFileCompilationCache;
        }

        public override Task<BicepDeploymentParametersResponse> Handle(string documentPath, string parametersFilePath, string template, CancellationToken cancellationToken)
        {
            var updatedParams = GetUpdatedParams(documentPath, parametersFilePath, template);
            return Task.FromResult(updatedParams);
        }

        private BicepDeploymentParametersResponse GetUpdatedParams(string documentPath, string parametersFilePath, string template)
        {
            var parametersFileName = GetParameterFileName(documentPath, parametersFilePath);

            try
            {
                var parametersFromProvidedParametersFile = GetParametersInfoFromProvidedFile(parametersFilePath)
                    ?? new Dictionary<string, dynamic>();
                var templateObj = JObject.Parse(template);
                var defaultParametersFromTemplate = templateObj["parameters"];

                var missingArrayOrObjectTypes = new List<string>();
                var updatedDeploymentParameters = new List<BicepDeploymentParameter>();

                foreach (var parameterSymbol in GetParameterSymbols(documentPath))
                {
                    var parameterDeclarationSyntax = parameterSymbol.DeclaringParameter;
                    var modifier = parameterDeclarationSyntax.Modifier;
                    var parameterName = parameterSymbol.Name;
                    var parameterType = GetParameterType(parameterSymbol);

                    if (modifier is null)
                    {
                        if (!parametersFromProvidedParametersFile.ContainsKey(parameterName))
                        {
                            if (IsOfTypeArrayOrObject(parameterType))
                            {
                                missingArrayOrObjectTypes.Add(parameterName);
                                continue;
                            }

                            var updatedDeploymentParameter = new BicepDeploymentParameter(
                                name: parameterName,
                                value: null,
                                isMissingParam: true,
                                isExpression: false,
                                isSecure: parameterSymbol.IsSecure(),
                                parameterType: parameterType);
                            updatedDeploymentParameters.Add(updatedDeploymentParameter);
                        }
                    }
                    else
                    {
                        // If param is of type array or object, we don't want to provide an option to override.
                        // We'll simply ignore and continue
                        if (IsOfTypeArrayOrObject(parameterType))
                        {
                            continue;
                        }

                        // If the parameter:
                        // - contains default value in bicep file
                        // - is also mentioned in parameters file
                        // then the value specified in the parameters file will take precedence.
                        // We will not provide an option to override in the UI
                        if (parametersFromProvidedParametersFile.ContainsKey(parameterName))
                        {
                            continue;
                        }

                        if (defaultParametersFromTemplate?[parameterName]?["defaultValue"] is JToken defaultValueObject &&
                            defaultValueObject is not null &&
                            defaultValueObject.ToString() is string defaultValue)
                        {
                            bool isExpression = IsExpression(modifier);

                            if (isExpression)
                            {
                                defaultValue = defaultValue.TrimStart('[').TrimEnd(']');
                            }
                            var updatedDeploymentParameter = new BicepDeploymentParameter(
                                name: parameterName,
                                value: defaultValue.ToString(),
                                isMissingParam: false,
                                isExpression: isExpression,
                                isSecure: parameterSymbol.IsSecure(),
                                parameterType: parameterType);
                            updatedDeploymentParameters.Add(updatedDeploymentParameter);
                        }
                    }
                }

                return new BicepDeploymentParametersResponse(
                    updatedDeploymentParameters,
                    parametersFileName,
                    GetErrorMessageForMissingArrayOrObjectTypes(missingArrayOrObjectTypes));

            }
            catch (Exception e)
            {
                return new BicepDeploymentParametersResponse(new List<BicepDeploymentParameter>(), parametersFileName, e.Message);
            }
        }

        private bool IsExpression(SyntaxBase modifier)
        {
            return modifier is ParameterDefaultValueSyntax parameterDefaultValueSyntax &&
                parameterDefaultValueSyntax.DefaultValue is ExpressionSyntax expressionSyntax &&
                expressionSyntax is not null &&
                // Complex Evaluation of StringSyntax is required for nested functions like 'resource${uniqueString(resourceGroup().id)}'
                // Fixes: https://github.com/Azure/bicep/issues/8154
                (expressionSyntax is not StringSyntax || expressionSyntax.IsInterpolated()) &&
                expressionSyntax is not IntegerLiteralSyntax &&
                expressionSyntax is not BooleanLiteralSyntax;
        }

        private string GetParameterFileName(string documentPath, string parametersFilePath)
        {
            var parametersFileExists = !string.IsNullOrWhiteSpace(parametersFilePath) && File.Exists(parametersFilePath);

            if (parametersFileExists)
            {
                return Path.GetFileName(parametersFilePath);
            }

            return Path.GetFileNameWithoutExtension(documentPath) + ".parameters.json";
        }

        private string? GetErrorMessageForMissingArrayOrObjectTypes(List<string> missingArrayOrObjectTypes)
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

            // Reuse the compilation cached by BicepDeploymentScopeRequestHandler
            var compilation = deploymentFileCompilationCache.FindAndRemoveCompilation(documentUri);

            if (compilation is null)
            {
                return Enumerable.Empty<ParameterSymbol>();
            }

            var semanticModel = compilation.GetEntrypointSemanticModel();
            return semanticModel.Root.ParameterDeclarations;
        }

        private bool IsOfTypeArrayOrObject(ParameterType? parameterType)
        {
            return parameterType is not null &&
                (parameterType == ParameterType.Array || parameterType == ParameterType.Object);
        }

        public ParameterType? GetParameterType(ParameterSymbol parameterSymbol) => parameterSymbol.Type switch
        {
            var type when ReferenceEquals(type, LanguageConstants.Any) => null,
            var type when TypeValidator.AreTypesAssignable(type, LanguageConstants.Array) => ParameterType.Array,
            var type when TypeValidator.AreTypesAssignable(type, LanguageConstants.Bool) => ParameterType.Bool,
            var type when TypeValidator.AreTypesAssignable(type, LanguageConstants.Int) => ParameterType.Int,
            var type when TypeValidator.AreTypesAssignable(type, LanguageConstants.Object) => ParameterType.Object,
            var type when TypeValidator.AreTypesAssignable(type, LanguageConstants.String) => ParameterType.String,
            _ => null,
        };
    }
}
