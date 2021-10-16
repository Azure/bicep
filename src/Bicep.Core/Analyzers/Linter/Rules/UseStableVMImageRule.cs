// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // Virtual machines shouldn't use preview images.
    // Below class checks imageReference properties - offer, sku and version and fails the rule
    // if they contain string "preview"
    public sealed class UseStableVMImageRule : LinterRuleBase
    {
        public new const string Code = "use-stable-vm-image";

        public UseStableVMImageRule() : base(
            code: Code,
            description: CoreResources.UseStableVMImage,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        {
        }
        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UseStableVMImageFormat, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var visitor = new ResourceVisitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class ResourceVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private readonly SemanticModel model;
            private readonly UseStableVMImageRule parent;

            public ResourceVisitor(UseStableVMImageRule parent, SemanticModel model)
            {
                this.model = model;
                this.parent = parent;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                var visitor = new PropertiesVisitor(this.parent, diagnostics, model);
                visitor.Visit(syntax);

                // Note: PropertiesVisitor will navigate through all child resources, so don't call base
            }
        }

        private class PropertiesVisitor : SyntaxVisitor
        {
            private List<IDiagnostic> diagnostics;

            private const string imageReferencePropertyName = "imageReference";

            private readonly SemanticModel model;
            private readonly UseStableVMImageRule parent;

            public PropertiesVisitor(UseStableVMImageRule parent, List<IDiagnostic> diagnostics, SemanticModel model)
            {
                this.model = model;
                this.parent = parent;
                this.diagnostics = diagnostics;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                string? propertyName = syntax.TryGetKeyText();
                // Check all properties with the name 'imageReference', regardless of casing
                if (propertyName != null && StringComparer.OrdinalIgnoreCase.Equals(propertyName, imageReferencePropertyName))
                {
                    if (syntax.Value is ObjectSyntax objectSyntax)
                    {
                        AddDiagnosticsIfImageReferencePropertiesContainPreview(objectSyntax);
                    }
                    else if (syntax.Value is VariableAccessSyntax &&
                             model.GetSymbolInfo(syntax.Value) is VariableSymbol variableSymbol &&
                             variableSymbol.Value is ObjectSyntax variableValueSyntax)
                    {
                        AddDiagnosticsIfImageReferencePropertiesContainPreview(variableValueSyntax);
                    }
                }
                else
                {
                    base.VisitObjectPropertySyntax(syntax);
                }
            }

            private void AddDiagnosticsIfImageReferencePropertiesContainPreview(ObjectSyntax objectSyntax)
            {
                foreach (var child in objectSyntax.Children)
                {
                    if (child is ObjectPropertySyntax objectPropertySyntax &&
                        objectPropertySyntax.TryGetKeyText() is string key &&
                        objectPropertySyntax.Value is StringSyntax valueSyntax &&
                        valueSyntax.TryGetLiteralValue() is string value &&
                        ValueContainsPreview(key, value))
                    {
                        diagnostics.Add(parent.CreateDiagnosticForSpan(valueSyntax.Span, key));
                    }
                }
            }

            private bool ValueContainsPreview(string key, string value)
            {
                return (key.Equals("offer", StringComparison.OrdinalIgnoreCase) ||
                        key.Equals("sku", StringComparison.OrdinalIgnoreCase) ||
                        key.Equals("version", StringComparison.OrdinalIgnoreCase)) &&
                        value.Contains("preview");
            }
        }
    }
}
