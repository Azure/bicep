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
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Exceptions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public class ArmTemplateSemanticModel : ISemanticModel
    {
        private readonly Lazy<ResourceScope> targetScopeLazy;
        private readonly Lazy<ImmutableSortedDictionary<string, ParameterMetadata>> parametersLazy;
        private readonly Lazy<ImmutableSortedDictionary<string, ExportMetadata>> exportsLazy;
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

                return schemaUri.AbsolutePath switch
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
                    return ImmutableSortedDictionary<string, ParameterMetadata>.Empty;
                }

                // the source here is of type InsensitiveDictionary<TemplateInputParameter>,
                // which is basically a Dictionary<string, TemplateInputParameter> that uses an
                // InvariantCultureIgnoreCase key comparer
                // there should be no possibility of clashes since the target dictionary is using
                // Ordinal key comparer, which looks purely at the key string code points
                // without applying any additional rules
                return this.SourceFile.Template.Parameters
                    .ToImmutableSortedDictionary(
                        parameterProperty => parameterProperty.Key,
                        parameterProperty =>
                        {
                            var type = GetType(parameterProperty.Value);

                            return new ParameterMetadata(
                                parameterProperty.Key,
                                type,
                                parameterProperty.Value.DefaultValue is null && !TypeHelper.IsNullable(type),
                                GetMostSpecificDescription(parameterProperty.Value));
                        },
                        LanguageConstants.IdentifierComparer);
            });

            this.exportsLazy = new(FindExports);

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

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => this.parametersLazy.Value;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => exportsLazy.Value;

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

            foreach (var parameter in this.Parameters.Values)
            {
                visitor.Visit(parameter.TypeReference.Type);
            }

            foreach (var output in this.Outputs)
            {
                visitor.Visit(output.TypeReference.Type);
            }

            return diagnosticWriter.GetDiagnostics().Count > 0;
        }

        private TypeSymbol GetType(TemplateInputParameter parameter) => parameter.Type?.Value switch
        {
            TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(GetMetadata(parameter), out var resourceType) =>
                resourceType,

            _ => GetType((ITemplateSchemaNode)parameter, allowLooseAssignment: true),
        };

        private TypeSymbol GetType(ITemplateSchemaNode schemaNode, bool allowLooseAssignment = false)
            => ArmTemplateTypeLoader.ToTypeSymbol(SchemaValidationContext.ForTemplate(SourceFile.Template),
                schemaNode,
                allowLooseAssignment ? TypeSymbolValidationFlags.AllowLooseAssignment : TypeSymbolValidationFlags.Default);

        /// <summary>
        /// Metadata may be attached to $ref nodes, and the appropriate description for a given parameter or property will be the first one (if any) encountered while following $ref pointers to a concrete type.
        /// </summary>
        /// <param name="schemaNode">The starting point for the search</param>
        /// <returns></returns>
        private string? GetMostSpecificDescription(ITemplateSchemaNode schemaNode)
        {
            if (GetMetadata(schemaNode) is JObject metadataObject &&
                metadataObject.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName, out var descriptionToken) &&
                descriptionToken is JValue { Value: string description })
            {
                return description;
            }

            return null;
        }

        private JToken? GetMetadata(ITemplateSchemaNode schemaNode)
        {
            try
            {
                return TemplateEngine.ResolveSchemaReferences(SourceFile.Template, schemaNode).Metadata?.Value;
            }
            catch (TemplateValidationException)
            {
                return null;
            }
        }

        private TypeSymbol GetType(TemplateOutputParameter output)
        {
            return output.Type?.Value switch
            {
                TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(output.Metadata?.Value, out var resourceType) =>
                    resourceType,

                _ => GetType((ITemplateSchemaNode)output),
            };
        }

        private ImmutableSortedDictionary<string, ExportMetadata> FindExports()
        {
            if (SourceFile.Template is not { } template || SourceFile.TemplateObject is not { } templateObject)
            {
                return ImmutableSortedDictionary<string, ExportMetadata>.Empty;
            }

            List<ExportMetadata> exports = new();

            if (template.Definitions is { } typeDefinitions)
            {
                exports.AddRange(typeDefinitions.Where(kvp => IsExported(kvp.Value))
                    .Select(kvp => new ExportedTypeMetadata(kvp.Key, GetType(kvp.Value), GetMostSpecificDescription(kvp.Value))));
            }

            if (template.Metadata?.TryGetValue(LanguageConstants.TemplateMetadataExportedVariablesName, out var exportedVariables) is true
                && exportedVariables.Value is JArray exportedVariablesList)
            {
                TemplateVariablesEvaluator evaluator = new(template);

                foreach (var exportedVariable in exportedVariablesList)
                {
                    if (exportedVariable is JObject exportedVariableObject &&
                        exportedVariableObject.TryGetValue("name", StringComparison.OrdinalIgnoreCase, out var nameToken) &&
                        nameToken is JValue { Value: string name } &&
                        evaluator.TryGetEvaluatedVariableValue(name) is JToken evaluatedValue)
                    {
                        exports.Add(new ExportedVariableMetadata(name, SystemNamespaceType.ConvertJsonToBicepType(evaluatedValue), GetDescription(exportedVariableObject)));
                    }
                }
            }

            var exportsBuilder = ImmutableSortedDictionary.CreateBuilder<string, ExportMetadata>();

            foreach (var exportsByName in exports.ToLookup(e => e.Name))
            {
                // Each export needs a unique name, so if two or more exports of different types share a name, they are no longer uniquely identifiable by name
                // This will never come up for templates authored in Bicep, but it may for templates authored directly in ARM JSON.
                // A necessary consequence of skipping exports that share a name is that adding a second export of a given name (e.g., if there is already a
                // type export named 'foo' and the template author exports a variable named 'foo') is a **backwards incompatible change**.
                if (exportsByName.Count() == 1)
                {
                    exportsBuilder.Add(exportsByName.Key, exportsByName.Single());
                }
                else
                {
                    exportsBuilder.Add(exportsByName.Key, new DuplicatedExportMetadata(exportsByName.Key, exportsByName.Select(e => e.Kind.ToString()).ToImmutableArray()));
                }
            }

            return exportsBuilder.ToImmutable();
        }

        private static bool TryCreateUnboundResourceTypeParameter(JToken? metadataToken, [NotNullWhen(true)] out TypeSymbol? type)
        {
            if (metadataToken is JObject metadata &&
                metadata.TryGetValue(LanguageConstants.MetadataResourceTypePropertyName, out var obj) &&
                obj.Value<string>() is string resourceTypeRaw)
            {
                if (ResourceTypeReference.TryParse(resourceTypeRaw) is { } parsed)
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

        /// <summary>
        /// Determines if the provided type definition should be allowlisted for use in <c>import</c> statements
        /// </summary>
        /// <remarks>
        /// This method does not use <see cref="GetMetadata"/> because <see cref="GetMetadata"/> merges metadata across $refs.
        /// We only want to look at the metadata explicitly applied to this type.
        /// E.g., in the following, `public` should match the predicate and `private` should not:
        /// <code>
        ///   {
        ///      "public": {"type": "string", "metadata": {"__bicep_export!": true}},
        ///      "private": {"$ref": "#/definitions/public"}
        ///   }
        /// </code>
        /// The above would be compiled from the following Bicep:
        /// <code>
        ///   @export()
        ///   type public = string
        ///   type private = public
        /// </code>
        /// </remarks>
        private static bool IsExported(TemplateTypeDefinition typeDefinition) => typeDefinition.Metadata?.Value is JObject metadataDict &&
            metadataDict.TryGetValue(LanguageConstants.MetadataExportedPropertyName, out var exportMarkerToken) &&
            exportMarkerToken is JValue { Value: true };

        private static string? GetDescription(JObject jObject)
            => jObject.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName, out var descriptionToken) && descriptionToken is JValue { Value: string description }
                ? description
                : null;
    }
}
