// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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

        public bool IsCollection => this.Type is ArrayType;

        public ResourceType? TryGetResourceType() => this.Type switch
        {
            ResourceType resourceType => resourceType,
            ArrayType { Item: ResourceType resourceType } => resourceType,
            _ => null,
        };

        public ResourceTypeReference GetResourceTypeReference() =>
            this.TryGetResourceTypeReference() ??  throw new ArgumentException($"Resource symbol does not have a valid type (found {this.Type.Name})");

        public ResourceTypeReference? TryGetResourceTypeReference() => this.TryGetResourceType()?.TypeReference;

        public ObjectType? TryGetBodyObjectType() => this.TryGetResourceType()?.Body.Type as ObjectType;
    }
}
