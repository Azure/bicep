// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class DeploySymbol : DeclaredSymbol
    {
        public DeploySymbol(ISymbolContext context, string name, DeployDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public DeployDeclarationSyntax DeclaringDeploySyntax => (DeployDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitDeploySymbol(this);

        public override SymbolKind Kind => SymbolKind.Deploy;

        public ResultWithDiagnostic<ISemanticModel> TryGetSemanticModel()
            => SemanticModelHelper.TryGetTemplateModelForArtifactReference(Context.Compilation.SourceFileGrouping,
                DeclaringDeploySyntax,
                b => b.ModuleDeclarationMustReferenceBicepModule(),
                Context.Compilation);

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}
