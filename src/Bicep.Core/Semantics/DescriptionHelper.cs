// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.Parsing;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public static class DescriptionHelper
    {
        /// <summary>
        /// Retrieves description for a given syntax from a @description decorator
        /// </summary>
        public static string? TryGetFromDecorator(SemanticModel semanticModel, DecorableSyntax decorable)
            => TryGetFromDecorator(semanticModel.Binder, semanticModel.TypeManager, decorable);

        /// <summary>
        /// Retrieves description for a given syntax from a @description decorator
        /// </summary>
        public static string? TryGetFromDecorator(IBinder binder, ITypeManager typeManager, DecorableSyntax decorable)
        {
            var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(binder,
                typeManager.GetDeclaredType,
                decorable,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.MetadataDescriptionPropertyName);

            if (decorator is not null &&
                decorator.Arguments.FirstOrDefault()?.Expression is StringSyntax stringSyntax
                && stringSyntax.TryGetLiteralValue() is string description)
            {
                if (stringSyntax.IsVerbatimString())
                {
                    return StringUtils.NormalizeIndent(description);
                }

                return description;
            }

            return null;
        }

        /// <summary>
        /// Retrieves description for a given module repesented by a semantic model (bicep or json ARM)
        /// </summary>
        public static string? TryGetFromSemanticModel(ISemanticModel semanticModel)
        {
            if (semanticModel is SemanticModel bicepSemanticModel)
            {
                // Bicep - search for `metadata description = 'xxx'`
                var descriptionMetadata = bicepSemanticModel.Root.MetadataDeclarations
                    .FirstOrDefault(ms => ms.Name.Equals(LanguageConstants.MetadataDescriptionPropertyName, StringComparison.Ordinal));
                return (descriptionMetadata?.Value as StringSyntax)?.TryGetLiteralValue();
            }
            else if (semanticModel is ArmTemplateSemanticModel armSemanticModel)
            {
                // JSON - search for top-level "metadata.description" property
                return armSemanticModel.SourceFile.Template?.Metadata.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName)?.Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Retrieves description for a given module's OCI manifest
        /// </summary>
        /// <param name="ociAnnotations">The top-level "annotations" section of the manifest</param>
        public static string? TryGetFromOciManifestAnnotations(ImmutableDictionary<string, string>? ociAnnotations)
        {
            if (ociAnnotations is not null
                && ociAnnotations.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation, out string? description)
                && !string.IsNullOrWhiteSpace(description))
            {
                return description;
            }

            return null;
        }

        /// <summary>
        /// Retrieves description for a given module repesented by a JSON ARM template
        /// </summary>
        public static string? TryGetFromArmTemplate(Stream jsonArmTemplateContents)
        {
            var root = JsonElementFactory.CreateElementFromStream(jsonArmTemplateContents);
            return root.TryGetPropertyByPath($"metadata.{LanguageConstants.MetadataDescriptionPropertyName}")?.GetString();
        }

        public static string? TryGetFromArmTemplate(BinaryData jsonArmTemplateContents)
            => TryGetFromArmTemplate(jsonArmTemplateContents.ToStream());

        /// <summary>
        /// Retrieves description for a given module repesented by a template spec's contents
        /// </summary>
        public static string? TryGetFromTemplateSpec(Stream templateSpec)
        {
            var root = JsonElementFactory.CreateElementFromStream(templateSpec);
            return root.TryGetPropertyByPath($"properties.mainTemplate.metadata.{LanguageConstants.MetadataDescriptionPropertyName}")?.GetString();
        }
    }
}
