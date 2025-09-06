// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class ComponentSymbol : DeclaredSymbol
{
    public ComponentSymbol(ISymbolContext context, string name, ComponentDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
    }

    public ComponentDeclarationSyntax DeclaringComponent => (ComponentDeclarationSyntax)this.DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitComponentSymbol(this);

    public override SymbolKind Kind => SymbolKind.Component;

    public ResultWithDiagnostic<ISemanticModel> TryGetSemanticModel()
        => DeclaringComponent.TryGetReferencedModel(Context.SourceFileLookup, Context.ModelLookup, b => b.ModuleDeclarationMustReferenceBicepModule());

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