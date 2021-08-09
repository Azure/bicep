// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public static class LanguageConstants
    {
        public const string LanguageId = "bicep";
        public const string LanguageFileExtension = ".bicep";

        public const string JsonLanguageId = "json";
        public const string JsoncLanguageId = "jsonc";
        public const string ArmTemplateLanguageId = "arm-template"; // Provided by the ARM Tools VSCode extension.

        public const string JsonFileExtension = ".json";
        public const string JsoncFileExtension = ".jsonc";
        public const string ArmTemplateFileExtension = ".arm";

        public const string Build = "build";

        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;
        public const int MaxLiteralCharacterLimit = 131072;

        public const string ErrorName = "<error>";
        public const string MissingName = "<missing>";

        public const string TargetScopeKeyword = "targetScope";
        public const string ParameterKeyword = "param";
        public const string OutputKeyword = "output";
        public const string VariableKeyword = "var";
        public const string ResourceKeyword = "resource";
        public const string ModuleKeyword = "module";
        public const string ExistingKeyword = "existing";

        public const string IfKeyword = "if";
        public const string ForKeyword = "for";
        public const string InKeyword = "in";

        public const string TargetScopeTypeTenant = "tenant";
        public const string TargetScopeTypeManagementGroup = "managementGroup";
        public const string TargetScopeTypeSubscription = "subscription";
        public const string TargetScopeTypeResourceGroup = "resourceGroup";

        public static readonly Regex ArmTemplateSchemaRegex = new(@"https?:\/\/schema\.management\.azure\.com\/schemas\/([^""\/]+\/[a-zA-Z]*[dD]eploymentTemplate\.json)#?");

        public static readonly ImmutableSortedSet<string> DeclarationKeywords = new[] { ParameterKeyword, VariableKeyword, ResourceKeyword, OutputKeyword, ModuleKeyword }.ToImmutableSortedSet(StringComparer.Ordinal);

        public static readonly ImmutableSortedSet<string> ContextualKeywords = DeclarationKeywords
            .Add(TargetScopeKeyword)
            .Add(IfKeyword)
            .Add(ForKeyword)
            .Add(InKeyword);

        public const string TrueKeyword = "true";
        public const string FalseKeyword = "false";
        public const string NullKeyword = "null";

        public static readonly ImmutableDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            [TrueKeyword] = TokenType.TrueKeyword,
            [FalseKeyword] = TokenType.FalseKeyword,
            [NullKeyword] = TokenType.NullKeyword
        }.ToImmutableDictionary();

        public const string ParameterAllowedPropertyName = "allowed";
        public const string ParameterDefaultPropertyName = "default";
        public const string ParameterSecurePropertyName = "secure";

        public const string ModuleParamsPropertyName = "params";
        public const string ModuleOutputsPropertyName = "outputs";

        public const string ResourceIdPropertyName = "id";
        public const string ResourceLocationPropertyName = "location";
        public const string ResourceNamePropertyName = "name";
        public const string ResourceTypePropertyName = "type";
        public const string ResourceApiVersionPropertyName = "apiVersion";
        public const string ResourceScopePropertyName = "scope";
        public const string ResourceParentPropertyName = "parent";
        public const string ResourceDependsOnPropertyName = "dependsOn";
        public const string TypeNameString = "string";
        public const string TypeNameModule = "module";

        /*
         * The following top-level properties must be set deploy-time constant values,
         * and it is safe to read them at deploy-time because their values cannot be changed.
         */
        public static readonly string[] ReadWriteDeployTimeConstantPropertyNames = new[]
        {
            ResourceIdPropertyName,
            ResourceNamePropertyName,
            ResourceTypePropertyName,
            ResourceApiVersionPropertyName,
        };

        /*
         * The following top-level properties must be set deploy-time constant values
         * when declared in resource bodies. However, it is not safe to read their values
         * at deploy-time due to the fact that:
         *   - They can be changed by Policy Modify effect (e.g. tags, sku)
         *   - Their values may be normalized by RPs
         *   - Some RPs are doing Put-as-Patch
         */
        public static readonly string[] WriteOnlyDeployTimeConstantPropertyNames = new[]
        {
            "location",
            "kind",
            "subscriptionId",
            "resourceGroup",
            "managedBy",
            "extendedLocation",
            "zones",
            "plan",
            "sku",
            "identity",
            "managedByExtended",
            "tags",
        };

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public const string StringDelimiter = "'";
        public const string StringHoleOpen = "${";
        public const string StringHoleClose = "}";

        public const string AnyFunction = "any";
        public static readonly TypeSymbol Any = new AnyType();

        public static readonly TypeSymbol ResourceRef = CreateResourceScopeReference(ResourceScope.Module | ResourceScope.Resource);

        // type used for the item type in the dependsOn array type
        public static readonly TypeSymbol ResourceOrResourceCollectionRefItem = UnionType.Create(
            ResourceRef,
            new TypedArrayType(CreateResourceScopeReference(ResourceScope.Module), TypeSymbolValidationFlags.Default),
            new TypedArrayType(CreateResourceScopeReference(ResourceScope.Resource), TypeSymbolValidationFlags.Default));

        // the type of the dependsOn property in module and resource bodies
        public static readonly TypeSymbol ResourceOrResourceCollectionRefArray = new TypedArrayType(ResourceOrResourceCollectionRefItem, TypeSymbolValidationFlags.Default);

        public static readonly TypeSymbol String = new PrimitiveType(TypeNameString, TypeSymbolValidationFlags.Default);
        // LooseString should be regarded as equal to the 'string' type, but with different validation behavior
        public static readonly TypeSymbol LooseString = new PrimitiveType(TypeNameString, TypeSymbolValidationFlags.AllowLooseStringAssignment);
        // SecureString should be regarded as equal to the 'string' type, but with different validation behavior
        public static readonly TypeSymbol SecureString = new PrimitiveType(TypeNameString, TypeSymbolValidationFlags.AllowLooseStringAssignment | TypeSymbolValidationFlags.IsSecure);
        public static readonly TypeSymbol Object = new ObjectType("object", TypeSymbolValidationFlags.Default, Enumerable.Empty<TypeProperty>(), LanguageConstants.Any);
        public static readonly TypeSymbol SecureObject = new ObjectType("object", TypeSymbolValidationFlags.Default | TypeSymbolValidationFlags.IsSecure, Enumerable.Empty<TypeProperty>(), LanguageConstants.Any);
        public static readonly TypeSymbol Int = new PrimitiveType("int", TypeSymbolValidationFlags.Default);
        public static readonly TypeSymbol Bool = new PrimitiveType("bool", TypeSymbolValidationFlags.Default);
        public static readonly TypeSymbol Null = new PrimitiveType(NullKeyword, TypeSymbolValidationFlags.Default);
        public static readonly TypeSymbol Array = new ArrayType("array");
        //Type for available loadTextContent encoding

        public static readonly ImmutableArray<(string name, Encoding encoding)> SupportedEncodings = new[]{
            ("us-ascii", Encoding.ASCII),
            ("iso-8859-1", Encoding.GetEncoding("iso-8859-1")),
            ("utf-8", Encoding.UTF8),
            ("utf-16BE", Encoding.BigEndianUnicode),
            ("utf-16", Encoding.Unicode)
        }.ToImmutableArray();

        public static readonly TypeSymbol LoadTextContentEncodings = UnionType.Create(SupportedEncodings.Select(s => new StringLiteralType(s.name)));

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new ObjectType(nameof(ParameterModifierMetadata), TypeSymbolValidationFlags.Default, CreateParameterModifierMetadataProperties(), Any, TypePropertyFlags.Constant);

        public static readonly TypeSymbol Tags = new ObjectType(nameof(Tags), TypeSymbolValidationFlags.Default, Enumerable.Empty<TypeProperty>(), String, TypePropertyFlags.None);

        // types allowed to use in output and parameter declarations
        public static readonly ImmutableSortedDictionary<string, TypeSymbol> DeclarationTypes = new[] { String, Object, Int, Bool, Array }.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static TypeSymbol? TryGetDeclarationType(string? typeName)
        {
            if (typeName != null && DeclarationTypes.TryGetValue(typeName, out var primitiveType))
            {
                return primitiveType;
            }

            return null;
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierMetadataProperties()
        {
            yield return new TypeProperty("description", String, TypePropertyFlags.Constant);
        }

        public static IEnumerable<TypeProperty> GetCommonResourceProperties(ResourceTypeReference reference)
        {
            yield return new TypeProperty(ResourceIdPropertyName, String, TypePropertyFlags.ReadOnly | TypePropertyFlags.DeployTimeConstant);
            yield return new TypeProperty(ResourceNamePropertyName, String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.LoopVariant);
            yield return new TypeProperty(ResourceTypePropertyName, new StringLiteralType(reference.FullyQualifiedType), TypePropertyFlags.ReadOnly | TypePropertyFlags.DeployTimeConstant);
            yield return new TypeProperty(ResourceApiVersionPropertyName, new StringLiteralType(reference.ApiVersion), TypePropertyFlags.ReadOnly | TypePropertyFlags.DeployTimeConstant);
        }

        public static IEnumerable<string> GetResourceScopeDescriptions(ResourceScope resourceScope)
        {
            if (resourceScope == ResourceScope.None)
            {
                yield return "none";
            }

            if (resourceScope.HasFlag(ResourceScope.Resource))
            {
                yield return "resource";
            }
            if (resourceScope.HasFlag(ResourceScope.Module))
            {
                yield return "module";
            }
            if (resourceScope.HasFlag(ResourceScope.Tenant))
            {
                yield return "tenant";
            }
            if (resourceScope.HasFlag(ResourceScope.ManagementGroup))
            {
                yield return "managementGroup";
            }
            if (resourceScope.HasFlag(ResourceScope.Subscription))
            {
                yield return "subscription";
            }
            if (resourceScope.HasFlag(ResourceScope.ResourceGroup))
            {
                yield return "resourceGroup";
            }
        }

        public static ResourceScopeType CreateResourceScopeReference(ResourceScope resourceScope)
        {
            var scopeDescriptions = string.Join(" | ", GetResourceScopeDescriptions(resourceScope));

            return new ResourceScopeType(scopeDescriptions, resourceScope);
        }

        public static TypeSymbol CreateModuleType(IEnumerable<TypeProperty> paramsProperties, IEnumerable<TypeProperty> outputProperties, ResourceScope moduleScope, ResourceScope containingScope, string typeName)
        {
            var paramsType = new ObjectType(ModuleParamsPropertyName, TypeSymbolValidationFlags.Default, paramsProperties, null);
            // If none of the params are reqired, we can allow the 'params' declaration to be omitted entirely
            var paramsRequiredFlag = paramsProperties.Any(x => x.Flags.HasFlag(TypePropertyFlags.Required)) ? TypePropertyFlags.Required : TypePropertyFlags.None;

            var outputsType = new ObjectType(ModuleOutputsPropertyName, TypeSymbolValidationFlags.Default, outputProperties, null);

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant;
            if (moduleScope != containingScope)
            {
                // If the module scope matches the parent scope, we can safely omit the scope property
                scopePropertyFlags |= TypePropertyFlags.Required;
            }

            var moduleBody = new ObjectType(
                typeName,
                TypeSymbolValidationFlags.Default,
                new[]
                {
                    new TypeProperty(ResourceNamePropertyName, LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.LoopVariant),
                    new TypeProperty(ResourceScopePropertyName, CreateResourceScopeReference(moduleScope), scopePropertyFlags),
                    new TypeProperty(ModuleParamsPropertyName, paramsType, paramsRequiredFlag | TypePropertyFlags.WriteOnly),
                    new TypeProperty(ModuleOutputsPropertyName, outputsType, TypePropertyFlags.ReadOnly),
                    new TypeProperty(ResourceDependsOnPropertyName, ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny),
                },
                null);

            return new ModuleType(typeName, moduleScope, moduleBody);
        }

        public static IEnumerable<TypeProperty> CreateResourceProperties(ResourceTypeReference resourceTypeReference)
        {
            /*
             * The following properties are intentionally excluded from this model:
             * - SystemData - this is a read-only property that doesn't belong on PUTs
             * - id - that is not allowed in templates
             * - type - included in resource type on resource declarations
             * - apiVersion - included in resource type on resource declarations
             */

            foreach (var prop in GetCommonResourceProperties(resourceTypeReference))
            {
                yield return prop;
            }

            foreach (var prop in KnownTopLevelResourceProperties())
            {
                yield return prop;
            }
        }

        public static IEnumerable<TypeProperty> KnownTopLevelResourceProperties()
        {
            yield return new TypeProperty("location", String);

            yield return new TypeProperty("tags", Tags);

            yield return new TypeProperty("properties", Object);

            // TODO: Model type fully
            yield return new TypeProperty("sku", Object);

            yield return new TypeProperty("kind", String);
            yield return new TypeProperty("managedBy", String);

            var stringArray = new TypedArrayType(String, TypeSymbolValidationFlags.Default);
            yield return new TypeProperty("managedByExtended", stringArray);

            // TODO: Model type fully
            yield return new TypeProperty("extendedLocation", Object);

            yield return new TypeProperty("zones", stringArray);

            yield return new TypeProperty("plan", Object);

            yield return new TypeProperty("eTag", String);

            // TODO: Model type fully
            yield return new TypeProperty("scale", Object);

            // TODO: Model type fully
            yield return new TypeProperty("identity", Object);

        }
    }
}
