// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ResourceSymbol : DeclaredSymbol
    {
        public ResourceSymbol(ISymbolContext context, string name, ResourceDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public ResourceDeclarationSyntax DeclaringResource => (ResourceDeclarationSyntax) this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitResourceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Resource;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public bool IsCollection => this.Context.TypeManager.GetTypeInfo(this.DeclaringResource) is ArrayType;

        public ResourceTypeReference? TryGetResourceTypeReference()
        {
            if (this.Type is ResourceType resourceType)
            {
                return resourceType.TypeReference;
            }

            if (this.Type is ArrayType arrayType && arrayType.Item.Type is ResourceType itemType)
            {
                return itemType.TypeReference;
            }

            return null;
        }
    }
}