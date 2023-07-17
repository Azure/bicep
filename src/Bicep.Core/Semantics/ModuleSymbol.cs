// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class ModuleSymbol : DeclaredSymbol
    {
        public ModuleSymbol(ISymbolContext context, string name, ModuleDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public ModuleDeclarationSyntax DeclaringModule => (ModuleDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitModuleSymbol(this);

        public override SymbolKind Kind => SymbolKind.Module;

        public bool TryGetSemanticModel([NotNullWhen(true)] out ISemanticModel? semanticModel, [NotNullWhen(false)] out ErrorDiagnostic? failureDiagnostic)
            => SemanticModelHelper.TryGetSemanticModelForForeignTemplateReference(Context.Compilation.SourceFileGrouping,
                DeclaringModule,
                b => b.ModuleDeclarationMustReferenceBicepModule(),
                Context.Compilation,
                out semanticModel,
                out failureDiagnostic);

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public bool IsCollection => this.Type is ArrayType;

        public ModuleType? TryGetModuleType() => ModuleType.TryUnwrap(this.Type);

        public ObjectType? TryGetBodyObjectType() => this.TryGetModuleType()?.Body.Type as ObjectType;
    }
}
