// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private readonly EmitterContext context;

        public ExpressionConverter(EmitterContext context)
        {
            this.context = context;
        }

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
                    
                case NumericLiteralSyntax numericSyntax:
                    return new JTokenExpression(numericSyntax.Value);

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
                    return AppendProperties(
                        ToFunctionExpression(arrayAccess.BaseExpression),
                        ConvertExpression(arrayAccess.IndexExpression));

                case PropertyAccessSyntax propertyAccess:
                    if (propertyAccess.BaseExpression is VariableAccessSyntax propVariableAccess &&
                        context.SemanticModel.GetSymbolInfo(propVariableAccess) is ResourceSymbol resourceSymbol)
                    {
                        // special cases for certain resource property access. if we recurse normally, we'll end up
                        // generating statements like reference(resourceId(...)).id which are not accepted by ARM

                        var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
                        switch (propertyAccess.PropertyName.IdentifierName)
                        {
                            case "id":
                                return GetLocallyScopedResourceId(resourceSymbol);
                            case "name":
                                return GetResourceNameExpression(resourceSymbol);
                            case "type":
                                return new JTokenExpression(typeReference.FullyQualifiedType);
                            case "apiVersion":
                                return new JTokenExpression(typeReference.ApiVersion);
                            case "properties":
                                // use the reference() overload without "full" to generate a shorter expression
                                return GetReferenceExpression(resourceSymbol, typeReference, false);
                        }
                    }

                    var moduleAccess = TryGetModulePropertyAccess(propertyAccess);
                    if (moduleAccess != null)
                    {
                        var (moduleSymbol, outputName) = moduleAccess.Value;
                        return AppendProperties(
                            GetModuleOutputsReferenceExpression(moduleSymbol),
                            new JTokenExpression(outputName),
                            new JTokenExpression("value"));
                    }

                    return AppendProperties(
                        ToFunctionExpression(propertyAccess.BaseExpression),
                        new JTokenExpression(propertyAccess.PropertyName.IdentifierName));

                case VariableAccessSyntax variableAccess:
                    return ConvertVariableAccess(variableAccess);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        private (ModuleSymbol moduleSymbol, string outputName)? TryGetModulePropertyAccess(PropertyAccessSyntax propertyAccess)
        {
            // is this a (<child>.outputs).<prop> propertyAccess?
            if (!(propertyAccess.BaseExpression is PropertyAccessSyntax childPropertyAccess) || childPropertyAccess.PropertyName.IdentifierName != LanguageConstants.ModuleOutputsPropertyName)
            {
                return null;
            }

            // is <child> a variable which points to a module symbol?
            if (!(childPropertyAccess.BaseExpression is VariableAccessSyntax grandChildVariableAccess) || !(context.SemanticModel.GetSymbolInfo(grandChildVariableAccess) is ModuleSymbol moduleSymbol))
            {
                return null;
            }

            return (moduleSymbol, propertyAccess.PropertyName.IdentifierName);
        }

        private LanguageExpression GetResourceNameExpression(ResourceSymbol resourceSymbol)
        {
            // this condition should have already been validated by the type checker
            var nameValueSyntax = resourceSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) ?? throw new ArgumentException($"Expected resource syntax body to contain property 'name'");
            return ConvertExpression(nameValueSyntax);
        }

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol)
        {
            // this condition should have already been validated by the type checker
            var nameValueSyntax = moduleSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
            return ConvertExpression(nameValueSyntax);
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

        private LanguageExpression GenerateScopedResourceId(ResourceSymbol resourceSymbol, ResourceScopeType? targetScope)
        {
            var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
            var nameSegments = GetResourceNameSegments(resourceSymbol, typeReference);

            if (context.ResoureScopeData[resourceSymbol] is {} parentResourceSymbol)
            {
                // this should be safe because we've already checked for cycles by now
                var parentResourceId = GetUnqualifiedResourceId(parentResourceSymbol);

                return ExpressionConverter.GenerateScopedResourceId(parentResourceId, typeReference.FullyQualifiedType, nameSegments);
            }

            return ScopeHelper.FormatLocallyScopedResourceId(targetScope, typeReference.FullyQualifiedType, nameSegments);
        }

        public LanguageExpression GetUnqualifiedResourceId(ResourceSymbol resourceSymbol)
            => GenerateScopedResourceId(resourceSymbol, null);

        public LanguageExpression GetLocallyScopedResourceId(ResourceSymbol resourceSymbol)
            => GenerateScopedResourceId(resourceSymbol, context.SemanticModel.TargetScope);

        public LanguageExpression GetModuleResourceIdExpression(ModuleSymbol moduleSymbol)
        {
            return ScopeHelper.FormatCrossScopeResourceId(
                this,
                context.ModuleScopeData[moduleSymbol],
                TemplateWriter.NestedDeploymentResourceType,
                GetModuleNameExpression(moduleSymbol).AsEnumerable());
        }
        
        public FunctionExpression GetModuleOutputsReferenceExpression(ModuleSymbol moduleSymbol)
            => AppendProperties(
                CreateFunction(
                    "reference",
                    GetModuleResourceIdExpression(moduleSymbol),
                    new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion)),
                new JTokenExpression("outputs"));

        public FunctionExpression GetReferenceExpression(ResourceSymbol resourceSymbol, ResourceTypeReference typeReference, bool full)
        {
            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                return CreateFunction(
                    "reference",
                    GetLocallyScopedResourceId(resourceSymbol),
                    new JTokenExpression(typeReference.ApiVersion),
                    new JTokenExpression("full"));
            }

            return CreateFunction(
                "reference",
                GetLocallyScopedResourceId(resourceSymbol));
        }

        private LanguageExpression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            // TODO: This will change to support inlined functions like reference() or list*()
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

                default:
                    throw new NotImplementedException($"Encountered an unexpected symbol kind '{symbol?.Kind}' when generating a variable access expression.");
            }
        }

        private LanguageExpression ConvertString(StringSyntax syntax)
        {
            if (syntax.TryGetLiteralValue() is string literalStringValue)
            {
                // no need to build a format string
                return new JTokenExpression(literalStringValue);;
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
                parameters[index] = new JTokenExpression(propertySyntax.TryGetKeyText());
                index++;

                parameters[index] = ConvertExpression(propertySyntax.Value);
                index++;
            }

            // we are using the createObject() funciton as a proy for an object literal
            return GetCreateObjectExpression(parameters);
        }

        private static FunctionExpression GetCreateObjectExpression(params LanguageExpression[] parameters)
            =>  CreateFunction("createObject", parameters);

        private LanguageExpression ConvertBinary(BinaryOperationSyntax syntax)
        {
            LanguageExpression operand1 = ConvertExpression(syntax.LeftExpression);
            LanguageExpression operand2 = ConvertExpression(syntax.RightExpression);

            switch (syntax.Operator)
            {
                case BinaryOperator.LogicalOr:
                    return CreateFunction("or", operand1, operand2);

                case BinaryOperator.LogicalAnd:
                    return CreateFunction("and", operand1, operand2);

                case BinaryOperator.Equals:
                    return CreateFunction("equals", operand1, operand2);

                case BinaryOperator.NotEquals:
                    return CreateFunction("not", 
                        CreateFunction("equals", operand1, operand2));

                case BinaryOperator.EqualsInsensitive:
                    return CreateFunction("equals",
                        CreateFunction("toLower", operand1),
                        CreateFunction("toLower", operand2));

                case BinaryOperator.NotEqualsInsensitive:
                    return CreateFunction("not",
                        CreateFunction("equals",
                            CreateFunction("toLower", operand1),
                            CreateFunction("toLower", operand2)));

                case BinaryOperator.LessThan:
                    return CreateFunction("less", operand1, operand2);

                case BinaryOperator.LessThanOrEqual:
                    return CreateFunction("lessOrEquals", operand1, operand2);

                case BinaryOperator.GreaterThan:
                    return CreateFunction("greater", operand1, operand2);

                case BinaryOperator.GreaterThanOrEqual:
                    return CreateFunction("greaterOrEquals", operand1, operand2);

                case BinaryOperator.Add:
                    return CreateFunction("add", operand1, operand2);

                case BinaryOperator.Subtract:
                    return CreateFunction("sub", operand1, operand2);

                case BinaryOperator.Multiply:
                    return CreateFunction("mul", operand1, operand2);

                case BinaryOperator.Divide:
                    return CreateFunction("div", operand1, operand2);

                case BinaryOperator.Modulo:
                    return CreateFunction("mod", operand1, operand2);

                default:
                    throw new NotImplementedException($"Cannot emit unexpected binary operator '{syntax.Operator}'.");
            }
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

        public static LanguageExpression GenerateScopedResourceId(LanguageExpression scope, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
            => CreateFunction(
                "extensionResourceId",
                new [] { scope, new JTokenExpression(fullyQualifiedType), }.Concat(nameSegments));

        public static LanguageExpression GenerateResourceGroupScope(LanguageExpression subscriptionId, LanguageExpression resourceGroup)
            => CreateFunction(
                "format",
                new JTokenExpression("/subscriptions/{0}/resourceGroups/{1}"),
                subscriptionId,
                resourceGroup);

        public static LanguageExpression GetManagementGroupScopeExpression(LanguageExpression managementGroupName)
            => CreateFunction(
                "tenantResourceId",
                new JTokenExpression("Microsoft.Management/managementGroups"),
                managementGroupName);

        private static FunctionExpression CreateFunction(string name, params LanguageExpression[] parameters)
            => CreateFunction(name, parameters as IEnumerable<LanguageExpression>);

        private static FunctionExpression CreateFunction(string name, IEnumerable<LanguageExpression> parameters)
            => new FunctionExpression(name, parameters.ToArray(), Array.Empty<LanguageExpression>());

        private static FunctionExpression AppendProperties(FunctionExpression function, params LanguageExpression[] properties)
            => AppendProperties(function, properties as IEnumerable<LanguageExpression>);

        private static FunctionExpression AppendProperties(FunctionExpression function, IEnumerable<LanguageExpression> properties)
            => new FunctionExpression(function.Function, function.Parameters, function.Properties.Concat(properties).ToArray());

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

