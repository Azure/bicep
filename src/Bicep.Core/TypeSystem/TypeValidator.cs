// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.TypeSystem
{
    public class TypeValidator
    {
        private delegate void TypeMismatchDiagnosticWriter(TypeSymbol targetType, TypeSymbol expressionType, SyntaxBase expression);

        private readonly ITypeManager typeManager;

        private readonly IBinder binder;

        private readonly IDiagnosticLookup parsingErrorLookup;

        private readonly IDiagnosticWriter diagnosticWriter;

        private record TypeValidatorConfig(
            bool SkipTypeErrors,
            bool SkipConstantCheck,
            bool DisallowAny,
            SyntaxBase? OriginSyntax,
            TypeMismatchDiagnosticWriter? OnTypeMismatch,
            bool IsResourceDeclaration,
            HashSet<(SyntaxBase expression, TypeSymbol expressionType, TypeSymbol targetType)> currentlyProcessing);

        private TypeValidator(ITypeManager typeManager, IBinder binder, IDiagnosticLookup parsingErrorLookup, IDiagnosticWriter diagnosticWriter)
        {
            this.typeManager = typeManager;
            this.binder = binder;
            this.parsingErrorLookup = parsingErrorLookup;
            this.diagnosticWriter = diagnosticWriter;
        }

        /// <summary>
        /// Gets the list of compile-time constant violations. An error is logged for every occurrence of an expression that is not entirely composed of literals.
        /// It may return inaccurate results for malformed trees.
        /// </summary>
        /// <param name="expression">the expression to check for compile-time constant violations</param>
        /// <param name="diagnosticWriter">Diagnostic writer instance</param>
        public static void GetCompileTimeConstantViolation(SyntaxBase expression, IDiagnosticWriter diagnosticWriter, string? decoratorName = null)
        {
            if (decoratorName != null && string.Equals(decoratorName, LanguageConstants.WaitUntilPropertyName, LanguageConstants.IdentifierComparison))
            {
                if (expression is FunctionArgumentSyntax functionArgumentSyntax && functionArgumentSyntax.Expression is LambdaSyntax)
                {
                    // With introduction of waitUntil() decorator, there is a need to ignore
                    // lambda expressions at compile-time, as so far decorators always had compile-time constant expressions.
                    return;
                }

            }

            var visitor = new CompileTimeConstantVisitor(diagnosticWriter);

            visitor.Visit(expression);
        }

        /// <summary>
        /// Checks if a value of the specified source type can be assigned to the specified target type. (Does not validate properties/schema on object types.)
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="targetType">The target type</param>
        /// <returns>Returns true if values of the specified source type are assignable to the target type. Returns false otherwise or null if assignability cannot be determined.</returns>
        public static bool AreTypesAssignable(TypeSymbol sourceType, TypeSymbol targetType)
        {
            if (sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.PreventAssignment) || targetType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.PreventAssignment))
            {
                return false;
            }

            if (sourceType is AnyType)
            {
                // "any" type is assignable to all types
                return true;
            }

            switch (sourceType, targetType)
            {
                case (_, AnyType):
                    // values of all types can be assigned to the "any" type
                    return true;

                case (LambdaType sourceLambda, LambdaType targetLambda):
                    return AreLambdaTypesAssignable(sourceLambda, targetLambda);

                case (IScopeReference, IScopeReference):
                    // checking for valid combinations of scopes happens after type checking. this allows us to provide a richer & more intuitive error message.
                    return true;

                case (_, UnionType targetUnion) when ReferenceEquals(targetUnion, LanguageConstants.ResourceOrResourceCollectionRefItem):
                    return sourceType is IScopeReference || sourceType is ArrayType { Item: IScopeReference };

                case (ResourceType sourceResourceType, ResourceParentType targetResourceParentType):
                    // Assigning a resource to a parent property.
                    return sourceResourceType.TypeReference.IsParentOf(targetResourceParentType.ChildTypeReference);

                case (ResourceType sourceResourceType, ResourceParameterType resourceParameterType):
                    // Assigning a resource to a parameter ignores the API Version
                    return sourceResourceType.TypeReference.FormatType().Equals(resourceParameterType.TypeReference.FormatType(), StringComparison.OrdinalIgnoreCase);

                case (ResourceType sourceResourceType, _):
                    // When assigning a resource, we're really assigning the value of the resource body.
                    return AreTypesAssignable(sourceResourceType.Body.Type, targetType);

                case (ModuleType sourceModuleType, _):
                    // When assigning a module, we're really assigning the value of the module body.
                    return AreTypesAssignable(sourceModuleType.Body.Type, targetType);

                case (TestType sourceTestType, _):
                    // When assigning a module, we're really assigning the value of the test body.
                    return AreTypesAssignable(sourceTestType.Body.Type, targetType);

                case (StringLiteralType, StringLiteralType):
                    // The name *is* the escaped string value, so we must have an exact match.
                    return targetType.Name == sourceType.Name;

                case (IntegerLiteralType sourceInt, IntegerLiteralType targetInt):
                    return targetInt.Value == sourceInt.Value;

                case (BooleanLiteralType sourceBool, BooleanLiteralType targetBool):
                    return sourceBool.Value == targetBool.Value;

                case (StringType, StringLiteralType):
                    // We allow primitive to like-typed literal assignment only in the case where the "AllowLooseAssignment" validation flag has been set.
                    // This is to allow parameters without 'allowed' values to be assigned to fields expecting enums.
                    // At some point we may want to consider flowing the enum type backwards to solve this more elegantly.
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseAssignment);

                case (IntegerType, IntegerLiteralType):
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseAssignment);

                case (BooleanType, BooleanLiteralType):
                    return sourceType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.AllowLooseAssignment);

                case (StringLiteralType, StringType):
                    return true;

                case (IntegerLiteralType, IntegerType):
                    // integer literals can be assigned to ints
                    return true;

                case (BooleanLiteralType, BooleanType):
                    // boolean literals can be assigned to bools
                    return true;

                case (IntegerType, IntegerType):
                    return true;

                case (StringType, StringType):
                    return true;

                case (BooleanType, BooleanType):
                    return true;

                case (NullType, NullType):
                    return true;

                case (ArrayType sourceArray, ArrayType targetArray):
                    // both types are arrays
                    // this function does not validate item types
                    return true;

                case (ObjectType, ObjectType):
                case (DiscriminatedObjectType, DiscriminatedObjectType):
                case (ObjectType, DiscriminatedObjectType):
                case (DiscriminatedObjectType, ObjectType):
                    // validation left for later
                    return true;

                case (UnionType sourceUnion, _):
                    // union types are guaranteed to be flat

                    // TODO: Replace with some sort of set intersection
                    // are all source type members assignable to the target type?
                    return sourceUnion.Members.All(sourceMember => AreTypesAssignable(sourceMember.Type, targetType) == true);

                case (_, UnionType targetUnion):
                    // the source type should be a singleton type
                    Debug.Assert(!(sourceType is UnionType), "!(sourceType is UnionType)");

                    // can source type be assigned to any union member types
                    return targetUnion.Members.Any(targetMember => AreTypesAssignable(sourceType, targetMember.Type) == true);

                default:
                    // expression cannot be assigned to the type
                    return false;
            }
        }

        public static bool ShouldWarn(TypeSymbol targetType) => targetType switch
        {
            UnionType union => union.Members.Any(m => ShouldWarn(m.Type)),
            _ => targetType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.WarnOnTypeMismatch),
        };

        public static bool ShouldWarnForPropertyMismatch(TypeSymbol targetType) => targetType switch
        {
            UnionType union => union.Members.Any(m => ShouldWarnForPropertyMismatch(m.Type)),
            _ => targetType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.WarnOnPropertyTypeMismatch),
        };

        public static TypeSymbol NarrowTypeAndCollectDiagnostics(ITypeManager typeManager, IBinder binder, IDiagnosticLookup parsingErrorLookup, IDiagnosticWriter diagnosticWriter, SyntaxBase expression, TypeSymbol targetType, bool isResourceDeclaration = false)
        {
            var config = new TypeValidatorConfig(
                SkipTypeErrors: false,
                SkipConstantCheck: false,
                DisallowAny: false,
                OriginSyntax: null,
                OnTypeMismatch: null,
                IsResourceDeclaration: isResourceDeclaration,
                currentlyProcessing: new());

            var validator = new TypeValidator(typeManager, binder, parsingErrorLookup, diagnosticWriter);

            return validator.NarrowType(config, expression, targetType);
        }

        private TypeSymbol NarrowType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol targetType)
            => NarrowType(config, expression, typeManager.GetTypeInfo(expression), targetType);

        private TypeSymbol NarrowType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, TypeSymbol targetType)
        {
            if (config.DisallowAny && expressionType is AnyType)
            {
                // certain properties such as scope, parent, dependsOn do not allow values of "any" type
                diagnosticWriter.Write(config.OriginSyntax ?? expression, x => x.AnyTypeIsNotAllowed());

                // if we let the type narrowing continue, we could get more diagnostics
                // but it also leads to duplicate "disallow any" diagnostics caused by the union type narrowing
                // (occurs with "dependsOn: [ any(true) ]")
                return targetType;
            }

            if (config.SkipTypeErrors == false && expressionType is ErrorType)
            {
                // since we dynamically checked type, we need to collect the errors but only if the caller wants them
                diagnosticWriter.WriteMultiple(expressionType.GetDiagnostics());
                return targetType;
            }

            switch (targetType)
            {
                case ResourceType targetResourceType:
                    {
                        var narrowedBody = NarrowType(config, expression, targetResourceType.Body.Type);

                        return new ResourceType(
                            targetResourceType.DeclaringNamespace,
                            targetResourceType.TypeReference,
                            targetResourceType.ValidParentScopes,
                            targetResourceType.ReadOnlyScopes,
                            targetResourceType.Flags,
                            narrowedBody,
                            targetResourceType.UniqueIdentifierProperties);
                    }
                case ModuleType targetModuleType:
                    {
                        var narrowedBody = NarrowType(config, expression, targetModuleType.Body.Type);

                        return new ModuleType(targetModuleType.Name, targetModuleType.ValidParentScopes, narrowedBody);
                    }
                case TestType targetTestType:
                    {
                        var narrowedBody = NarrowType(config, expression, targetTestType.Body.Type);

                        return new TestType(targetTestType.Name, narrowedBody);
                    }
                case ArrayType loopArrayType when expression is ForSyntax @for:
                    {
                        // for-expression assignability check
                        var narrowedBody = NarrowType(config, @for.Body, loopArrayType.Item.Type);

                        return new TypedArrayType(narrowedBody, TypeSymbolValidationFlags.Default);
                    }
                case UnionType when TypeCollapser.TryCollapse(targetType) is TypeSymbol collapsed:
                    targetType = collapsed;
                    break;
            }

            // basic assignability check
            if (AreTypesAssignable(expressionType, targetType) == false)
            {
                if (TypeHelper.WouldBeAssignableIfNonNullable(expressionType, targetType, out var nonNullableExpressionType))
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).PossibleNullReferenceAssignment(targetType, expressionType.Type, expression));
                    return NarrowType(config, expression, nonNullableExpressionType, targetType);
                }

                // fundamentally different types - cannot assign
                if (config.OnTypeMismatch is not null)
                {
                    config.OnTypeMismatch(targetType, expressionType, expression);
                }
                else
                {
                    diagnosticWriter.Write(config.OriginSyntax ?? expression, x => x.ExpectedValueTypeMismatch(ShouldWarn(targetType), targetType, expressionType));
                }

                return targetType;
            }

            if (targetType is ResourceParameterType)
            {
                return targetType;
            }

            // integer assignability check
            if (targetType is IntegerType targetInteger)
            {
                return NarrowIntegerAssignmentType(config, expression, expressionType, targetInteger);
            }

            if (targetType is IntegerLiteralType targetIntegerLiteral)
            {
                return NarrowIntegerLiteralAssignmentType(config, expression, expressionType, targetIntegerLiteral);
            }

            // string assignability check
            if (targetType is StringType targetString)
            {
                return NarrowStringAssignmentType(config, expression, expressionType, targetString);
            }

            if (targetType is StringLiteralType targetStringLiteral)
            {
                return NarrowStringLiteralAssignmentType(config, expression, expressionType, targetStringLiteral);
            }

            // object assignability check
            if (targetType is ObjectType targetObjectType &&
                NarrowObjectAssignmentType(config, UnwrapIfConditionBody(expression), expressionType, targetObjectType) is TypeSymbol narrowedObject)
            {
                return narrowedObject;
            }

            if (targetType is DiscriminatedObjectType targetDiscriminatedObjectType &&
                NarrowDiscriminatedObjectType(config, UnwrapIfConditionBody(expression), expressionType, targetDiscriminatedObjectType) is TypeSymbol narrowedDiscriminatedObject)
            {
                return narrowedDiscriminatedObject;
            }

            // array assignability check
            if (targetType is ArrayType targetArrayType &&
                NarrowArrayAssignmentType(config, expression, expressionType, targetArrayType) is TypeSymbol narrowedArray)
            {
                return narrowedArray;
            }

            if (expression is VariableAccessSyntax variableAccess &&
                NarrowVariableAccessType(config, variableAccess, targetType) is TypeSymbol narrowedVariableAccess)
            {
                return narrowedVariableAccess;
            }

            if (targetType is UnionType targetUnionType)
            {
                return NarrowUnionType(config, expression, expressionType, targetUnionType);
            }

            if (expression is LambdaSyntax sourceLambda && targetType is LambdaType targetLambdaType)
            {
                return NarrowLambdaType(config, sourceLambda, targetLambdaType);
            }

            return expressionType;
        }

        private static SyntaxBase UnwrapIfConditionBody(SyntaxBase expression) => expression switch
        {
            IfConditionSyntax ifCondition => ifCondition.Body,
            _ => expression,
        };

        private static bool AreLambdaTypesAssignable(LambdaType source, LambdaType target)
        {
            if (source.MaximumArgCount < target.MinimumArgCount ||
                source.MinimumArgCount > target.MaximumArgCount)
            {
                return false;
            }

            if (!AreTypesAssignable(source.ReturnType.Type, target.ReturnType.Type))
            {
                return false;
            }

            for (var i = 0; i < Math.Min(source.MaximumArgCount, target.MaximumArgCount); i++)
            {
                if (!AreTypesAssignable(source.GetArgumentType(i).Type, target.GetArgumentType(i).Type))
                {
                    return false;
                }
            }

            return true;
        }

        private TypeSymbol NarrowIntegerAssignmentType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, IntegerType targetType)
        {
            bool shouldWarn = config.IsResourceDeclaration || ShouldWarn(targetType);

            switch (expressionType)
            {
                case IntegerType expressionInteger:
                    if (expressionInteger.MinValue.HasValue && targetType.MaxValue.HasValue && expressionInteger.MinValue.Value > targetType.MaxValue.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SourceIntDomainDisjointFromTargetIntDomain_SourceHigh(shouldWarn, expressionInteger.MinValue.Value, targetType.MaxValue.Value));
                        break;
                    }

                    if (expressionInteger.MaxValue.HasValue && targetType.MinValue.HasValue && expressionInteger.MaxValue.Value < targetType.MinValue.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SourceIntDomainDisjointFromTargetIntDomain_SourceLow(shouldWarn, expressionInteger.MaxValue.Value, targetType.MinValue.Value));
                        break;
                    }

                    if (expressionInteger.MinValue.HasValue && targetType.MinValue.HasValue && expressionInteger.MinValue.Value < targetType.MinValue.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceIntDomainExtendsBelowTargetIntDomain(expressionInteger.MinValue.Value, targetType.MinValue.Value));
                    }

                    if (expressionInteger.MaxValue.HasValue && targetType.MaxValue.HasValue && expressionInteger.MaxValue.Value > targetType.MaxValue.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceIntDomainExtendsAboveTargetIntDomain(expressionInteger.MaxValue.Value, targetType.MaxValue.Value));
                    }

                    return TypeFactory.CreateIntegerType(
                        minValue: Math.Max(expressionInteger.MinValue ?? long.MinValue, targetType.MinValue ?? long.MinValue) switch
                        {
                            long.MinValue => null,
                            long otherwise => otherwise,
                        },
                        maxValue: Math.Min(expressionInteger.MaxValue ?? long.MaxValue, targetType.MaxValue ?? long.MaxValue) switch
                        {
                            long.MaxValue => null,
                            long otherwise => otherwise,
                        },
                        targetType.ValidationFlags);
                case IntegerLiteralType expressionIntegerLiteral:
                    if (targetType.MaxValue.HasValue && expressionIntegerLiteral.Value > targetType.MaxValue.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SourceIntDomainDisjointFromTargetIntDomain_SourceHigh(shouldWarn, expressionIntegerLiteral.Value, targetType.MaxValue.Value));
                        break;
                    }

                    if (targetType.MinValue.HasValue && expressionIntegerLiteral.Value < targetType.MinValue.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SourceIntDomainDisjointFromTargetIntDomain_SourceLow(shouldWarn, expressionIntegerLiteral.Value, targetType.MinValue.Value));
                        break;
                    }

                    // if a integer literal falls within the target int's domain, the literal will always be the most narrow type
                    return expressionIntegerLiteral;
            }

            return expressionType;
        }

        private TypeSymbol NarrowIntegerLiteralAssignmentType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, IntegerLiteralType targetType)
        {
            bool shouldWarn = config.IsResourceDeclaration || ShouldWarn(targetType);

            if (expressionType is IntegerType expressionInteger)
            {
                if (expressionInteger.MinValue.HasValue && expressionInteger.MinValue.Value > targetType.Value)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                        .SourceIntDomainDisjointFromTargetIntDomain_SourceHigh(shouldWarn, expressionInteger.MinValue.Value, targetType.Value));
                    return expressionType;
                }

                if (expressionInteger.MaxValue.HasValue && expressionInteger.MaxValue.Value < targetType.Value)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                        .SourceIntDomainDisjointFromTargetIntDomain_SourceLow(shouldWarn, expressionInteger.MaxValue.Value, targetType.Value));
                    return expressionType;
                }
            }

            // outside of the cases handled above, if anything was assignable to an integer literal target, the target will always be the most narrow type
            return targetType;
        }

        private TypeSymbol NarrowStringAssignmentType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, StringType targetType)
        {
            bool shouldWarn = config.IsResourceDeclaration || ShouldWarn(targetType);

            switch (expressionType)
            {
                case StringType expressionString:
                    if (expressionString.MinLength.HasValue && targetType.MaxLength.HasValue && expressionString.MinLength.Value > targetType.MaxLength.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(shouldWarn, expressionString.MinLength.Value, targetType.MaxLength.Value));
                        break;
                    }

                    if (expressionString.MaxLength.HasValue && targetType.MinLength.HasValue && expressionString.MaxLength.Value < targetType.MinLength.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(shouldWarn, expressionString.MaxLength.Value, targetType.MinLength.Value));
                        break;
                    }

                    if (expressionString.MinLength.HasValue && targetType.MinLength.HasValue && expressionString.MinLength.Value < targetType.MinLength.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainExtendsBelowTargetValueLengthDomain(expressionString.MinLength.Value, targetType.MinLength.Value));
                    }

                    if (expressionString.MaxLength.HasValue && targetType.MaxLength.HasValue && expressionString.MaxLength.Value > targetType.MaxLength.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainExtendsAboveTargetValueLengthDomain(expressionString.MaxLength.Value, targetType.MaxLength.Value));
                    }

                    return TypeFactory.CreateStringType(
                        minLength: Math.Max(expressionString.MinLength ?? 0, targetType.MinLength ?? 0) switch
                        {
                            <= 0 => null,
                            long otherwise => otherwise,
                        },
                        maxLength: Math.Min(expressionString.MaxLength ?? long.MaxValue, targetType.MaxLength ?? long.MaxValue) switch
                        {
                            long.MaxValue => null,
                            long otherwise => otherwise,
                        },
                        validationFlags: targetType.ValidationFlags);
                case StringLiteralType expressionStringLiteral:
                    if (targetType.MaxLength.HasValue && expressionStringLiteral.RawStringValue.Length > targetType.MaxLength.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(
                            shouldWarn,
                            expressionStringLiteral.RawStringValue.Length,
                            targetType.MaxLength.Value));
                    }
                    else if (targetType.MinLength.HasValue && expressionStringLiteral.RawStringValue.Length < targetType.MinLength.Value)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(
                            shouldWarn,
                            expressionStringLiteral.RawStringValue.Length,
                            targetType.MinLength.Value));
                    }

                    if (targetType.Pattern is not null &&
                        TypeHelper.MatchesPattern(targetType.Pattern, expressionStringLiteral.RawStringValue) is false)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression)
                            .SuppliedStringDoesNotMatchExpectedPattern(shouldWarn, targetType.Pattern));
                    }

                    // if a literal was assignable to a string-typed target, the literal will always be the most narrow type
                    return expressionStringLiteral;
            }

            return expressionType;
        }

        private TypeSymbol NarrowStringLiteralAssignmentType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, StringLiteralType targetType)
        {
            if (expressionType is StringType expressionString)
            {
                if (expressionString.MinLength.HasValue && expressionString.MinLength.Value > targetType.RawStringValue.Length)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(
                        config.IsResourceDeclaration || ShouldWarn(targetType),
                        expressionString.MinLength.Value,
                        targetType.RawStringValue.Length));
                    return expressionType;
                }

                if (expressionString.MaxLength.HasValue && expressionString.MaxLength.Value < targetType.RawStringValue.Length)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(
                        config.IsResourceDeclaration || ShouldWarn(targetType),
                        expressionString.MaxLength.Value,
                        targetType.RawStringValue.Length));
                    return expressionType;
                }
            }

            // outside of the cases handled above, if anything was assignable to a literal target, the target will always be the most narrow type
            return targetType;
        }

        private TypeSymbol? NarrowArrayAssignmentType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, ArrayType targetType)
        {
            switch (expression)
            {
                case ArraySyntax arrayValue:
                    return NarrowArrayAssignmentType(config, arrayValue, targetType);
                case VariableAccessSyntax variableAccess when DeclaringSyntax(variableAccess) is SyntaxBase declaringSyntax:
                    var newConfig = config with { OriginSyntax = variableAccess };
                    return NarrowType(newConfig, declaringSyntax, targetType);
            }

            if (expressionType is TupleType expressionTuple && targetType is TupleType targetTuple)
            {
                if (expressionTuple.Items.Length != targetTuple.Items.Length)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(
                        config.IsResourceDeclaration || ShouldWarn(targetType),
                        expressionTuple.Items.Length,
                        targetTuple.Items.Length));
                    return expressionType;
                }

                return new TupleType(validationFlags: targetTuple.ValidationFlags,
                    items: [.. Enumerable.Range(0, expressionTuple.Items.Length).Select(idx => NarrowType(config, expression, expressionTuple.Items[idx].Type, targetTuple.Items[idx].Type))]);
            }

            if (expressionType is ArrayType expressionArrayType)
            {
                if (expressionArrayType.MinLength.HasValue && targetType.MaxLength.HasValue && expressionArrayType.MinLength.Value > targetType.MaxLength.Value)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(
                        config.IsResourceDeclaration || ShouldWarn(targetType),
                        expressionArrayType.MinLength.Value,
                        targetType.MaxLength.Value));
                    return expressionType;
                }

                if (expressionArrayType.MaxLength.HasValue && targetType.MinLength.HasValue && expressionArrayType.MaxLength.Value < targetType.MinLength.Value)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(
                        config.IsResourceDeclaration || ShouldWarn(targetType),
                        expressionArrayType.MaxLength.Value,
                        targetType.MinLength.Value));
                    return expressionType;
                }

                if (expressionArrayType.MinLength.HasValue && targetType.MinLength.HasValue && expressionArrayType.MinLength.Value < targetType.MinLength.Value)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainExtendsBelowTargetValueLengthDomain(expressionArrayType.MinLength.Value, targetType.MinLength.Value));
                }

                if (expressionArrayType.MaxLength.HasValue && targetType.MaxLength.HasValue && expressionArrayType.MaxLength.Value > targetType.MaxLength.Value)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainExtendsAboveTargetValueLengthDomain(expressionArrayType.MaxLength.Value, targetType.MaxLength.Value));
                }

                return new TypedArrayType(NarrowType(config, expression, expressionArrayType.Item.Type, targetType.Item.Type),
                    targetType.ValidationFlags,
                    minLength: Math.Max(expressionArrayType.MinLength ?? 0, targetType.MinLength ?? 0) switch
                    {
                        <= 0 => null,
                        long otherwise => otherwise,
                    },
                    maxLength: Math.Min(expressionArrayType.MaxLength ?? long.MaxValue, targetType.MaxLength ?? long.MaxValue) switch
                    {
                        long.MaxValue => null,
                        long otherwise => otherwise,
                    });
            }

            return null;
        }

        private TypeSymbol NarrowArrayAssignmentType(TypeValidatorConfig config, ArraySyntax expression, ArrayType targetType)
        {
            // if we have parse errors, no need to check assignability
            // we should not return the parse errors however because they will get double collected
            if (this.parsingErrorLookup.Contains(expression))
            {
                return targetType;
            }

            var childTypes = new List<(ITypeReference type, bool isSpread)>();
            var hasSpread = false;
            foreach (var child in expression.Children)
            {
                if (child is ArrayItemSyntax item)
                {
                    var itemTarget = targetType switch
                    {
                        // If the target is a tuple, find the correct target type by index.
                        // If we've already encountered a spread expression, the index cannot be relied upon.
                        // If we walk off the end of the tuple schema, just use `any` and report a length violation.
                        TupleType tt when !hasSpread => tt.Items.Skip(childTypes.Count).FirstOrDefault()?.Type ?? LanguageConstants.Any,
                        _ => targetType.Item.Type,
                    };

                    var newConfig = config with
                    {
                        SkipTypeErrors = true,
                        OnTypeMismatch = (expected, actual, position) => diagnosticWriter.Write(position, x => x.ArrayTypeMismatch(ShouldWarn(targetType), expected, actual)),
                    };

                    var narrowedItem = NarrowType(newConfig, item.Value, itemTarget);
                    childTypes.Add((narrowedItem, isSpread: false));
                }
                else if (child is SpreadExpressionSyntax spread)
                {
                    hasSpread = true;
                    var spreadTarget = new TypedArrayType(targetType.Item, targetType.ValidationFlags);

                    var newConfig = config with
                    {
                        SkipTypeErrors = true,
                        OnTypeMismatch = (expected, actual, position) => diagnosticWriter.Write(position, x => x.ArrayTypeMismatchSpread(ShouldWarn(targetType), expected, actual)),
                    };

                    var narrowedSpread = NarrowType(newConfig, spread.Expression, spreadTarget);
                    childTypes.Add((narrowedSpread, isSpread: true));
                    // TODO could further optimize accuracy here if the spread returns a tuple type
                }
            }

            if (!hasSpread && targetType.MaxLength.HasValue && targetType.MaxLength.Value < childTypes.Count)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(
                    config.IsResourceDeclaration || ShouldWarn(targetType),
                    childTypes.Count,
                    targetType.MaxLength.Value));
            }
            else if (!hasSpread && targetType.MinLength.HasValue && targetType.MinLength.Value > childTypes.Count)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(
                    config.IsResourceDeclaration || ShouldWarn(targetType),
                    childTypes.Count,
                    targetType.MinLength.Value));
            }

            var itemProps = childTypes.Select((t, i) => t switch
            {
                (type: { } itemType, isSpread: false) => itemType,
                (type: ArrayType arrayType, isSpread: true) => arrayType.Item,
                _ => LanguageConstants.Any,
            });

            return hasSpread ?
                new TypedArrayType(TypeHelper.CreateTypeUnion(itemProps), targetType.ValidationFlags) :
                new TupleType(itemProps.ToImmutableArray(), targetType.ValidationFlags);
        }

        private TypeSymbol NarrowLambdaType(TypeValidatorConfig config, LambdaSyntax lambdaSyntax, LambdaType targetType)
        {
            var returnType = NarrowType(config, lambdaSyntax.Body, targetType.ReturnType.Type);

            var variables = lambdaSyntax.GetLocalVariables().ToImmutableArray();
            if (variables.Length < targetType.MinimumArgCount || variables.Length > targetType.MaximumArgCount)
            {
                diagnosticWriter.Write(lambdaSyntax.VariableSection, x => x.LambdaExpectedArgCountMismatch(targetType, targetType.MinimumArgCount, targetType.MaximumArgCount, variables.Length));
                return targetType;
            }

            var narrowedVariables = new ITypeReference[variables.Length];
            for (var i = 0; i < variables.Length; i++)
            {
                narrowedVariables[i] = NarrowType(config, variables[i], targetType.GetArgumentType(i).Type);
            }

            return new LambdaType([.. narrowedVariables], [], returnType);
        }

        private TypeSymbol? NarrowVariableAccessType(TypeValidatorConfig config, VariableAccessSyntax variableAccess, TypeSymbol targetType)
        {
            if (DeclaringSyntax(variableAccess) is SyntaxBase declaringSyntax)
            {
                var newConfig = config with { OriginSyntax = variableAccess };
                return NarrowType(newConfig, declaringSyntax, targetType);
            }

            return null;
        }

        // TODO: Implement for non-variable variable access (resource, module, param)
        private SyntaxBase? DeclaringSyntax(VariableAccessSyntax variableAccess) => binder.GetSymbolInfo(variableAccess) switch
        {
            VariableSymbol variableSymbol => variableSymbol.DeclaringVariable.Value,
            LocalVariableSymbol localVariableSymbol => localVariableSymbol.DeclaringSyntax,
            _ => null,
        };


        private TypeSymbol NarrowUnionType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, UnionType targetType)
            => expressionType switch
            {
                UnionType expressionUnion => NarrowUnionTypeForUnionExpressionType(config, expression, expressionUnion, targetType),
                _ => NarrowUnionTypeForSingleExpressionType(config, expression, expressionType, targetType),
            };

        private TypeSymbol NarrowUnionTypeForUnionExpressionType(TypeValidatorConfig config, SyntaxBase expression, UnionType expressionType, UnionType targetType)
        {
            List<UnionTypeMemberViabilityInfo> candidacyEvaluations = new();

            foreach (var candidateExpressionType in expressionType.Members)
            {
                if (!AreTypesAssignable(candidateExpressionType.Type, targetType))
                {
                    candidacyEvaluations.Add(new(candidateExpressionType, null, ImmutableArray<IDiagnostic>.Empty));
                    continue;
                }

                var candidateDiagnostics = ToListDiagnosticWriter.Create();
                var candidateCollector = new TypeValidator(typeManager, binder, parsingErrorLookup, candidateDiagnostics);
                var narrowed = candidateCollector.NarrowUnionType(config, expression, candidateExpressionType.Type, targetType);
                candidacyEvaluations.Add(new(candidateExpressionType, narrowed, candidateDiagnostics.GetDiagnostics()));
            }

            // report all informational and warning diagnostics
            diagnosticWriter.WriteMultiple(candidacyEvaluations.SelectMany(e => e.Diagnostics));

            var errors = candidacyEvaluations.SelectMany(c =>
            {
                if (c.NarrowedType is null || c.Errors.Any())
                {
                    return c.Errors.Append(DiagnosticBuilder.ForPosition(expression).ArgumentTypeMismatch(c.UnionTypeMemberEvaluated.Type, targetType));
                }

                return c.Errors;
            });

            diagnosticWriter.WriteMultiple(errors);

            // If *any* variant of the expression type is not assignable, the expression type is not assignable
            if (errors.Any())
            {
                return targetType;
            }

            return TypeHelper.CreateTypeUnion(candidacyEvaluations.Select(c => c.NarrowedType!));
        }

        private TypeSymbol NarrowUnionTypeForSingleExpressionType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, UnionType targetType)
        {
            List<UnionTypeMemberViabilityInfo> candidacyEvaluations = new();

            foreach (var candidateTargetType in targetType.Members)
            {
                if (!AreTypesAssignable(expressionType, candidateTargetType.Type))
                {
                    candidacyEvaluations.Add(new(candidateTargetType, null, ImmutableArray<IDiagnostic>.Empty));
                    continue;
                }

                var candidateDiagnostics = ToListDiagnosticWriter.Create();
                var candidateCollector = new TypeValidator(typeManager, binder, parsingErrorLookup, candidateDiagnostics);
                var narrowed = candidateCollector.NarrowType(config, expression, expressionType, candidateTargetType.Type);
                candidacyEvaluations.Add(new(candidateTargetType, narrowed, candidateDiagnostics.GetDiagnostics()));
            }

            var viableCandidates = candidacyEvaluations
                .Select(c => c.NarrowedType is { } Narrowed && !c.Errors.Any()
                    // If this node was encountered in a resource declaration, use the target type rather than the narrowed type, as the
                    // target type describes what will be returned by the service (included derived and read-only fields)
                    ? new ViableTypeCandidate(config.IsResourceDeclaration ? c.UnionTypeMemberEvaluated.Type : Narrowed, c.Diagnostics)
                    : null)
                .WhereNotNull()
                .ToImmutableArray();

            if (viableCandidates.Any())
            {
                // It's unclear what we should do with warning and informational diagnostics. Should we report only the intersection of diagnostics raised by every viable candidate?
                // Erring on the side of caution here and just reporting them all unless there are one or more candidates that are assignable w/o warning.
                if (viableCandidates.All(c => c.Diagnostics.Any()))
                {
                    diagnosticWriter.WriteMultiple(viableCandidates.SelectMany(c => c.Diagnostics));
                }

                return TypeHelper.CreateTypeUnion(viableCandidates.Select(c => c.Type));
            }

            diagnosticWriter.WriteMultiple(candidacyEvaluations.SelectMany(e => e.Diagnostics.Concat(e.Errors)));

            if (candidacyEvaluations.All(c => c.NarrowedType is null))
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(expression).ArgumentTypeMismatch(expressionType, targetType));
            }

            return targetType;
        }

        private record UnionTypeMemberViabilityInfo
        {
            internal UnionTypeMemberViabilityInfo(ITypeReference unionTypeMemberEvaluated, TypeSymbol? narrowedType, IEnumerable<IDiagnostic> diagnostics)
            {
                UnionTypeMemberEvaluated = unionTypeMemberEvaluated;
                NarrowedType = narrowedType;

                List<IDiagnostic> nonErrorDiagnostics = new();
                List<IDiagnostic> errorDiagnostics = new();

                foreach (var diagnostic in diagnostics)
                {
                    if (diagnostic.IsError())
                    {
                        errorDiagnostics.Add(diagnostic);
                    }
                    else
                    {
                        nonErrorDiagnostics.Add(diagnostic);
                    }
                }

                Diagnostics = nonErrorDiagnostics;
                Errors = errorDiagnostics;
            }

            /// <summary>
            /// The type being checked for assignability and narrowing
            /// </summary>
            public ITypeReference UnionTypeMemberEvaluated { get; }

            /// <summary>
            /// The narrowed type. Will be null if the type is unassignable
            /// </summary>
            public TypeSymbol? NarrowedType { get; }

            /// <summary>
            /// Any warning or informational diagnostics raised during type narrowing
            /// </summary>
            public IReadOnlyList<IDiagnostic> Diagnostics { get; }

            /// <summary>
            /// Any error-level diagnostics raised during type narrowing
            /// </summary>
            public IReadOnlyList<IDiagnostic> Errors { get; }
        }

        private record ViableTypeCandidate(TypeSymbol Type, IEnumerable<IDiagnostic> Diagnostics);

        private TypeSymbol? NarrowDiscriminatedObjectType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, DiscriminatedObjectType targetType)
        {
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            if (this.parsingErrorLookup.Contains(expression))
            {
                return LanguageConstants.Any;
            }

            if (expressionType is not ObjectType expressionObjectType)
            {
                // the provided value can be assigned to an discriminated object, but we don't know enough about the value to narrow it any further
                return null;
            }

            if (expressionObjectType.Properties.TryGetValue(targetType.DiscriminatorKey) is not TypeProperty discriminatorTypeProperty)
            {
                // there is no explicit discriminator specified, so we can't determine its value
                // if the expression type allows additional properties, the provided value may include the discriminator; otherwise, it certainly does not
                if (!expressionObjectType.HasExplicitAdditionalPropertiesType)
                {
                    var shouldWarn = (expressionObjectType.AdditionalProperties?.TypeReference.Type is { } addlPropertiesType && AreTypesAssignable(addlPropertiesType, LanguageConstants.String)) ||
                        ShouldWarnForPropertyMismatch(targetType);
                    diagnosticWriter.Write(config.OriginSyntax ?? expression, x => x.MissingRequiredProperty(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType));

                    // do a reverse lookup to check if there's any misspelled discriminator key
                    if (SpellChecker.GetSpellingSuggestion(targetType.DiscriminatorKey, expressionObjectType.Properties.Keys) is string misspelledDiscriminatorKey)
                    {
                        var misspelledDiscriminatorProperty = (expression as ObjectSyntax)?.Properties.First(x => LanguageConstants.IdentifierComparer.Equals(x.TryGetKeyText(), misspelledDiscriminatorKey));
                        diagnosticWriter.Write(config.OriginSyntax ?? misspelledDiscriminatorProperty?.Key ?? expression,
                            x => x.DisallowedPropertyWithSuggestion(ShouldWarnForPropertyMismatch(targetType), misspelledDiscriminatorKey, targetType.DiscriminatorKeysUnionType, targetType.DiscriminatorKey));
                    }
                }

                return LanguageConstants.Any;
            }

            var discriminatorDiagnosticTarget = config.OriginSyntax
                ?? (expression as ObjectSyntax)?.Properties.FirstOrDefault(p => targetType.TryGetDiscriminatorProperty(p.TryGetKeyText()) is not null)?.Value
                ?? expression;

            // At some point in the future we may want to relax the expectation of a string literal key, and allow a generic string.
            // In this case, the best we can do is validate against the union of all the settable properties.
            // Let's not do this just yet, and see if a use-case arises.
            var discriminatorType = discriminatorTypeProperty.TypeReference.Type;
            switch (discriminatorType)
            {
                case AnyType:
                    return LanguageConstants.Any;

                case StringLiteralType stringLiteralDiscriminator:
                    if (!targetType.UnionMembersByKey.TryGetValue(stringLiteralDiscriminator.Name, out var selectedObjectReference))
                    {
                        // no matches
                        var discriminatorCandidates = targetType.UnionMembersByKey.Keys.OrderBy(x => x);

                        // Treat as a warning, regardless of whether a property is a 'SystemProperty'.
                        // We don't want to block compilation if the RP has an incomplete discriminator on the 'name' field.
                        var shouldWarn = config.IsResourceDeclaration || ShouldWarnForPropertyMismatch(targetType);

                        diagnosticWriter.Write(
                            discriminatorDiagnosticTarget,
                            x =>
                            {
                                var sourceDeclaration = TryGetSourceDeclaration(config);

                                if (sourceDeclaration is null && SpellChecker.GetSpellingSuggestion(stringLiteralDiscriminator.Name, discriminatorCandidates) is { } suggestion)
                                {
                                    // only look up suggestions if we're not sourcing this type from another declaration.
                                    return x.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, stringLiteralDiscriminator.Name, suggestion);
                                }

                                return x.PropertyTypeMismatch(shouldWarn, sourceDeclaration, targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType, config.IsResourceDeclaration);
                            });

                        return LanguageConstants.Any;
                    }

                    if (selectedObjectReference.Type is not ObjectType selectedObjectType)
                    {
                        throw new InvalidOperationException($"Discriminated type {targetType.Name} contains non-object member");
                    }

                    // we have a match!
                    return NarrowObjectAssignmentType(config, expression, expressionType, selectedObjectType);

                // ReSharper disable once ConvertTypeCheckPatternToNullCheck - using null pattern check causes compiler to think that discriminatorType might be null in the default clause.
                case TypeSymbol when AreTypesAssignable(discriminatorType, targetType.DiscriminatorKeysUnionType):
                    //check if discriminatorType is a subset of targetType.DiscriminatorKeysUnionType.
                    //If match - then warn with message that using property is not recommended and type validation is suspended and return generic object type
                    diagnosticWriter.Write(discriminatorDiagnosticTarget, x => x.AmbiguousDiscriminatorPropertyValue(targetType.DiscriminatorKey));
                    //TODO: make a deep merge of the discriminator types to return combined object for type checking. Additionally, we need to cover hints.
                    return LanguageConstants.Any;

                default:
                    {
                        var shouldWarn = (config.IsResourceDeclaration && !targetType.DiscriminatorProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)) || ShouldWarnForPropertyMismatch(targetType);
                        diagnosticWriter.Write(
                            discriminatorDiagnosticTarget,
                            x => x.PropertyTypeMismatch(shouldWarn, TryGetSourceDeclaration(config), targetType.DiscriminatorKey, targetType.DiscriminatorKeysUnionType, discriminatorType, config.IsResourceDeclaration && !targetType.DiscriminatorProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)));
                        return LanguageConstants.Any;
                    }
            }
        }

        private TypeSymbol? NarrowObjectAssignmentType(TypeValidatorConfig config, SyntaxBase expression, TypeSymbol expressionType, ObjectType targetType)
        {
            static (TypeSymbol type, bool typeWasPreserved) AddImplicitNull(TypeSymbol propertyType, TypePropertyFlags propertyFlags)
            {
                bool preserveType = propertyFlags.HasFlag(TypePropertyFlags.Required) || !propertyFlags.HasFlag(TypePropertyFlags.AllowImplicitNull);
                return (preserveType ? propertyType : TypeHelper.CreateTypeUnion(propertyType, LanguageConstants.Null), preserveType);
            }

            static TypeSymbol RemoveImplicitNull(TypeSymbol type, bool typeWasPreserved)
            {
                return typeWasPreserved || type is not UnionType unionType
                    ? type
                    : TypeHelper.CreateTypeUnion(unionType.Members.Where(m => m != LanguageConstants.Null));
            }

            if (expression is VariableAccessSyntax variableAccess && DeclaringSyntax(variableAccess) is SyntaxBase declaringSyntax)
            {
                var newConfig = config with { OriginSyntax = variableAccess };
                return NarrowObjectAssignmentType(newConfig, declaringSyntax, expressionType, targetType);
            }

            // TODO: Short-circuit on any object to avoid unnecessary processing?
            // TODO: Consider doing the schema check even if there are parse errors
            // if we have parse errors, there's no point to check assignability
            // we should not return the parse errors however because they will get double collected
            if (this.parsingErrorLookup.Contains(expression))
            {
                return targetType;
            }

            if (expressionType is ObjectType expressionObjectType)
            {
                var missingRequiredProperties = expressionObjectType.AdditionalProperties is not null
                    // if the assigned value allows additional properties, we can't know if it's missing any
                    ? []
                    // otherwise, look for required properties on the target for which there is no declared counterpart on the assigned value
                    : targetType.Properties.Values
                        .Where(p => p.Flags.HasFlag(TypePropertyFlags.Required) &&
                            !AreTypesAssignable(LanguageConstants.Null, p.TypeReference.Type) &&
                            !expressionObjectType.Properties.ContainsKey(p.Name))
                        .OrderBy(p => p.Name)
                        .ToImmutableArray();

                if (missingRequiredProperties.Length > 0)
                {
                    var (positionable, blockName) = GetMissingPropertyContext(expression);

                    diagnosticWriter.Write(
                        config.OriginSyntax ?? positionable,
                        x => x.MissingRequiredProperties(
                            warnInsteadOfError: (config.IsResourceDeclaration && missingRequiredProperties.All(p => !p.Flags.HasFlag(TypePropertyFlags.SystemProperty))) ||
                                ShouldWarnForPropertyMismatch(targetType),
                            TryGetSourceDeclaration(config),
                            expression as ObjectSyntax,
                            [.. missingRequiredProperties.Select(p => p.Name)],
                            blockName,
                            config.IsResourceDeclaration && missingRequiredProperties.Any(p => !p.Flags.HasFlag(TypePropertyFlags.SystemProperty)),
                            parsingErrorLookup));
                }

                var narrowedProperties = new List<NamedTypeProperty>();
                foreach (var declaredProperty in targetType.Properties.Values)
                {
                    if (expressionObjectType.Properties.TryGetValue(declaredProperty.Name, out var expressionTypeProperty))
                    {
                        var declaredPropertySyntax = (expression as ObjectSyntax)?.TryGetPropertyByName(declaredProperty.Name);
                        var skipConstantCheckForProperty = config.SkipConstantCheck;

                        // is the property marked as requiring compile-time constants and has the parent already validated this?
                        if (skipConstantCheckForProperty == false && declaredProperty.Flags.HasFlag(TypePropertyFlags.Constant))
                        {
                            // validate that values are compile-time constants
                            GetCompileTimeConstantViolation(declaredPropertySyntax?.Value ?? expression, diagnosticWriter);

                            // disable compile-time constant validation for children
                            skipConstantCheckForProperty = true;
                        }

                        if (declaredProperty.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                        {
                            var diagnosticTarget = config.OriginSyntax ?? declaredPropertySyntax?.Key ?? expression;
                            // the declared property is read-only
                            // value cannot be assigned to a read-only property
                            bool? isExistingResource = binder.GetParent(expression) switch
                            {
                                ResourceDeclarationSyntax rds => rds.IsExistingResource(),
                                // we previously "unwrapped" an if condition body, so there may be one more ancestor to check
                                IfConditionSyntax ifCondition => (binder.GetParent(ifCondition) as ResourceDeclarationSyntax)?.IsExistingResource(),
                                _ => null,
                            };
                            if (isExistingResource is true)
                            {
                                diagnosticWriter.Write(diagnosticTarget, x => x.CannotUsePropertyInExistingResource(declaredProperty.Name));
                            }
                            else if (!expressionTypeProperty.Flags.HasFlag(TypePropertyFlags.ReadOnly))
                            {
                                var resourceTypeInaccuracy = !declaredProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty) && config.IsResourceDeclaration;
                                diagnosticWriter.Write(diagnosticTarget, x => x.CannotAssignToReadOnlyProperty(resourceTypeInaccuracy || ShouldWarnForPropertyMismatch(targetType), declaredProperty.Name, resourceTypeInaccuracy));
                            }

                            narrowedProperties.Add(declaredProperty);
                            continue;
                        }

                        if (declaredProperty.Flags.HasFlag(TypePropertyFlags.FallbackProperty))
                        {
                            diagnosticWriter.Write(config.OriginSyntax ?? declaredPropertySyntax?.Key ?? expression,
                                x => x.FallbackPropertyUsed(shouldDowngrade: false, declaredProperty.Name));
                        }

                        var newConfig = config with
                        {
                            SkipConstantCheck = skipConstantCheckForProperty,
                            SkipTypeErrors = true,
                            DisallowAny = declaredProperty.Flags.HasFlag(TypePropertyFlags.DisallowAny),
                            OnTypeMismatch = GetPropertyMismatchDiagnosticWriter(
                                config: config,
                                shouldWarn: (config.IsResourceDeclaration && !declaredProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)) || ShouldWarn(declaredProperty.TypeReference.Type),
                                propertyName: declaredProperty.Name,
                                showTypeInaccuracyClause: config.IsResourceDeclaration && !declaredProperty.Flags.HasFlag(TypePropertyFlags.SystemProperty)),
                        };

                        var propertyExpression = declaredPropertySyntax?.Value ?? expression;
                        TypeSymbol propertyTargetType = declaredProperty.TypeReference.Type;
                        TypeSymbol propertyExpressionType = expressionTypeProperty.TypeReference.Type;

                        TypeSymbol GetNarrowedPropertyType()
                        {
                            // append "| null" to the property type for non-required properties
                            var (propertyAssignmentType, typeWasPreserved) = AddImplicitNull(propertyTargetType, declaredProperty.Flags);

                            var narrowedType = NarrowType(newConfig, propertyExpression, propertyExpressionType, propertyAssignmentType);
                            return RemoveImplicitNull(narrowedType, typeWasPreserved);
                        }

                        // In the case of a recursive type, eager narrowing can lead to infinite recursion. If we've
                        // already narrowed this (expressionSyntax, expressionType, targetType) triple, then all
                        // relevant diagnostics have already been raised. Use a deferred type reference to stop eagerly
                        // comparing and narrowing types from this point forward.
                        ITypeReference narrowedPropertyType = config.currentlyProcessing.Add((propertyExpression, propertyExpressionType, propertyTargetType))
                            ? GetNarrowedPropertyType()
                            : new DeferredTypeReference(GetNarrowedPropertyType);

                        narrowedProperties.Add(new NamedTypeProperty(declaredProperty.Name, narrowedPropertyType, declaredProperty.Flags));
                    }
                    else
                    {
                        // TODO should this be narrowed against expressionObjectType.AdditionalPropertiesType ?
                        narrowedProperties.Add(declaredProperty);
                    }
                }

                // find properties that are specified on in the expression object but not declared in the schema
                var extraProperties = expressionObjectType.Properties
                    .Where(p => !targetType.Properties.ContainsKey(p.Key));

                // extra properties should raise a diagnostic if the target does not allow additional properties OR the additional properties schema on the target is a "fallback"
                // No diagnostic should be raised if the receiver accepts but discourages additional properties and the assigned value is not an object literal
                if (targetType.AdditionalProperties is null || (expression is ObjectSyntax && targetType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.FallbackProperty)))
                {
                    var shouldWarn = (targetType.AdditionalProperties is not null && targetType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.FallbackProperty)) || ShouldWarnForPropertyMismatch(targetType);
                    var validUnspecifiedProperties = targetType.Properties.Values
                        .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly) &&
                            !p.Flags.HasFlag(TypePropertyFlags.FallbackProperty) &&
                            !expressionObjectType.Properties.ContainsKey(p.Name))
                        .Select(p => p.Name)
                        .OrderBy(x => x)
                        .ToList();

                    foreach (var extraProperty in extraProperties)
                    {
                        var extraPropertySyntax = (expression as ObjectSyntax)?.TryGetPropertyByName(extraProperty.Key);
                        diagnosticWriter.Write(config.OriginSyntax ?? extraPropertySyntax?.Key ?? expression, x =>
                        {
                            var sourceDeclaration = TryGetSourceDeclaration(config);

                            if (sourceDeclaration is null && SpellChecker.GetSpellingSuggestion(extraProperty.Key, validUnspecifiedProperties) is { } suggestedKeyName)
                            {
                                // only look up suggestions if we're not sourcing this type from another declaration.
                                return x.DisallowedPropertyWithSuggestion(shouldWarn, extraProperty.Key, targetType, suggestedKeyName);
                            }

                            return x.DisallowedProperty(shouldWarn, sourceDeclaration, extraProperty.Key, targetType, validUnspecifiedProperties, config.IsResourceDeclaration);
                        });
                    }

                    foreach (var unknownProperty in (expression as ObjectSyntax)?.Properties.Where(p => p.TryGetKeyText() is null) ?? [])
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(unknownProperty.Key).DisallowedInterpolatedKeyProperty(shouldWarn,
                            TryGetSourceDeclaration(config),
                            targetType,
                            validUnspecifiedProperties));
                    }
                }
                else
                {
                    // extra properties must be assignable to the right type
                    foreach (var extraProperty in extraProperties)
                    {
                        var skipConstantCheckForProperty = config.SkipConstantCheck;
                        var extraPropertySyntax = (expression as ObjectSyntax)?.TryGetPropertyByName(extraProperty.Key);

                        // is the property marked as requiring compile-time constants and has the parent already validated this?
                        if (skipConstantCheckForProperty == false && targetType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.Constant))
                        {
                            // validate that values are compile-time constants
                            GetCompileTimeConstantViolation(extraPropertySyntax?.Value ?? expression, diagnosticWriter);

                            // disable compile-time constant validation for children
                            skipConstantCheckForProperty = true;
                        }

                        var newConfig = config with
                        {
                            SkipConstantCheck = skipConstantCheckForProperty,
                            SkipTypeErrors = true,
                            DisallowAny = targetType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.DisallowAny),
                            OnTypeMismatch = GetPropertyMismatchDiagnosticWriter(config, ShouldWarn(targetType.AdditionalProperties.TypeReference.Type), extraProperty.Key, false),
                        };

                        // append "| null" to the type on non-required properties
                        var (additionalPropertiesAssignmentType, _) = AddImplicitNull(targetType.AdditionalProperties.TypeReference.Type, targetType.AdditionalProperties.Flags);

                        // although we don't use the result here, it's important to call NarrowType to collect diagnostics
                        if (config.currentlyProcessing.Add((extraPropertySyntax?.Value ?? expression, extraProperty.Value.TypeReference.Type, additionalPropertiesAssignmentType)))
                        {
                            var narrowedType = NarrowType(newConfig, extraPropertySyntax?.Value ?? expression, extraProperty.Value.TypeReference.Type, additionalPropertiesAssignmentType);
                        }

                        // TODO should we try and narrow the additional properties type? May be difficult
                    }
                }

                var narrowedObject = new ObjectType(targetType.Name, targetType.ValidationFlags, narrowedProperties, targetType.AdditionalProperties, targetType.MethodResolver.CopyToObject);

                return config.IsResourceDeclaration
                    ? TypeHelper.RemovePropertyFlagsRecursively(narrowedObject, TypePropertyFlags.ReadOnly)
                    : narrowedObject;
            }

            return null;
        }

        private (IPositionable positionable, string blockName) GetMissingPropertyContext(SyntaxBase expression)
        {
            var parent = binder.GetParent(expression);

            // determine where to place the missing property error
            return parent switch
            {
                // for properties, put it on the property name in the parent object
                ObjectPropertySyntax objectPropertyParent => (objectPropertyParent.Key, "object"),

                // for extension declarations, mark the entire configuration object
                ExtensionWithClauseSyntax _ => (expression, "object"),

                // for declaration bodies, put it on the declaration identifier
                ITopLevelNamedDeclarationSyntax declarationParent => (declarationParent.Name, declarationParent.Keyword.Text),

                // for conditionals, put it on the parent declaration identifier
                // (the parent of a conditional can only be a resource or module declaration)
                IfConditionSyntax ifCondition => GetMissingPropertyContext(ifCondition),

                // for loops, put it on the parent declaration identifier
                // (the parent of a loop can only be a resource or module declaration)
                ForSyntax @for => GetMissingPropertyContext(@for),

                // fall back to marking the entire object with the error
                _ => (expression, "object")
            };
        }

        private DeclaredSymbol? TryGetSourceDeclaration(TypeValidatorConfig config)
        {
            if (config.OriginSyntax is not null && binder.GetSymbolInfo(config.OriginSyntax) is DeclaredSymbol declaration)
            {
                return declaration;
            }

            return null;
        }

        private TypeMismatchDiagnosticWriter GetPropertyMismatchDiagnosticWriter(TypeValidatorConfig config, bool shouldWarn, string propertyName, bool showTypeInaccuracyClause)
        {
            return (expectedType, actualType, errorExpression) =>
            {
                diagnosticWriter.Write(
                    config.OriginSyntax ?? errorExpression,
                    x =>
                    {
                        var sourceDeclaration = TryGetSourceDeclaration(config);

                        if (sourceDeclaration is not null)
                        {
                            // only look up suggestions if we're not sourcing this type from another declaration.
                            return x.PropertyTypeMismatch(shouldWarn, sourceDeclaration, propertyName, expectedType, actualType, showTypeInaccuracyClause);
                        }

                        if (actualType is StringLiteralType actualStringLiteral && TryGetStringLiteralSuggestion(actualStringLiteral, expectedType) is { } suggestion)
                        {
                            return x.PropertyStringLiteralMismatchWithSuggestion(shouldWarn, propertyName, expectedType, actualType.Name, suggestion);
                        }

                        return x.PropertyTypeMismatch(shouldWarn, sourceDeclaration, propertyName, expectedType, actualType, showTypeInaccuracyClause);
                    });
            };
        }

        private static string? TryGetStringLiteralSuggestion(StringLiteralType actualType, TypeSymbol expectedType)
        {
            if (expectedType is StringLiteralType)
            {
                return SpellChecker.GetSpellingSuggestion(actualType.Name, expectedType.Name.AsEnumerable());
            }

            if (expectedType is UnionType unionType && unionType.Members.All(typeReference => typeReference.Type is StringLiteralType))
            {
                var stringLiteralCandidates = unionType.Members.Select(typeReference => typeReference.Type.Name).OrderBy(s => s);

                return SpellChecker.GetSpellingSuggestion(actualType.Name, stringLiteralCandidates);
            }

            return null;
        }
    }
}
