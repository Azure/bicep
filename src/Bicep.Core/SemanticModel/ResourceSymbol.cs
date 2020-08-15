﻿using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class ResourceSymbol : DeclaredSymbol
    {
        public ResourceSymbol(ITypeManager typeManager, string name, ResourceDeclarationSyntax declaringSyntax, SyntaxBase body)
            : base(typeManager, name, declaringSyntax)
        {
            this.Body = body;
        }

        public ResourceDeclarationSyntax DeclaringResource => (ResourceDeclarationSyntax) this.DeclaringSyntax;

        public TypeSymbol Type
        {
            get
            {
                // if type string is malformed, the type value will be null which will resolve to a null type
                // below this will be corrected into an error type
                var stringSyntax = this.DeclaringResource.TryGetType();
                TypeSymbol? resourceType;

                if (stringSyntax != null && stringSyntax.IsInterpolated())
                {
                    // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                    // right now, codegen will still generate a format string however, which will cause problems for the type.
                    resourceType = new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(this.DeclaringResource.Type).ResourceTypeInterpolationUnsupported());
                }
                else
                {
                    var stringContent = stringSyntax?.GetLiteralValue();
                    resourceType = this.TypeManager.GetTypeByName(stringContent);

                    // TODO: This check is likely too simplistic
                    if (resourceType?.TypeKind != TypeKind.Resource)
                    {
                        resourceType = new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(this.DeclaringResource.Type).InvalidResourceType());
                    }
                }

                return resourceType;
            }
        }

        public SyntaxBase Body { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitResourceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Resource;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            return TypeValidator.GetExpressionAssignmentDiagnostics(this.TypeManager, this.Body, this.Type);
        }

        public override SyntaxBase? NameSyntax => (this.DeclaringSyntax as ResourceDeclarationSyntax)?.Name;
    }
}
