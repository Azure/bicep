// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class StackSymbol : DeclaredSymbol
{
    public StackSymbol(ISymbolContext context, string name, StackDeclarationSyntax declaringSyntax, GroupDeclarationSyntax? declaringGroup = null)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
        DeclaringGroup = declaringGroup;
    }

    public StackDeclarationSyntax DeclaringStack => (StackDeclarationSyntax)this.DeclaringSyntax;

    public GroupDeclarationSyntax? DeclaringGroup { get; }

    public string SymbolicName => DeclaringGroup is { } group ? $"{group.Name.IdentifierName}::{Name}" : Name;

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

    public bool IsCollection => this.Type is ArrayType || DeclaringGroup?.Value is ForSyntax;

    public ModuleType? TryGetModuleType() => ModuleType.TryUnwrap(this.Type);

    public ObjectType? TryGetBodyObjectType() => this.TryGetModuleType()?.Body.Type as ObjectType;
}