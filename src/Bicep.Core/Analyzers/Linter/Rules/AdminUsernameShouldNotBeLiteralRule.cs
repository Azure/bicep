// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class AdminUsernameShouldNotBeLiteralRule : LinterRuleBase
    {
        public new const string Code = "adminusername-should-not-be-literal";

        public AdminUsernameShouldNotBeLiteralRule() : base(
            code: Code,
            description: CoreResources.AdminUsernameShouldNotBeLiteralRuleDescription,
            LinterRuleCategory.Security
        )
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found literal string value \"{1}\"", this.Description, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var visitor = new ResourceVisitor(this, model, diagnosticLevel);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class ResourceVisitor : AstVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly AdminUsernameShouldNotBeLiteralRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public ResourceVisitor(AdminUsernameShouldNotBeLiteralRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                var visitor = new PropertiesVisitor(this.parent, diagnostics, model, diagnosticLevel);
                visitor.Visit(syntax);

                // Note: PropertiesVisitor will navigate through all child resources, so don't call base
            }
        }

        private class PropertiesVisitor : AstVisitor
        {
            private readonly List<IDiagnostic> diagnostics;

            private const string adminUsernamePropertyName = "adminusername";
            private readonly AdminUsernameShouldNotBeLiteralRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public PropertiesVisitor(AdminUsernameShouldNotBeLiteralRule parent, List<IDiagnostic> diagnostics, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnostics = diagnostics;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                string? propertyName = syntax.TryGetKeyText();
                // Check all properties with the name 'adminUsername', regardless of casing
                if (propertyName != null && StringComparer.OrdinalIgnoreCase.Equals(propertyName, adminUsernamePropertyName))
                {
                    var type = model.GetTypeInfo(syntax.Value);
                    if (type is StringLiteralType stringType)
                    {
                        diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Value.Span, stringType.RawStringValue));
                    }

                    // No need to traverse deeper
                }
                else
                {
                    base.VisitObjectPropertySyntax(syntax);
                }
            }
        }
    }
}
