// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class OutputSymbol : DeclaredSymbol
    {
        public OutputSymbol(ISymbolContext context, string name, OutputDeclarationSyntax declaringSyntax, SyntaxBase value)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Value = value;
        }

        public OutputDeclarationSyntax DeclaringOutput => (OutputDeclarationSyntax) this.DeclaringSyntax;

        public override TypeSymbol Type => this.GetPrimitiveTypeByName(this.DeclaringOutput.Type.TypeName) ?? new ErrorTypeSymbol(DiagnosticBuilder.ForPosition(this.DeclaringOutput.Type).InvalidOutputType());

        public SyntaxBase Value { get; }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitOutputSymbol(this);
        }

        public override SymbolKind Kind => SymbolKind.Output;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            TypeSymbol valueType = this.Context.TypeManager.GetTypeInfo(this.Value, new TypeManagerContext());

            // this type is not a property in a symbol so the semantic error visitor won't collect the errors automatically
            if (valueType is ErrorTypeSymbol)
            {
                return valueType.GetDiagnostics();
            }

            if (TypeValidator.AreTypesAssignable(valueType, this.Type) == false)
            {
                return this.CreateError(this.Value, b => b.OutputTypeMismatch(this.Type.Name, valueType.Name)).AsEnumerable();
            }

            return Enumerable.Empty<ErrorDiagnostic>();
        }
    }
}

