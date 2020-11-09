// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeManager
    {
        // maps syntax nodes to their declared types
        // processed nodes found not to have a declared type will have a null value
        private readonly IDictionary<SyntaxBase, DeclaredTypeAssignment?> declaredTypes = new Dictionary<SyntaxBase, DeclaredTypeAssignment?>();

        private readonly SyntaxHierarchy hierarchy;
        private readonly ITypeManager typeManager;
        private readonly IResourceTypeProvider resourceTypeProvider;
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly IReadOnlyDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySyntax;
        private readonly ResourceScopeType targetScope;

        public DeclaredTypeManager(SyntaxHierarchy hierarchy, ITypeManager typeManager, IResourceTypeProvider resourceTypeProvider, IReadOnlyDictionary<SyntaxBase, Symbol> bindings, IReadOnlyDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySyntax, ResourceScopeType targetScope)
        {
            this.hierarchy = hierarchy;
            this.typeManager = typeManager;
            this.resourceTypeProvider = resourceTypeProvider;
            this.bindings = bindings;
            this.cyclesBySyntax = cyclesBySyntax;
            this.targetScope = targetScope;
        }

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax)
        {
            if (this.declaredTypes.TryGetValue(syntax, out var assignment))
            {
                // syntax node has already been processed
                // return result as-is (even if null)
                return assignment;
            }

            // the node has not been processed
            // figure out the type
            var newAssignment = GetTypeAssignment(syntax);

            // cache the result
            this.declaredTypes[syntax] = newAssignment;

            return newAssignment;
        }

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax) => this.GetDeclaredTypeAssignment(syntax)?.Reference.Type;

        private DeclaredTypeAssignment? GetTypeAssignment(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case ParameterDeclarationSyntax parameter:
                    return GetParameterType(parameter);

                case ResourceDeclarationSyntax resource:
                    return GetResourceType(resource);

                case ModuleDeclarationSyntax module:
                    return GetModuleType(module);

                case VariableAccessSyntax variableAccess:
                    return GetVariableAccessType(variableAccess);

                case TargetScopeSyntax targetScopeSyntax:
                    return new DeclaredTypeAssignment(targetScopeSyntax.GetDeclaredType(), targetScopeSyntax, DeclaredTypeFlags.Constant);

                case PropertyAccessSyntax propertyAccess:
                    return GetPropertyAccessType(propertyAccess);

                case ArrayAccessSyntax arrayAccess:
                    return GetArrayAccessType(arrayAccess);

                case VariableDeclarationSyntax variable:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(syntax), variable);

                case FunctionCallSyntax _:
                case InstanceFunctionCallSyntax _:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(syntax), declaringSyntax: null);

                case ArraySyntax array:
                    return GetArrayType(array);

                case ArrayItemSyntax arrayItem:
                    return GetArrayItemType(arrayItem);

                case ObjectSyntax @object:
                    return GetObjectType(@object);

                case ObjectPropertySyntax objectProperty:
                    return GetObjectPropertyType(objectProperty);
            }

            return null;
        }

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax) => new DeclaredTypeAssignment(syntax.GetDeclaredType(), syntax);

        private DeclaredTypeAssignment GetResourceType(ResourceDeclarationSyntax syntax) => new DeclaredTypeAssignment(syntax.GetDeclaredType(this.resourceTypeProvider), syntax);

        private DeclaredTypeAssignment GetModuleType(ModuleDeclarationSyntax syntax)
        {
            if (!(bindings.TryGetValue(syntax, out var symbol) && symbol is ModuleSymbol moduleSymbol))
            {
                return new DeclaredTypeAssignment(ErrorType.Empty(), syntax);
            }

            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var failureDiagnostic))
            {
                return new DeclaredTypeAssignment(ErrorType.Create(failureDiagnostic), syntax);
            }

            return new DeclaredTypeAssignment(syntax.GetDeclaredType(targetScope, moduleSemanticModel), syntax);
        }

        private DeclaredTypeAssignment? GetVariableAccessType(VariableAccessSyntax syntax)
        {
            // references to symbols can be involved in cycles
            // we should not try to obtain the declared type for such symbols because we will likely never finish
            bool IsCycleFree(DeclaredSymbol declaredSymbol) => !this.cyclesBySyntax.ContainsKey(declaredSymbol);

            // because all variable access nodes are normally bound to something, this should always return true
            // (if not, the following code handles that gracefully)
            this.bindings.TryGetValue(syntax, out var symbol);
            
            switch (symbol)
            {
                case ResourceSymbol resourceSymbol when IsCycleFree(resourceSymbol):
                    // the declared type of the body is more useful to us than the declared type of the resource itself
                    return this.GetDeclaredTypeAssignment(resourceSymbol.DeclaringResource.Body);

                case ModuleSymbol moduleSymbol when IsCycleFree(moduleSymbol):
                    // the declared type of the body is more useful to us than the declared type of the module itself
                    return this.GetDeclaredTypeAssignment(moduleSymbol.DeclaringModule.Body);

                case DeclaredSymbol declaredSymbol when IsCycleFree(declaredSymbol):
                    // the syntax node is referencing a declared symbol
                    // use its declared type
                    return this.GetDeclaredTypeAssignment(declaredSymbol.DeclaringSyntax);

                case NamespaceSymbol namespaceSymbol:
                    // the syntax node is referencing a namespace - use its type
                    return new DeclaredTypeAssignment(namespaceSymbol.Type, declaringSyntax: null);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetPropertyAccessType(PropertyAccessSyntax syntax)
        {
            if (!syntax.PropertyName.IsValid)
            {
                return null;
            }

            var baseExpressionAssignment = GetDeclaredTypeAssignment(syntax.BaseExpression);
            
            // it's ok to rely on useSyntax=true because those types have already been established
            return GetObjectPropertyType(
                baseExpressionAssignment?.Reference.Type,
                baseExpressionAssignment?.DeclaringSyntax as ObjectSyntax,
                syntax.PropertyName.IdentifierName,
                useSyntax: true);
        }

        private DeclaredTypeAssignment? GetArrayAccessType(ArrayAccessSyntax syntax)
        {
            var baseExpressionAssignment = GetDeclaredTypeAssignment(syntax.BaseExpression);
            var indexAssignedType = this.typeManager.GetTypeInfo(syntax.IndexExpression);

            // TODO: Currently array access is broken with discriminated object types - revisit when that is fixed
            switch (baseExpressionAssignment?.Reference.Type)
            {
                case ArrayType arrayType when TypeValidator.AreTypesAssignable(indexAssignedType, LanguageConstants.Int):
                    // we are accessing an array by an expression of a numeric type
                    // return the item type of the array
                    // TODO: We can flow the syntax nodes through declared type assignments if we are able to evaluate the array index - skipping for now
                    return new DeclaredTypeAssignment(arrayType.Item.Type, declaringSyntax: null);

                case ObjectType objectType when syntax.IndexExpression is StringSyntax potentialLiteralValue && potentialLiteralValue.TryGetLiteralValue() is { } propertyName:
                    // string literal indexing over an object is the same as dot property access
                    // it's ok to rely on useSyntax=true because those types have already been established
                    return this.GetObjectPropertyType(
                        objectType,
                        baseExpressionAssignment.DeclaringSyntax as ObjectSyntax,
                        propertyName,
                        useSyntax: true);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetArrayType(ArraySyntax syntax)
        {
            var parent = this.hierarchy.GetParent(syntax);

            // we are only handling paths in the AST that are going to produce a declared type
            // arrays can exist under a variable declaration, but variables don't have declared types,
            // so we don't need to check that case
            if (parent is ObjectPropertySyntax)
            {
                // this array is a value of the property
                // the declared type should be the same as the array and we should propagate the flags
                return GetDeclaredTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetArrayItemType(ArrayItemSyntax syntax)
        {
            var parent = this.hierarchy.GetParent(syntax);
            switch (parent)
            {
                case ArraySyntax _:
                    // array items can only have array parents
                    // use the declared item type
                    var parentType = GetDeclaredTypeAssignment(parent)?.Reference.Type;
                    if (parentType is ArrayType arrayType)
                    {
                        return new DeclaredTypeAssignment(arrayType.Item.Type, syntax);
                    }

                    break;
            }

            return null;
        }

        private DeclaredTypeAssignment? GetObjectType(ObjectSyntax syntax)
        {
            // local function
            DeclaredTypeAssignment? CreateAssignment(ITypeReference? typeRef, DeclaredTypeFlags flags = DeclaredTypeFlags.None) => typeRef == null
                ? null
                : new DeclaredTypeAssignment(typeRef, syntax, flags);

            var parent = this.hierarchy.GetParent(syntax);
            if (parent == null)
            {
                return null;
            }

            var parentTypeAssignment = GetDeclaredTypeAssignment(parent);
            if (parentTypeAssignment == null)
            {
                return null;
            }

            var parentType = parentTypeAssignment.Reference.Type;

            switch (parent)
            {
                case ResourceDeclarationSyntax _ when parentType is ResourceType resourceType:
                    // the object literal's parent is a resource declaration, which makes this the body of the resource
                    // the declared type will be the same as the parent
                    return CreateAssignment(ResolveDiscriminatedObjects(resourceType.Body.Type, syntax));

                case ModuleDeclarationSyntax _ when parentType is ModuleType moduleType:
                    // the object literal's parent is a module declaration, which makes this the body of the module
                    // the declared type will be the same as the parent
                    return CreateAssignment(ResolveDiscriminatedObjects(moduleType.Body.Type, syntax));

                case ParameterDeclarationSyntax parameterDeclaration when ReferenceEquals(parameterDeclaration.Modifier, syntax):
                    // the object is a modifier of a parameter type
                    // the declared type should be the appropriate modifier type
                    // however we need the parameter's assigned type to determine the modifier type
                    var parameterAssignedType = parameterDeclaration.GetAssignedType(this.typeManager);
                    return CreateAssignment(LanguageConstants.CreateParameterModifierType(parentType, parameterAssignedType));

                case ObjectPropertySyntax _:
                    // the object is the value of a property of another object
                    // use the declared type of the property and propagate the flags
                    return CreateAssignment(ResolveDiscriminatedObjects(parentType, syntax), parentTypeAssignment.Flags);

                case ArrayItemSyntax _:
                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return CreateAssignment(ResolveDiscriminatedObjects(parentType, syntax), parentTypeAssignment.Flags);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetObjectPropertyType(ObjectPropertySyntax syntax)
        {
            var propertyName = syntax.TryGetKeyText();
            var parent = this.hierarchy.GetParent(syntax);
            if (propertyName == null || !(parent is ObjectSyntax parentObject))
            {
                // the property name is an interpolated string (expression) OR the parent is missing OR the parent is not ObjectSyntax
                // cannot establish declared type
                // TODO: Improve this when we have constant folding
                return null;
            }

            var assignment = GetDeclaredTypeAssignment(parent);

            // we are in the process of establishing the declared type for the syntax nodes,
            // so we must set useSyntax to false to avoid a stack overflow
            return GetObjectPropertyType(assignment?.Reference.Type, parentObject, propertyName, useSyntax: false);
        }

        private DeclaredTypeAssignment? GetObjectPropertyType(TypeSymbol? type, ObjectSyntax? objectSyntax, string propertyName, bool useSyntax)
        {
            // local function
            DeclaredTypeFlags ConvertFlags(TypePropertyFlags flags) => flags.HasFlag(TypePropertyFlags.Constant) ? DeclaredTypeFlags.Constant : DeclaredTypeFlags.None;

            // the declared types on the declaration side of things will take advantage of properties
            // set on objects to resolve discriminators at all levels
            // to take advantage of this, we should first try looking up the property's declared type
            var declaringProperty = objectSyntax?.SafeGetPropertyByName(propertyName);
            if (useSyntax && declaringProperty != null)
            {
                // it is important to get the property value's decl type instead of the property's decl type
                // (the property has the unresolved discriminated object type and the value will have it resolved)
                var declaredPropertyAssignment = this.GetDeclaredTypeAssignment(declaringProperty.Value);
                if (declaredPropertyAssignment != null)
                {
                    return declaredPropertyAssignment;
                }
            }

            // could not get the declared type via syntax
            // let's use the type info instead
            switch (type)
            {
                case ObjectType objectType:
                    // lookup declared property
                    if (objectType.Properties.TryGetValue(propertyName, out var property))
                    {
                        return new DeclaredTypeAssignment(property.TypeReference.Type, declaringProperty, ConvertFlags(property.Flags));
                    }

                    // if there are additional properties, try those
                    if (objectType.AdditionalPropertiesType != null)
                    {
                        return new DeclaredTypeAssignment(objectType.AdditionalPropertiesType.Type, declaringProperty, ConvertFlags(objectType.AdditionalPropertiesFlags));
                    }

                    break;

                case DiscriminatedObjectType discriminated:
                    if (string.Equals(propertyName, discriminated.DiscriminatorProperty.Name, LanguageConstants.IdentifierComparison))
                    {
                        // the property is the discriminator property - use its type
                        return new DeclaredTypeAssignment(discriminated.DiscriminatorProperty.TypeReference.Type, declaringProperty);
                    }

                    break;
            }

            return null;
        }

        private static TypeSymbol? ResolveDiscriminatedObjects(TypeSymbol type, ObjectSyntax syntax)
        {
            if (!(type is DiscriminatedObjectType discriminated))
            {
                // not a discriminated object type - return as-is
                return type;
            }

            var discriminatorProperties = syntax.Properties
                .Where(p => string.Equals(p.TryGetKeyText(), discriminated.DiscriminatorKey, LanguageConstants.IdentifierComparison))
                .ToList();

            if (discriminatorProperties.Count != 1)
            {
                // the object has duplicate properties with name matching the discriminator key
                // don't select any of the union members
                return type;
            }

            // calling the type check here would prevent the declared type from being assigned to the property
            // because we haven't yet assigned the declared type to the object
            // for the purposes of resolving the discriminated object, we just need to check if it's a literal string
            // which doesn't require the full type check, so we're fine
            var discriminatorProperty = discriminatorProperties.Single();
            if (!(discriminatorProperty.Value is StringSyntax stringSyntax))
            {
                // the discriminator property value is not a string
                return type;
            }

            var discriminatorValue = stringSyntax.TryGetLiteralValue();
            if (discriminatorValue == null)
            {
                // the string value was interpolated
                return type;
            }

            // discriminator values are stored in the dictionary as bicep literal string text
            // we must escape the literal value to successfully retrieve a match
            var matchingObjectType = discriminated.UnionMembersByKey.TryGetValue(StringUtils.EscapeBicepString(discriminatorValue));

            // return the match if we have it
            return matchingObjectType?.Type;
        }
    }
}
