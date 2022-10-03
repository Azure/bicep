// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // Virtual machines shouldn't use preview images.
    // Below class checks imageReference properties - offer, sku and version and fails the rule
    // if they contain string "preview"
    public sealed class UseStableVMImageRule : LinterRuleBase
    {
        public new const string Code = "use-stable-vm-image";

        private readonly ImmutableHashSet<string> imageReferenceProperties = ImmutableHashSet.Create<string>("offer", "sku", "version");

        public UseStableVMImageRule() : base(
            code: Code,
            description: CoreResources.UseStableVMImage,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        {
        }
        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UseStableVMImageRuleFixMessageFormat, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var diagnosticLevel = GetDiagnosticLevel(model);
            List<IDiagnostic> diagnostics = new();

            foreach (DeclaredResourceMetadata resource in model.DeclaredResources)
            {
                ResourceDeclarationSyntax resourceSyntax = resource.Symbol.DeclaringResource;
                if (resourceSyntax.TryGetBody()?.TryGetPropertyByNameRecursive("properties", "storageProfile", "imageReference") is ObjectPropertySyntax imageReferenceSyntax)
                {
                    var imageReferenceValue = imageReferenceSyntax.Value;

                    if (imageReferenceValue is ObjectSyntax imageReferenceProperties)
                    {
                        AddDiagnosticsIfImageReferencePropertiesContainPreview(diagnosticLevel, imageReferenceProperties, diagnostics);
                    }
                    else if (imageReferenceValue is VariableAccessSyntax &&
                             model.GetSymbolInfo(imageReferenceValue) is VariableSymbol variableSymbol &&
                             variableSymbol.Value is ObjectSyntax variableValueSyntax)
                    {
                        AddDiagnosticsIfImageReferencePropertiesContainPreview(diagnosticLevel, variableValueSyntax, diagnostics);
                    }
                }
            }

            return diagnostics;
        }

        private void AddDiagnosticsIfImageReferencePropertiesContainPreview(DiagnosticLevel diagnosticLevel, ObjectSyntax objectSyntax, List<IDiagnostic> diagnostics)
        {
            foreach (string property in imageReferenceProperties)
            {
                if (objectSyntax.TryGetPropertyByNameRecursive(property) is ObjectPropertySyntax objectPropertySyntax &&
                    objectPropertySyntax.Value is StringSyntax valueSyntax &&
                    valueSyntax.TryGetLiteralValue() is string value &&
                    value.Contains("preview", StringComparison.OrdinalIgnoreCase))
                {
                    diagnostics.Add(CreateDiagnosticForSpan(diagnosticLevel, valueSyntax.Span, property));
                }
            }
        }
    }
}
