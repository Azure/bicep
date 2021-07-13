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

        public ModuleDeclarationSyntax DeclaringModule => (ModuleDeclarationSyntax) this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitModuleSymbol(this);

        public override SymbolKind Kind => SymbolKind.Module;

        public bool TryGetSemanticModel([NotNullWhen(true)] out ISemanticModel? semanticModel, [NotNullWhen(false)] out ErrorDiagnostic? failureDiagnostic)
        {            
            if (Context.Compilation.SourceFileGrouping.ModuleFailureLookup.TryGetValue(this.DeclaringModule, out var moduleFailureDiagnostic))
            {
                failureDiagnostic = moduleFailureDiagnostic(DiagnosticBuilder.ForPosition(this.DeclaringModule.Path));
                semanticModel = null;
                return false;
            }

            // SyntaxTreeGroupingBuilder should have already visited every module declaration and either recorded a failure or mapped it to a syntax tree.
            // So it is safe to assume that this lookup will succeed without throwing an exception.
            var sourceFile = Context.Compilation.SourceFileGrouping.ModuleLookup[this.DeclaringModule];

            failureDiagnostic = null;
            semanticModel = Context.Compilation.GetSemanticModel(sourceFile);
            return true;
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public bool IsCollection => this.Type is ArrayType;

        public ModuleType? TryGetModuleType() => this.Type switch
        {
            ModuleType moduleType => moduleType,
            ArrayType { Item: ModuleType moduleType } => moduleType,
            _ => null,
        };

        public ObjectType? TryGetBodyObjectType() => this.TryGetModuleType()?.Body.Type as ObjectType;
    }
}
