// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class AdminUsernameShouldNotBeLiteralRule : LinterRuleBase
    {
        public new const string Code = "adminusername-should-not-be-literal";

        public AdminUsernameShouldNotBeLiteralRule() : base(
            code: Code,
            description: CoreResources.AdminUsernameShouldNotBeLiteralRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/${Code}")
        )
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found literal string value \"{1}\"", this.Description, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var visitor = new ResourceVisitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class ResourceVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private readonly AdminUsernameShouldNotBeLiteralRule parent;
            private readonly SemanticModel model;

            public ResourceVisitor(AdminUsernameShouldNotBeLiteralRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
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

            private const string adminUsernamePropertyName = "adminusername";
            private readonly AdminUsernameShouldNotBeLiteralRule parent;
            private readonly SemanticModel model;

            public PropertiesVisitor(AdminUsernameShouldNotBeLiteralRule parent, List<IDiagnostic> diagnostics, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
                this.diagnostics = diagnostics;
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
                        diagnostics.Add(parent.CreateDiagnosticForSpan(syntax.Value.Span, stringType.RawStringValue));
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
