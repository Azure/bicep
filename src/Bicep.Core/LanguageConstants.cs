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
        public const string ImportKeyword = "import";
        public const string FromKeyword = "from";

        public const string IfKeyword = "if";
        public const string ForKeyword = "for";
        public const string InKeyword = "in";

        public const string TargetScopeTypeTenant = "tenant";
        public const string TargetScopeTypeManagementGroup = "managementGroup";
        public const string TargetScopeTypeSubscription = "subscription";
        public const string TargetScopeTypeResourceGroup = "resourceGroup";

        public const string BicepConfigurationFileName = "bicepconfig.json";

        public const string DisableLinterRuleCommandName = "bicep.DisableLinterRule";

        public const string DisableNextLineDiagnosticsKeyword = "disable-next-line";

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

        // Decorators
        public const string ParameterAllowedPropertyName = "allowed";
        public const string ParameterSecurePropertyName = "secure";
        public const string ParameterMinValuePropertyName = "minValue";
        public const string ParameterMaxValuePropertyName = "maxValue";
        public const string ParameterMinLengthPropertyName = "minLength";
        public const string ParameterMaxLengthPropertyName = "maxLength";
        public const string ParameterMetadataPropertyName = "metadata";
        public const string MetadataDescriptionPropertyName = "description";
        public const string BatchSizePropertyName = "batchSize";

        // module properties
        public const string ModuleParamsPropertyName = "params";
        public const string ModuleOutputsPropertyName = "outputs";
        public const string ModuleNamePropertyName = "name";

        // resource properties
        public const string ResourceScopePropertyName = "scope";
        public const string ResourceParentPropertyName = "parent";
        public const string ResourceDependsOnPropertyName = "dependsOn";

        // types
        public const string TypeNameString = "string";
        public const string TypeNameModule = "module";

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public const string StringDelimiter = "'";
        public const string StringHoleOpen = "${";
        public const string StringHoleClose = "}";

        public const string AnyFunction = "any";
        public static readonly TypeSymbol Any = new AnyType();
        public static readonly TypeSymbol Never = new UnionType("never", ImmutableArray<ITypeReference>.Empty);

        public static readonly TypeSymbol ResourceRef = CreateResourceScopeReference(ResourceScope.Module | ResourceScope.Resource);

        // type used for the item type in the dependsOn array type
        public static readonly TypeSymbol ResourceOrResourceCollectionRefItem = TypeHelper.CreateTypeUnion(
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

        public static readonly TypeSymbol LoadTextContentEncodings = TypeHelper.CreateTypeUnion(SupportedEncodings.Select(s => new StringLiteralType(s.name)));

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new ObjectType(nameof(ParameterModifierMetadata), TypeSymbolValidationFlags.Default, CreateParameterModifierMetadataProperties(), Any, TypePropertyFlags.Constant);

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
                    new TypeProperty(ModuleNamePropertyName, LanguageConstants.String, TypePropertyFlags.Required | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.LoopVariant),
                    new TypeProperty(ResourceScopePropertyName, CreateResourceScopeReference(moduleScope), scopePropertyFlags),
                    new TypeProperty(ModuleParamsPropertyName, paramsType, paramsRequiredFlag | TypePropertyFlags.WriteOnly),
                    new TypeProperty(ModuleOutputsPropertyName, outputsType, TypePropertyFlags.ReadOnly),
                    new TypeProperty(ResourceDependsOnPropertyName, ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny),
                },
                null);

            return new ModuleType(typeName, moduleScope, moduleBody);
        }
    }
}
