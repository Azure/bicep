// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public class FunctionSymbol : Symbol, IFunctionSymbol
    {
        public FunctionSymbol(ObjectType declaringObject, string name, IEnumerable<FunctionOverload> overloads)
            : base(name)
        {
            Overloads = [.. overloads];
            FunctionFlags = Overloads.First().Flags;

            if (Overloads.Skip(1).Any(fo => fo.Flags != FunctionFlags))
            {
                // we should catch this as early as possible
                throw new ArgumentException("Inconsistent function flags found on overloads");
            }
            DeclaringObject = declaringObject;
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitFunctionSymbol(this);

        public override SymbolKind Kind => SymbolKind.Function;

        public ImmutableArray<FunctionOverload> Overloads { get; }

        public FunctionFlags FunctionFlags { get; }

        public ObjectType DeclaringObject { get; }
    }
}
