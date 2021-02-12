// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeManager
    {
        // maps syntax nodes to their declared types
        // processed nodes found not to have a declared type will have a null value
        private readonly IDictionary<SyntaxBase, DeclaredTypeAssignment?> declaredTypes = new Dictionary<SyntaxBase, DeclaredTypeAssignment?>();

        private readonly IResourceTypeProvider resourceTypeProvider;
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;

        public DeclaredTypeManager(IResourceTypeProvider resourceTypeProvider, TypeManager typeManager, IBinder binder)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.typeManager = typeManager;
            this.binder = binder;
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

                case TargetScopeSyntax targetScope:
                    return new DeclaredTypeAssignment(targetScope.GetDeclaredType(), targetScope, DeclaredTypeFlags.Constant);

                case IfConditionSyntax ifCondition:
                    return GetIfConditionType(ifCondition);

                case ForSyntax @for:
                    return GetForSyntaxType(@for);

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

                case StringSyntax @string:
                    return GetStringType(@string);
            }

            return null;
        }

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax) => new DeclaredTypeAssignment(syntax.GetDeclaredType(), syntax);

        private DeclaredTypeAssignment GetResourceType(ResourceDeclarationSyntax syntax)
        {
            var declaredResourceType = syntax.GetDeclaredType(this.resourceTypeProvider);

            // if the value is a loop (not a condition or object), the type is an array of the declared resource type
            return new DeclaredTypeAssignment(
                syntax.Value is ForSyntax ? new TypedArrayType(declaredResourceType, TypeSymbolValidationFlags.Default) : declaredResourceType,
                syntax);
        }

        private DeclaredTypeAssignment GetModuleType(ModuleDeclarationSyntax syntax)
        {
            if (this.binder.GetSymbolInfo(syntax) is not ModuleSymbol moduleSymbol)
            {
                return new DeclaredTypeAssignment(ErrorType.Empty(), syntax);
            }

            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var failureDiagnostic))
            {
                return new DeclaredTypeAssignment(ErrorType.Create(failureDiagnostic), syntax);
            }

            var declaredModuleType = syntax.GetDeclaredType(this.binder.TargetScope, moduleSemanticModel);
            
            // if the value is a loop (not a condition or object), the type is an array of the declared module type
            return new DeclaredTypeAssignment(
                syntax.Value is ForSyntax ? new TypedArrayType(declaredModuleType, TypeSymbolValidationFlags.Default) : declaredModuleType,
                syntax);
        }

        private DeclaredTypeAssignment? GetVariableAccessType(VariableAccessSyntax syntax)
        {
            // references to symbols can be involved in cycles
            // we should not try to obtain the declared type for such symbols because we will likely never finish
            bool IsCycleFree(DeclaredSymbol declaredSymbol) => this.binder.TryGetCycle(declaredSymbol) is null;

            // because all variable access nodes are normally bound to something, this should always return true
            // (if not, the following code handles that gracefully)
            var symbol = this.binder.GetSymbolInfo(syntax);

            switch (symbol)
            {
                case ResourceSymbol resourceSymbol when IsCycleFree(resourceSymbol):
                    // the declared type of the resource/loop/if body is more useful to us than the declared type of the resource itself
                    var innerResourceBody = resourceSymbol.DeclaringResource.Value;
                    return this.GetDeclaredTypeAssignment(innerResourceBody);

                case ModuleSymbol moduleSymbol when IsCycleFree(moduleSymbol):
                    // the declared type of the module/loop/if body is more useful to us than the declared type of the module itself
                    var innerModuleBody = moduleSymbol.DeclaringModule.Value;
                    return this.GetDeclaredTypeAssignment(innerModuleBody);

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
                    
                    // for regular array we can't evaluate the array index at this point, but for loops the index is irrelevant
                    // and we need to set declaring syntax, so property access can provide completions correctly for resource and module loops
                    var declaringSyntax = baseExpressionAssignment.DeclaringSyntax is ForSyntax {Body: ObjectSyntax loopBody} ? loopBody : null;
                    
                    return new DeclaredTypeAssignment(arrayType.Item.Type, declaringSyntax);

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
            var parent = this.binder.GetParent(syntax);

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

        private DeclaredTypeAssignment? GetStringType(StringSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);

            // we are only handling paths in the AST that are going to produce a declared type
            // strings can exist under a variable declaration, but variables don't have declared types,
            // so we don't need to check that case
            if (parent is ObjectPropertySyntax || parent is ArrayItemSyntax)
            {
                // this string is a value of the property
                // the declared type should be the same as the string and we should propagate the flags
                return GetDeclaredTypeAssignment(parent)?.ReplaceDeclaringSyntax(syntax);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetArrayItemType(ArrayItemSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);
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

        private static DeclaredTypeAssignment? TryCreateAssignment(ITypeReference? typeRef, SyntaxBase declaringSyntax, DeclaredTypeFlags flags = DeclaredTypeFlags.None) => typeRef == null
            ? null
            : new DeclaredTypeAssignment(typeRef, declaringSyntax, flags);

        private DeclaredTypeAssignment? GetIfConditionType(IfConditionSyntax syntax)
        {
            if (syntax.Body is not ObjectSyntax @object)
            {
                // no point to propagate types if body isn't an object
                return null;
            }

            var parent = this.binder.GetParent(syntax);
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
            switch (parentType)
            {
                case ResourceType resourceType:
                    // parent is an if-condition under a resource
                    // use the object as declaring syntax to make property access and variable access code easier
                    return TryCreateAssignment(ResolveDiscriminatedObjects(resourceType.Body.Type, @object), @object, parentTypeAssignment.Flags);

                case ModuleType moduleType:
                    // parent is an if-condition under a module
                    // use the object as declaring syntax to make property access and variable access code easier
                    return TryCreateAssignment(ResolveDiscriminatedObjects(moduleType.Body.Type, @object), @object, parentTypeAssignment.Flags);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetForSyntaxType(ForSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);
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
            
            // a for-loop expressions are semantically valid in places that allow array values
            // for non-array types, there's no need to propagate them further since it won't lead to anything useful
            if (parentType is not ArrayType arrayType)
            {
                return null;
            }

            if (syntax.Body is ObjectSyntax @object)
            {
                // the object may be a discriminated object type - we need to resolve it
                var itemType = arrayType.Item.Type switch
                {
                    ResourceType resourceType => ResolveDiscriminatedObjects(resourceType.Body.Type, @object),

                    ModuleType moduleType => ResolveDiscriminatedObjects(moduleType.Body.Type, @object),

                    _ => ResolveDiscriminatedObjects(arrayType.Item.Type, @object)
                };

                return itemType is null
                    ? null
                    : TryCreateAssignment(new TypedArrayType(itemType, TypeSymbolValidationFlags.Default), syntax, parentTypeAssignment.Flags);
            }

            // pass the type through
            return new DeclaredTypeAssignment(parentType, syntax, parentTypeAssignment.Flags);
        }

        private DeclaredTypeAssignment? GetObjectType(ObjectSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);

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
                case ResourceDeclarationSyntax when parentType is ResourceType resourceType:
                    // the object literal's parent is a resource declaration, which makes this the body of the resource
                    // the declared type will be the same as the parent
                    return TryCreateAssignment(ResolveDiscriminatedObjects(resourceType.Body.Type, syntax), syntax);

                case ModuleDeclarationSyntax when parentType is ModuleType moduleType:
                    // the object literal's parent is a module declaration, which makes this the body of the module
                    // the declared type will be the same as the parent
                    return TryCreateAssignment(ResolveDiscriminatedObjects(moduleType.Body.Type, syntax), syntax);

                case IfConditionSyntax:
                    // if-condition declared type already resolved discriminators and used the object as the declaring syntax
                    Debug.Assert(ReferenceEquals(syntax, parentTypeAssignment.DeclaringSyntax), "ReferenceEquals(syntax,parentTypeAssignment.DeclaringSyntax)");
                    
                    // the declared type will be the same as the parent
                    return parentTypeAssignment;

                case ForSyntax when parentType is ArrayType arrayType:
                    // the parent is a for-expression
                    // this object is the body of the array, so its declared type is the type of the item
                    // (discriminators have already been resolved when declared type was determined for the for-expression
                    return TryCreateAssignment(arrayType.Item.Type, syntax, parentTypeAssignment.Flags);

                case ParameterDeclarationSyntax parameterDeclaration when ReferenceEquals(parameterDeclaration.Modifier, syntax):
                    // the object is a modifier of a parameter type
                    // the declared type should be the appropriate modifier type
                    // however we need the parameter's assigned type to determine the modifier type
                    var parameterAssignedType = parameterDeclaration.GetAssignedType(this.typeManager);
                    return TryCreateAssignment(LanguageConstants.CreateParameterModifierType(parentType, parameterAssignedType), syntax);

                case ObjectPropertySyntax:
                    // the object is the value of a property of another object
                    // use the declared type of the property and propagate the flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(parentType, syntax), syntax, parentTypeAssignment.Flags);

                case ArrayItemSyntax:
                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(parentType, syntax), syntax, parentTypeAssignment.Flags);
            }

            return null;
        }

        private DeclaredTypeAssignment? GetObjectPropertyType(ObjectPropertySyntax syntax)
        {
            var propertyName = syntax.TryGetKeyText();
            var parent = this.binder.GetParent(syntax);
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
