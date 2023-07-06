// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class TestSymbol : DeclaredSymbol
    {
        public TestSymbol(ISymbolContext context, string name, TestDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public TestDeclarationSyntax DeclaringTest => (TestDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitTestSymbol(this);

        public override SymbolKind Kind => SymbolKind.Test;

        public bool TryGetSemanticModel([NotNullWhen(true)] out ISemanticModel? semanticModel, [NotNullWhen(false)] out ErrorDiagnostic? failureDiagnostic)
        {
            if (Context.Compilation.SourceFileGrouping.TryGetErrorDiagnostic(this.DeclaringTest) is {} errorBuilder)
            {
                semanticModel = null;
                failureDiagnostic = errorBuilder(DiagnosticBuilder.ForPosition(DeclaringTest.Path));
                return false;
            }

            // SourceFileGroupingBuilder should have already visited every test declaration and either recorded a failure or mapped it to a syntax tree.
            // So it is safe to assume that this lookup will succeed without throwing an exception.
            var sourceFile = Context.Compilation.SourceFileGrouping.TryGetSourceFile(this.DeclaringTest) ?? throw new InvalidOperationException($"Failed to find source file for Test");

            // when we inevitably add a third language ID,
            // the inclusion list style below will prevent the new language ID from being
            // automatically allowed to be referenced via module declarations
            if (sourceFile is not BicepFile and not ArmTemplateFile and not TemplateSpecFile)
            {
                semanticModel = null;
                failureDiagnostic = DiagnosticBuilder.ForPosition(DeclaringTest.Path).TestDeclarationMustReferenceBicepTest();
                return false;
            }

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

        // public bool IsCollection => this.Type is ArrayType;

        // public TestType? TryGetModuleType() => TestType.TryUnwrap(this.Type);

        // public ObjectType? TryGetBodyObjectType() => this.TryGetModuleType()?.Body.Type as ObjectType;
    }
}
