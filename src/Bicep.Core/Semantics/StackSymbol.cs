// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class StackSymbol : DeclaredSymbol
{
    public StackSymbol(ISymbolContext context, string name, StackDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
    }

    public StackDeclarationSyntax DeclaringStack => (StackDeclarationSyntax)this.DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitStackSymbol(this);

    public override SymbolKind Kind => SymbolKind.Stack;

    public ResultWithDiagnostic<ISemanticModel> TryGetSemanticModel()
        => DeclaringStack.TryGetReferencedModel(Context.SourceFileLookup, Context.ModelLookup, b => b.ModuleDeclarationMustReferenceBicepModule());

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