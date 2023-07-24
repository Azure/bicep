// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Intermediate;

public record IndexReplacementContext(
    ImmutableDictionary<LocalVariableSymbol, Expression> LocalReplacements,
    Expression Index);

[DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
public abstract record Expression(
    SyntaxBase? SourceSyntax)
{
    public abstract void Accept(IExpressionVisitor visitor);

    public string GetDebuggerDisplay()
    {
        var name = this.GetType().Name;
        var attributes = GetDebugAttributes();

        if (attributes is null)
        {
            return name;
        }

        return $"{name} {attributes}";
    }

    protected virtual object? GetDebugAttributes() => null;
}

public record BooleanLiteralExpression(
    SyntaxBase? SourceSyntax,
    bool Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBooleanLiteralExpression(this);

    protected override object? GetDebugAttributes() => new { Value };
}

public record IntegerLiteralExpression(
    SyntaxBase? SourceSyntax,
    long Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitIntegerLiteralExpression(this);

    protected override object? GetDebugAttributes() => new { Value };
}

public record StringLiteralExpression(
    SyntaxBase? SourceSyntax,
    string Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitStringLiteralExpression(this);

    protected override object? GetDebugAttributes() => new { Value };
}

public record NullLiteralExpression(
    SyntaxBase? SourceSyntax
): Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitNullLiteralExpression(this);
}

public record InterpolatedStringExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<string> SegmentValues,
    ImmutableArray<Expression> Expressions
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitInterpolatedStringExpression(this);
}

public record ObjectPropertyExpression(
    SyntaxBase? SourceSyntax,
    Expression Key,
    Expression Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectPropertyExpression(this);
}

public record ObjectExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<ObjectPropertyExpression> Properties
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectExpression(this);
}

public record ArrayExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<Expression> Items
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayExpression(this);
}

public record TernaryExpression(
    SyntaxBase? SourceSyntax,
    Expression Condition,
    Expression True,
    Expression False
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitTernaryExpression(this);
}

public record BinaryExpression(
    SyntaxBase? SourceSyntax,
    BinaryOperator Operator,
    Expression Left,
    Expression Right
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBinaryExpression(this);

    protected override object? GetDebugAttributes() => new { Operator };
}

public record UnaryExpression(
    SyntaxBase? SourceSyntax,
    UnaryOperator Operator,
    Expression Expression
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitUnaryExpression(this);

    protected override object? GetDebugAttributes() => new { Operator };
}

public record FunctionCallExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    ImmutableArray<Expression> Parameters
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitFunctionCallExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record ResourceFunctionCallExpression(
    SyntaxBase? SourceSyntax,
    ResourceReferenceExpression Resource,
    string Name,
    ImmutableArray<Expression> Parameters
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitResourceFunctionCallExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public abstract record AccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    Expression Access,
    AccessExpressionFlags Flags
) : Expression(SourceSyntax) { }

public record ArrayAccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    Expression Access,
    AccessExpressionFlags Flags
) : AccessExpression(SourceSyntax, Base, Access, Flags)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayAccessExpression(this);
}

public record PropertyAccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    string PropertyName,
    AccessExpressionFlags Flags
) : AccessExpression(SourceSyntax, Base, new StringLiteralExpression(null, PropertyName), Flags)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitPropertyAccessExpression(this);

    protected override object? GetDebugAttributes() => new { PropertyName };
}

public record ResourceReferenceExpression(
    SyntaxBase? SourceSyntax,
    ResourceMetadata Metadata,
    IndexReplacementContext? IndexContext
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitResourceReferenceExpression(this);
}

public record ModuleReferenceExpression(
    SyntaxBase? SourceSyntax,
    ModuleSymbol Module,
    IndexReplacementContext? IndexContext
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitModuleReferenceExpression(this);
}

public record ModuleOutputPropertyAccessExpression(
    SyntaxBase? SourceSyntax,
    Expression Base,
    string PropertyName,
    AccessExpressionFlags Flags
) : AccessExpression(SourceSyntax, Base, new StringLiteralExpression(null, PropertyName), Flags)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitModuleOutputPropertyAccessExpression(this);

    protected override object? GetDebugAttributes() => new { PropertyName };
}

public record VariableReferenceExpression(
    SyntaxBase? SourceSyntax,
    VariableSymbol Variable
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitVariableReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Variable = Variable.Name };
}

    /// <summary>
    ///   Represents a variable which has been synthesized rather than explicitly declared by the user.
    ///   This is used for example when in-lining JSON blocks for the loadJsonContent() function.
    /// </summary>
public record SynthesizedVariableReferenceExpression(
    SyntaxBase? SourceSyntax,
    string Name
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitSynthesizedVariableReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record ParametersReferenceExpression(
    SyntaxBase? SourceSyntax,
    ParameterSymbol Parameter
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitParametersReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Parameter = Parameter.Name };
}

public record ParametersAssignmentReferenceExpression(
    SyntaxBase? SourceSyntax,
    ParameterAssignmentSymbol Parameter
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitParametersAssignmentReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Parameter = Parameter.Name };
}

public record LambdaVariableReferenceExpression(
    SyntaxBase? SourceSyntax,
    LocalVariableSymbol Variable,
    bool IsFunctionLambda
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitLambdaVariableReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Variable = Variable.Name };
}

public record ForLoopExpression(
    SyntaxBase? SourceSyntax,
    Expression Expression,
    Expression Body,
    string? Name,
    ulong? BatchSize
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitForLoopExpression(this);
}

public record CopyIndexExpression(
    SyntaxBase? SourceSyntax,
    string? Name
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitCopyIndexExpression(this);
}

public record ConditionExpression(
    SyntaxBase? SourceSyntax,
    Expression Expression,
    Expression Body
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitConditionExpression(this);
}

public record LambdaExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<string> Parameters,
    ImmutableArray<TypeExpression?> ParameterTypes,
    Expression Body,
    TypeExpression? OutputType
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitLambdaExpression(this);
}

public abstract record DescribableExpression(
    SyntaxBase? SourceSyntax,
    Expression? Description
) : Expression(SourceSyntax) {}

public record DeclaredMetadataExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    Expression Value,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredMetadataExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredProviderExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    NamespaceType NamespaceType,
    Expression? Config,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredProviderExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public abstract record TypeDeclaringExpression(
    SyntaxBase? SourceSyntax,
    Expression? Description,
    Expression? Metadata,
    Expression? Secure,
    Expression? MinLength,
    Expression? MaxLength,
    Expression? MinValue,
    Expression? MaxValue,
    Expression? Sealed
) : DescribableExpression(SourceSyntax, Description) {}

public record DeclaredParameterExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    TypeExpression Type,
    Expression? DefaultValue,
    Expression? Description = null,
    Expression? Metadata = null,
    Expression? Secure = null,
    Expression? MinLength = null,
    Expression? MaxLength = null,
    Expression? MinValue = null,
    Expression? MaxValue = null,
    Expression? Sealed = null,
    Expression? AllowedValues = null
) : TypeDeclaringExpression(SourceSyntax, Description, Metadata, Secure, MinLength, MaxLength, MinValue, MaxValue, Sealed)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredParameterExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredVariableExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    Expression Value,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredVariableExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredOutputExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    TypeExpression Type,
    Expression Value,
    Expression? Description = null,
    Expression? Metadata = null,
    Expression? Secure = null,
    Expression? MinLength = null,
    Expression? MaxLength = null,
    Expression? MinValue = null,
    Expression? MaxValue = null,
    Expression? Sealed = null
) : TypeDeclaringExpression(SourceSyntax, Description, Metadata, Secure, MinLength, MaxLength, MinValue, MaxValue, Sealed)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredOutputExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredAssertExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    Expression Value,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredAssertExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredResourceExpression(
    SyntaxBase? SourceSyntax,
    DeclaredResourceMetadata ResourceMetadata,
    ScopeHelper.ScopeData ScopeData,
    SyntaxBase BodySyntax,
    Expression Body,
    ImmutableArray<ResourceDependencyExpression> DependsOn,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredResourceExpression(this);
}

public record DeclaredModuleExpression(
    SyntaxBase? SourceSyntax,
    ModuleSymbol Symbol,
    ScopeHelper.ScopeData ScopeData,
    SyntaxBase BodySyntax,
    Expression Body,
    Expression? Parameters,
    ImmutableArray<ResourceDependencyExpression> DependsOn,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredModuleExpression(this);
}

public record ResourceDependencyExpression(
    SyntaxBase? SourceSyntax,
    Expression Reference
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitResourceDependencyExpression(this);
}

public record ProgramExpression(
    SyntaxBase? SourceSyntax,
    ImmutableArray<DeclaredMetadataExpression> Metadata,
    ImmutableArray<DeclaredProviderExpression> Providers,
    ImmutableArray<DeclaredTypeExpression> Types,
    ImmutableArray<DeclaredParameterExpression> Parameters,
    ImmutableArray<DeclaredVariableExpression> Variables,
    ImmutableArray<DeclaredFunctionExpression> Functions,
    ImmutableArray<DeclaredResourceExpression> Resources,
    ImmutableArray<DeclaredModuleExpression> Modules,
    ImmutableArray<DeclaredOutputExpression> Outputs,
    ImmutableArray<DeclaredAssertExpression> Asserts
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitProgramExpression(this);
}

public record AccessChainExpression(
    SyntaxBase? SourceSyntax,
    AccessExpression FirstLink,
    ImmutableArray<Expression> AdditionalProperties
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitAccessChainExpression(this);
}

public record DeclaredFunctionExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    Expression Lambda,
    Expression? Description = null
) : DescribableExpression(SourceSyntax, Description)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredFunctionExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record UserDefinedFunctionCallExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    ImmutableArray<Expression> Parameters
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitUserDefinedFunctionCallExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredTypeExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    TypeExpression Value,
    Expression? Description = null,
    Expression? Metadata = null,
    Expression? Secure = null,
    Expression? MinLength = null,
    Expression? MaxLength = null,
    Expression? MinValue = null,
    Expression? MaxValue = null,
    Expression? Sealed = null,
    Expression? Exported = null
) : TypeDeclaringExpression(SourceSyntax, Description, Metadata, Secure, MinLength, MaxLength, MinValue, MaxValue, Sealed)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredTypeExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public abstract record TypeExpression(
    SyntaxBase? SourceSyntax,
    TypeSymbol ExpressedType
) : Expression(SourceSyntax)
{
    protected override object? GetDebugAttributes() => new { Name = ExpressedType.Name };
}

public record AmbientTypeReferenceExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    TypeSymbol ExpressedType
) : TypeExpression(SourceSyntax, ExpressedType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitAmbientTypeReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record FullyQualifiedAmbientTypeReferenceExpression(
    SyntaxBase? SourceSyntax,
    string ProviderName,
    string Name,
    TypeSymbol ExpressedType
) : TypeExpression(SourceSyntax, ExpressedType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitFullyQualifiedAmbientTypeReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Name = $"{ProviderName}.{Name}" };
}

public record TypeAliasReferenceExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    TypeSymbol ExpressedType
) : TypeExpression(SourceSyntax, ExpressedType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitTypeAliasReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record ImportedTypeReferenceExpression(
    SyntaxBase? SourceSyntax,
    ImportedTypeSymbol Symbol,
    TypeSymbol ExpressedType
) : TypeExpression(SourceSyntax, ExpressedType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitImportedTypeReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Name = Symbol.Name };
}

public record WildcardImportPropertyReferenceExpression(
    SyntaxBase? SourceSyntax,
    WildcardImportSymbol ImportSymbol,
    string PropertyName,
    TypeSymbol ExpressedType
) : TypeExpression(SourceSyntax, ExpressedType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitWildcardImportPropertyReferenceExpression(this);

    protected override object? GetDebugAttributes() => new { Name = $"{ImportSymbol.Name}.{PropertyName}" };
}

public record StringLiteralTypeExpression(
    SyntaxBase? SourceSyntax,
    StringLiteralType ExpressedStringLiteralType
) : TypeExpression(SourceSyntax, ExpressedStringLiteralType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitStringLiteralTypeExpression(this);

    public string Value => ExpressedStringLiteralType.RawStringValue;
}

public record IntegerLiteralTypeExpression(
    SyntaxBase? SourceSyntax,
    IntegerLiteralType ExpressedIntegerLiteralType
) : TypeExpression(SourceSyntax, ExpressedIntegerLiteralType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitIntegerLiteralTypeExpression(this);

    public long Value => ExpressedIntegerLiteralType.Value;
}

public record BooleanLiteralTypeExpression(
    SyntaxBase? SourceSyntax,
    BooleanLiteralType ExpressedBooleanLiteralType
) : TypeExpression(SourceSyntax, ExpressedBooleanLiteralType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitBooleanLiteralTypeExpression(this);

    public bool Value => ExpressedBooleanLiteralType.Value;
}

public record NullLiteralTypeExpression(
    SyntaxBase? SourceSyntax,
    NullType ExpressedNullLiteralType
) : TypeExpression(SourceSyntax, ExpressedNullLiteralType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitNullLiteralTypeExpression(this);
}

public record ResourceTypeExpression(
    SyntaxBase? SourceSyntax,
    ResourceType ExpressedResourceType
) : TypeExpression(SourceSyntax, ExpressedResourceType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitResourceTypeExpression(this);
}

public record ObjectTypePropertyExpression(
    SyntaxBase? SourceSyntax,
    string PropertyName,
    TypeExpression Value,
    Expression? Description = null,
    Expression? Metadata = null,
    Expression? Secure = null,
    Expression? MinLength = null,
    Expression? MaxLength = null,
    Expression? MinValue = null,
    Expression? MaxValue = null,
    Expression? Sealed = null
) : TypeDeclaringExpression(SourceSyntax, Description, Metadata, Secure, MinLength, MaxLength, MinValue, MaxValue, Sealed)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectTypePropertyExpression(this);
}

public record ObjectTypeAdditionalPropertiesExpression(
    SyntaxBase? SourceSyntax,
    TypeExpression Value,
    Expression? Description = null,
    Expression? Metadata = null,
    Expression? Secure = null,
    Expression? MinLength = null,
    Expression? MaxLength = null,
    Expression? MinValue = null,
    Expression? MaxValue = null,
    Expression? Sealed = null
) : TypeDeclaringExpression(SourceSyntax, Description, Metadata, Secure, MinLength, MaxLength, MinValue, MaxValue, Sealed)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectTypeAdditionalPropertiesExpression(this);
}

public record ObjectTypeExpression(
    SyntaxBase? SourceSyntax,
    ObjectType ExpressedObjectType,
    ImmutableArray<ObjectTypePropertyExpression> PropertyExpressions,
    ObjectTypeAdditionalPropertiesExpression? AdditionalPropertiesExpression
) : TypeExpression(SourceSyntax, ExpressedObjectType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitObjectTypeExpression(this);
}

public record TupleTypeItemExpression(
    SyntaxBase? SourceSyntax,
    TypeExpression Value,
    Expression? Description = null,
    Expression? Metadata = null,
    Expression? Secure = null,
    Expression? MinLength = null,
    Expression? MaxLength = null,
    Expression? MinValue = null,
    Expression? MaxValue = null,
    Expression? Sealed = null
) : TypeDeclaringExpression(SourceSyntax, Description, Metadata, Secure, MinLength, MaxLength, MinValue, MaxValue, Sealed)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitTupleTypeItemExpression(this);
}

public record TupleTypeExpression(
    SyntaxBase? SourceSyntax,
    TupleType ExpressedTupleType,
    ImmutableArray<TupleTypeItemExpression> ItemExpressions
) : TypeExpression(SourceSyntax, ExpressedTupleType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitTupleTypeExpression(this);
}

public record ArrayTypeExpression(
    SyntaxBase? SourceSyntax,
    ArrayType ExpressedArrayType,
    TypeExpression BaseExpression
) : TypeExpression(SourceSyntax, ExpressedArrayType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitArrayTypeExpression(this);
}

public record NullableTypeExpression(
    SyntaxBase? SourceSyntax,
    TypeExpression BaseExpression
) : TypeExpression(SourceSyntax, TypeHelper.CreateTypeUnion(BaseExpression.ExpressedType, LanguageConstants.Null))
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitNullableTypeExpression(this);
}

public record NonNullableTypeExpression(
    SyntaxBase? SourceSyntax,
    TypeExpression BaseExpression
) : TypeExpression(SourceSyntax, TypeHelper.CreateTypeUnion(BaseExpression.ExpressedType, TypeHelper.TryRemoveNullability(BaseExpression.ExpressedType) ?? BaseExpression.ExpressedType))
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitNonNullableTypeExpression(this);
}

public record UnionTypeExpression(
    SyntaxBase? SourceSyntax,
    UnionType ExpressedUnionType,
    ImmutableArray<TypeExpression> MemberExpressions
) : TypeExpression(SourceSyntax, ExpressedUnionType)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitUnionTypeExpression(this);
}
