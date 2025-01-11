// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics
{
    public static class SymbolHelper
    {
        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="binder">the binder </param>
        /// <param name="getDeclaredTypeFunc">lambda to retrieve declared type from syntax base (to avoid cyclic dependencies)</param>
        /// <param name="syntax">the syntax node</param>
        public static Symbol? TryGetSymbolInfo(IBinder binder, Func<SyntaxBase, TypeSymbol?> getDeclaredTypeFunc, SyntaxBase syntax)
        {
            // The decision to pass in getDeclaredTypeFunc as a lambda rather than the ITypeManager interface is deliberate.
            // We should be conscious about not introducing cyclic dependencies, as this code is also used within the type manager, but it only needs declared types.

            static PropertySymbol? GetPropertySymbol(TypeSymbol? baseType, string property, bool useAdditionalPropertiesType)
            {
                if (baseType is null)
                {
                    return null;
                }

                var typeProperty = TypeAssignmentVisitor.UnwrapType(baseType) switch
                {
                    ObjectType x => x.Properties.TryGetValue(property, out var tp) ? tp : null,
                    DiscriminatedObjectType x => x.TryGetDiscriminatorProperty(property),
                    _ => null
                };

                if (typeProperty is null)
                {
                    if (useAdditionalPropertiesType)
                    {
                        return GetAdditionalPropertiesSymbol(baseType);
                    }

                    return null;
                }

                return new PropertySymbol(property, typeProperty.Description, typeProperty.TypeReference.Type);
            }

            static PropertySymbol? GetAdditionalPropertiesSymbol(TypeSymbol? baseType)
            {
                if (baseType is null ||
                    TypeAssignmentVisitor.UnwrapType(baseType) is not ObjectType objectType ||
                    objectType.AdditionalProperties is null ||
                    objectType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.FallbackProperty))
                {
                    return null;
                }

                return new PropertySymbol("*", objectType.AdditionalProperties.Description, objectType.AdditionalProperties.TypeReference.Type);
            }

            switch (syntax)
            {
                case InstanceFunctionCallSyntax ifc:
                    {
                        var baseType = getDeclaredTypeFunc(ifc.BaseExpression);

                        if (baseType is null)
                        {
                            return null;
                        }

                        switch (TypeAssignmentVisitor.UnwrapType(baseType))
                        {
                            case NamespaceType when binder.GetSymbolInfo(ifc.BaseExpression) is WildcardImportSymbol wildcardImport &&
                                wildcardImport.SourceModel.Exports.TryGetValue(ifc.Name.IdentifierName, out var exportMetadata) &&
                                exportMetadata is ExportedFunctionMetadata exportedFunctionMetadata:
                                return new WildcardImportInstanceFunctionSymbol(wildcardImport, ifc.Name.IdentifierName, exportedFunctionMetadata);
                            case NamespaceType namespaceType when binder.GetParent(ifc) is DecoratorSyntax:
                                return namespaceType.DecoratorResolver.TryGetSymbol(ifc.Name);
                            case ObjectType objectType:
                                return objectType.MethodResolver.TryGetSymbol(ifc.Name);
                        }

                        return null;
                    }
                case InstanceParameterizedTypeInstantiationSyntax iptic:
                    return GetPropertySymbol(getDeclaredTypeFunc(iptic.BaseExpression), iptic.PropertyName.IdentifierName, true);
                case PropertyAccessSyntax propertyAccess:
                    {
                        var baseType = getDeclaredTypeFunc(propertyAccess.BaseExpression);
                        var property = propertyAccess.PropertyName.IdentifierName;

                        return GetPropertySymbol(baseType, property, true);
                    }
                case TypePropertyAccessSyntax typePropertyAccess:
                    {
                        var baseType = getDeclaredTypeFunc(typePropertyAccess.BaseExpression);
                        if (baseType is not null)
                        {
                            baseType = TypeAssignmentVisitor.UnwrapType(baseType);
                        }
                        var property = typePropertyAccess.PropertyName.IdentifierName;

                        return GetPropertySymbol(baseType, property, false);
                    }
                case TypeAdditionalPropertiesAccessSyntax addlPropertiesAccess:
                    {
                        var baseType = getDeclaredTypeFunc(addlPropertiesAccess.BaseExpression);
                        if (baseType is not null)
                        {
                            baseType = TypeAssignmentVisitor.UnwrapType(baseType);
                        }
                        return GetAdditionalPropertiesSymbol(baseType);
                    }
                case ObjectPropertySyntax objectProperty:
                    {
                        if (binder.GetParent(objectProperty) is not { } parentSyntax)
                        {
                            return null;
                        }

                        var baseType = getDeclaredTypeFunc(parentSyntax);
                        if (objectProperty.TryGetKeyText() is not { } property)
                        {
                            return null;
                        }

                        return GetPropertySymbol(baseType, property, true);
                    }
                case ObjectTypePropertySyntax objectTypeProperty:
                    {
                        if (objectTypeProperty.TryGetKeyText() is not string propertyName || binder.GetParent(objectTypeProperty) is not SyntaxBase parentSyntax)
                        {
                            return null;
                        }

                        return GetPropertySymbol(getDeclaredTypeFunc(parentSyntax), propertyName, false);
                    }
                case ObjectTypeAdditionalPropertiesSyntax addlProperties:
                    {
                        if (binder.GetParent(addlProperties) is not SyntaxBase parentSyntax)
                        {
                            return null;
                        }

                        return GetAdditionalPropertiesSymbol(getDeclaredTypeFunc(parentSyntax));
                    }
            }

            return binder.GetSymbolInfo(syntax);
        }
    }
}
