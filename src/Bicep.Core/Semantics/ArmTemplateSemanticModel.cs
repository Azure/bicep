// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public class ArmTemplateSemanticModel : ISemanticModel
    {
        private readonly Lazy<ResourceScope> targetScopeLazy;

        private readonly Lazy<ImmutableArray<TypeProperty>> parameterTypePropertiesLazy;

        private readonly Lazy<ImmutableArray<TypeProperty>> outputTypePropertiesLazy;

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

            this.parameterTypePropertiesLazy = new(() =>
            {
                if (this.SourceFile.Template?.Parameters is null)
                {
                    return ImmutableArray<TypeProperty>.Empty;
                }

                return this.SourceFile.Template.Parameters
                    .Select(parameterProperty => new TypeProperty(
                        parameterProperty.Key,
                        GetType(parameterProperty.Value),
                        parameterProperty.Value.DefaultValue is null
                            ? TypePropertyFlags.WriteOnly | TypePropertyFlags.Required
                            : TypePropertyFlags.WriteOnly,
                        TryGetMetadataDescription(parameterProperty.Value.Metadata)))
                    .ToImmutableArray();
            });

            this.outputTypePropertiesLazy = new(() =>
            {
                if (this.SourceFile.Template?.Outputs is null)
                {
                    return ImmutableArray<TypeProperty>.Empty;
                }

                return this.SourceFile.Template.Outputs
                    .Select(outputProperty => new TypeProperty(
                        outputProperty.Key,
                        GetType(outputProperty.Value),
                        TypePropertyFlags.ReadOnly,
                        TryGetMetadataDescription(outputProperty.Value.Metadata)))
                    .ToImmutableArray();
            });
        }

        public ArmTemplateFile SourceFile { get; }

        public ResourceScope TargetScope => this.targetScopeLazy.Value == ResourceScope.None
            ? ResourceScope.ResourceGroup
            : this.targetScopeLazy.Value;

        public ImmutableArray<TypeProperty> ParameterTypeProperties => this.parameterTypePropertiesLazy.Value;

        public ImmutableArray<TypeProperty> OutputTypeProperties => this.outputTypePropertiesLazy.Value;

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

            foreach (var parameterTypeProperty in this.ParameterTypeProperties)
            {
                visitor.Visit(parameterTypeProperty.TypeReference.Type);
            }

            foreach (var outputTypeProperty in this.OutputTypeProperties)
            {
                visitor.Visit(outputTypeProperty.TypeReference.Type);
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
                    UnionType.Create(allowedValueTypes),

                TemplateParameterType.Array when AllowedStringLiteralsProvided() =>
                    new TypedArrayType(UnionType.Create(allowedValueTypes), TypeSymbolValidationFlags.Default),

                _ => GetType((TemplateParameter)parameter),
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
