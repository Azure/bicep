// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.SemanticModel
{
    public class ModuleSymbol : DeclaredSymbol
    {
        public ModuleSymbol(ISymbolContext context, string name, ModuleDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public ModuleDeclarationSyntax DeclaringModule => (ModuleDeclarationSyntax) this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitModuleSymbol(this);

        public override SymbolKind Kind => SymbolKind.Module;

        public bool TryGetSemanticModel([NotNullWhen(true)] out SemanticModel? semanticModel, [NotNullWhen(false)] out ErrorDiagnostic? failureDiagnostic)
        {            
            if (Context.Compilation.SyntaxTreeGrouping.ModuleFailureLookup.TryGetValue(this.DeclaringModule, out var moduleFailureDiagnostic))
            {
                failureDiagnostic = moduleFailureDiagnostic(DiagnosticBuilder.ForPosition(this.DeclaringModule.Path));
                semanticModel = null;
                return false;
            }

            if (!Context.Compilation.SyntaxTreeGrouping.ModuleLookup.TryGetValue(this.DeclaringModule, out var syntaxTree))
            {
                failureDiagnostic = DiagnosticBuilder.ForPosition(this.DeclaringModule.Path).GenericModuleLoadFailure();
                semanticModel = null;
                return false;
            }

            failureDiagnostic = null;
            semanticModel = Context.Compilation.GetSemanticModel(syntaxTree);
            return true;
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}