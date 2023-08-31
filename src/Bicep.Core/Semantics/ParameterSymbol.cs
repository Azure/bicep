// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System.Collections.Generic;

namespace Bicep.Core.Semantics
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(ISymbolContext context, string name, ParameterDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public ParameterDeclarationSyntax DeclaringParameter => (ParameterDeclarationSyntax)this.DeclaringSyntax;

        public override SymbolKind Kind => SymbolKind.Parameter;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParameterSymbol(this);
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public bool IsCollection => this.Type is ArrayType;

        public ResourceType? TryGetResourceType() => ResourceType.TryUnwrap(this.Type);

        public ResourceTypeReference? TryGetResourceTypeReference() => this.TryGetResourceType()?.TypeReference;

        public ObjectType? TryGetBodyObjectType() => this.TryGetResourceType()?.Body.Type as ObjectType;
    }
}
