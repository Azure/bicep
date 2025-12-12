// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Reflection;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Syntax;

public static class SyntaxExtensions
{
    private static TypeProperty? TryGetTypeProperty(SemanticModel model, SyntaxBase objectSyntax, string propertyName)
    {
        // Cannot use assigned type here because it won't handle the case where the property value
        // is an array access or a string interpolation.
        return model.TypeManager.GetDeclaredType(objectSyntax) switch
        {
            ObjectType { Properties: var properties }
                when properties.TryGetValue(propertyName, out var typeProperty) => typeProperty,
            DiscriminatedObjectType { DiscriminatorKey: var discriminatorKey, DiscriminatorProperty: var typeProperty }
                when LanguageConstants.IdentifierComparer.Equals(propertyName, discriminatorKey) => typeProperty,
            _ => null,
        };
    }

    public static TypeProperty? TryGetTypeProperty(this ObjectPropertySyntax syntax, SemanticModel model)
    {
        if (syntax.TryGetKeyText() is not string propertyName || model.Binder.GetParent(syntax) is not ObjectSyntax objectSyntax)
        {
            return null;
        }

        return TryGetTypeProperty(model, objectSyntax, propertyName);
    }

    /// <remarks>
    /// If a chain of accesses starts with a "safe" access (e.g., <code><i>base</i>[?0].property</code> or <code><i>base</i>.?some.deeply.nested.property</code>),
    /// it may short-circuit at runtime, meaning that <code>.deeply.nested.property</code> will only be evaluated if <code><i>base</i>.?some</code> returns a non-null value.
    /// The upshot of this is that we will need to mark <code><i>base</i>.?some</code> as non-nullable when evaluating any chained property accesses, then
    /// mark the resultant type as nullable iff the original "safe" access might return null.
    /// Because of this requirement, it's necessary to evaluate the full access chain and determine if it is kicked off by a .? or [?] operator rather than
    /// just evaluating <code>syntax.BaseExpression</code> recursively
    /// </remarks>
    public static Stack<AccessExpressionSyntax> ToAccessExpressionStack(this AccessExpressionSyntax syntax)
    {
        Stack<AccessExpressionSyntax> chainedAccesses = new();
        chainedAccesses.Push(syntax);

        while (chainedAccesses.TryPeek(out var current) && !current.IsSafeAccess && current.BaseExpression is AccessExpressionSyntax baseAccessExpression)
        {
            chainedAccesses.Push(baseAccessExpression);
        }

        return chainedAccesses;
    }

    /// <summary>
    /// Checks whether the a particular access expression is accessing an expression matching the target expression.
    /// For example, if the target expression is `'bar'`, this would return true for either `foo['bar']` or `foo.bar`.
    /// If the target expression is `myExpr`. this would return true for `foo[myExpr]`.
    /// </summary>
    public static bool AccessExpressionMatches(this AccessExpressionSyntax access, SyntaxBase targetExpression)
    {
        if (access is ArrayAccessSyntax arrayAccess &&
            SyntaxIgnoringTriviaComparer.Instance.Equals(arrayAccess.IndexExpression, targetExpression))
        {
            return true;
        }

        if (access is PropertyAccessSyntax propertyAccess &&
            targetExpression is StringSyntax stringSyntax &&
            stringSyntax.TryGetLiteralValue() is { } literalValue &&
            propertyAccess.PropertyName.NameEquals(literalValue))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks for secure decorator in output decleration.
    /// </summary>
    public static bool HasSecureDecorator(this DecorableSyntax syntax, IBinder binder, ITypeManager typeManager)
    {
        return SemanticModelHelper.TryGetDecoratorInNamespace(binder, typeManager.GetDeclaredType, syntax, SystemNamespaceType.BuiltInName, LanguageConstants.ParameterSecurePropertyName) is not null;
    }

    /// <summary>
    /// Checks if the syntax has a decorator with the specified name.
    /// </summary>
    public static bool HasDecorator(this DecorableSyntax syntax, string decoratorName)
        => syntax.Decorators.Any(d => d.Expression is FunctionCallSyntax functionCall && functionCall.NameEquals(decoratorName));

    public static bool HasPropertyName(this ObjectPropertySyntax syntax, string name)
        => syntax.TryGetKeyText() is { } keyText && LanguageConstants.IdentifierComparer.Equals(keyText, name);
}
