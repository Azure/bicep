// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit;

public class RequiresReferencesFunctionVisitor(IBinder binder) : AstVisitor
{
    private bool hasResourceReferencesAccess;
    private ObjectPropertySyntax? currentDependsOnProperty;

    public static bool RequiresReferencesFunction(IBinder binder)
    {
        var visitor = new RequiresReferencesFunctionVisitor(binder);
        visitor.Visit(binder.FileSymbol.Syntax);

        return visitor.hasResourceReferencesAccess;
    }

    public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
    {
        this.currentDependsOnProperty = syntax.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ResourceDependsOnPropertyName);
        base.VisitResourceDeclarationSyntax(syntax);
    }

    public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
    {
        this.currentDependsOnProperty = syntax.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ResourceDependsOnPropertyName);
        base.VisitModuleDeclarationSyntax(syntax);
    }

    public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
    {
        if (ReferenceEquals(syntax, currentDependsOnProperty))
        {
            return;
        }

        base.VisitObjectPropertySyntax(syntax);
    }

    public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
    {
        var symbol = binder.GetSymbolInfo(syntax);
        if (symbol is ResourceSymbol { IsCollection: true } or ModuleSymbol { IsCollection: true } &&
            binder.GetParentIgnoringParentheses(syntax) is not AccessExpressionSyntax)
        {
            hasResourceReferencesAccess = true;
        }
    }
}