// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
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

        public DeclaredTypeManager(SyntaxHierarchy hierarchy, ITypeManager typeManager, IResourceTypeProvider resourceTypeProvider, IReadOnlyDictionary<SyntaxBase,Symbol> bindings)
        {
            this.hierarchy = hierarchy;
            this.typeManager = typeManager;
            this.resourceTypeProvider = resourceTypeProvider;
            this.bindings = bindings;
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

        private DeclaredTypeAssignment GetParameterType(ParameterDeclarationSyntax syntax) => new DeclaredTypeAssignment(syntax.GetDeclaredType());

        private DeclaredTypeAssignment GetResourceType(ResourceDeclarationSyntax syntax) => new DeclaredTypeAssignment(syntax.GetDeclaredType(this.resourceTypeProvider));

        private DeclaredTypeAssignment GetModuleType(ModuleDeclarationSyntax syntax)
        {
            if (!(bindings.TryGetValue(syntax, out var symbol) && symbol is ModuleSymbol moduleSymbol))
            {
                return new DeclaredTypeAssignment(ErrorType.Empty());
            }

            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out var failureDiagnostic))
            {
                return new DeclaredTypeAssignment(ErrorType.Create(failureDiagnostic));
            }

            return new DeclaredTypeAssignment(syntax.GetDeclaredType(moduleSemanticModel));
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
                return GetDeclaredTypeAssignment(parent);
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
                        return new DeclaredTypeAssignment(arrayType.Item.Type);
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
                : new DeclaredTypeAssignment(typeRef, flags);

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
            // local function
            DeclaredTypeFlags ConvertFlags(TypePropertyFlags flags) => flags.HasFlag(TypePropertyFlags.Constant) ? DeclaredTypeFlags.Constant : DeclaredTypeFlags.None;

            var propertyName = syntax.TryGetKeyText();
            var parent = this.hierarchy.GetParent(syntax);
            if (propertyName == null || parent == null)
            {
                // the property name is an interpolated string (expression) OR the parent is missing
                // cannot establish declared type
                // TODO: Improve this when we have constant folding
                return null;
            }

            var assignment = GetDeclaredTypeAssignment(parent);
            switch (assignment?.Reference.Type)
            {
                case ObjectType objectType:
                    // lookup declared property
                    if (objectType.Properties.TryGetValue(propertyName, out var property))
                    {
                        return new DeclaredTypeAssignment(property.TypeReference.Type, ConvertFlags(property.Flags));
                    }

                    // if there are additional properties, try those
                    if (objectType.AdditionalPropertiesType != null)
                    {
                        return new DeclaredTypeAssignment(objectType.AdditionalPropertiesType.Type, ConvertFlags(objectType.AdditionalPropertiesFlags));
                    }

                    break;

                case DiscriminatedObjectType discriminated:
                    if (string.Equals(propertyName, discriminated.DiscriminatorProperty.Name, LanguageConstants.IdentifierComparison))
                    {
                        // the property is the discriminator property - use its type
                        return new DeclaredTypeAssignment(discriminated.DiscriminatorProperty.TypeReference.Type);
                    }

                    break;
            }

            return null;
        }

        private TypeSymbol? ResolveDiscriminatedObjects(TypeSymbol type, ObjectSyntax syntax)
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
