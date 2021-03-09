// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;

        private readonly ImmutableDictionary<LocalVariableSymbol, LanguageExpression> localReplacements;

        public ExpressionConverter(EmitterContext context)
            : this(context, ImmutableDictionary<LocalVariableSymbol, LanguageExpression>.Empty)
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, LanguageExpression> localReplacements)
        {
            this.context = context;
            this.localReplacements = localReplacements;
        }

        public ExpressionConverter AppendReplacement(LocalVariableSymbol symbol, LanguageExpression replacement) =>
            new(this.context, this.localReplacements.Add(symbol, replacement));

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        public LanguageExpression ConvertExpression(SyntaxBase expression)
        {
            switch (expression)
            {
                case BooleanLiteralSyntax boolSyntax:
                    return CreateFunction(boolSyntax.Value ? "true" : "false");

                case IntegerLiteralSyntax integerSyntax:
                    return integerSyntax.Value > int.MaxValue || integerSyntax.Value < int.MinValue ? CreateFunction("json", new JTokenExpression(integerSyntax.Value.ToInvariantString())) : new JTokenExpression((int)integerSyntax.Value);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);

                case NullLiteralSyntax _:
                    return CreateFunction("null");

                case ObjectSyntax @object:
                    return ConvertObject(@object);

                case ArraySyntax array:
                    return ConvertArray(array);

                case ParenthesizedExpressionSyntax parenthesized:
                    // template expressions do not have operators so parentheses are irrelevant
                    return ConvertExpression(parenthesized.Expression);

                case UnaryOperationSyntax unary:
                    return ConvertUnary(unary);

                case BinaryOperationSyntax binary:
                    return ConvertBinary(binary);

                case TernaryOperationSyntax ternary:
                    return CreateFunction(
                        "if",
                        ConvertExpression(ternary.ConditionExpression),
                        ConvertExpression(ternary.TrueExpression),
                        ConvertExpression(ternary.FalseExpression));

                case FunctionCallSyntax function:
                    return ConvertFunction(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpression(a.Expression)));

                case InstanceFunctionCallSyntax instanceFunctionCall:
                    var namespaceSymbol = context.SemanticModel.GetSymbolInfo(instanceFunctionCall.BaseExpression);
                    Assert(namespaceSymbol is NamespaceSymbol, $"BaseExpression must be a NamespaceSymbol, instead got: '{namespaceSymbol?.Kind}'");

                    return ConvertFunction(
                        instanceFunctionCall.Name.IdentifierName,
                        instanceFunctionCall.Arguments.Select(a => ConvertExpression(a.Expression)));

                case ArrayAccessSyntax arrayAccess:
                    return ConvertArrayAccess(arrayAccess);

                case ResourceAccessSyntax resourceAccess:
                    return ConvertResourceAccess(resourceAccess);

                case PropertyAccessSyntax propertyAccess:
                    return ConvertPropertyAccess(propertyAccess);

                case VariableAccessSyntax variableAccess:
                    return ConvertVariableAccess(variableAccess);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        public ExpressionConverter CreateConverterForIndexReplacement(SyntaxBase nameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var inaccessibleLocals = this.context.DataFlowAnalyzer.GetInaccessibleLocalsAfterSyntaxMove(nameSyntax, newContext);
            var inaccessibleLocalLoops = inaccessibleLocals.Select(local => GetEnclosingForExpression(local)).Distinct().ToList();

            switch (inaccessibleLocalLoops.Count)
            {
                case 0:
                    // moving the name expression does not produce any inaccessible locals (no locals means no loops)
                    // regardless if there is an index expression or not, we don't need to append replacements 
                    return this;

                case 1 when indexExpression != null:
                    // TODO: Run data flow analysis on the array expression as well. (Will be needed for nested resource loops)
                    var @for = inaccessibleLocalLoops.Single();
                    var current = this;
                    foreach(var local in inaccessibleLocals)
                    {
                        var replacementValue = GetLoopVariableExpression(local, @for, this.ConvertExpression(indexExpression));
                        current = current.AppendReplacement(local, replacementValue);
                    }

                    return current;

                default:
                    throw new NotImplementedException("Mismatch between count of index expressions and inaccessible symbols during array access index replacement.");
            }
        }

        private LanguageExpression ConvertArrayAccess(ArrayAccessSyntax arrayAccess)
        {
            // if there is an array access on a resource/module reference, we have to generate differently
            // when constructing the reference() function call, the resource name expression needs to have its local
            // variable replaced with <loop array expression>[this array access' index expression]
            if (arrayAccess.BaseExpression is VariableAccessSyntax || arrayAccess.BaseExpression is ResourceAccessSyntax)
            {
                switch (this.context.SemanticModel.GetSymbolInfo(arrayAccess.BaseExpression))
                {
                    case ResourceSymbol { IsCollection: true } resourceSymbol:
                        var resourceConverter = this.CreateConverterForIndexReplacement(ExpressionConverter.GetResourceNameSyntax(resourceSymbol), arrayAccess.IndexExpression, arrayAccess);

                        // TODO: Can this return a language expression?
                        return resourceConverter.ToFunctionExpression(arrayAccess.BaseExpression);

                    case ModuleSymbol { IsCollection: true } moduleSymbol:
                        var moduleConverter = this.CreateConverterForIndexReplacement(ExpressionConverter.GetModuleNameSyntax(moduleSymbol), arrayAccess.IndexExpression, arrayAccess);

                        // TODO: Can this return a language expression?
                        return moduleConverter.ToFunctionExpression(arrayAccess.BaseExpression);
                }
            }

            return AppendProperties(
                ToFunctionExpression(arrayAccess.BaseExpression),
                ConvertExpression(arrayAccess.IndexExpression));
        }

        private LanguageExpression ConvertPropertyAccess(PropertyAccessSyntax propertyAccess)
        {
            // local function
            LanguageExpression? ConvertResourcePropertyAccess(ResourceSymbol resourceSymbol, SyntaxBase? indexExpression)
            {
                var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);

                // special cases for certain resource property access. if we recurse normally, we'll end up
                // generating statements like reference(resourceId(...)).id which are not accepted by ARM

                switch (propertyAccess.PropertyName.IdentifierName)
                {
                    case "id":
                        // the ID is dependent on the name expression which could involve locals in case of a resource collection
                        return this
                            .CreateConverterForIndexReplacement(GetResourceNameSyntax(resourceSymbol), indexExpression, propertyAccess)
                            .GetFullyQualifiedResourceId(resourceSymbol);
                    case "name":
                        // the name is dependent on the name expression which could involve locals in case of a resource collection
                        return this
                            .CreateConverterForIndexReplacement(GetResourceNameSyntax(resourceSymbol), indexExpression, propertyAccess)
                            .GetResourceNameExpression(resourceSymbol);
                    case "type":
                        return new JTokenExpression(typeReference.FullyQualifiedType);
                    case "apiVersion":
                        return new JTokenExpression(typeReference.ApiVersion);
                    case "properties":
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return this
                            .CreateConverterForIndexReplacement(GetResourceNameSyntax(resourceSymbol), indexExpression, propertyAccess)
                            .GetReferenceExpression(resourceSymbol, typeReference, false);
                }

                return null;
            }

            LanguageExpression? ConvertModulePropertyAccess(ModuleSymbol moduleSymbol, SyntaxBase? indexExpression)
            {
                switch (propertyAccess.PropertyName.IdentifierName)
                {
                    case "name":
                        // the name is dependent on the name expression which could involve locals in case of a resource collection
                        return this
                            .CreateConverterForIndexReplacement(GetModuleNameSyntax(moduleSymbol), indexExpression, propertyAccess)
                            .GetModuleNameExpression(moduleSymbol);
                }

                return null;
            }

            if ((propertyAccess.BaseExpression is VariableAccessSyntax || propertyAccess.BaseExpression is ResourceAccessSyntax) &&
                context.SemanticModel.GetSymbolInfo(propertyAccess.BaseExpression) is ResourceSymbol resourceSymbol &&
                ConvertResourcePropertyAccess(resourceSymbol, indexExpression: null) is { } convertedSingle)
            {
                // we are doing property access on a single resource
                // and we are dealing with special case properties
                return convertedSingle;
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax propArrayAccess &&
                (propArrayAccess.BaseExpression is VariableAccessSyntax || propArrayAccess.BaseExpression is ResourceAccessSyntax) && 
                context.SemanticModel.GetSymbolInfo(propArrayAccess.BaseExpression) is ResourceSymbol resourceCollectionSymbol &&
                ConvertResourcePropertyAccess(resourceCollectionSymbol, propArrayAccess.IndexExpression) is { } convertedCollection)
            {

                // we are doing property access on an array access of a resource collection
                // and we are dealing with special case properties
                return convertedCollection;
            }

            if (propertyAccess.BaseExpression is VariableAccessSyntax modulePropVariableAccess &&
                context.SemanticModel.GetSymbolInfo(modulePropVariableAccess) is ModuleSymbol moduleSymbol &&
                ConvertModulePropertyAccess(moduleSymbol, indexExpression: null) is { } moduleConvertedSingle)
            {
                // we are doing property access on a single module
                // and we are dealing with special case properties
                return moduleConvertedSingle;
            }

            if (propertyAccess.BaseExpression is ArrayAccessSyntax modulePropArrayAccess &&
                modulePropArrayAccess.BaseExpression is VariableAccessSyntax moduleArrayVariableAccess &&
                context.SemanticModel.GetSymbolInfo(moduleArrayVariableAccess) is ModuleSymbol moduleCollectionSymbol && 
                ConvertModulePropertyAccess(moduleCollectionSymbol, modulePropArrayAccess.IndexExpression) is { } moduleConvertedCollection)
            {

                // we are doing property access on an array access of a module collection
                // and we are dealing with special case properties
                return moduleConvertedCollection;
            }

            // is this a (<child>.outputs).<prop> propertyAccess?
            if (propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess && childPropertyAccess.PropertyName.IdentifierName == LanguageConstants.ModuleOutputsPropertyName)
            {
                // is <child> a variable which points to a non-collection module symbol?
                if (childPropertyAccess.BaseExpression is VariableAccessSyntax grandChildVariableAccess &&
                    context.SemanticModel.GetSymbolInfo(grandChildVariableAccess) is ModuleSymbol { IsCollection: false } outputsModuleSymbol)
                {
                    return AppendProperties(
                        this.GetModuleOutputsReferenceExpression(outputsModuleSymbol),
                        new JTokenExpression(propertyAccess.PropertyName.IdentifierName),
                        new JTokenExpression("value"));
                }

                // is <child> an array access operating on a module collection
                if (childPropertyAccess.BaseExpression is ArrayAccessSyntax grandChildArrayAccess &&
                    grandChildArrayAccess.BaseExpression is VariableAccessSyntax grandGrandChildVariableAccess &&
                    context.SemanticModel.GetSymbolInfo(grandGrandChildVariableAccess) is ModuleSymbol { IsCollection: true } outputsModuleCollectionSymbol)
                {
                    var updatedConverter = this.CreateConverterForIndexReplacement(GetModuleNameSyntax(outputsModuleCollectionSymbol), grandChildArrayAccess.IndexExpression, propertyAccess);
                    return AppendProperties(
                        updatedConverter.GetModuleOutputsReferenceExpression(outputsModuleCollectionSymbol),
                        new JTokenExpression(propertyAccess.PropertyName.IdentifierName),
                        new JTokenExpression("value"));
                }
            }

            return AppendProperties(
                ToFunctionExpression(propertyAccess.BaseExpression),
                new JTokenExpression(propertyAccess.PropertyName.IdentifierName));
        }

        public LanguageExpression GetResourceNameExpression(ResourceSymbol resourceSymbol)
        {
            var nameValueSyntax = GetResourceNameSyntax(resourceSymbol);

            // For a nested resource we need to compute the name
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resourceSymbol);
            if (ancestors.Length == 0)
            {
                return ConvertExpression(nameValueSyntax);
            }

            // Build an expression like '${parent.name}/${child.name}'
            //
            // This is a call to the `format` function with the first arg as a format string
            // and the remaining args the actual name segments.
            //
            // args.Length = 1 (format string) + N (ancestor names) + 1 (resource name)
            var args = new LanguageExpression[ancestors.Length + 2];

            // {0}/{1}/{2}....
            var format = string.Join("/", Enumerable.Range(0, ancestors.Length + 1).Select(i => $"{{{i}}}"));
            args[0] = new JTokenExpression(format);

            for (var i = 0; i < ancestors.Length; i++)
            {
                var ancestor = ancestors[i];
                var segment = GetResourceNameSyntax(ancestor);
                args[i + 1] = ConvertExpression(segment);
            }

            args[args.Length - 1] = ConvertExpression(nameValueSyntax);

            return CreateFunction("format", args);
        }

        public static SyntaxBase GetResourceNameSyntax(ResourceSymbol resourceSymbol)
        {
            // this condition should have already been validated by the type checker
            return resourceSymbol.UnsafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName);
        }

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol)
        {
            SyntaxBase nameValueSyntax = GetModuleNameSyntax(moduleSymbol);
            return ConvertExpression(nameValueSyntax);
        }

        public static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
        {
            // this condition should have already been validated by the type checker
            return moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }

        public IEnumerable<LanguageExpression> GetResourceNameSegments(ResourceSymbol resourceSymbol, ResourceTypeReference typeReference)
        {
            if (typeReference.Types.Length == 1)
            {
                return GetResourceNameExpression(resourceSymbol).AsEnumerable();
            }

            return typeReference.Types.Select(
                (type, i) => AppendProperties(
                    CreateFunction(
                        "split",
                        GetResourceNameExpression(resourceSymbol),
                        new JTokenExpression("/")),
                    new JTokenExpression(i)));
        }

        public LanguageExpression GetUnqualifiedResourceId(ResourceSymbol resourceSymbol)
        {
            var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);

            return ScopeHelper.FormatUnqualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resourceSymbol],
                typeReference.FullyQualifiedType,
                GetResourceNameSegments(resourceSymbol, typeReference));
        }

        public LanguageExpression GetFullyQualifiedResourceId(ResourceSymbol resourceSymbol)
        {
            var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);

            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resourceSymbol],
                typeReference.FullyQualifiedType,
                GetResourceNameSegments(resourceSymbol, typeReference));
        }

        public LanguageExpression GetFullyQualifiedResourceId(ModuleSymbol moduleSymbol)
        {
            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.ModuleScopeData[moduleSymbol],
                TemplateWriter.NestedDeploymentResourceType,
                GetModuleNameExpression(moduleSymbol).AsEnumerable());
        }

        public FunctionExpression GetModuleOutputsReferenceExpression(ModuleSymbol moduleSymbol) =>
            AppendProperties(
                CreateFunction(
                    "reference",
                    GetFullyQualifiedResourceId(moduleSymbol),
                    new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                new JTokenExpression("outputs"));

        public FunctionExpression GetReferenceExpression(ResourceSymbol resourceSymbol, ResourceTypeReference typeReference, bool full)
        {
            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                return CreateFunction(
                    "reference",
                    GetFullyQualifiedResourceId(resourceSymbol),
                    new JTokenExpression(typeReference.ApiVersion),
                    new JTokenExpression("full"));
            }

            if (resourceSymbol.DeclaringResource.IsExistingResource())
            {
                // we must include an API version for an existing resource, because it cannot be inferred from any deployed template resource
                return CreateFunction(
                    "reference",
                    GetFullyQualifiedResourceId(resourceSymbol),
                    new JTokenExpression(typeReference.ApiVersion));
            }

            return CreateFunction(
                "reference",
                GetFullyQualifiedResourceId(resourceSymbol));
        }

        private LanguageExpression GetLocalVariableExpression(LocalVariableSymbol localVariableSymbol)
        {
            if (this.localReplacements.TryGetValue(localVariableSymbol, out var replacement))
            {
                // the current context has specified an expression to be used for this local variable symbol
                // to override the regular conversion
                return replacement;
            }

            var @for = GetEnclosingForExpression(localVariableSymbol);
            return GetLoopVariableExpression(localVariableSymbol, @for, CreateCopyIndexFunction(@for));
        }

        private LanguageExpression GetLoopVariableExpression(LocalVariableSymbol localVariableSymbol, ForSyntax @for, LanguageExpression indexExpression)
        {
            return localVariableSymbol.LocalKind switch
            {
                // this is the "item" variable of a for-expression
                // to emit this, we need to index the array expression by the copyIndex() function
                LocalKind.ForExpressionItemVariable => GetLoopItemVariableExpression(@for, indexExpression),

                // this is the "index" variable of a for-expression inside a variable block
                // to emit this, we need to return a copyIndex(...) function
                LocalKind.ForExpressionIndexVariable => indexExpression,

                _ => throw new NotImplementedException($"Unexpected local variable kind '{localVariableSymbol.LocalKind}'."),
            };
        }

        private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
        {
            // we're following the symbol hierarchy rather than syntax hierarchy because
            // this guarantees a single hop in all cases
            var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
            if (symbolParent is not LocalScope localScope)
            {
                throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
            }

            if(localScope.DeclaringSyntax is ForSyntax @for)
            {
                return @for;
            }

            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{localScope.DeclaringSyntax?.GetType().Name}'.");
        }

        private string? GetCopyIndexName(ForSyntax @for)
        {
            return this.context.SemanticModel.Binder.GetParent(@for) switch
            {
                // copyIndex without name resolves to module/resource loop index in the runtime
                ResourceDeclarationSyntax => null,
                ModuleDeclarationSyntax => null,

                // output loops are only allowed at the top level and don't have names, either
                OutputDeclarationSyntax => null,

                ObjectPropertySyntax property when property.TryGetKeyText() is { } key && ReferenceEquals(property.Value, @for) => key,

                _ => throw new NotImplementedException("Unexpected for-expression grandparent.")
            };
        }

        private FunctionExpression CreateCopyIndexFunction(ForSyntax @for)
        {
            var copyIndexName = GetCopyIndexName(@for);
            return copyIndexName is null
                ? CreateFunction("copyIndex")
                : CreateFunction("copyIndex", new JTokenExpression(copyIndexName));
        }

        private FunctionExpression GetLoopItemVariableExpression(ForSyntax @for, LanguageExpression indexExpression)
        {
            // loop item variable should be replaced with <array expression>[<index expression>]
            var arrayExpression = ToFunctionExpression(@for.Expression);
            
            return AppendProperties(arrayExpression, indexExpression);
        }

        private LanguageExpression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            switch (symbol)
            {
                case ParameterSymbol _:
                    return CreateFunction("parameters", new JTokenExpression(name));

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpression(variableSymbol.DeclaringVariable.Value);
                    }
                    return CreateFunction("variables", new JTokenExpression(name));

                case ResourceSymbol resourceSymbol:
                    var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
                    return GetReferenceExpression(resourceSymbol, typeReference, true);

                case ModuleSymbol moduleSymbol:
                    return GetModuleOutputsReferenceExpression(moduleSymbol);

                case LocalVariableSymbol localVariableSymbol:
                    return GetLocalVariableExpression(localVariableSymbol);

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");
            }
        }

        private LanguageExpression ConvertResourceAccess(ResourceAccessSyntax resourceAccessSyntax)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(resourceAccessSyntax);
            if (symbol is ResourceSymbol resourceSymbol)
            {
                var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
                return GetReferenceExpression(resourceSymbol, typeReference, true);
            }

            throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a resource access expression.");
        }

        private LanguageExpression ConvertString(StringSyntax syntax)
        {
            if (syntax.TryGetLiteralValue() is string literalStringValue)
            {
                // no need to build a format string
                return new JTokenExpression(literalStringValue);
            }

            if (syntax.Expressions.Length == 1)
            {
                const string emptyStringOpen = LanguageConstants.StringDelimiter + LanguageConstants.StringHoleOpen; // '${
                const string emptyStringClose = LanguageConstants.StringHoleClose + LanguageConstants.StringDelimiter; // }'

                // Special-case interpolation of format '${myValue}' because it's a common pattern for userAssignedIdentities.
                // There's no need for a 'format' function because we just have a single expression with no outer formatting.
                if (syntax.StringTokens[0].Text == emptyStringOpen && syntax.StringTokens[1].Text == emptyStringClose)
                {
                    return ConvertExpression(syntax.Expressions[0]);
                }
            }

            var formatArgs = new LanguageExpression[syntax.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(syntax);
            formatArgs[0] = new JTokenExpression(formatString);

            for (var i = 0; i < syntax.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpression(syntax.Expressions[i]);
            }

            return CreateFunction("format", formatArgs);
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// This always returns a function expression, which is useful when converting property access or array access
        /// on literals.
        /// </summary>
        /// <param name="expression">The expression</param>
        public FunctionExpression ToFunctionExpression(SyntaxBase expression)
        {
            var converted = ConvertExpression(expression);
            switch (converted)
            {
                case FunctionExpression functionExpression:
                    return functionExpression;

                case JTokenExpression valueExpression:
                    JToken value = valueExpression.Value;

                    switch (value.Type)
                    {
                        case JTokenType.Integer:
                            // convert integer literal to a function call via int() function
                            return CreateFunction("int", valueExpression);

                        case JTokenType.String:
                            // convert string literal to function call via string() function
                            return CreateFunction("string", valueExpression);
                    }

                    break;
            }

            throw new NotImplementedException($"Unexpected expression type '{converted.GetType().Name}'.");
        }

        private static LanguageExpression ConvertFunction(string functionName, IEnumerable<LanguageExpression> arguments)
        {
            if (string.Equals("any", functionName, LanguageConstants.IdentifierComparison))
            {
                // this is the any function - don't generate a function call for it
                return arguments.Single();
            }

            if (ShouldReplaceUnsupportedFunction(functionName, arguments, out var replacementExpression))
            {
                return replacementExpression;
            }

            return CreateFunction(functionName, arguments);
        }

        private static bool ShouldReplaceUnsupportedFunction(string functionName, IEnumerable<LanguageExpression> arguments, [NotNullWhen(true)] out LanguageExpression? replacementExpression)
        {
            switch (functionName)
            {
                // These functions have not yet been implemented in ARM. For now, we will just return an empty object if they are accessed directly.
                case "tenant":
                case "managementGroup":
                case "subscription" when arguments.Any():
                case "resourceGroup" when arguments.Any():
                    replacementExpression = GetCreateObjectExpression();
                    return true;
            }

            replacementExpression = null;
            return false;
        }

        private FunctionExpression ConvertArray(ArraySyntax syntax)
        {
            // we are using the createArray() function as a proxy for an array literal
            return CreateFunction(
                "createArray",
                syntax.Items.Select(item => ConvertExpression(item.Value)));
        }

        private FunctionExpression ConvertObject(ObjectSyntax syntax)
        {
            // need keys and values in one array of parameters
            var parameters = new LanguageExpression[syntax.Properties.Count() * 2];

            int index = 0;
            foreach (var propertySyntax in syntax.Properties)
            {
                parameters[index] = propertySyntax.Key switch
                {
                    IdentifierSyntax identifier => new JTokenExpression(identifier.IdentifierName),
                    StringSyntax @string => ConvertString(@string),
                    _ => throw new NotImplementedException($"Encountered an unexpected type '{propertySyntax.Key.GetType().Name}' when generating object's property name.")
                };
                index++;

                parameters[index] = ConvertExpression(propertySyntax.Value);
                index++;
            }

            // we are using the createObject() function as a proxy for an object literal
            return GetCreateObjectExpression(parameters);
        }

        private static FunctionExpression GetCreateObjectExpression(params LanguageExpression[] parameters)
            => CreateFunction("createObject", parameters);

        private LanguageExpression ConvertBinary(BinaryOperationSyntax syntax)
        {
            LanguageExpression operand1 = ConvertExpression(syntax.LeftExpression);
            LanguageExpression operand2 = ConvertExpression(syntax.RightExpression);

            return syntax.Operator switch
            {
                BinaryOperator.LogicalOr => CreateFunction("or", operand1, operand2),
                BinaryOperator.LogicalAnd => CreateFunction("and", operand1, operand2),
                BinaryOperator.Equals => CreateFunction("equals", operand1, operand2),
                BinaryOperator.NotEquals => CreateFunction("not",
                    CreateFunction("equals", operand1, operand2)),
                BinaryOperator.EqualsInsensitive => CreateFunction("equals",
                    CreateFunction("toLower", operand1),
                    CreateFunction("toLower", operand2)),
                BinaryOperator.NotEqualsInsensitive => CreateFunction("not",
                    CreateFunction("equals",
                        CreateFunction("toLower", operand1),
                        CreateFunction("toLower", operand2))),
                BinaryOperator.LessThan => CreateFunction("less", operand1, operand2),
                BinaryOperator.LessThanOrEqual => CreateFunction("lessOrEquals", operand1, operand2),
                BinaryOperator.GreaterThan => CreateFunction("greater", operand1, operand2),
                BinaryOperator.GreaterThanOrEqual => CreateFunction("greaterOrEquals", operand1, operand2),
                BinaryOperator.Add => CreateFunction("add", operand1, operand2),
                BinaryOperator.Subtract => CreateFunction("sub", operand1, operand2),
                BinaryOperator.Multiply => CreateFunction("mul", operand1, operand2),
                BinaryOperator.Divide => CreateFunction("div", operand1, operand2),
                BinaryOperator.Modulo => CreateFunction("mod", operand1, operand2),
                BinaryOperator.Coalesce => CreateFunction("coalesce", operand1, operand2),
                _ => throw new NotImplementedException($"Cannot emit unexpected binary operator '{syntax.Operator}'."),
            };
        }

        private LanguageExpression ConvertUnary(UnaryOperationSyntax syntax)
        {
            LanguageExpression convertedOperand = ConvertExpression(syntax.Expression);

            switch (syntax.Operator)
            {
                case UnaryOperator.Not:
                    return CreateFunction("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (convertedOperand is JTokenExpression literal && literal.Value.Type == JTokenType.Integer)
                    {
                        // invert the integer literal
                        int literalValue = literal.Value.Value<int>();
                        return new JTokenExpression(-literalValue);
                    }

                    return CreateFunction(
                        "sub",
                        new JTokenExpression(0),
                        convertedOperand);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        public static LanguageExpression GenerateUnqualifiedResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            var typeSegments = fullyQualifiedType.Split("/");

            // Generate a format string that looks like: My.Rp/type1/{0}/type2/{1}
            var formatString = $"{typeSegments[0]}/" + string.Join('/', typeSegments.Skip(1).Select((type, i) => $"{type}/{{{i}}}"));

            return CreateFunction(
                "format",
                new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
        }

        public static LanguageExpression GenerateExtensionResourceId(LanguageExpression scope, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "extensionResourceId",
                new[] { scope, new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupScope(LanguageExpression subscriptionId, LanguageExpression resourceGroup)
            => CreateFunction(
                "format",
                new JTokenExpression("/subscriptions/{0}/resourceGroups/{1}"),
                subscriptionId,
                resourceGroup);

        public static LanguageExpression GenerateTenantResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "tenantResourceId",
                new[] { new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupResourceId(string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "resourceId",
                new[] { new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public LanguageExpression GenerateManagementGroupResourceId(SyntaxBase managementGroupNameProperty, bool fullyQualified)
        {
            const string managementGroupType = "Microsoft.Management/managementGroups";
            var managementGroupName = ConvertExpression(managementGroupNameProperty);

            if (fullyQualified)
            {
                return GenerateTenantResourceId(managementGroupType, new[] { managementGroupName });
            }
            else
            {
                return GenerateUnqualifiedResourceId(managementGroupType, new[] { managementGroupName });
            }
        }

        private static FunctionExpression CreateFunction(string name, params LanguageExpression[] parameters)
            => CreateFunction(name, parameters as IEnumerable<LanguageExpression>);

        private static FunctionExpression CreateFunction(string name, IEnumerable<LanguageExpression> parameters)
            => new(name, parameters.ToArray(), Array.Empty<LanguageExpression>());

        public static FunctionExpression AppendProperties(FunctionExpression function, params LanguageExpression[] properties)
            => AppendProperties(function, properties as IEnumerable<LanguageExpression>);

        public static FunctionExpression AppendProperties(FunctionExpression function, IEnumerable<LanguageExpression> properties)
            => new(function.Function, function.Parameters, function.Properties.Concat(properties).ToArray());

        protected static void Assert(bool predicate, string message)
        {
            if (predicate == false)
            {
                // we have a code defect - use the exception stack to debug
                throw new ArgumentException(message);
            }
        }
    }
}

