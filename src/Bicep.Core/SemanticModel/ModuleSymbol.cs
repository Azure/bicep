// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;
using System;

namespace Bicep.Core.SemanticModel
{
    public class ModuleSymbol : DeclaredSymbol
    {
        public ModuleSymbol(ISymbolContext context, string name, ModuleDeclarationSyntax declaringSyntax, SyntaxBase body)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.Body = body;
        }

        public ModuleDeclarationSyntax DeclaringModule => (ModuleDeclarationSyntax) this.DeclaringSyntax;

        public SyntaxBase Body { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitModuleSymbol(this);

        public override SymbolKind Kind => SymbolKind.Module;

        public SemanticModel? TryGetSemanticModel(out ErrorDiagnostic? failureDiagnostic)
        {
            if (Context.Compilation.SyntaxTreeGrouping.ModuleFailureLookup.TryGetValue(this.DeclaringModule, out var moduleFailureDiagnostic))
            {
                failureDiagnostic = moduleFailureDiagnostic(DiagnosticBuilder.ForPosition(this.DeclaringModule.Path));
                return null;
            }

            var syntaxTree = Context.Compilation.SyntaxTreeGrouping.ModuleLookup[this.DeclaringModule];

            failureDiagnostic = null;
            return Context.Compilation.GetSemanticModel(syntaxTree);
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