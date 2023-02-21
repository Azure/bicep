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

public record LambdaVariableReferenceExpression(
    SyntaxBase? SourceSyntax,
    LocalVariableSymbol Variable
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
    Expression Body
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitLambdaExpression(this);
}

public record DeclaredMetadataExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    Expression Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredMetadataExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredImportExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    NamespaceType NamespaceType,
    Expression? Config
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredImportExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredParameterExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    ParameterSymbol Symbol,
    Expression? DefaultValue
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredParameterExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredVariableExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    Expression Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredVariableExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredOutputExpression(
    SyntaxBase? SourceSyntax,
    string Name,
    OutputSymbol Symbol,
    Expression Value
) : Expression(SourceSyntax)
{
    public override void Accept(IExpressionVisitor visitor)
        => visitor.VisitDeclaredOutputExpression(this);

    protected override object? GetDebugAttributes() => new { Name };
}

public record DeclaredResourceExpression(
    SyntaxBase? SourceSyntax,
    DeclaredResourceMetadata Metadata,
    ScopeHelper.ScopeData ScopeData,
    SyntaxBase BodySyntax,
    Expression Body,
    ImmutableArray<ResourceDependencyExpression> DependsOn
) : Expression(SourceSyntax)
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
    ImmutableArray<ResourceDependencyExpression> DependsOn
) : Expression(SourceSyntax)
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
    ImmutableArray<DeclaredImportExpression> Imports,
    ImmutableArray<DeclaredParameterExpression> Parameters,
    ImmutableArray<DeclaredVariableExpression> Variables,
    ImmutableArray<DeclaredResourceExpression> Resources,
    ImmutableArray<DeclaredModuleExpression> Modules,
    ImmutableArray<DeclaredOutputExpression> Outputs
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
