// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter : ITemplateWriter
    {
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
        public const string TemplateHashPropertyName = "templateHash";

        // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
        public const string NestedDeploymentResourceApiVersion = "2020-10-01";

        private static readonly ImmutableHashSet<string> ResourcePropertiesToOmit = new[] {
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static readonly ImmutableHashSet<string> ModulePropertiesToOmit = new[] {
            LanguageConstants.ModuleParamsPropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceDependsOnPropertyName,
        }.ToImmutableHashSet();

        private static readonly ImmutableHashSet<string> DecoratorsToEmitAsResourceProperties = new[] {
            LanguageConstants.ParameterSecurePropertyName,
            LanguageConstants.ParameterAllowedPropertyName,
            LanguageConstants.ParameterMinValuePropertyName,
            LanguageConstants.ParameterMaxValuePropertyName,
            LanguageConstants.ParameterMinLengthPropertyName,
            LanguageConstants.ParameterMaxLengthPropertyName,
            LanguageConstants.ParameterMetadataPropertyName,
            LanguageConstants.MetadataDescriptionPropertyName,
            LanguageConstants.ParameterSealedPropertyName,
        }.ToImmutableHashSet();

        private static ISemanticModel GetModuleSemanticModel(ModuleSymbol moduleSymbol)
        {
            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out _))
            {
                // this should have already been checked during type assignment
                throw new InvalidOperationException($"Unable to find referenced compilation for module {moduleSymbol.Name}");
            }

            return moduleSemanticModel;
        }
        private static string GetSchema(ResourceScope targetScope)
        {
            if (targetScope.HasFlag(ResourceScope.Tenant))
            {
                return "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#";
            }

            if (targetScope.HasFlag(ResourceScope.ManagementGroup))
            {
                return "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#";
            }

            if (targetScope.HasFlag(ResourceScope.Subscription))
            {
                return "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#";
            }

            return "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";
        }

        private EmitterContext Context => ExpressionBuilder.Context;
        private ExpressionBuilder ExpressionBuilder { get; }

        public TemplateWriter(SemanticModel semanticModel)
        {
            ExpressionBuilder = new ExpressionBuilder(new EmitterContext(semanticModel));
        }

        public void Write(SourceAwareJsonTextWriter writer)
        {
            // Template is used for calcualting template hash, template jtoken is used for writing to file.
            var (template, templateJToken) = GenerateTemplateWithoutHash(writer.TrackingJsonWriter);
            var templateHash = TemplateHelpers.ComputeTemplateHash(template.ToJToken());
            if (templateJToken.SelectToken(GeneratorMetadataPath) is not JObject generatorObject)
            {
                throw new InvalidOperationException($"generated template doesn't contain a generator object at the path {GeneratorMetadataPath}");
            }
            generatorObject.Add("templateHash", templateHash);
            templateJToken.WriteTo(writer);
            writer.ProcessSourceMap(templateJToken);
        }

        private (Template, JToken) GenerateTemplateWithoutHash(PositionTrackingJsonTextWriter jsonWriter)
        {
            var emitter = new ExpressionEmitter(jsonWriter, this.Context);
            var program = (ProgramExpression)ExpressionBuilder.Convert(Context.SemanticModel.Root.Syntax);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(Context.SemanticModel.TargetScope));

            if (Context.Settings.EnableSymbolicNames)
            {
                emitter.EmitProperty("languageVersion", "1.9-experimental");
            }

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitMetadata(emitter, program.Metadata);

            this.EmitTypeDefinitionsIfPresent(jsonWriter, emitter);

            this.EmitParametersIfPresent(emitter, program.Parameters);

            this.EmitVariablesIfPresent(emitter, program.Variables);

            this.EmitImports(emitter, program.Imports);

            this.EmitResources(jsonWriter, emitter);

            this.EmitOutputsIfPresent(emitter, program.Outputs);

            jsonWriter.WriteEndObject();

            var content = jsonWriter.ToString();
            return (Template.FromJson<Template>(content), content.FromJson<JToken>());
        }

        private void EmitTypeDefinitionsIfPresent(PositionTrackingJsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            if (Context.SemanticModel.Root.TypeDeclarations.Length == 0)
            {
                return;
            }

            jsonWriter.WritePropertyName("definitions");
            jsonWriter.WriteStartObject();

            foreach (var declaredTypeSymbol in Context.SemanticModel.Root.TypeDeclarations)
            {
                jsonWriter.WritePropertyWithPosition(
                    declaredTypeSymbol.DeclaringType,
                    declaredTypeSymbol.Name,
                    () => EmitTypeDeclaration(jsonWriter, declaredTypeSymbol, emitter));
            }

            jsonWriter.WriteEndObject();
        }

        private void EmitParametersIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredParameterExpression> parameters)
        {
            if (!parameters.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("parameters", () => {
                foreach (var parameter in parameters)
                {
                    EmitParameter(emitter, parameter);
                }
            });
        }

        private ObjectExpression AddDecoratorsToBody(DecorableSyntax decorated, ObjectExpression input, TypeSymbol targetType)
        {
            var result = input;
            foreach (var decoratorSyntax in decorated.Decorators.Reverse())
            {
                var symbol = this.Context.SemanticModel.GetSymbolInfo(decoratorSyntax.Expression);

                if (symbol is FunctionSymbol decoratorSymbol &&
                    decoratorSymbol.DeclaringObject is NamespaceType namespaceType &&
                    DecoratorsToEmitAsResourceProperties.Contains(decoratorSymbol.Name))
                {
                    var argumentTypes = decoratorSyntax.Arguments
                        .Select(argument => this.Context.SemanticModel.TypeManager.GetTypeInfo(argument))
                        .ToArray();

                    // There should be exact one matching decorator since there's no errors.
                    var decorator = namespaceType.DecoratorResolver.GetMatches(decoratorSymbol, argumentTypes).Single();

                    var functionCall = ExpressionBuilder.Convert(decoratorSyntax.Expression) as FunctionCallExpression
                        ?? throw new InvalidOperationException($"Failed to convert decorator expression {decoratorSyntax.Expression.GetType()}");

                    var evaluated = decorator.Evaluate(functionCall, targetType, result);
                    if (evaluated is not null)
                    {
                        result = evaluated;
                    }
                }
            }

            return result;
        }

        private void EmitParameter(ExpressionEmitter emitter, DeclaredParameterExpression parameter)
        {
            emitter.EmitObjectProperty(parameter.Name, () =>
            {
                var declaringParameter = parameter.Symbol.DeclaringParameter;
                var parameterObject = TypePropertiesForTypeExpression(declaringParameter.Type);

                if (parameter.DefaultValue is not null)
                {
                    parameterObject = parameterObject.MergeProperty("defaultValue", parameter.DefaultValue);
                }

                parameterObject = AddDecoratorsToBody(declaringParameter, parameterObject, parameter.Symbol.Type);

                foreach (var property in parameterObject.Properties)
                {
                    if (property.TryGetKeyText() is string propertyName)
                    {
                        emitter.EmitProperty(propertyName, property.Value);
                    }
                }
            }, parameter.SourceSyntax);
        }

        private void EmitTypeDeclaration(JsonTextWriter jsonWriter, TypeAliasSymbol declaredTypeSymbol, ExpressionEmitter emitter)
        {
            jsonWriter.WriteStartObject();

            var typeDefinitionObject = TypePropertiesForTypeExpression(declaredTypeSymbol.DeclaringType.Value);

            typeDefinitionObject = AddDecoratorsToBody(declaredTypeSymbol.DeclaringType, typeDefinitionObject, declaredTypeSymbol.Type);

            foreach (var property in typeDefinitionObject.Properties)
            {
                if (property.TryGetKeyText() is string propertyName)
                {
                    emitter.EmitProperty(propertyName, property.Value);
                }
            }

            jsonWriter.WriteEndObject();
        }

        private ObjectExpression TypePropertiesForTypeExpression(SyntaxBase typeExpressionSyntax) => typeExpressionSyntax switch
        {
            VariableAccessSyntax variableAccess => TypePropertiesForUnqualifedReference(variableAccess),
            PropertyAccessSyntax propertyAccess => TypePropertiesForQualifiedReference(propertyAccess),
            ResourceTypeSyntax resourceType => GetTypePropertiesForResourceType(resourceType),
            ArrayTypeSyntax arrayType => GetTypePropertiesForArrayType(arrayType),
            ObjectTypeSyntax objectType => GetTypePropertiesForObjectType(objectType),
            TupleTypeSyntax tupleType => GetTypePropertiesForTupleType(tupleType),
            StringSyntax @string => GetTypePropertiesForStringSyntax(@string),
            IntegerLiteralSyntax integerLiteral => GetTypePropertiesForIntegerLiteralSyntax(integerLiteral),
            BooleanLiteralSyntax booleanLiteral => GetTypePropertiesForBooleanLiteralSyntax(booleanLiteral),
            UnaryOperationSyntax unaryOperation => GetTypePropertiesForUnaryOperationSyntax(unaryOperation),
            UnionTypeSyntax unionType => GetTypePropertiesForUnionTypeSyntax(unionType),
            ParenthesizedExpressionSyntax parenthesizedExpression => TypePropertiesForTypeExpression(parenthesizedExpression.Expression),
            // this should have been caught by the parser
            _ => throw new ArgumentException("Invalid type syntax encountered."),
        };

        private ObjectExpression TypePropertiesForUnqualifedReference(VariableAccessSyntax variableAccess) => Context.SemanticModel.GetSymbolInfo(variableAccess) switch
        {
            AmbientTypeSymbol ambientType => ExpressionFactory.CreateObject(TypeProperty(ambientType.Name).AsEnumerable()),
            TypeAliasSymbol typeAlias => ExpressionFactory.CreateObject(ExpressionFactory.CreateObjectProperty("$ref", ExpressionFactory.CreateStringLiteral($"#/definitions/{typeAlias.Name}")).AsEnumerable()),
            // should have been caught long ago by the type manager
            _ => throw new ArgumentException($"The symbolic name \"{variableAccess.Name.IdentifierName}\" does not refer to a type"),
        };

        private ObjectExpression TypePropertiesForQualifiedReference(PropertyAccessSyntax propertyAccess)
        {
            // The only property access scenario supported at the moment is dereferencing types from a namespace
            if (Context.SemanticModel.GetSymbolInfo(propertyAccess.BaseExpression) is not BuiltInNamespaceSymbol builtInNamespace || builtInNamespace.Type.ProviderName != SystemNamespaceType.BuiltInName)
            {
                throw new ArgumentException("Property access base expression did not resolve to the 'sys' namespace.");
            }

            return ExpressionFactory.CreateObject(TypeProperty(propertyAccess.PropertyName.IdentifierName).AsEnumerable());
        }

        private ObjectPropertyExpression TypeProperty(string typeName)
            => ExpressionFactory.CreateObjectProperty("type", new StringLiteralExpression(null, typeName));

        private ObjectExpression GetTypePropertiesForResourceType(ResourceTypeSyntax syntax)
        {
            var typeString = syntax.TypeString?.TryGetLiteralValue() ?? GetResourceTypeString(syntax);

            return ExpressionFactory.CreateObject(new[]
            {
                TypeProperty(LanguageConstants.TypeNameString),
                ExpressionFactory.CreateObjectProperty(LanguageConstants.ParameterMetadataPropertyName,
                    ExpressionFactory.CreateObject(
                        ExpressionFactory.CreateObjectProperty(LanguageConstants.MetadataResourceTypePropertyName,
                            ExpressionFactory.CreateStringLiteral(typeString)).AsEnumerable())),
            });
        }

        private string GetResourceTypeString(ResourceTypeSyntax syntax)
        {
            if (Context.SemanticModel.GetTypeInfo(syntax) is not ResourceType resourceType)
            {
                // This should have been caught during type checking
                throw new ArgumentException($"Unable to locate resource type.");
            }

            return resourceType.TypeReference.FormatName();
        }

        private ObjectExpression GetTypePropertiesForArrayType(ArrayTypeSyntax syntax)
        {
            var properties = new List<ObjectPropertyExpression> { TypeProperty(LanguageConstants.ArrayType) };

            if (TryGetAllowedValues(syntax.Item.Value) is {} allowedValues)
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("allowedValues", allowedValues));
            }
            else
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("items", TypePropertiesForTypeExpression(syntax.Item.Value)));
            }

            return ExpressionFactory.CreateObject(properties);
        }

        private ArrayExpression? TryGetAllowedValues(SyntaxBase syntax) => syntax switch
        {
            StringSyntax or
            IntegerLiteralSyntax or
            BooleanLiteralSyntax or
            UnaryOperationSyntax or
            NullLiteralSyntax => SingleElementArray(ExpressionBuilder.Convert(syntax)),
            ObjectTypeSyntax objectType when Context.SemanticModel.GetDeclaredType(objectType) is {} type && TypeHelper.IsLiteralType(type) => SingleElementArray(ToLiteralValue(type)),
            TupleTypeSyntax tupleType when Context.SemanticModel.GetDeclaredType(tupleType) is {} type && TypeHelper.IsLiteralType(type) => SingleElementArray(ToLiteralValue(type)),
            UnionTypeSyntax unionType => GetAllowedValuesForUnionType(unionType),
            ParenthesizedExpressionSyntax parenthesized => TryGetAllowedValues(parenthesized.Expression),
            _ => null,
        };

        private ArrayExpression SingleElementArray(Expression expression) => ExpressionFactory.CreateArray(expression.AsEnumerable());

        private ArrayExpression GetAllowedValuesForUnionType(UnionTypeSyntax syntax)
        {
            if (Context.SemanticModel.GetDeclaredType(syntax) is not UnionType unionType)
            {
                // This should have been caught during type checking
                throw new ArgumentException("Invalid union encountered during template serialization");
            }

            return GetAllowedValuesForUnionType(unionType);
        }

        private ArrayExpression GetAllowedValuesForUnionType(UnionType unionType)
            => ExpressionFactory.CreateArray(unionType.Members.Select(ToLiteralValue));

        private ObjectExpression GetTypePropertiesForObjectType(ObjectTypeSyntax syntax)
        {
            var properties = new List<ObjectPropertyExpression> { TypeProperty(LanguageConstants.ObjectType) };
            List<Expression> required = new();
            List<ObjectPropertyExpression> propertySchemata = new();

            foreach (var property in syntax.Properties)
            {
                if (property.TryGetKeyText() is not string keyText)
                {
                    // This should have been caught during type checking
                    throw new ArgumentException("Invalid object type key encountered during serialization.");
                }

                if (property.OptionalityMarker is null)
                {
                    required.Add(ExpressionFactory.CreateStringLiteral(keyText));
                }

                var propertySchema = TypePropertiesForTypeExpression(property.Value);
                propertySchema = AddDecoratorsToBody(property, propertySchema, Context.SemanticModel.GetDeclaredType(property) ?? ErrorType.Empty());

                propertySchemata.Add(ExpressionFactory.CreateObjectProperty(keyText, propertySchema));
            }

            if (required.Any())
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("required", ExpressionFactory.CreateArray(required)));
            }

            if (propertySchemata.Any())
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("properties", ExpressionFactory.CreateObject(propertySchemata)));
            }

            if (syntax.Children.OfType<ObjectTypeAdditionalPropertiesSyntax>().SingleOrDefault() is { } addlPropsType)
            {
                var addlPropertiesSchema = TypePropertiesForTypeExpression(addlPropsType.Value);
                addlPropertiesSchema = AddDecoratorsToBody(addlPropsType, addlPropertiesSchema, Context.SemanticModel.GetDeclaredType(addlPropsType) ?? ErrorType.Empty());

                properties.Add(ExpressionFactory.CreateObjectProperty("additionalProperties", addlPropertiesSchema));
            }

            return ExpressionFactory.CreateObject(properties);
        }

        private ObjectExpression GetTypePropertiesForTupleType(TupleTypeSyntax syntax) => ExpressionFactory.CreateObject(new[]
        {
            TypeProperty(LanguageConstants.ArrayType),
            // TODO uncomment the lines below when ARM w46 has finished rolling out
            // SyntaxFactory.CreateObjectProperty("prefixItems",
            //     SyntaxFactory.CreateArray(syntax.Items.Select(item => AddDecoratorsToBody(
            //         item,
            //         TypePropertiesForTypeExpression(item.Value),
            //         Context.SemanticModel.GetDeclaredType(item) ?? ErrorType.Empty())))),
            // SyntaxFactory.CreateObjectProperty("items", SyntaxFactory.CreateBooleanLiteral(false)),
        });

        private ObjectExpression GetTypePropertiesForStringSyntax(StringSyntax syntax) => ExpressionFactory.CreateObject(new[]
        {
            TypeProperty(LanguageConstants.TypeNameString),
            AllowedValuesForTypeExpression(syntax),
        });

        private ObjectExpression GetTypePropertiesForIntegerLiteralSyntax(IntegerLiteralSyntax syntax) => ExpressionFactory.CreateObject(new[]
        {
            TypeProperty(LanguageConstants.TypeNameInt),
            AllowedValuesForTypeExpression(syntax),
        });

        private ObjectExpression GetTypePropertiesForBooleanLiteralSyntax(BooleanLiteralSyntax syntax) => ExpressionFactory.CreateObject(new[]
        {
            TypeProperty(LanguageConstants.TypeNameBool),
            AllowedValuesForTypeExpression(syntax),
        });

        private ObjectExpression GetTypePropertiesForUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            // Within type syntax, unary operations are only permitted if they are resolvable to a literal type at compile-time
            if (Context.SemanticModel.GetDeclaredType(syntax) is not {} type || !TypeHelper.IsLiteralType(type))
            {
                throw new ArgumentException("Unary operator applied to unresolvable type symbol.");
            }

            return ExpressionFactory.CreateObject(new[]
            {
                TypeProperty(GetNonLiteralTypeName(type)),
                AllowedValuesForTypeExpression(syntax),
            });
        }

        private ObjectExpression GetTypePropertiesForUnionTypeSyntax(UnionTypeSyntax syntax)
        {
            // Union types permit symbolic references, unary operations, and literals, so long as the whole expression embodied in the UnionTypeSyntax can be
            // reduced to a flat union of literal types. If this didn't happen during type checking, the syntax will resolve to an ErrorType instead of a UnionType
            if (Context.SemanticModel.GetDeclaredType(syntax) is not UnionType unionType)
            {
                throw new ArgumentException("Invalid union encountered during template serialization");
            }

            return ExpressionFactory.CreateObject(new[]
            {
                TypeProperty(GetNonLiteralTypeName(unionType.Members.First().Type)),
                ExpressionFactory.CreateObjectProperty("allowedValues", GetAllowedValuesForUnionType(unionType)),
            });
        }

        private ObjectPropertyExpression AllowedValuesForTypeExpression(SyntaxBase syntax) => ExpressionFactory.CreateObjectProperty("allowedValues",
            TryGetAllowedValues(syntax) ?? throw new ArgumentException("Unable to resolve allowed values during template serialization."));

        private Expression ToLiteralValue(ITypeReference literalType) => literalType.Type switch
        {
            StringLiteralType @string => ExpressionFactory.CreateStringLiteral(@string.RawStringValue),
            IntegerLiteralType @int => new IntegerLiteralExpression(null, @int.Value),
            BooleanLiteralType @bool => ExpressionFactory.CreateBooleanLiteral(@bool.Value),
            PrimitiveType pt when pt.Name == LanguageConstants.NullKeyword => new NullLiteralExpression(null),
            ObjectType @object => ExpressionFactory.CreateObject(@object.Properties.Select(kvp => ExpressionFactory.CreateObjectProperty(kvp.Key, ToLiteralValue(kvp.Value.TypeReference)))),
            TupleType tuple => ExpressionFactory.CreateArray(tuple.Items.Select(ToLiteralValue)),
            // This would have been caught by the DeclaredTypeManager during initial type assignment
            _ => throw new ArgumentException("Union types used in ARM type checks must be composed entirely of literal types"),
        };

        private string GetNonLiteralTypeName(TypeSymbol? type) => type switch {
            StringLiteralType => "string",
            IntegerLiteralType => "int",
            BooleanLiteralType => "bool",
            ObjectType => "object",
            ArrayType => "array",
            PrimitiveType pt when pt.Name != LanguageConstants.NullKeyword => pt.Name,
            // This would have been caught by the DeclaredTypeManager during initial type assignment
            _ => throw new ArgumentException("Unresolvable type name"),
        };

        private void EmitVariablesIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredVariableExpression> variables)
        {
            if (!variables.Any() && !Context.FunctionVariables.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("variables", () => {
                //emit internal variables
                foreach (var functionVariable in Context.FunctionVariables.Values.OrderBy(x => x.Name, LanguageConstants.IdentifierComparer))
                {
                    emitter.EmitProperty(functionVariable.Name, functionVariable.Value);
                }

                var loopVariables = variables.Where(x => x is { Value: ForLoopExpression });
                var nonLoopVariables = variables.Where(x => x is { Value: not ForLoopExpression });

                if (loopVariables.Any())
                {
                    // we have variables whose values are loops
                    emitter.EmitArrayProperty("copy", () => {
                        foreach (var loopVariable in loopVariables)
                        {
                            var forLoopVariable = (ForLoopExpression)loopVariable.Value;
                            emitter.EmitCopyObject(loopVariable.Name, forLoopVariable.Expression, forLoopVariable.Body);
                        }
                    });
                }

                // emit non-loop variables
                foreach (var variable in nonLoopVariables)
                {
                    emitter.EmitProperty(variable.Name, variable.Value);
                }                
            });
        }

        private void EmitImports(ExpressionEmitter emitter, ImmutableArray<DeclaredImportExpression> imports)
        {
            if (!imports.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("imports", () => {
                foreach (var import in imports)
                {
                    var settings = import.NamespaceType.Settings;

                    emitter.EmitObjectProperty(import.Name, () => 
                    {
                        emitter.EmitProperty("provider", settings.ArmTemplateProviderName);
                        emitter.EmitProperty("version", settings.ArmTemplateProviderVersion);
                        if (import.Config is not null)
                        {
                            emitter.EmitProperty("config", import.Config);
                        }
                    }, import.SourceSyntax);
                }
            });
        }

        private void EmitResources(PositionTrackingJsonTextWriter jsonWriter, ExpressionEmitter emitter)
        {
            jsonWriter.WritePropertyName("resources");
            if (Context.Settings.EnableSymbolicNames)
            {
                jsonWriter.WriteStartObject();
            }
            else
            {
                jsonWriter.WriteStartArray();
            }

            foreach (var resource in this.Context.SemanticModel.DeclaredResources)
            {
                if (resource.IsExistingResource && !Context.Settings.EnableSymbolicNames)
                {
                    continue;
                }

                if (Context.Settings.EnableSymbolicNames)
                {
                    jsonWriter.WritePropertyName(emitter.GetSymbolicName(resource));
                }

                this.EmitResource(jsonWriter, resource, emitter);
            }

            foreach (var moduleSymbol in this.Context.SemanticModel.Root.ModuleDeclarations)
            {
                if (Context.Settings.EnableSymbolicNames)
                {
                    jsonWriter.WritePropertyName(moduleSymbol.Name);
                }

                this.EmitModule(jsonWriter, moduleSymbol, emitter);
            }

            if (Context.Settings.EnableSymbolicNames)
            {
                jsonWriter.WriteEndObject();
            }
            else
            {
                jsonWriter.WriteEndArray();
            }
        }

        private ulong? GetBatchSize(StatementSyntax statement)
        {
            var decorator = SemanticModelHelper.TryGetDecoratorInNamespace(
                Context.SemanticModel,
                statement,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.BatchSizePropertyName);

            if (decorator?.Arguments is { } arguments
                && arguments.Count() == 1
                && arguments.ToList()[0].Expression is IntegerLiteralSyntax integerLiteral)
            {
                return integerLiteral.Value;
            }
            return null;
        }

        private void EmitResource(PositionTrackingJsonTextWriter jsonWriter, DeclaredResourceMetadata resource, ExpressionEmitter emitter)
        {
            jsonWriter.WriteObjectWithPosition(resource.Symbol.DeclaringResource, () =>
            {
                // Note: conditions STACK with nesting.
                //
                // Children inherit the conditions of their parents, etc. This avoids a problem
                // where we emit a dependsOn to something that's not in the template, or not
                // being evaulated i the template.
                var conditions = new List<SyntaxBase>();
                var loops = new List<(string name, ForSyntax @for, SyntaxBase? input)>();

                var ancestors = this.Context.SemanticModel.ResourceAncestors.GetAncestors(resource);
                foreach (var ancestor in ancestors)
                {
                    if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                        ancestor.Resource.Symbol.DeclaringResource.Value is IfConditionSyntax ifCondition)
                    {
                        conditions.Add(ifCondition.ConditionExpression);
                    }

                    if (ancestor.AncestorType == ResourceAncestorGraph.ResourceAncestorType.Nested &&
                        ancestor.Resource.Symbol.DeclaringResource.Value is ForSyntax @for)
                    {
                        loops.Add((ancestor.Resource.Symbol.Name, @for, null));
                    }
                }

                // Unwrap the 'real' resource body if there's a condition
                var body = resource.Symbol.DeclaringResource.Value;
                switch (body)
                {
                    case IfConditionSyntax ifCondition:
                        body = ifCondition.Body;
                        conditions.Add(ifCondition.ConditionExpression);
                        break;

                    case ForSyntax @for:
                        loops.Add((resource.Symbol.Name, @for, null));
                        if (@for.Body is IfConditionSyntax loopFilter)
                        {
                            body = loopFilter.Body;
                            conditions.Add(loopFilter.ConditionExpression);
                        }
                        else
                        {
                            body = @for.Body;
                        }

                        break;
                }

                if (conditions.Count == 1)
                {
                    emitter.EmitProperty("condition", conditions[0]);
                }
                else if (conditions.Count > 1)
                {
                    var @operator = new BinaryOperationSyntax(
                        conditions[0],
                        SyntaxFactory.CreateToken(TokenType.LogicalAnd),
                        conditions[1]);
                    for (var i = 2; i < conditions.Count; i++)
                    {
                        @operator = new BinaryOperationSyntax(
                            @operator,
                            SyntaxFactory.CreateToken(TokenType.LogicalAnd),
                            conditions[i]);
                    }

                    emitter.EmitProperty("condition", @operator);
                }

                if (loops.Count == 1)
                {
                    var batchSize = GetBatchSize(resource.Symbol.DeclaringResource);
                    emitter.EmitCopyProperty(() => emitter.EmitCopyObject(loops[0].name, loops[0].@for.Expression, loops[0].input, batchSize: batchSize));
                }
                else if (loops.Count > 1)
                {
                    throw new InvalidOperationException("nested loops are not supported");
                }

                if (Context.Settings.EnableSymbolicNames && resource.IsExistingResource)
                {
                    jsonWriter.WritePropertyName("existing");
                    jsonWriter.WriteValue(true);
                }

                var importSymbol = Context.SemanticModel.Root.ImportDeclarations.FirstOrDefault(i => resource.Type.DeclaringNamespace.AliasNameEquals(i.Name));

                if (importSymbol is not null)
                {
                    emitter.EmitProperty("import", importSymbol.Name);
                }

                if (resource.IsAzResource)
                {
                    emitter.EmitProperty("type", resource.TypeReference.FormatType());
                    if (resource.TypeReference.ApiVersion is not null)
                    {
                        emitter.EmitProperty("apiVersion", resource.TypeReference.ApiVersion);
                    }
                }
                else
                {
                    emitter.EmitProperty("type", resource.TypeReference.FormatName());
                }

                if (Context.SemanticModel.EmitLimitationInfo.ResourceScopeData.TryGetValue(resource, out var scopeData))
                {
                    ScopeHelper.EmitResourceScopeProperties(Context.SemanticModel, scopeData, emitter, body);
                }

                if (resource.IsAzResource)
                {
                    emitter.EmitProperty(AzResourceTypeProvider.ResourceNamePropertyName, emitter.GetFullyQualifiedResourceName(resource));
                    emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit.Add(AzResourceTypeProvider.ResourceNamePropertyName));
                }
                else
                {
                    jsonWriter.WritePropertyName("properties");
                    jsonWriter.WriteStartObject();

                    emitter.EmitObjectProperties((ObjectSyntax)body, ResourcePropertiesToOmit);

                    jsonWriter.WriteEndObject();
                }

                this.EmitDependsOn(jsonWriter, resource.Symbol, emitter, body);

                // Since we don't want to be mutating the body of the original ObjectSyntax, we create an placeholder body in place
                // and emit its properties to merge decorator properties.
                foreach (var property in AddDecoratorsToBody(
                    resource.Symbol.DeclaringResource,
                    ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty),
                    resource.Symbol.Type).Properties)
                {
                    emitter.EmitProperty(property);
                }
            });
        }

        private void EmitModuleParameters(JsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            var paramsValue = moduleSymbol.TryGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName);
            if (paramsValue is not ObjectSyntax paramsObjectSyntax)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            jsonWriter.WritePropertyName("parameters");

            jsonWriter.WriteStartObject();

            foreach (var propertySyntax in paramsObjectSyntax.Properties)
            {
                if (!(propertySyntax.TryGetKeyText() is string keyName))
                {
                    // should have been caught by earlier validation
                    throw new ArgumentException("Disallowed interpolation in module parameter");
                }

                // we can't just call EmitObjectProperties here because the ObjectSyntax is flatter than the structure we're generating
                // because nested deployment parameters are objects with a single value property
                jsonWriter.WritePropertyName(keyName);
                if (propertySyntax.Value is ForSyntax @for)
                {
                    jsonWriter.WriteStartObject();
                    // the value is a for-expression
                    // write a single property copy loop
                    emitter.EmitCopyProperty(() =>
                    {
                        jsonWriter.WriteStartArray();
                        emitter.EmitCopyObject("value", @for.Expression, @for.Body, "value");
                        jsonWriter.WriteEndArray();
                    });
                    jsonWriter.WriteEndObject();
                }
                else if (
                    this.Context.SemanticModel.ResourceMetadata.TryLookup(propertySyntax.Value) is { } resourceMetadata &&
                    moduleSymbol.TryGetModuleType() is ModuleType moduleType &&
                    moduleType.TryGetParameterType(keyName) is ResourceParameterType parameterType)
                {
                    jsonWriter.WriteStartObject();
                    // This is a resource being passed into a module, we actually want to pass in its id
                    // rather than the whole resource.
                    emitter.EmitProperty("value", new PropertyAccessSyntax(propertySyntax.Value, SyntaxFactory.DotToken, SyntaxFactory.CreateIdentifier("id")));
                    jsonWriter.WriteEndObject();
                }
                else
                {
                    // the value is not a for-expression - can emit normally
                    emitter.EmitModuleParameterValue(propertySyntax.Value);
                }

            }

            jsonWriter.WriteEndObject();
        }

        private void EmitModule(PositionTrackingJsonTextWriter jsonWriter, ModuleSymbol moduleSymbol, ExpressionEmitter emitter)
        {
            jsonWriter.WriteObjectWithPosition(moduleSymbol.DeclaringModule, () =>
            {
                var body = moduleSymbol.DeclaringModule.Value;
                switch (body)
                {
                    case IfConditionSyntax ifCondition:
                        body = ifCondition.Body;
                        emitter.EmitProperty("condition", ifCondition.ConditionExpression);
                        break;

                    case ForSyntax @for:
                        if (@for.Body is IfConditionSyntax loopFilter)
                        {
                            body = loopFilter.Body;
                            emitter.EmitProperty("condition", loopFilter.ConditionExpression);
                        }
                        else
                        {
                            body = @for.Body;
                        }

                        var batchSize = GetBatchSize(moduleSymbol.DeclaringModule);
                        emitter.EmitCopyProperty(() => emitter.EmitCopyObject(moduleSymbol.Name, @for.Expression, input: null, batchSize: batchSize));
                        break;
                }

                emitter.EmitProperty("type", NestedDeploymentResourceType);
                emitter.EmitProperty("apiVersion", NestedDeploymentResourceApiVersion);

                // emit all properties apart from 'params'. In practice, this currrently only allows 'name', but we may choose to allow other top-level resource properties in future.
                // params requires special handling (see below).
                emitter.EmitObjectProperties((ObjectSyntax)body, ModulePropertiesToOmit);

                var scopeData = Context.ModuleScopeData[moduleSymbol];
                ScopeHelper.EmitModuleScopeProperties(Context.SemanticModel, scopeData, emitter, body);

                if (scopeData.RequestedScope != ResourceScope.ResourceGroup)
                {
                    // if we're deploying to a scope other than resource group, we need to supply a location
                    if (this.Context.SemanticModel.TargetScope == ResourceScope.ResourceGroup)
                    {
                        // the deployment() object at resource group scope does not contain a property named 'location', so we have to use resourceGroup().location
                        emitter.EmitProperty("location", new FunctionExpression(
                            "resourceGroup",
                            Array.Empty<LanguageExpression>(),
                            new LanguageExpression[] { new JTokenExpression("location") }));
                    }
                    else
                    {
                        // at all other scopes we can just use deployment().location
                        emitter.EmitProperty("location", new FunctionExpression(
                            "deployment",
                            Array.Empty<LanguageExpression>(),
                            new LanguageExpression[] { new JTokenExpression("location") }));
                    }
                }

                jsonWriter.WritePropertyName("properties");
                {
                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("expressionEvaluationOptions");
                    {
                        jsonWriter.WriteStartObject();
                        emitter.EmitProperty("scope", "inner");
                        jsonWriter.WriteEndObject();
                    }

                    emitter.EmitProperty("mode", "Incremental");

                    EmitModuleParameters(jsonWriter, moduleSymbol, emitter);

                    var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);

                    // If it is a template spec module, emit templateLink instead of template contents.
                    jsonWriter.WritePropertyName(moduleSemanticModel is TemplateSpecSemanticModel ? "templateLink" : "template");
                    {
                        var moduleWriter = TemplateWriterFactory.CreateTemplateWriter(moduleSemanticModel);
                        var moduleBicepFile = (moduleSemanticModel as SemanticModel)?.SourceFile;
                        var moduleTextWriter = new StringWriter();
                        var moduleJsonWriter = new SourceAwareJsonTextWriter(this.Context.SemanticModel.FileResolver, moduleTextWriter, moduleBicepFile);

                        moduleWriter.Write(moduleJsonWriter);
                        jsonWriter.AddNestedSourceMap(moduleJsonWriter.TrackingJsonWriter);

                        var nestedTemplate = moduleTextWriter.ToString().FromJson<JToken>();
                        nestedTemplate.WriteTo(jsonWriter);
                    }

                    jsonWriter.WriteEndObject();
                }

                this.EmitDependsOn(jsonWriter, moduleSymbol, emitter, body);

                // Since we don't want to be mutating the body of the original ObjectSyntax, we create an placeholder body in place
                // and emit its properties to merge decorator properties.
                foreach (var property in AddDecoratorsToBody(
                    moduleSymbol.DeclaringModule,
                    ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty),
                    moduleSymbol.Type).Properties)
                {
                    emitter.EmitProperty(property);
                }
            });
        }

        private static bool ShouldGenerateDependsOn(ResourceDependency dependency)
        {
            if (dependency.Kind == ResourceDependencyKind.Transitive)
            {
                // transitive dependencies do not have to be emitted
                return false;
            }

            return dependency.Resource switch
            {   // We only want to add a 'dependsOn' for resources being deployed in this file.
                ResourceSymbol resource => !resource.DeclaringResource.IsExistingResource(),
                ModuleSymbol => true,
                _ => throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}"),
            };
        }

        private void EmitSymbolicNameDependsOnEntry(JsonTextWriter jsonWriter, ExpressionEmitter emitter, SyntaxBase newContext, ResourceDependency dependency)
        {
            switch (dependency.Resource)
            {
                case ResourceSymbol resourceDependency:
                    var resource = Context.SemanticModel.ResourceMetadata.TryLookup(resourceDependency.DeclaringSyntax) as DeclaredResourceMetadata ??
                        throw new ArgumentException($"Unable to find resource metadata for dependency '{dependency.Resource.Name}'");

                    switch ((resourceDependency.IsCollection, dependency.IndexExpression))
                    {
                        case (false, _):
                            emitter.EmitSymbolReference(resource);
                            Debug.Assert(dependency.IndexExpression is null);
                            break;
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        case (true, null):
                            emitter.EmitSymbolReference(resource);
                            break;
                        case (true, { } indexExpression):
                            emitter.EmitIndexedSymbolReference(resource, indexExpression, newContext);
                            break;
                    }
                    break;
                case ModuleSymbol moduleDependency:
                    switch ((moduleDependency.IsCollection, dependency.IndexExpression))
                    {
                        case (false, _):
                            jsonWriter.WriteValue(moduleDependency.Name);
                            Debug.Assert(dependency.IndexExpression is null);
                            break;
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        case (true, null):
                            jsonWriter.WriteValue(moduleDependency.Name);
                            break;
                        case (true, { } indexExpression):
                            emitter.EmitIndexedSymbolReference(moduleDependency, indexExpression, newContext);
                            break;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
            }
        }

        private void EmitClassicDependsOnEntry(JsonTextWriter jsonWriter, ExpressionEmitter emitter, SyntaxBase newContext, ResourceDependency dependency)
        {
            switch (dependency.Resource)
            {
                case ResourceSymbol resourceDependency:
                    if (resourceDependency.IsCollection && dependency.IndexExpression == null)
                    {
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        jsonWriter.WriteValue(resourceDependency.DeclaringResource.Name.IdentifierName);

                        break;
                    }

                    var resource = Context.SemanticModel.ResourceMetadata.TryLookup(resourceDependency.DeclaringSyntax) as DeclaredResourceMetadata ??
                        throw new ArgumentException($"Unable to find resource metadata for dependency '{dependency.Resource.Name}'");

                    emitter.EmitResourceIdReference(resource, dependency.IndexExpression, newContext);
                    break;
                case ModuleSymbol moduleDependency:
                    if (moduleDependency.IsCollection && dependency.IndexExpression == null)
                    {
                        // dependency is on the entire module collection
                        // write the name of the module collection as the dependency
                        jsonWriter.WriteValue(moduleDependency.DeclaringModule.Name.IdentifierName);

                        break;
                    }

                    emitter.EmitResourceIdReference(moduleDependency, dependency.IndexExpression, newContext);

                    break;
                default:
                    throw new InvalidOperationException($"Found dependency '{dependency.Resource.Name}' of unexpected type {dependency.GetType()}");
            }
        }

        private void EmitDependsOn(JsonTextWriter jsonWriter, DeclaredSymbol declaredSymbol, ExpressionEmitter emitter, SyntaxBase newContext)
        {
            var dependencies = Context.ResourceDependencies[declaredSymbol]
                .Where(dep => ShouldGenerateDependsOn(dep));

            if (!dependencies.Any())
            {
                return;
            }

            emitter.EmitArrayProperty("dependsOn", () => {
                // need to put dependencies in a deterministic order to generate a deterministic template
                foreach (var dependency in dependencies.OrderBy(x => x.Resource.Name))
                {
                    if (Context.Settings.EnableSymbolicNames)
                    {
                        EmitSymbolicNameDependsOnEntry(jsonWriter, emitter, newContext, dependency);
                    }
                    else
                    {
                        EmitClassicDependsOnEntry(jsonWriter, emitter, newContext, dependency);
                    }
                }
            });
        }

        private void EmitOutputsIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredOutputExpression> outputs)
        {
            if (!outputs.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("outputs", () => {
                foreach (var output in outputs)
                {
                    EmitOutput(emitter, output);
                }
            });
        }

        private void EmitOutput(ExpressionEmitter emitter, DeclaredOutputExpression output)
        {
            emitter.EmitObjectProperty(output.Name, () =>
            {
                var properties = new List<ObjectPropertyExpression>();
                if (output.Symbol.Type is ResourceType resourceType)
                {
                    // Resource-typed outputs are encoded as strings
                    emitter.EmitProperty("type", LanguageConstants.String.Name);

                    properties.Add(
                        ExpressionFactory.CreateObjectProperty(
                            LanguageConstants.ParameterMetadataPropertyName,
                            ExpressionFactory.CreateObject(
                                new [] {
                                    ExpressionFactory.CreateObjectProperty(
                                        LanguageConstants.MetadataResourceTypePropertyName,
                                        ExpressionFactory.CreateStringLiteral(resourceType.TypeReference.FormatName()))
                                })));
                }
                else
                {
                    emitter.EmitProperty("type", output.Symbol.Type.Name);
                }

                if (output.Value is ForLoopExpression @for)
                {
                    emitter.EmitCopyProperty(() => emitter.EmitCopyObject(name: null, @for.Expression, @for.Body));
                }
                else if (output.Symbol.Type is ResourceType)
                {
                    // Resource-typed outputs are serialized using the resource id.
                    var value = new PropertyAccessExpression(output.SourceSyntax, output.Value, "id");

                    emitter.EmitProperty("value", value);
                }
                else
                {
                    emitter.EmitProperty("value", output.Value);
                }

                // emit any decorators on this output
                foreach (var property in AddDecoratorsToBody(
                    output.Symbol.DeclaringOutput,
                    ExpressionFactory.CreateObject(properties),
                    output.Symbol.Type).Properties)
                {
                    emitter.EmitProperty(property);
                }
            }, output.SourceSyntax);
        }

        public void EmitMetadata(ExpressionEmitter emitter, ImmutableArray<DeclaredMetadataExpression> metadata)
        {
            emitter.EmitObjectProperty("metadata", () => {
                if (Context.Settings.EnableSymbolicNames)
                {
                    emitter.EmitProperty("_EXPERIMENTAL_WARNING", "Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!");
                }

                emitter.EmitObjectProperty("_generator", () => {
                    emitter.EmitProperty("name", LanguageConstants.LanguageId);
                    emitter.EmitProperty("version", this.Context.SemanticModel.Features.AssemblyVersion);
                });

                foreach (var item in metadata)
                {
                    emitter.EmitProperty(item.Name, item.Value);
                }
            });
        }
    }
}
