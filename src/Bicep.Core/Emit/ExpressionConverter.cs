// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
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
                    return CreateParameterlessFunction(boolSyntax.Value ? "true" : "false");
                    
                case NumericLiteralSyntax numericSyntax:
                    return new JTokenExpression(numericSyntax.Value);

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    return ConvertString(stringSyntax);
                    
                case NullLiteralSyntax _:
                    return CreateParameterlessFunction("null");

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
                    return new FunctionExpression(
                        "if",
                        new[]
                        {
                            ConvertExpression(ternary.ConditionExpression),
                            ConvertExpression(ternary.TrueExpression),
                            ConvertExpression(ternary.FalseExpression)
                        },
                        Array.Empty<LanguageExpression>());

                case FunctionCallSyntax function:
                    return ConvertFunction(
                        function.Name.IdentifierName,
                        function.Arguments.Select(a => ConvertExpression(a.Expression)).ToArray());

                case InstanceFunctionCallSyntax instanceFunctionCall:
                    var namespaceSymbol = context.SemanticModel.GetSymbolInfo(instanceFunctionCall.BaseExpression);
                    Assert(namespaceSymbol is NamespaceSymbol, $"BaseExpression must be a NamespaceSymbol, instead got: '{namespaceSymbol?.Kind}'");

                    return ConvertFunction(
                        instanceFunctionCall.Name.IdentifierName,
                        instanceFunctionCall.Arguments.Select(a => ConvertExpression(a.Expression)).ToArray());

                case ArrayAccessSyntax arrayAccess:
                    return AppendProperty(
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
                                return GetResourceIdExpression(resourceSymbol.DeclaringResource, typeReference);
                            case "name":
                                return GetResourceNameExpression(resourceSymbol.DeclaringResource);
                            case "type":
                                return new JTokenExpression(typeReference.FullyQualifiedType);
                            case "apiVersion":
                                return new JTokenExpression(typeReference.ApiVersion);
                            case "properties":
                                // use the reference() overload without "full" to generate a shorter expression
                                return GetReferenceExpression(resourceSymbol.DeclaringResource, typeReference, false);
                        }
                    }

                    var moduleAccess = TryGetModulePropertyAccess(propertyAccess);
                    if (moduleAccess != null)
                    {
                        var (moduleSymbol, outputName) = moduleAccess.Value;
                        return AppendProperty(
                            AppendProperty(
                                GetModuleOutputsReferenceExpression(moduleSymbol.DeclaringModule),
                                new JTokenExpression(outputName)),
                            new JTokenExpression("value"));
                    }

                    return AppendProperty(
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

        private LanguageExpression GetResourceNameExpression(ResourceDeclarationSyntax resourceSyntax)
        {
            if (!(resourceSyntax.Body is ObjectSyntax objectSyntax))
            {
                // this condition should have already been validated by the type checker
                throw new ArgumentException($"Expected resource syntax to have type {typeof(ObjectSyntax)}, but found {resourceSyntax.Body.GetType()}");
            }
            
            // this condition should have already been validated by the type checker
            return TryGetNamedPropertyExpression(objectSyntax, "name") ?? throw new ArgumentException($"Expected resource syntax body to contain property 'name'");
        }

        private LanguageExpression GetModuleNameExpression(ModuleDeclarationSyntax moduleSyntax)
        {
            if (!(moduleSyntax.Body is ObjectSyntax objectSyntax))
            {
                // this condition should have already been validated by the type checker
                throw new ArgumentException($"Expected module syntax to have type {typeof(ObjectSyntax)}, but found {moduleSyntax.Body.GetType()}");
            }
            
            // this condition should have already been validated by the type checker
            return TryGetNamedPropertyExpression(objectSyntax, "name") ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }

        /// <summary>
        /// Tries to get an ARM expression for a named property from a source object syntax. Returns null if no property is found.
        /// </summary>
        private LanguageExpression? TryGetNamedPropertyExpression(ObjectSyntax objectSyntax, string propertyName)
        {
            var namePropertySyntax = objectSyntax.Properties.FirstOrDefault(p => LanguageConstants.IdentifierComparer.Equals(p.TryGetKeyText(), propertyName));
            if (namePropertySyntax == null)
            {
                return null;
            }

            return ConvertExpression(namePropertySyntax.Value);
        }

        private static FunctionExpression GenerateResourceIdExpression(SemanticModel.SemanticModel semanticModel, string fullyQualifiedType, IEnumerable<LanguageExpression> nameSegments)
        {
            var initialArgs = new JTokenExpression(fullyQualifiedType).AsEnumerable();
            switch (semanticModel.TargetScope)
            {
                case ResourceScopeType.TenantScope:
                    var tenantArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("tenantResourceId", tenantArgs.ToArray(), new LanguageExpression[0]);
                case ResourceScopeType.SubscriptionScope:
                    var subscriptionArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("subscriptionResourceId", subscriptionArgs.ToArray(), new LanguageExpression[0]);
                case ResourceScopeType.ResourceGroupScope:
                    var resourceGroupArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("resourceId", resourceGroupArgs.ToArray(), new LanguageExpression[0]);
                case ResourceScopeType.ManagementGroupScope:
                    // We need to do things slightly differently for Management Groups, because there is no IL to output for "Give me a fully-qualified resource id at the current scope",
                    // and we don't even have a mechanism for reliably getting the current scope (e.g. something like 'deployment().scope'). There are plans to add a managementGroupResourceId function,
                    // but until we have it, we should generate unqualified resource Ids. There should not be a risk of collision, because we do not allow mixing of resource scopes in a single bicep file.
                    var typeSegments = fullyQualifiedType.Split("/");

                    // Generate a format string that looks like: My.Rp/type1/{0}/type2/{1}
                    var formatString = $"{typeSegments[0]}/" + string.Join('/', typeSegments.Skip(1).Select((type, i) => $"{type}/{{{i}}}"));
                    initialArgs = new JTokenExpression(formatString).AsEnumerable();

                    // This is going to generate some fairly wonky IL, but we can live with it - there aren't many child resources at management group scope, and this will ultimately be removed when we have a better way to do it (previous comment).
                    var managementGroupArgs = initialArgs.Concat(nameSegments);
                    return new FunctionExpression("format", managementGroupArgs.ToArray(), new LanguageExpression[0]);
                default:
                    // this should have already been caught during compilation
                    throw new InvalidOperationException($"Invalid target scope {semanticModel.TargetScope} for module");
            }
        }

        public FunctionExpression GetResourceIdExpression(ResourceDeclarationSyntax resourceSyntax, ResourceTypeReference typeReference)
        {
            if (typeReference.Types.Length == 1)
            {
                return GenerateResourceIdExpression(
                    context.SemanticModel,
                    typeReference.FullyQualifiedType,
                    GetResourceNameExpression(resourceSyntax).AsEnumerable());
            }

            var nameSegments = typeReference.Types.Select(
                (type, i) => new FunctionExpression(
                    "split",
                    new LanguageExpression[] { GetResourceNameExpression(resourceSyntax), new JTokenExpression("/") },
                    new LanguageExpression[] { new JTokenExpression(i) }));

            return GenerateResourceIdExpression(
                context.SemanticModel,
                typeReference.FullyQualifiedType,
                nameSegments);
        }

        public FunctionExpression GetModuleResourceIdExpression(SemanticModel.SemanticModel semanticModel, ModuleDeclarationSyntax moduleDeclarationSyntax)
        {
            return GenerateResourceIdExpression(
                context.SemanticModel,
                TemplateWriter.NestedDeploymentResourceType,
                GetModuleNameExpression(moduleDeclarationSyntax).AsEnumerable());
        }
        
        public FunctionExpression GetModuleOutputsReferenceExpression(ModuleDeclarationSyntax moduleDeclarationSyntax)
        {
            return new FunctionExpression(
                "reference",
                new LanguageExpression[]
                {
                    GetModuleResourceIdExpression(context.SemanticModel, moduleDeclarationSyntax),
                    new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion),
                },
                new LanguageExpression[]
                {
                    new JTokenExpression("outputs"),
                });
        }

        public FunctionExpression GetReferenceExpression(ResourceDeclarationSyntax resourceSyntax, ResourceTypeReference typeReference, bool full)
        {
            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                return new FunctionExpression(
                    "reference",
                    new LanguageExpression[]
                    {
                        GetResourceIdExpression(resourceSyntax, typeReference),
                        new JTokenExpression(typeReference.ApiVersion),
                        new JTokenExpression("full"),
                    },
                    Array.Empty<LanguageExpression>());
            }

            return new FunctionExpression(
                "reference",
                new LanguageExpression[]
                {
                    GetResourceIdExpression(resourceSyntax, typeReference),
                },
                Array.Empty<LanguageExpression>());
        }

        private LanguageExpression ConvertVariableAccess(VariableAccessSyntax variableAccessSyntax)
        {
            string name = variableAccessSyntax.Name.IdentifierName;

            var symbol = context.SemanticModel.GetSymbolInfo(variableAccessSyntax);

            // TODO: This will change to support inlined functions like reference() or list*()
            switch (symbol)
            {
                case ParameterSymbol _:
                    return CreateUnaryFunction("parameters", new JTokenExpression(name));

                case VariableSymbol variableSymbol:
                    if (context.VariablesToInline.Contains(variableSymbol))
                    {
                        // we've got a runtime dependency, so we have to inline the variable usage
                        return ConvertExpression(variableSymbol.DeclaringVariable.Value);
                    }
                    return CreateUnaryFunction("variables", new JTokenExpression(name));

                case ResourceSymbol resourceSymbol:
                    var typeReference = EmitHelpers.GetTypeReference(resourceSymbol);
                    return GetReferenceExpression(resourceSymbol.DeclaringResource, typeReference, true);

                case ModuleSymbol moduleSymbol:
                    return GetModuleOutputsReferenceExpression(moduleSymbol.DeclaringModule);

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

            return new FunctionExpression("format", formatArgs, Array.Empty<LanguageExpression>());
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
                            return CreateUnaryFunction("int", valueExpression);

                        case JTokenType.String:
                            // convert string literal to function call via string() function
                            return CreateUnaryFunction("string", valueExpression);
                    }

                    break;
            }

            throw new NotImplementedException($"Unexpected expression type '{converted.GetType().Name}'.");
        }

        private static LanguageExpression ConvertFunction(string functionName, LanguageExpression[] arguments)
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

            return new FunctionExpression(functionName, arguments, Array.Empty<LanguageExpression>());
        }

        private static bool ShouldReplaceUnsupportedFunction(string functionName, LanguageExpression[] arguments, [NotNullWhen(true)] out LanguageExpression? replacementExpression)
        {
            switch (functionName)
            {
                // These functions have not yet been implemented in ARM. For now, we will just return an empty object if they are accessed directly.
                case "tenant":
                case "managementGroup":
                case "subscription" when arguments.Length > 0:
                case "resourceGroup" when arguments.Length > 0:
                    replacementExpression = GetCreateObjectExpression();
                    return true;
            }

            replacementExpression = null;
            return false;
        }

        private FunctionExpression ConvertArray(ArraySyntax syntax)
        {
            // we are using the createArray() function as a proxy for an array literal
            return new FunctionExpression(
                "createArray",
                syntax.Items.Select(item => ConvertExpression(item.Value)).ToArray(),
                Array.Empty<LanguageExpression>());
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
            =>  new FunctionExpression("createObject", parameters, Array.Empty<LanguageExpression>());

        private LanguageExpression ConvertBinary(BinaryOperationSyntax syntax)
        {
            LanguageExpression operand1 = ConvertExpression(syntax.LeftExpression);
            LanguageExpression operand2 = ConvertExpression(syntax.RightExpression);

            switch (syntax.Operator)
            {
                case BinaryOperator.LogicalOr:
                    return CreateBinaryFunction("or", operand1, operand2);

                case BinaryOperator.LogicalAnd:
                    return CreateBinaryFunction("and", operand1, operand2);

                case BinaryOperator.Equals:
                    return CreateBinaryFunction("equals", operand1, operand2);

                case BinaryOperator.NotEquals:
                    return CreateUnaryFunction("not", 
                        CreateBinaryFunction("equals", operand1, operand2));

                case BinaryOperator.EqualsInsensitive:
                    return CreateBinaryFunction("equals",
                        CreateUnaryFunction("toLower", operand1),
                        CreateUnaryFunction("toLower", operand2));

                case BinaryOperator.NotEqualsInsensitive:
                    return CreateUnaryFunction("not",
                        CreateBinaryFunction("equals",
                            CreateUnaryFunction("toLower", operand1),
                            CreateUnaryFunction("toLower", operand2)));

                case BinaryOperator.LessThan:
                    return CreateBinaryFunction("less", operand1, operand2);

                case BinaryOperator.LessThanOrEqual:
                    return CreateBinaryFunction("lessOrEquals", operand1, operand2);

                case BinaryOperator.GreaterThan:
                    return CreateBinaryFunction("greater", operand1, operand2);

                case BinaryOperator.GreaterThanOrEqual:
                    return CreateBinaryFunction("greaterOrEquals", operand1, operand2);

                case BinaryOperator.Add:
                    return CreateBinaryFunction("add", operand1, operand2);

                case BinaryOperator.Subtract:
                    return CreateBinaryFunction("sub", operand1, operand2);

                case BinaryOperator.Multiply:
                    return CreateBinaryFunction("mul", operand1, operand2);

                case BinaryOperator.Divide:
                    return CreateBinaryFunction("div", operand1, operand2);

                case BinaryOperator.Modulo:
                    return CreateBinaryFunction("mod", operand1, operand2);

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
                    return CreateUnaryFunction("not", convertedOperand);

                case UnaryOperator.Minus:
                    if (convertedOperand is JTokenExpression literal && literal.Value.Type == JTokenType.Integer)
                    {
                        // invert the integer literal
                        int literalValue = literal.Value.Value<int>();
                        return new JTokenExpression(-literalValue);
                    }

                    return new FunctionExpression(
                        "sub",
                        new[] {new JTokenExpression(0), convertedOperand},
                        Array.Empty<LanguageExpression>());

                default:
                    throw new NotImplementedException($"Cannot emit unexpected unary operator '{syntax.Operator}.");
            }
        }

        private static FunctionExpression AppendProperty(FunctionExpression function, LanguageExpression newProperty) => 
            // technically we could just mutate the provided function object, but let's hold off on that optimization
            // until we have evidence that it is needed
            new FunctionExpression(function.Function, function.Parameters, function.Properties.Append(newProperty).ToArray());

        private static FunctionExpression CreateParameterlessFunction(string function) =>
            new FunctionExpression(function, Array.Empty<LanguageExpression>(), Array.Empty<LanguageExpression>());

        private static FunctionExpression CreateUnaryFunction(string function, LanguageExpression operand) => 
            new FunctionExpression(function, new[] {operand}, Array.Empty<LanguageExpression>());

        private static FunctionExpression CreateBinaryFunction(string name, LanguageExpression operand1, LanguageExpression operand2) =>
            new FunctionExpression(name, new[] {operand1, operand2}, Array.Empty<LanguageExpression>());

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

