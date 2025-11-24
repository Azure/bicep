// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionConverter
    {
        private const string secureOutputsApi = "listOutputsWithSecureValues";

        private readonly EmitterContext context;
        private readonly ExpressionBuilder expressionBuilder;

        public ExpressionConverter(EmitterContext context)
            : this(context, [])
        {
        }

        private ExpressionConverter(EmitterContext context, ImmutableDictionary<LocalVariableSymbol, Expression> localReplacements)
        {
            this.context = context;
            this.expressionBuilder = new ExpressionBuilder(context, localReplacements);
        }

        public Expression ConvertToIntermediateExpression(SyntaxBase syntax)
            => expressionBuilder.Convert(syntax);

        public LanguageExpression ConvertExpression(SyntaxBase syntax)
        {
            var expression = ConvertToIntermediateExpression(syntax);

            return ConvertExpression(expression);
        }

        /// <summary>
        /// Converts the specified bicep expression tree into an ARM template expression tree.
        /// The returned tree may be rooted at either a function expression or jtoken expression.
        /// </summary>
        /// <param name="expression">The expression</param>
        public LanguageExpression ConvertExpression(Expression expression)
        {
            switch (expression)
            {
                case BooleanLiteralExpression @bool:
                    return CreateFunction(@bool.Value ? "true" : "false");

                case IntegerLiteralExpression @int:
                    return @int.Value switch
                    {
                        // Bicep permits long values, but ARM's parser only permits int jtoken expressions.
                        // We can work around this by using the `json()` function to represent non-integer numerics.
                        > int.MaxValue or < int.MinValue => CreateFunction("json", new JTokenExpression(@int.Value.ToString(CultureInfo.InvariantCulture))),
                        _ => new JTokenExpression(@int.Value),
                    };

                case StringLiteralExpression @string:
                    return new JTokenExpression(@string.Value);

                case NullLiteralExpression _:
                    return CreateFunction("null");

                case InterpolatedStringExpression @string:
                    return ConvertString(@string);

                case ObjectExpression @object:
                    return ConvertObject(@object);

                case ArrayExpression array:
                    return ConvertArray(array);

                case UnaryExpression unary:
                    return ConvertUnary(unary);

                case BinaryExpression binary:
                    return ConvertBinary(binary);

                case TernaryExpression ternary:
                    return CreateFunction(
                        "if",
                        ConvertExpression(ternary.Condition),
                        ConvertExpression(ternary.True),
                        ConvertExpression(ternary.False));

                case FunctionCallExpression function:
                    return CreateFunction(
                        function.Name,
                        function.Parameters.Select(ConvertExpression));

                case UserDefinedFunctionCallExpression function:
                    return CreateFunction(
                        $"{EmitConstants.UserDefinedFunctionsNamespace}.{function.Symbol.Name}",
                        function.Parameters.Select(ConvertExpression));

                case SynthesizedUserDefinedFunctionCallExpression function:
                    return CreateFunction(
                        $"{function.Namespace}.{function.Name}",
                        function.Parameters.Select(ConvertExpression));

                case ImportedUserDefinedFunctionCallExpression importedFunction:
                    {
                        var (namespaceName, functionName) = GetFunctionName(context.SemanticModel.ImportClosureInfo.ImportedSymbolNames[importedFunction.Symbol]);
                        return CreateFunction(
                            $"{namespaceName}.{functionName}",
                            importedFunction.Parameters.Select(ConvertExpression));
                    }

                case WildcardImportInstanceFunctionCallExpression importedFunction:
                    {
                        var (namespaceName, functionName) = GetFunctionName(
                            context.SemanticModel.ImportClosureInfo.WildcardImportPropertyNames[new(importedFunction.ImportSymbol, importedFunction.MethodName)]);
                        return CreateFunction(
                            $"{namespaceName}.{functionName}",
                            importedFunction.Parameters.Select(ConvertExpression));
                    }

                case ResourceFunctionCallExpression listFunction when listFunction.Name.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix):
                    {
                        var resource = listFunction.Resource.Metadata;

                        var functionTargetExpression =
                            context.Settings.EnableSymbolicNames && resource is DeclaredResourceMetadata declared
                                ? GenerateSymbolicReference(declared, listFunction.Resource.IndexContext)
                                : ConvertExpression(new PropertyAccessExpression(
                                    listFunction.Resource.SourceSyntax,
                                    listFunction.Resource,
                                    "id",
                                    AccessExpressionFlags.None));

                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                        var apiVersionExpression = new JTokenExpression(apiVersion);

                        var listArgs = listFunction.Parameters.Length switch
                        {
                            0 => new LanguageExpression[] { functionTargetExpression, apiVersionExpression, },
                            _ => new LanguageExpression[] { functionTargetExpression, }.Concat(listFunction.Parameters.Select(ConvertExpression)),
                        };

                        return CreateFunction(listFunction.Name, listArgs);
                    }

                case PropertyAccessExpression exp:
                    return ConvertPropertyAccessExpression(exp);

                case ModuleOutputPropertyAccessExpression exp:
                    return ConvertModuleOutputPropertyAccessExpression(exp);

                case AccessExpression exp:
                    return ConvertAccessExpression(exp);

                case ResourceReferenceExpression exp:
                    return GetReferenceExpression(exp.Metadata, exp.IndexContext, true);

                case ModuleReferenceExpression exp:
                    return GetModuleReferenceExpression(exp.Module, exp.IndexContext, false);

                case VariableReferenceExpression exp:
                    return CreateFunction("variables", new JTokenExpression(exp.Variable.Name));

                case SynthesizedVariableReferenceExpression exp:
                    return CreateFunction("variables", new JTokenExpression(exp.Name));

                case ImportedVariableReferenceExpression exp:
                    return CreateFunction("variables", new JTokenExpression(context.SemanticModel.ImportClosureInfo.ImportedSymbolNames[exp.Variable]));

                case WildcardImportVariablePropertyReferenceExpression exp:
                    return CreateFunction("variables",
                    new JTokenExpression(context.SemanticModel.ImportClosureInfo.WildcardImportPropertyNames[new(exp.ImportSymbol, exp.PropertyName)]));

                case ParametersReferenceExpression exp:
                    return CreateFunction("parameters", new JTokenExpression(exp.Parameter.Name));

                case ParametersAssignmentReferenceExpression exp:
                    return CreateFunction("parameters", new JTokenExpression(exp.Parameter.Name));

                case ExtensionReferenceExpression exp:
                    return CreateFunction("extensions", new JTokenExpression(exp.ExtensionNamespace.Name));

                case LambdaExpression exp:
                    var variableNames = exp.Parameters.Select(x => new JTokenExpression(x));
                    var body = ConvertExpression(exp.Body);

                    return CreateFunction(
                        "lambda",
                        variableNames.Concat(body));

                case LambdaVariableReferenceExpression exp:
                    if (exp.IsFunctionLambda)
                    {
                        return CreateFunction("parameters", new JTokenExpression(exp.Variable.Name));
                    }
                    return CreateFunction("lambdaVariables", new JTokenExpression(exp.Variable.Name));

                case CopyIndexExpression exp:
                    return exp.Name is null
                        ? CreateFunction("copyIndex")
                        : CreateFunction("copyIndex", new JTokenExpression(exp.Name));

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {expression.GetType().Name}");
            }
        }

        public ExpressionConverter GetConverter(IndexReplacementContext? replacementContext)
        {
            if (replacementContext is not null)
            {
                return new(this.context, replacementContext.LocalReplacements);
            }

            return this;
        }

        private LanguageExpression ConvertPropertyAccessExpression(PropertyAccessExpression exp) => exp.Base switch
        {
            ResourceReferenceExpression resource
                => GetConverter(resource.IndexContext).ConvertResourcePropertyAccess(resource, exp),
            ModuleReferenceExpression module
                => GetConverter(module.IndexContext).ConvertModulePropertyAccess(module, exp),
            _ => ConvertAccessExpression(exp),
        };

        private LanguageExpression ConvertModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression exp)
        {
            LanguageExpression baseExpr;
            // the listOutputsWithSecureValues API has a different format from regular outputs
            List<JTokenExpression> tail = [new(exp.PropertyName)];

            if (exp.Base is PropertyAccessExpression { Base: ModuleReferenceExpression module } baseAccessExpr)
            {
                baseExpr = GetConverter(module.IndexContext)
                    .ConvertModulePropertyAccess(module, baseAccessExpr, exp.IsSecureOutput);
                if (!exp.IsSecureOutput)
                {
                    tail.Add(new("value"));
                }
            }
            else
            {
                baseExpr = ConvertExpression(exp.Base);
                tail.Add(new("value"));
            }

            var @base = ToFunctionExpression(baseExpr);

            if (exp.Flags.HasFlag(AccessExpressionFlags.SafeAccess))
            {
                // there are two scenarios we want to handle in this case:
                //   - conditional outputs (where the accessed property will be omitted from the `outputs` object)
                //   - outputs with a null value (where the accessed property will be present in the `outputs` object, but its `value` property will be omitted)
                return tail.Aggregate(@base, (acc, cur) => CreateFunction("tryGet", acc, cur));
            }

            if (SafeDereferenceFunctions.Contains(@base.Function) && @base.Properties.Length == 0)
            {
                return CreateFunction(@base.Function, @base.Parameters.Concat(tail));
            }

            if (@base.Function == "if" &&
                (@base.Parameters[^1] is FunctionExpression ifFalseFunc && ifFalseFunc.Function == "null"))
            {
                return CreateFunction("tryGet", @base.AsEnumerable<LanguageExpression>().Concat(tail));
            }

            return AppendProperties(@base, tail);
        }

        private static readonly FrozenSet<string> SafeDereferenceFunctions = new[] { "tryGet", "tryIndexFromEnd" }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        private LanguageExpression ConvertAccessExpression(AccessExpression expression)
        {
            var @base = ConvertExpression(expression.Base);
            var accessor = ConvertExpression(expression.Access);

            if (expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess))
            {
                return CreateFunction(
                    expression.Flags.HasFlag(AccessExpressionFlags.FromEnd) ? "tryIndexFromEnd" : "tryGet",
                    @base,
                    accessor);
            }

            if (expression.Flags.HasFlag(AccessExpressionFlags.Chained) &&
                @base is FunctionExpression baseFunc &&
                baseFunc.Properties.Length == 0 &&
                SafeDereferenceFunctions.Contains(baseFunc.Function))
            {
                var extraArg = expression.Flags.HasFlag(AccessExpressionFlags.FromEnd)
                    ? CreateFunction(
                        "createObject",
                        new JTokenExpression("value"),
                        accessor,
                        new JTokenExpression("fromEnd"),
                        CreateFunction("true"))
                    : accessor;
                return CreateFunction(baseFunc.Function, baseFunc.Parameters.Append(extraArg));
            }

            if (expression.Flags.HasFlag(AccessExpressionFlags.FromEnd))
            {
                return CreateFunction("indexFromEnd", @base, accessor);
            }

            return AppendProperties(ToFunctionExpression(@base), accessor);
        }

        private LanguageExpression ConvertResourcePropertyAccess(ResourceReferenceExpression reference, PropertyAccessExpression expression)
        {
            var resource = reference.Metadata;
            var indexContext = reference.IndexContext;
            var propertyName = expression.PropertyName;
            var safeAccess = expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess);

            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                var refExpression = GetReferenceExpression(resource, indexContext, false);
                return expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess)
                    ? new FunctionExpression("tryGet", [refExpression, new JTokenExpression(propertyName)], [])
                    : AppendProperties(refExpression, new JTokenExpression(propertyName));
            }

            // creates an expression like: `last(split(<resource id>, '/'))`
            LanguageExpression NameFromIdExpression(LanguageExpression idExpression) => new FunctionExpression("last",
                [
                    new FunctionExpression("split",
                        [
                            idExpression,
                            new JTokenExpression("/"),
                        ],
                        []),
                ],
                []);

            // The cases for a parameter resource are much simpler and can be handled up front. These do not
            // support symbolic names they are somewhat different from the declared resource case since we just have an
            // ID and type.
            if (resource is ParameterResourceMetadata parameter)
            {
                switch (propertyName)
                {
                    case "id":
                        return GetFullyQualifiedResourceId(parameter);
                    case "type":
                        return new JTokenExpression(resource.TypeReference.FormatType());
                    case "apiVersion":
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new UnreachableException();
                        return new JTokenExpression(apiVersion);
                    case "name":
                        return NameFromIdExpression(GetFullyQualifiedResourceId(parameter));
                    case string when expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess):
                        return new FunctionExpression(
                            "tryGet",
                            [GetReferenceExpression(resource, indexContext, true), new JTokenExpression(propertyName)],
                            []);
                    case "properties":
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return GetReferenceExpression(resource, indexContext, false);
                    default:
                        return AppendProperties(GetReferenceExpression(resource, indexContext, true), new JTokenExpression(propertyName));
                }
            }
            else if (resource is ModuleOutputResourceMetadata output)
            {
                // there are some slight variations if a safe dereference operator was used on the output itself, e.g., `mod.outputs.?myResource.<prop>`
                var shortCircuitableResourceRef = reference.SourceSyntax is AccessExpressionSyntax accessExpression && accessExpression.IsSafeAccess;
                switch (propertyName)
                {
                    case "id" when shortCircuitableResourceRef:
                        return new FunctionExpression(
                            "tryGet",
                            [
                                AppendProperties(GetModuleReferenceExpression(output.Module, null, true), new JTokenExpression("outputs")),
                                new JTokenExpression(output.OutputName),
                                new JTokenExpression("value"),
                            ],
                            []);
                    case "id":
                        return AppendProperties(
                            GetModuleReferenceExpression(output.Module, null, true),
                            new JTokenExpression("outputs"),
                            new JTokenExpression(output.OutputName),
                            new JTokenExpression("value"));
                    case "type":
                        return new JTokenExpression(resource.TypeReference.FormatType());
                    case "apiVersion":
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new UnreachableException();
                        return new JTokenExpression(apiVersion);
                    case "name" when shortCircuitableResourceRef:
                        // this expression will execute a `reference` expression against the module twice (once to make sure the named output exists, then again to
                        // retrieve the value of that output), but this inefficiency is unavoidable since passing `null` to `split` will cause the deployment to fail
                        return new FunctionExpression("if",
                            [
                                new FunctionExpression("contains",
                                    [
                                        AppendProperties(GetModuleReferenceExpression(output.Module, null, true), new JTokenExpression("outputs")),
                                        new JTokenExpression(output.OutputName),
                                    ],
                                    []),
                                NameFromIdExpression(GetFullyQualifiedResourceId(output)),
                                new FunctionExpression("null", [], []),
                            ],
                            []);
                    case "name":
                        return NameFromIdExpression(GetFullyQualifiedResourceId(output));
                    default:
                        // this would have been blocked by EmitLimitationCalculator
                        throw new InvalidOperationException($"Unsupported module output resource property '{propertyName}'.");
                }
            }
            else if (resource is DeclaredResourceMetadata declaredResource)
            {
                (LanguageExpression guarded, bool wasWrapped) guardOnResourceCondition(LanguageExpression expression)
                {
                    if (safeAccess && expressionBuilder.GetResourceCondition(declaredResource) is { } condition)
                    {
                        // Embed the base expression in a ternary guarded on the resource's condition, falling back to `null`.
                        return (CreateFunction("if", GetConverter(indexContext).ConvertExpression(condition), expression, CreateFunction("null")), true);
                    }

                    return (expression, false);
                }

                LanguageExpression guardAccessOnResourceCondition(FunctionExpression baseExpression, LanguageExpression property)
                {
                    var (guarded, wasWrapped) = guardOnResourceCondition(baseExpression);
                    return safeAccess || wasWrapped
                        ? CreateFunction("tryGet", guarded, property)
                        : AppendProperties(ToFunctionExpression(guarded), property);
                }

                if (context.SemanticModel.Features.ResourceInfoCodegenEnabled &&
                    !context.SemanticModel.SymbolsToInline.ExistingResourcesToInline.Contains(declaredResource.Symbol))
                {
                    // Use simplified "resourceInfo" code generation.

                    LanguageExpression getResourceInfoExpression()
                    {
                        var symbolExpression = GenerateSymbolicReference(declaredResource, indexContext);
                        return guardAccessOnResourceCondition(CreateFunction("resourceInfo", symbolExpression), new JTokenExpression(propertyName));
                    }

                    switch (propertyName)
                    {
                        case AzResourceTypeProvider.ResourceIdPropertyName:
                        case AzResourceTypeProvider.ResourceTypePropertyName:
                        case AzResourceTypeProvider.ResourceApiVersionPropertyName:
                        case AzResourceTypeProvider.ResourceNamePropertyName when declaredResource.Parent is null:
                            return getResourceInfoExpression();
                        case AzResourceTypeProvider.ResourceNamePropertyName:
                            // resourceInfo('foo').name will always return a fully-qualified name, whereas using "foo.name" in Bicep
                            // will give different results depending on whether the resource has a parent (either syntactically, or with the 'parent' property) or not.
                            // We must preserve this behavior by using "last(split(..., '/'))" to convert from qualified -> unqualified, to avoid this being a breaking change.
                            var qualifiedNameExpression = AppendProperties(
                                CreateFunction("resourceInfo", GenerateSymbolicReference(declaredResource, indexContext)),
                                new JTokenExpression(AzResourceTypeProvider.ResourceNamePropertyName));
                            return guardOnResourceCondition(CreateFunction(
                                "last",
                                CreateFunction(
                                    "split",
                                    qualifiedNameExpression,
                                    new JTokenExpression("/")))).guarded;
                    }
                }

                switch (propertyName)
                {
                    case AzResourceTypeProvider.ResourceIdPropertyName:
                        // the ID is dependent on the name expression which could involve locals in case of a resource collection
                        return guardOnResourceCondition(GetFullyQualifiedResourceId(resource)).guarded;
                    case AzResourceTypeProvider.ResourceNamePropertyName:
                        // the name is dependent on the name expression which could involve locals in case of a resource collection

                        // Note that we don't want to return the fully-qualified resource name in the case of name property access.
                        // we should return whatever the user has set as the value of the 'name' property for a predictable user experience.
                        return guardOnResourceCondition(ConvertExpression(declaredResource.NameSyntax)).guarded;
                    case AzResourceTypeProvider.ResourceTypePropertyName:
                        return new JTokenExpression(resource.TypeReference.FormatType());
                    case AzResourceTypeProvider.ResourceApiVersionPropertyName:
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                        return new JTokenExpression(apiVersion);
                    case "properties" when safeAccess:
                        return guardAccessOnResourceCondition(
                            GetReferenceExpression(resource, indexContext, true),
                            new JTokenExpression(propertyName));
                    case "properties":
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return GetReferenceExpression(resource, indexContext, false);
                    default:
                        return guardAccessOnResourceCondition(GetReferenceExpression(resource, indexContext, true), new JTokenExpression(propertyName));
                }
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource.GetType()}");
            }
        }

        private LanguageExpression ConvertModulePropertyAccess(
            ModuleReferenceExpression reference,
            PropertyAccessExpression expression,
            bool? isSecureOutput = null)
        {
            LanguageExpression guardOnModuleCondition(LanguageExpression toGuard)
            {
                if (expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess) &&
                    reference.Module.DeclaringModule.TryGetCondition() is { } condition)
                {
                    // Embed the base expression in a ternary guarded on the resource's condition, falling back to `null`.
                    return CreateFunction("if", GetConverter(reference.IndexContext).ConvertExpression(condition), toGuard, CreateFunction("null"));
                }

                return toGuard;
            }

            switch (expression.PropertyName)
            {
                case "name":
                    // the name is dependent on the name expression which could involve locals in case of a resource collection
                    return guardOnModuleCondition(GetModuleNameExpression(reference.Module, reference.IndexContext?.Index));

                case "outputs":
                    // When referencing secure outputs, convert to listOutputsWithSecureValues function
                    if (isSecureOutput ?? reference.Module.TryGetSemanticModel().Unwrap().Outputs.Any(o => o.IsSecure))
                    {
                        var target = context.Settings.EnableSymbolicNames
                            ? GenerateSymbolicReference(reference.Module, reference.IndexContext)
                            : GetFullyQualifiedResourceId(reference.Module, reference.IndexContext?.Index);
                        var apiVersion = new JTokenExpression(EmitConstants.NestedDeploymentResourceApiVersion);
                        return guardOnModuleCondition(CreateFunction(secureOutputsApi, target, apiVersion));
                    }

                    var baseExpression = GetModuleReferenceExpression(reference.Module, reference.IndexContext, false);
                    return expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess)
                        ? new FunctionExpression("tryGet", [guardOnModuleCondition(baseExpression), new JTokenExpression("outputs")], [])
                        : AppendProperties(baseExpression, new JTokenExpression("outputs"));

                default:
                    throw new InvalidOperationException($"Unsupported module property: {expression.PropertyName}");
            }
        }

        public IEnumerable<LanguageExpression> GetResourceNameSegments(DeclaredResourceMetadata resource)
            => GetResourceNameSegments(resource, [.. expressionBuilder.GetResourceNameSyntaxSegments(resource)]);

        public IEnumerable<LanguageExpression> GetResourceNameSegments(DeclaredResourceMetadata resource, ImmutableArray<SyntaxBase> nameSegments)
        {
            // TODO move this into az extension
            var typeReference = resource.TypeReference;
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
            var nameExpression = ConvertExpression(nameSegments.Last());

            var typesAfterProvider = typeReference.TypeSegments.Skip(1).ToImmutableArray();

            if (ancestors.Length > 0)
            {
                var firstAncestorNameLength = typesAfterProvider.Length - ancestors.Length;

                var parentNames = ancestors.SelectMany((x, i) =>
                {
                    var nameExpression = this.ConvertExpression(nameSegments[i]);

                    if (i == 0 && firstAncestorNameLength > 1)
                    {
                        return Enumerable.Range(0, firstAncestorNameLength).Select(
                            (_, i) => AppendProperties(
                                CreateFunction("split", nameExpression, new JTokenExpression("/")),
                                new JTokenExpression(i)));
                    }

                    return nameExpression.AsEnumerable();
                });

                return parentNames.Concat(nameExpression.AsEnumerable());
            }

            if (typesAfterProvider.Length == 1)
            {
                return nameExpression.AsEnumerable();
            }

            return typesAfterProvider.Select(
                (type, i) => AppendProperties(
                    CreateFunction("split", nameExpression, new JTokenExpression("/")),
                    new JTokenExpression(i)));
        }

        public LanguageExpression GetFullyQualifiedResourceName(DeclaredResourceMetadata resource)
        {
            var nameValueSyntax = resource.NameSyntax;

            // For a nested resource we need to compute the name
            var ancestors = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource);
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

            var nameSegments = GetResourceNameSegments(resource);
            // {0}/{1}/{2}....
            var formatString = string.Join("/", nameSegments.Select((_, i) => $"{{{i}}}"));

            return CreateFunction("format", new JTokenExpression(formatString).AsEnumerable().Concat(nameSegments));
        }

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol, Expression? indexExpression)
        {
            // FIXME use resourceInfo function if enabled
            if (moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) is { } nameValueSyntax)
            {
                return ConvertExpression(nameValueSyntax);
            }

            var generatedModuleNameExpression = ExpressionFactory.CreateGeneratedModuleName(moduleSymbol, indexExpression);

            return ConvertExpression(generatedModuleNameExpression);
        }

        public LanguageExpression GetUnqualifiedResourceId(DeclaredResourceMetadata resource)
        {
            return ScopeHelper.FormatUnqualifiedResourceId(
                context,
                this,
                context.SemanticModel.ResourceScopeData[resource],
                resource.TypeReference.FormatType(),
                GetResourceNameSegments(resource));
        }

        public LanguageExpression GetFullyQualifiedResourceId(ResourceMetadata resource)
        {
            if (resource is ParameterResourceMetadata parameter)
            {
                return new FunctionExpression(
                    "parameters",
                    [new JTokenExpression(parameter.Symbol.Name),],
                    []);
            }
            else if (resource is ModuleOutputResourceMetadata output)
            {
                return AppendProperties(
                    GetModuleReferenceExpression(output.Module, null, true),
                    new JTokenExpression("outputs"),
                    new JTokenExpression(output.OutputName),
                    new JTokenExpression("value"));
            }
            else if (resource is DeclaredResourceMetadata declared)
            {
                var nameSegments = GetResourceNameSegments(declared);
                return ScopeHelper.FormatFullyQualifiedResourceId(
                    context,
                    this,
                    context.SemanticModel.ResourceScopeData[declared],
                    resource.TypeReference.FormatType(),
                    nameSegments);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }
        }

        public LanguageExpression GetFullyQualifiedResourceId(ModuleSymbol moduleSymbol, Expression? indexExpression)
        {
            return ScopeHelper.FormatFullyQualifiedResourceId(
                context,
                this,
                context.SemanticModel.ModuleScopeData[moduleSymbol],
                TemplateWriter.NestedDeploymentResourceType,
                GetModuleNameExpression(moduleSymbol, indexExpression).AsEnumerable());
        }

        public FunctionExpression GetModuleReferenceExpression(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext, bool isModuleOutputResource)
        {
            var isDirectCollectionAccess = !isModuleOutputResource && indexContext == null && moduleSymbol is { IsCollection: true };
            var referenceFunctionName = isDirectCollectionAccess ? "references" : "reference";

            if (context.Settings.EnableSymbolicNames)
            {
                return CreateFunction(
                    referenceFunctionName,
                    GenerateSymbolicReference(moduleSymbol, indexContext));
            }

            if (isDirectCollectionAccess)
            {
                return CreateFunction(
                    referenceFunctionName,
                    new JTokenExpression(moduleSymbol.Name)); // this is the copy name
            }

            return CreateFunction(
                referenceFunctionName,
                GetConverter(indexContext).GetFullyQualifiedResourceId(moduleSymbol, indexContext?.Index),
                new JTokenExpression(EmitConstants.NestedDeploymentResourceApiVersion));
        }

        public FunctionExpression GetReferenceExpression(ResourceMetadata resource, IndexReplacementContext? indexContext, bool full)
        {
            var isCollectionAccess = resource is DeclaredResourceMetadata { Symbol.IsCollection: true } && indexContext == null;
            var referenceExpression = resource switch
            {
                ParameterResourceMetadata parameter => new FunctionExpression(
                    "parameters",
                    [new JTokenExpression(parameter.Symbol.Name),],
                    []),

                ModuleOutputResourceMetadata output => AppendProperties(
                    GetModuleReferenceExpression(output.Module, null, true),
                    new JTokenExpression("outputs"),
                    new JTokenExpression(output.OutputName),
                    new JTokenExpression("value")),

                DeclaredResourceMetadata declared when context.Settings.EnableSymbolicNames =>
                    GenerateSymbolicReference(declared, indexContext),
                DeclaredResourceMetadata declared => declared.Symbol.IsCollection && indexContext == null
                    ? new JTokenExpression(GetSymbolicName(declared)) // this is the copy name
                    : GetFullyQualifiedResourceId(resource),

                _ => throw new InvalidOperationException($"Unexpected resource metadata type: {resource.GetType()}"),
            };

            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return CreateFunction(
                    isCollectionAccess ? "references" : "reference",
                    referenceExpression);
            }

            if (isCollectionAccess)
            {
                return full
                    ? CreateFunction(
                        "references",
                        referenceExpression,
                        new JTokenExpression("full"))
                    : CreateFunction(
                        "references",
                        referenceExpression);
            }

            // full gives access to top-level resource properties, but generates a longer statement
            if (full)
            {
                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");

                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(apiVersion),
                    new JTokenExpression("full"));
            }

            if (!context.Settings.EnableSymbolicNames)
            {
                var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");

                return CreateFunction(
                    "reference",
                    referenceExpression,
                    new JTokenExpression(apiVersion));
            }

            return CreateFunction(
                "reference",
                referenceExpression);
        }

        private LanguageExpression ConvertString(InterpolatedStringExpression expression)
        {
            var formatArgs = new LanguageExpression[expression.Expressions.Length + 1];

            var formatString = StringFormatConverter.BuildFormatString(expression.SegmentValues);
            formatArgs[0] = new JTokenExpression(formatString);

            for (var i = 0; i < expression.Expressions.Length; i++)
            {
                formatArgs[i + 1] = ConvertExpression(expression.Expressions[i]);
            }

            return CreateFunction("format", formatArgs);
        }

        public static FunctionExpression ToFunctionExpression(LanguageExpression converted)
        {
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

        private FunctionExpression ConvertArray(ArrayExpression expression)
        {
            // we are using the createArray() function as a proxy for an array literal
            return CreateFunction(
                "createArray",
                expression.Items.Select(ConvertExpression));
        }

        private FunctionExpression ConvertObject(ObjectExpression expression)
        {
            // need keys and values in one array of parameters
            var parameters = new LanguageExpression[expression.Properties.Count() * 2];

            int index = 0;
            foreach (var property in expression.Properties)
            {
                parameters[index] = property.Key switch
                {
                    StringLiteralExpression @string => new JTokenExpression(@string.Value),
                    InterpolatedStringExpression @string => ConvertString(@string),
                    _ => throw new NotImplementedException($"Encountered an unexpected type '{property.Key.GetType().Name}' when generating object's property name.")
                };
                index++;

                parameters[index] = ConvertExpression(property.Value);
                index++;
            }

            // we are using the createObject() function as a proxy for an object literal
            return GetCreateObjectExpression(parameters);
        }

        private static FunctionExpression GetCreateObjectExpression(params LanguageExpression[] parameters)
            => CreateFunction("createObject", parameters);

        private LanguageExpression ConvertBinary(BinaryExpression binary)
        {
            var operand1 = ConvertExpression(binary.Left);
            var operand2 = ConvertExpression(binary.Right);

            return binary.Operator switch
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
                _ => throw new NotImplementedException($"Cannot emit unexpected binary operator '{binary.Operator}'."),
            };
        }

        private LanguageExpression ConvertUnary(UnaryExpression unary)
        {
            var operand = ConvertExpression(unary.Expression);
            return unary.Operator switch
            {
                UnaryOperator.Not => CreateFunction("not", operand),
                UnaryOperator.Minus => CreateFunction("sub", new JTokenExpression(0), operand),
                _ => throw new NotImplementedException($"Cannot emit unexpected unary operator '{unary.Operator}."),
            };
        }

        public string GetSymbolicName(DeclaredResourceMetadata resource)
            => GetSymbolicName(context.SemanticModel.ResourceAncestors, resource);

        public static string GetSymbolicName(ResourceAncestorGraph resourceAncestorGraph, DeclaredResourceMetadata resource)
        {
            var nestedHierarchy = resourceAncestorGraph.GetAncestors(resource)
                .Reverse()
                .TakeWhile(x => x.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested)
                .Select(x => x.Resource)
                .Reverse()
                .Concat(resource);

            return string.Join("::", nestedHierarchy.Select(x => x.Symbol.Name));
        }

        private LanguageExpression GenerateSymbolicReference(string symbolName, IndexReplacementContext? indexContext)
        {
            if (indexContext is null)
            {
                return new JTokenExpression(symbolName);
            }

            return CreateFunction(
                "format",
                new JTokenExpression($"{symbolName}[{{0}}]"),
                ConvertExpression(indexContext.Index));
        }

        public LanguageExpression GenerateSymbolicReference(DeclaredResourceMetadata resource, IndexReplacementContext? indexContext)
            => GenerateSymbolicReference(GetSymbolicName(resource), indexContext);

        public LanguageExpression GenerateSymbolicReference(ModuleSymbol module, IndexReplacementContext? indexContext)
            => GenerateSymbolicReference(module.Name, indexContext);

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

        /// <summary>
        /// Generates a management group id, using the managementGroup() function. Only suitable for use if the template being generated is targeting the management group scope.
        /// </summary>
        public static LanguageExpression GenerateCurrentManagementGroupId()
            => AppendProperties(
                CreateFunction("managementGroup"),
                new JTokenExpression("id"));

        private static FunctionExpression CreateFunction(string name, params LanguageExpression[] parameters)
            => CreateFunction(name, parameters as IEnumerable<LanguageExpression>);

        private static FunctionExpression CreateFunction(string name, IEnumerable<LanguageExpression> parameters)
            => new(name, [.. parameters], []);

        private static FunctionExpression AppendProperties(FunctionExpression function, params LanguageExpression[] properties)
            => AppendProperties(function, properties as IEnumerable<LanguageExpression>);

        private static FunctionExpression AppendProperties(FunctionExpression function, IEnumerable<LanguageExpression> properties)
            => new(function.Function, function.Parameters, [.. function.Properties, .. properties]);

        private static (string namespaceName, string functionName) GetFunctionName(string potentiallyQualifiedName) => potentiallyQualifiedName.IndexOf('.') switch
        {
            int separatorLocation when separatorLocation > -1 => (potentiallyQualifiedName[..separatorLocation], potentiallyQualifiedName[(separatorLocation + 1)..]),
            _ => (EmitConstants.UserDefinedFunctionsNamespace, potentiallyQualifiedName),
        };
    }
}
