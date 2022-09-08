// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public class ArmTemplateSemanticModel : ISemanticModel
    {
        private readonly Lazy<ResourceScope> targetScopeLazy;

        private readonly Lazy<ImmutableArray<ParameterMetadata>> parametersLazy;

        private readonly Lazy<ImmutableArray<OutputMetadata>> outputsLazy;

        public ArmTemplateSemanticModel(ArmTemplateFile sourceFile)
        {
            Trace.WriteLine($"Building semantic model for {sourceFile.FileUri}");

            this.SourceFile = sourceFile;

            this.targetScopeLazy = new(() =>
            {
                if (sourceFile.Template?.Schema is not { Value: var schema })
                {
                    return ResourceScope.None;
                }

                if (!Uri.TryCreate(schema, UriKind.Absolute, out var schemaUri))
                {
                    return ResourceScope.None;
                }

                return (schemaUri.AbsolutePath) switch
                {
                    "/schemas/2019-08-01/tenantDeploymentTemplate.json" => ResourceScope.Tenant,
                    "/schemas/2019-08-01/managementGroupDeploymentTemplate.json" => ResourceScope.ManagementGroup,
                    "/schemas/2018-05-01/subscriptionDeploymentTemplate.json" => ResourceScope.Subscription,
                    "/schemas/2014-04-01-preview/deploymentTemplate.json" or
                    "/schemas/2015-01-01/deploymentTemplate.json" or
                    "/schemas/2019-04-01/deploymentTemplate.json" => ResourceScope.ResourceGroup,
                    _ => ResourceScope.None,
                };
            });

            this.parametersLazy = new(() =>
            {
                if (this.SourceFile.Template?.Parameters is null)
                {
                    return ImmutableArray<ParameterMetadata>.Empty;
                }

                return this.SourceFile.Template.Parameters
                    .Select(parameterProperty => new ParameterMetadata(
                        parameterProperty.Key,
                        GetType(parameterProperty.Value),
                        parameterProperty.Value.DefaultValue is null,
                        TryGetMetadataDescription(parameterProperty.Value.Metadata)))
                    .ToImmutableArray();
            });

            this.outputsLazy = new(() =>
            {
                if (this.SourceFile.Template?.Outputs is null)
                {
                    return ImmutableArray<OutputMetadata>.Empty;
                }

                return this.SourceFile.Template.Outputs
                    .Select(outputProperty => new OutputMetadata(
                        outputProperty.Key,
                        GetType(outputProperty.Value),
                        TryGetMetadataDescription(outputProperty.Value.Metadata)))
                    .ToImmutableArray();
            });
        }

        public ArmTemplateFile SourceFile { get; }

        public ResourceScope TargetScope => this.targetScopeLazy.Value == ResourceScope.None
            ? ResourceScope.ResourceGroup
            : this.targetScopeLazy.Value;

        public ImmutableArray<ParameterMetadata> Parameters => this.parametersLazy.Value;

        public ImmutableArray<OutputMetadata> Outputs => this.outputsLazy.Value;

        public bool HasErrors()
        {
            if (this.SourceFile.HasErrors())
            {
                return true;
            }

            if (this.targetScopeLazy.Value == ResourceScope.None)
            {
                return true;
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var visitor = new SemanticDiagnosticVisitor(diagnosticWriter);

            foreach (var parameter in this.Parameters)
            {
                visitor.Visit(parameter.TypeReference.Type);
            }

            foreach (var output in this.Outputs)
            {
                visitor.Visit(output.TypeReference.Type);
            }

            return diagnosticWriter.GetDiagnostics().Count > 0;
        }

        private static TypeSymbol GetType(TemplateInputParameter parameter)
        {
            var allowedValueTypes = GetAllowedValueTypes(parameter);

            bool AllowedStringLiteralsProvided() => allowedValueTypes.Any() && allowedValueTypes.All(x => x is StringLiteralType);

            return parameter.Type.Value switch
            {
                TemplateParameterType.String or TemplateParameterType.SecureString when AllowedStringLiteralsProvided() =>
                    TypeHelper.CreateTypeUnion(allowedValueTypes),

                TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(parameter, out var resourceType) =>
                    resourceType,

                TemplateParameterType.Array when AllowedStringLiteralsProvided() =>
                    new TypedArrayType(TypeHelper.CreateTypeUnion(allowedValueTypes), TypeSymbolValidationFlags.Default),

                _ => GetType((TemplateParameter)parameter),
            };
        }

        private static TypeSymbol GetType(TemplateOutputParameter output)
        {
            return output.Type.Value switch
            {
                TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(output, out var resourceType) =>
                    resourceType,

                _ => GetType((TemplateParameter)output),
            };
        }

        private static TypeSymbol GetType(TemplateParameter parameterOrOutput) => parameterOrOutput.Type.Value switch
        {
            TemplateParameterType.String => LanguageConstants.LooseString,
            TemplateParameterType.Int => LanguageConstants.Int,
            TemplateParameterType.Bool => LanguageConstants.Bool,
            TemplateParameterType.Array => LanguageConstants.Array,
            TemplateParameterType.Object => LanguageConstants.Object,
            TemplateParameterType.SecureString => LanguageConstants.SecureString,
            TemplateParameterType.SecureObject => LanguageConstants.SecureObject,
            _ => ErrorType.Empty(),
        };

        private static IEnumerable<TypeSymbol> GetAllowedValueTypes(TemplateInputParameter parameter)
        {
            if (parameter.AllowedValues is null)
            {
                yield break;
            }

            foreach (var allowedValue in parameter.AllowedValues.Value)
            {
                yield return allowedValue switch
                {
                    JArray => LanguageConstants.Array,
                    JObject => LanguageConstants.Object,
                    JValue when allowedValue.Type == JTokenType.Integer => LanguageConstants.Int,
                    JValue when allowedValue.Type == JTokenType.Boolean => LanguageConstants.Bool,
                    _ => new StringLiteralType(allowedValue.ToString()),
                };
            }
        }

        private static bool TryCreateUnboundResourceTypeParameter(TemplateParameter parameterOrOutput, [NotNullWhen(true)] out TypeSymbol? type)
        {
            if (parameterOrOutput.Metadata?.Value is JObject metadata &&
                metadata.TryGetValue(LanguageConstants.MetadataResourceTypePropertyName, out var obj) &&
                obj.Value<string>() is string resourceTypeRaw)
            {
                if (ResourceTypeReference.TryParse(resourceTypeRaw) is {} parsed)
                {
                    type = new UnboundResourceType(parsed);
                    return true;
                }

                // Return true here because it *was* a resource type, just a wrong one.
                type = ErrorType.Create(DiagnosticBuilder.ForDocumentStart().InvalidResourceType());
                return true;
            }

            type = null;
            return false;
        }

        private static string? TryGetMetadataDescription(TemplateGenericProperty<JToken>? metadata)
        {
            if (metadata?.Value?.SelectToken(LanguageConstants.MetadataDescriptionPropertyName) is { } descriptionToken
            && descriptionToken.Type is JTokenType.String)
            {
                return descriptionToken.ToString();
            }
            return null;
        }
    }
}
