// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public class StepSymbol : DeclaredSymbol
    {
        public StepSymbol(ISymbolContext context, string name, StepDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public StepDeclarationSyntax DeclaringModule => (StepDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitStepSymbol(this);

        public override SymbolKind Kind => SymbolKind.Step;

        public ResultWithDiagnostic<ISemanticModel> TryGetSemanticModel()
            => DeclaringModule.TryGetReferencedModel(Context.SourceFileLookup, Context.ModelLookup, b => b.ModuleDeclarationMustReferenceBicepModule());

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public bool IsCollection => this.Type is ArrayType;
    }
}
