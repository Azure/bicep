// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
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
            : this(context, ImmutableDictionary<LocalVariableSymbol, Expression>.Empty)
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
                        var (namespaceName, functionName) = GetFunctionName(context.ImportClosureInfo.ImportedSymbolNames[importedFunction.Symbol]);
                        return CreateFunction(
                            $"{namespaceName}.{functionName}",
                            importedFunction.Parameters.Select(ConvertExpression));
                    }

                case WildcardImportInstanceFunctionCallExpression importedFunction:
                    {
                        var (namespaceName, functionName) = GetFunctionName(
                            context.ImportClosureInfo.WildcardImportPropertyNames[new(importedFunction.ImportSymbol, importedFunction.MethodName)]);
                        return CreateFunction(
                            $"{namespaceName}.{functionName}",
                            importedFunction.Parameters.Select(ConvertExpression));
                    }

                case ResourceFunctionCallExpression listFunction when listFunction.Name.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix):
                    {
                        var resource = listFunction.Resource.Metadata;
                        var resourceIdExpression = new PropertyAccessExpression(
                            listFunction.Resource.SourceSyntax,
                            listFunction.Resource,
                            "id",
                            AccessExpressionFlags.None);

                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                        var apiVersionExpression = new StringLiteralExpression(listFunction.Resource.SourceSyntax, apiVersion);

                        var listArgs = listFunction.Parameters.Length switch
                        {
                            0 => new Expression[] { resourceIdExpression, apiVersionExpression, },
                            _ => new Expression[] { resourceIdExpression, }.Concat(listFunction.Parameters),
                        };

                        return CreateFunction(
                            listFunction.Name,
                            listArgs.Select(p => ConvertExpression(p)));
                    }

                case AccessChainExpression exp:
                    return ConvertAccessChain(exp);

                case AccessExpression exp:
                    return ConvertAccessExpression(exp, []);

                case ResourceReferenceExpression exp:
                    return GetReferenceExpression(exp.Metadata, exp.IndexContext, true);

                case ModuleReferenceExpression exp:
                    return GetModuleReferenceExpression(exp.Module, exp.IndexContext, false);

                case VariableReferenceExpression exp:
                    return CreateFunction("variables", new JTokenExpression(exp.Variable.Name));

                case SynthesizedVariableReferenceExpression exp:
                    return CreateFunction("variables", new JTokenExpression(exp.Name));

                case ImportedVariableReferenceExpression exp:
                    return CreateFunction("variables", new JTokenExpression(context.ImportClosureInfo.ImportedSymbolNames[exp.Variable]));

                case WildcardImportVariablePropertyReferenceExpression exp:
                    return CreateFunction("variables",
                    new JTokenExpression(context.ImportClosureInfo.WildcardImportPropertyNames[new(exp.ImportSymbol, exp.PropertyName)]));

                case ParametersReferenceExpression exp:
                    return CreateFunction("parameters", new JTokenExpression(exp.Parameter.Name));

                case ParametersAssignmentReferenceExpression exp:
                    return CreateFunction("parameters", new JTokenExpression(exp.Parameter.Name));

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

        private LanguageExpression ConvertAccessChain(AccessChainExpression expression)
            => ConvertAccessExpression(expression.FirstLink, expression.AdditionalProperties.Select(ConvertExpression));

        private LanguageExpression ConvertAccessExpression(AccessExpression expression, IEnumerable<LanguageExpression> additionalProperties)
        {
            var (@base, properties, safeAccess) = ProcessAccessExpression(expression);
            properties = properties.Concat(additionalProperties);

            if (!properties.Any())
            {
                return @base;
            }

            return safeAccess
                ? CreateFunction("tryGet", @base.AsEnumerable().Concat(properties))
                : AppendProperties(ToFunctionExpression(@base), properties);
        }

        private (LanguageExpression @base, IEnumerable<LanguageExpression> properties, bool safeAccess) ProcessAccessExpression(AccessExpression expression)
        {
            var (@base, properties, safeAccess) = ConvertBaseExpression(expression);

            if (expression is ModuleOutputPropertyAccessExpression)
            {
                var isSecureOutput = @base is FunctionExpression functionExpression && functionExpression.Function == secureOutputsApi;

                if (safeAccess)
                {
                    // there are two scenarios we want to handle in this case:
                    //   - conditional outputs (where the accessed property will be omitted from the `outputs` object)
                    //   - outputs with a null value (where the accessed property will be present in the `outputs` object, but its `value` property will be omitted)
                    @base = CreateFunction("tryGet", @base.AsEnumerable().Concat(properties));
                    properties = isSecureOutput ? Enumerable.Empty<LanguageExpression>() : new[] { new JTokenExpression("value") };
                }
                else
                {
                    properties = isSecureOutput ? properties : properties.Append(new JTokenExpression("value"));
                }
            }

            return (@base, properties, safeAccess);
        }

        private (LanguageExpression @base, IEnumerable<LanguageExpression> properties, bool safeAccess) ConvertBaseExpression(AccessExpression expression) => expression.Base switch
        {
            ResourceReferenceExpression resource when expression is PropertyAccessExpression exp => GetConverter(resource.IndexContext).ConvertResourcePropertyAccess(resource, exp),
            ModuleReferenceExpression module when expression is PropertyAccessExpression exp => GetConverter(module.IndexContext).ConvertModulePropertyAccess(module, exp),
            _ => (ConvertExpression(expression.Base), ConvertExpression(expression.Access).AsEnumerable(), expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess)),
        };

        private (LanguageExpression @base, IEnumerable<LanguageExpression> properties, bool safeAccess) ConvertResourcePropertyAccess(ResourceReferenceExpression reference, PropertyAccessExpression expression)
        {
            var resource = reference.Metadata;
            var indexContext = reference.IndexContext;
            var propertyName = expression.PropertyName;
            var safeAccess = expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess);

            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return (GetReferenceExpression(resource, indexContext, false), new[] { new JTokenExpression(propertyName) }, safeAccess);
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
                        return (GetFullyQualifiedResourceId(parameter), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "type":
                        return (new JTokenExpression(resource.TypeReference.FormatType()), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "apiVersion":
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new UnreachableException();
                        return (new JTokenExpression(apiVersion), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "name":
                        return (NameFromIdExpression(GetFullyQualifiedResourceId(parameter)), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "properties" when !safeAccess:
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return (GetReferenceExpression(resource, indexContext, false), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    default:
                        return (GetReferenceExpression(resource, indexContext, true), new[] { new JTokenExpression(propertyName) }, safeAccess);
                }
            }
            else if (resource is ModuleOutputResourceMetadata output)
            {
                // there are some slight variations if a safe dereference operator was used on the output itself, e.g., `mod.outputs.?myResource.<prop>`
                var shortCircuitableResourceRef = reference.SourceSyntax is AccessExpressionSyntax accessExpression && accessExpression.IsSafeAccess;
                switch (propertyName)
                {
                    case "id" when shortCircuitableResourceRef:
                        return (
                            AppendProperties(GetModuleReferenceExpression(output.Module, null, true), new JTokenExpression("outputs")),
                            new LanguageExpression[]
                            {
                                new JTokenExpression(output.OutputName),
                                new JTokenExpression("value"),
                            },
                            true);
                    case "id":
                        return (
                            AppendProperties(
                                GetModuleReferenceExpression(output.Module, null, true),
                                new JTokenExpression("outputs"),
                                new JTokenExpression(output.OutputName),
                                new JTokenExpression("value")),
                            Enumerable.Empty<LanguageExpression>(),
                            safeAccess);
                    case "type":
                        return (new JTokenExpression(resource.TypeReference.FormatType()), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "apiVersion":
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new UnreachableException();
                        return (new JTokenExpression(apiVersion), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "name" when shortCircuitableResourceRef:
                        // this expression will execute a `reference` expression against the module twice (once to make sure the named output exists, then again to
                        // retrieve the value of that output), but this inefficiency is unavoidable since passing `null` to `split` will cause the deployment to fail
                        return (
                            new FunctionExpression("if",
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
                                []),
                            Enumerable.Empty<LanguageExpression>(),
                            true);
                    case "name":
                        return (NameFromIdExpression(GetFullyQualifiedResourceId(output)), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    default:
                        // this would have been blocked by EmitLimitationCalculator
                        throw new InvalidOperationException($"Unsupported module output resource property '{propertyName}'.");
                }
            }
            else if (resource is DeclaredResourceMetadata declaredResource)
            {
                // For symbolic-named resources we have the option of simplifying codegen by emitting expressions like "resourceInfo('symbolicName').id".
                // However, there are numerous cases where resourceInfo can & can't be used, and it's too difficult to try and address them all here.
                // See https://github.com/Azure/bicep/issues/9450 & https://github.com/Azure/bicep/issues/9246 for examples.
                // For now, let's stick with emitting the more verbose expressions that we know work.

                switch (propertyName)
                {
                    case "id":
                        // the ID is dependent on the name expression which could involve locals in case of a resource collection
                        return (GetFullyQualifiedResourceId(resource), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "name":
                        // the name is dependent on the name expression which could involve locals in case of a resource collection

                        // Note that we don't want to return the fully-qualified resource name in the case of name property access.
                        // we should return whatever the user has set as the value of the 'name' property for a predictable user experience.
                        return (ConvertExpression(declaredResource.NameSyntax), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "type":
                        return (new JTokenExpression(resource.TypeReference.FormatType()), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "apiVersion":
                        var apiVersion = resource.TypeReference.ApiVersion ?? throw new InvalidOperationException($"Expected resource type {resource.TypeReference.FormatName()} to contain version");
                        return (new JTokenExpression(apiVersion), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    case "properties" when !safeAccess:
                        // use the reference() overload without "full" to generate a shorter expression
                        // this is dependent on the name expression which could involve locals in case of a resource collection
                        return (GetReferenceExpression(resource, indexContext, false), Enumerable.Empty<LanguageExpression>(), safeAccess);
                    default:
                        return (GetReferenceExpression(resource, indexContext, true), new[] { new JTokenExpression(propertyName) }, safeAccess);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource.GetType()}");
            }
        }

        private (LanguageExpression @base, IEnumerable<LanguageExpression> properties, bool safeAccess) ConvertModulePropertyAccess(ModuleReferenceExpression reference, PropertyAccessExpression expression)
        {
            switch (expression.PropertyName)
            {

                case "name":
                    // the name is dependent on the name expression which could involve locals in case of a resource collection

                    return (GetModuleNameExpression(reference.Module), Enumerable.Empty<LanguageExpression>(), false);

                case "outputs":
                    var moduleSymbol = reference.Module;

                    if (context.SemanticModel.Features.SecureOutputsEnabled &&
                        FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(context.SemanticModel, moduleSymbol.DeclaringModule).Any())
                    {
                        var deploymentResourceId = GetFullyQualifiedResourceId(moduleSymbol);
                        var apiVersion = new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion);
                        return (CreateFunction(secureOutputsApi, deploymentResourceId,apiVersion),
                            Enumerable.Empty<LanguageExpression>(), expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess));
                    }

                    return (GetModuleReferenceExpression(reference.Module, reference.IndexContext, false),
                new[] { new JTokenExpression("outputs") },
                expression.Flags.HasFlag(AccessExpressionFlags.SafeAccess));

                default:
                    throw new InvalidOperationException($"Unsupported module property: {expression.PropertyName}");
            }
        }

        public IEnumerable<LanguageExpression> GetResourceNameSegments(DeclaredResourceMetadata resource)
            => GetResourceNameSegments(resource, expressionBuilder.GetResourceNameSyntaxSegments(resource).ToImmutableArray());

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

        private LanguageExpression GetModuleNameExpression(ModuleSymbol moduleSymbol)
        {
            SyntaxBase nameValueSyntax = GetModuleNameSyntax(moduleSymbol);
            return ConvertExpression(nameValueSyntax);
        }

        public static SyntaxBase GetModuleNameSyntax(ModuleSymbol moduleSymbol)
        {
            // this condition should have already been validated by the type checker
            return moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleNamePropertyName) ?? throw new ArgumentException($"Expected module syntax body to contain property 'name'");
        }

        public LanguageExpression GetUnqualifiedResourceId(DeclaredResourceMetadata resource)
        {
            return ScopeHelper.FormatUnqualifiedResourceId(
                context,
                this,
                context.ResourceScopeData[resource],
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
                    context.ResourceScopeData[declared],
                    resource.TypeReference.FormatType(),
                    nameSegments);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported resource metadata type: {resource}");
            }
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
                GetConverter(indexContext).GetFullyQualifiedResourceId(moduleSymbol),
                new JTokenExpression(TemplateWriter.NestedDeploymentResourceApiVersion));
        }

        public FunctionExpression GetReferenceExpression(ResourceMetadata resource, IndexReplacementContext? indexContext, bool full)
        {
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
                    ? new JTokenExpression(declared.Symbol.Name) // this is the copy name
                    : GetFullyQualifiedResourceId(resource),

                _ => throw new InvalidOperationException($"Unexpected resource metadata type: {resource.GetType()}"),
            };

            if (!resource.IsAzResource)
            {
                // For an extensible resource, always generate a 'reference' statement.
                // User-defined properties appear inside "properties", so use a non-full reference.
                return CreateFunction(
                    "reference",
                    referenceExpression);
            }

            if (resource is DeclaredResourceMetadata { Symbol.IsCollection: true } && indexContext == null)
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

        private SyntaxBase GetEnclosingDeclaringSyntax(LocalVariableSymbol localVariable)
        {
            // we're following the symbol hierarchy rather than syntax hierarchy because
            // this guarantees a single hop in all cases
            var symbolParent = this.context.SemanticModel.GetSymbolParent(localVariable);
            if (symbolParent is not LocalScope localScope)
            {
                throw new NotImplementedException($"{nameof(LocalVariableSymbol)} has un unexpected parent of type '{symbolParent?.GetType().Name}'.");
            }

            return localScope.DeclaringSyntax;
        }

        private ForSyntax GetEnclosingForExpression(LocalVariableSymbol localVariable)
        {
            var declaringSyntax = GetEnclosingDeclaringSyntax(localVariable);

            if (declaringSyntax is ForSyntax @for)
            {
                return @for;
            }

            throw new NotImplementedException($"{nameof(LocalVariableSymbol)} was declared by an unexpected syntax type '{declaringSyntax?.GetType().Name}'.");
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
        {
            var nestedHierarchy = this.context.SemanticModel.ResourceAncestors.GetAncestors(resource)
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
            => new(name, parameters.ToArray(), []);

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
