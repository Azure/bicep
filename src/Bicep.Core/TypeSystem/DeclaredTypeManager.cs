// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeManager
    {
        // maps syntax nodes to their declared types
        // processed nodes found not to have a declared type will have a null value
        private readonly ConcurrentDictionary<SyntaxBase, DeclaredTypeAssignment?> declaredTypes = new();
        private readonly ITypeManager typeManager;
        private readonly IBinder binder;

        public DeclaredTypeManager(TypeManager typeManager, IBinder binder)
        {
            this.typeManager = typeManager;
            this.binder = binder;
        }

        public DeclaredTypeAssignment? GetDeclaredTypeAssignment(SyntaxBase syntax) =>
            this.declaredTypes.GetOrAdd(syntax, key => GetTypeAssignment(key));

        public TypeSymbol? GetDeclaredType(SyntaxBase syntax) => this.GetDeclaredTypeAssignment(syntax)?.Reference.Type;

        private DeclaredTypeAssignment? GetTypeAssignment(SyntaxBase syntax)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            switch (syntax)
            {
                case ImportDeclarationSyntax import:
                    return GetImportType(import);

                case ParameterDeclarationSyntax parameter:
                    return GetParameterType(parameter);

                case ResourceDeclarationSyntax resource:
                    return GetResourceType(resource);

                case ModuleDeclarationSyntax module:
                    return GetModuleType(module);

                case VariableAccessSyntax variableAccess:
                    return GetVariableAccessType(variableAccess);

                case OutputDeclarationSyntax output:
                    return GetOutputType(output);

                case TargetScopeSyntax targetScope:
                    return new DeclaredTypeAssignment(targetScope.GetDeclaredType(), targetScope, DeclaredTypeFlags.Constant);

                case IfConditionSyntax ifCondition:
                    return GetIfConditionType(ifCondition);

                case ForSyntax @for:
                    return GetForSyntaxType(@for);

                case PropertyAccessSyntax propertyAccess:
                    return GetPropertyAccessType(propertyAccess);

                case ResourceAccessSyntax resourceAccess:
                    return GetResourceAccessType(resourceAccess);

                case ArrayAccessSyntax arrayAccess:
                    return GetArrayAccessType(arrayAccess);

                case VariableDeclarationSyntax variable:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(variable.Value), variable);

                case LocalVariableSyntax localVariable:
                    return new DeclaredTypeAssignment(this.typeManager.GetTypeInfo(localVariable), localVariable);

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

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax) => new(syntax.GetDeclaredType(), syntax);

        private DeclaredTypeAssignment GetOutputType(OutputDeclarationSyntax syntax) => new(syntax.GetDeclaredType(), syntax);

        private DeclaredTypeAssignment? GetImportType(ImportDeclarationSyntax syntax)
        {
            if (this.binder.GetSymbolInfo(syntax) is ImportedNamespaceSymbol importedNamespace)
            {
                return new(importedNamespace.DeclaredType, syntax);
            }

            return null;
        }

        private DeclaredTypeAssignment GetResourceType(ResourceDeclarationSyntax syntax)
        {
            var declaredResourceType = GetDeclaredResourceType(this.binder, syntax);

            // if the value is a loop (not a condition or object), the type is an array of the declared resource type
            return new DeclaredTypeAssignment(
                syntax.Value is ForSyntax ? new TypedArrayType(declaredResourceType, TypeSymbolValidationFlags.Default) : declaredResourceType,
                syntax);
        }

        private DeclaredTypeAssignment GetModuleType(ModuleDeclarationSyntax syntax)
        {
            var declaredModuleType = syntax.GetDeclaredType(this.binder);
            
            // if the value is a loop (not a condition or object), the type is an array of the declared module type
            return new DeclaredTypeAssignment(
                syntax.Value is ForSyntax ? new TypedArrayType(declaredModuleType, TypeSymbolValidationFlags.Default) : declaredModuleType,
                syntax);
        }

        private DeclaredTypeAssignment? GetVariableAccessType(VariableAccessSyntax syntax)
        {
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

                case BuiltInNamespaceSymbol namespaceSymbol:
                    // the syntax node is referencing a built in namespace - use its type
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

            var body = baseExpressionAssignment?.DeclaringSyntax switch
            {
                ResourceDeclarationSyntax resourceDeclarationSyntax => resourceDeclarationSyntax.TryGetBody(),
                ModuleDeclarationSyntax moduleDeclarationSyntax => moduleDeclarationSyntax.TryGetBody(),
                _ => baseExpressionAssignment?.DeclaringSyntax as ObjectSyntax,
            };
            return GetObjectPropertyType(
                baseExpressionAssignment?.Reference.Type,
                body,
                syntax.PropertyName.IdentifierName,
                useSyntax: true);
        }

        private DeclaredTypeAssignment? GetResourceAccessType(ResourceAccessSyntax syntax)
        {
            if (!syntax.ResourceName.IsValid)
            {
                return null;
            }

            // We should already have a symbol, use its type.
            var symbol = this.binder.GetSymbolInfo(syntax);
            if (symbol == null)
            {
                throw new InvalidOperationException("ResourceAccessSyntax was not assigned a symbol during name binding.");
            }

            if (symbol is ErrorSymbol error)
            {
                return new DeclaredTypeAssignment(ErrorType.Create(error.GetDiagnostics()), syntax);
            }
            else if (symbol is not ResourceSymbol resourceSymbol)
            {
                var baseType = GetDeclaredType(syntax.BaseExpression);
                var typeString = baseType?.Kind.ToString() ?? LanguageConstants.ErrorName;
                return new DeclaredTypeAssignment(ErrorType.Create(DiagnosticBuilder.ForPosition(syntax.ResourceName).ResourceRequiredForResourceAccess(typeString)), syntax);
            }
            else if (IsCycleFree(resourceSymbol))
            {
                // cycle: bail
            }

            // This is a valid nested resource. Return its type.
            return this.GetDeclaredTypeAssignment(((ResourceSymbol)symbol).DeclaringResource.Value);
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
                    var declaringSyntax = baseExpressionAssignment.DeclaringSyntax switch
                    {
                        ForSyntax { Body: ObjectSyntax loopBody } => loopBody,
                        ForSyntax { Body: IfConditionSyntax { Body: ObjectSyntax loopBody } } => loopBody,
                        _ => null
                    };
                    
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

                case ArrayType arrayType:
                    // parent is an if-condition used as a resource/module loop filter
                    // discriminated objects are already resolved by the parent
                    return TryCreateAssignment(arrayType.Item.Type, @object, parentTypeAssignment.Flags);
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

            var parentTypeAssignment = parent switch {
                // variable declared type is calculated using its assigned type, so querying it here causes endless recursion.
                // we can shortcut that by returning any[] here
                VariableDeclarationSyntax var => new DeclaredTypeAssignment(LanguageConstants.Array, var),
                _ => GetDeclaredTypeAssignment(parent),
            };

            if (parentTypeAssignment is null)
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

            // local function
            DeclaredTypeAssignment? ResolveType(ObjectSyntax @object)
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

            return syntax.Body switch
            {
                ObjectSyntax @object => ResolveType(@object),
                IfConditionSyntax { Body: ObjectSyntax @object } => ResolveType(@object),

                // pass the type through
                _ => new DeclaredTypeAssignment(parentType, syntax, parentTypeAssignment.Flags)
            };
        }

        private DeclaredTypeAssignment? GetObjectType(ObjectSyntax syntax)
        {
            var parent = this.binder.GetParent(syntax);
            if (parent is null)
            {
                return null;
            }

            switch (parent)
            {
                case ResourceDeclarationSyntax:
                    if (GetDeclaredTypeAssignment(parent)?.Reference.Type is not ResourceType resourceType)
                    {
                        return null;
                    }

                    // the object literal's parent is a resource declaration, which makes this the body of the resource
                    // the declared type will be the same as the parent
                    return TryCreateAssignment(ResolveDiscriminatedObjects(resourceType.Body.Type, syntax), syntax);

                case ModuleDeclarationSyntax:
                    if (GetDeclaredTypeAssignment(parent)?.Reference.Type is not ModuleType moduleType)
                    {
                        return null;
                    }

                    // the object literal's parent is a module declaration, which makes this the body of the module
                    // the declared type will be the same as the parent
                    return TryCreateAssignment(ResolveDiscriminatedObjects(moduleType.Body.Type, syntax), syntax);

                case IfConditionSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not {} ifParentTypeAssignment)
                    {
                        return null;
                    }

                    // if-condition declared type already resolved discriminators and used the object as the declaring syntax
                    Debug.Assert(ReferenceEquals(syntax, ifParentTypeAssignment.DeclaringSyntax), "ReferenceEquals(syntax,parentTypeAssignment.DeclaringSyntax)");
                    
                    // the declared type will be the same as the parent
                    return ifParentTypeAssignment;

                case ForSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not {} forParentTypeAssignment ||
                        forParentTypeAssignment.Reference.Type is not ArrayType arrayType)
                    {
                        return null;
                    }

                    // the parent is a for-expression
                    // this object is the body of the array, so its declared type is the type of the item
                    // (discriminators have already been resolved when declared type was determined for the for-expression
                    return TryCreateAssignment(arrayType.Item.Type, syntax, forParentTypeAssignment.Flags);

                case ObjectPropertySyntax:
                    if (GetDeclaredTypeAssignment(parent) is not {} objectPropertyAssignment ||
                        objectPropertyAssignment.Reference.Type is not {} objectPropertyParent)
                    {
                        return null;
                    }

                    // the object is the value of a property of another object
                    // use the declared type of the property and propagate the flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(objectPropertyParent, syntax), syntax, objectPropertyAssignment.Flags);

                case ArrayItemSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not {} arrayItemAssignment ||
                        arrayItemAssignment.Reference.Type is not {} arrayParent)
                    {
                        return null;
                    }

                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(arrayParent, syntax), syntax, arrayItemAssignment.Flags);

                case ImportDeclarationSyntax:
                    if (GetDeclaredTypeAssignment(parent) is not {} importAssignment ||
                        importAssignment.Reference.Type  is not NamespaceType namespaceType)
                    {
                        return null;
                    }

                    if (namespaceType.ConfigurationType is null)
                    {
                        // this namespace doesn't support configuration, but it has been provided.
                        // we'll check for this during type assignment.
                        return null;
                    }

                    // the object is an item in an array
                    // use the item's type and propagate flags
                    return TryCreateAssignment(ResolveDiscriminatedObjects(namespaceType.ConfigurationType.Type, syntax), syntax, importAssignment.Flags);
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
            if (type is not DiscriminatedObjectType discriminated)
            {
                // not a discriminated object type - return as-is
                return type;
            }

            var discriminatorProperties = syntax.Properties
                .Where(p => discriminated.TryGetDiscriminatorProperty(p.TryGetKeyText()) is not null)
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
            if (discriminatorProperty.Value is not StringSyntax stringSyntax)
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

        // references to symbols can be involved in cycles
        // we should not try to obtain the declared type for such symbols because we will likely never finish
        private bool IsCycleFree(DeclaredSymbol declaredSymbol) => this.binder.TryGetCycle(declaredSymbol) is null;

        /// <summary>
        /// Returns the declared type of the resource body (based on the type string).
        /// Returns the same value for single resource or resource loops declarations.
        /// </summary>
        /// <param name="resourceTypeProvider">resource type provider</param>
        private static TypeSymbol GetDeclaredResourceType(IBinder binder, ResourceDeclarationSyntax resource)
        {
            var stringSyntax = resource.TypeString;

            if (stringSyntax != null && stringSyntax.IsInterpolated())
            {
                // TODO: in the future, we can relax this check to allow interpolation with compile-time constants.
                // right now, codegen will still generate a format string however, which will cause problems for the type.
                return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).ResourceTypeInterpolationUnsupported());
            }

            var stringContent = stringSyntax?.TryGetLiteralValue();
            if (stringContent == null)
            {
                return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidResourceType());
            }

            // Before we parse the type name we need to determine if it's a top level resource or not.
            ResourceTypeReference? typeReference;
            var hasParentDeclaration = false;
            var nestedParents = binder.GetAllAncestors<ResourceDeclarationSyntax>(resource);
            bool isTopLevelResourceDeclaration = nestedParents.Length == 0;
            if (isTopLevelResourceDeclaration)
            {
                // This is a top level resource - the type is a fully-qualified type.
                typeReference = ResourceTypeReference.TryParse(stringContent);
                if (typeReference == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidResourceType());
                }

                if (binder.GetSymbolInfo(resource) is ResourceSymbol resourceSymbol &&
                    binder.TryGetCycle(resourceSymbol) is null &&
                    resourceSymbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is {} referenceParentSyntax &&
                    binder.GetSymbolInfo(referenceParentSyntax) is ResourceSymbol parentResourceSymbol)
                {
                    hasParentDeclaration = true;

                    var parentType = GetDeclaredResourceType(binder, parentResourceSymbol.DeclaringResource);
                    if (parentType is not ResourceType parentResourceType)
                    {
                        // TODO should we raise an error, or just rely on the error on the parent?
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(referenceParentSyntax).ParentResourceTypeHasErrors(parentResourceSymbol.DeclaringResource.Name.IdentifierName));
                    }

                    if (!parentResourceType.TypeReference.IsParentOf(typeReference))
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(referenceParentSyntax).ResourceTypeIsNotValidParent(
                            typeReference.FullyQualifiedType,
                            parentResourceType.TypeReference.FullyQualifiedType));
                    }
                }
            }
            else
            {
                // This is a nested resource, the type name is a compound of all of the ancestor
                // type names.
                //
                // Ex: 'My.Rp/someType@2020-01-01' -> 'someChild' -> 'someGrandchild'

                // The top-most resource must have a qualified type name.
                hasParentDeclaration = true;
                var baseTypeStringContent = nestedParents[0].TypeString?.TryGetLiteralValue();
                if (baseTypeStringContent == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidAncestorResourceType(nestedParents[0].Name.IdentifierName));
                }

                var baseTypeReference = ResourceTypeReference.TryParse(baseTypeStringContent);
                if (baseTypeReference == null)
                {
                    return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidAncestorResourceType(nestedParents[0].Name.IdentifierName));
                }

                // Collect each other ancestor resource's type.
                var typeSegments = new List<string>();
                for (var i = 1; i < nestedParents.Length; i++)
                {
                    var typeSegmentStringContent = nestedParents[i].TypeString?.TryGetLiteralValue();
                    if (typeSegmentStringContent == null)
                    {
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidAncestorResourceType(nestedParents[i].Name.IdentifierName));
                    }

                    typeSegments.Add(typeSegmentStringContent);
                }

                // Add *this* resource's type
                typeSegments.Add(stringContent);

                // If this fails, let's walk through and find the root cause. This could be confusing
                // for people seeing it the first time.
                typeReference = ResourceTypeReference.TryCombine(baseTypeReference, typeSegments);
                if (typeReference == null)
                {
                    // We'll special case the last one since it refers to *this* resource. We don't
                    // want to cascade a bunch of noisy errors for parents, they get their own errors.
                    for (var j = 0; j < typeSegments.Count - 1; j++)
                    {
                        if (!ResourceTypeReference.TryParseSingleTypeSegment(typeSegments[j], out _, out _))
                        {
                            return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidAncestorResourceType(nestedParents[j+1].Name.IdentifierName));
                        }
                    }

                    if (!ResourceTypeReference.TryParseSingleTypeSegment(stringContent, out _, out _))
                    {
                        // OK this resource is the one that's wrong.
                        return ErrorType.Create(DiagnosticBuilder.ForPosition(resource.Type).InvalidResourceTypeSegment(stringContent));
                    }

                    // Something went wrong, this should be unreachable.
                    throw new InvalidOperationException("Failed to find the root cause of an invalid compound resource type.");
                }
            }

            var flags = ResourceTypeGenerationFlags.None;
            if (resource.IsExistingResource())
            {
                flags |= ResourceTypeGenerationFlags.ExistingResource;
            }

            if(!isTopLevelResourceDeclaration)
            {
                flags |= ResourceTypeGenerationFlags.NestedResource;
            }

            if (typeReference.IsRootType || hasParentDeclaration)
            {
                flags |= ResourceTypeGenerationFlags.PermitLiteralNameProperty;
            }

            return binder.NamespaceResolver.GetResourceType(typeReference, flags);
        }
    }
}
