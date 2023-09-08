// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public static class LanguageConstants
    {
        public const string LanguageId = "bicep";
        public const string LanguageFileExtension = ".bicep";

        public static bool IsBicepLanguage(string? languageId) => string.Equals(LanguageId, languageId, StringComparison.OrdinalIgnoreCase);

        public const string ParamsLanguageId = "bicep-params";
        public const string ParamsFileExtension = ".bicepparam";

        public static bool IsParamsLanguage(string? languageId) => string.Equals(ParamsLanguageId, languageId, StringComparison.OrdinalIgnoreCase);

        public static bool IsBicepOrParamsLanguage([NotNullWhen(true)] string? languageId) => IsBicepLanguage(languageId) || IsParamsLanguage(languageId);

        public const string JsonLanguageId = "json";
        public const string JsoncLanguageId = "jsonc";
        public const string ArmTemplateLanguageId = "arm-template"; // Provided by the ARM Tools VSCode extension.

        public const string JsonFileExtension = ".json";
        public const string JsoncFileExtension = ".jsonc";
        public const string ArmTemplateFileExtension = ".arm";

        public const string BicepPublicMcrRegistry = "mcr.microsoft.com";

        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;
        public const int MaxLiteralCharacterLimit = 131072;
        public const int MaxJsonFileCharacterLimit = 1048576; // arbitrary value of 1024*1024 characters.
                                                              // since max ARM template size is 4MB, and it's encoded in UTF that each character can be 1-4 bytes,
                                                              // we can limit maximum size of JSON file loaded to not exceed 1M characters.
                                                              // even though loading files near this limit will make user eventually hit the 4MB limit
                                                              // but it will not be hard to exceed the limit just by loading a single file.


        public const string ErrorName = "<error>";
        public const string MissingName = "<missing>";

        public const string TargetScopeKeyword = "targetScope";
        public const string MetadataKeyword = "metadata";
        public const string TypeKeyword = "type";
        public const string ParameterKeyword = "param";
        public const string UsingKeyword = "using";
        public const string OutputKeyword = "output";
        public const string VariableKeyword = "var";
        public const string ResourceKeyword = "resource";
        public const string ModuleKeyword = "module";
        public const string TestKeyword = "test";
        public const string FunctionKeyword = "func";
        public const string ExistingKeyword = "existing";
        public const string ImportKeyword = "import";
        public const string AssertKeyword = "assert";
        public const string WithKeyword = "with";
        public const string AsKeyword = "as";
        public const string FromKeyword = "from";

        public const string IfKeyword = "if";
        public const string ForKeyword = "for";
        public const string InKeyword = "in";

        public const string ArrayType = "array";
        public const string ObjectType = "object";

        public const string TargetScopeTypeTenant = "tenant";
        public const string TargetScopeTypeManagementGroup = "managementGroup";
        public const string TargetScopeTypeSubscription = "subscription";
        public const string TargetScopeTypeResourceGroup = "resourceGroup";

        public const string CopyLoopIdentifier = "copy";

        public const string BicepConfigurationFileName = "bicepconfig.json";

        public const string DisableNextLineDiagnosticsKeyword = "disable-next-line";

        public static readonly Regex ArmTemplateSchemaRegex = new(@"https?:\/\/schema\.management\.azure\.com\/schemas\/([^""\/]+\/[a-zA-Z]*[dD]eploymentTemplate\.json)#?");

        public static readonly ImmutableSortedSet<string> DeclarationKeywords = ImmutableSortedSet.Create(
            StringComparer.Ordinal,
            new[]
            {
                AssertKeyword,
                ImportKeyword,
                MetadataKeyword,
                ParameterKeyword,
                VariableKeyword,
                ResourceKeyword,
                OutputKeyword,
                ModuleKeyword,
                TypeKeyword
            });

        public static readonly ImmutableSortedSet<string> ContextualKeywords = DeclarationKeywords
            .Add(TargetScopeKeyword)
            .Add(IfKeyword)
            .Add(ForKeyword)
            .Add(InKeyword)
            .Add(FromKeyword);

        public const string TrueKeyword = "true";
        public const string FalseKeyword = "false";
        public const string NullKeyword = "null";

        public const string ListFunctionPrefix = "list";

        public const string McrRepositoryPrefix = "bicep/";

        // https://github.com/opencontainers/image-spec/blob/main/annotations.md
        public const string OciOpenContainerImageDocumentationAnnotation = "org.opencontainers.image.documentation";
        public const string OciOpenContainerImageDescriptionAnnotation = "org.opencontainers.image.description";
        public const string OciOpenContainerImageCreatedAnnotation = "org.opencontainers.image.created";

        public static readonly ImmutableDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            [TrueKeyword] = TokenType.TrueKeyword,
            [FalseKeyword] = TokenType.FalseKeyword,
            [NullKeyword] = TokenType.NullKeyword,
            [WithKeyword] = TokenType.WithKeyword,
            [AsKeyword] = TokenType.AsKeyword,
        }.ToImmutableDictionary();

        // Decorators
        public const string ParameterAllowedPropertyName = "allowed";
        public const string ParameterSecurePropertyName = "secure";
        public const string ParameterMinValuePropertyName = "minValue";
        public const string ParameterMaxValuePropertyName = "maxValue";
        public const string ParameterMinLengthPropertyName = "minLength";
        public const string ParameterMaxLengthPropertyName = "maxLength";
        public const string ParameterMetadataPropertyName = "metadata";
        public const string ParameterSealedPropertyName = "sealed";
        public const string MetadataDescriptionPropertyName = "description";
        public const string MetadataResourceTypePropertyName = "resourceType";
        public const string MetadataExportedPropertyName = "__bicep_export!";
        public const string MetadataImportedFromPropertyName = "__bicep_imported_from!";
        public const string ImportMetadataSourceTemplatePropertyName = "sourceTemplate";
        public const string ImportMetadataOriginalIdentifierPropertyName = "originalIdentifier";
        public const string BatchSizePropertyName = "batchSize";
        public const string ExportPropertyName = "export";
        public const string TypeDiscriminatorDecoratorName = "discriminator";

        // module properties
        public const string ModuleParamsPropertyName = "params";
        public const string ModuleOutputsPropertyName = "outputs";
        public const string ModuleNamePropertyName = "name";

        // test properties
        public const string TestParamsPropertyName = "params";

        // resource properties
        public const string ResourceScopePropertyName = "scope";
        public const string ResourceParentPropertyName = "parent";
        public const string ResourceDependsOnPropertyName = "dependsOn";
        public const string ResourceLocationPropertyName = "location";
        public const string ResourcePropertiesPropertyName = "properties";
        public const string ResourceAssertPropertyName = "asserts";

        // types
        public const string TypeNameString = "string";
        public const string TypeNameBool = "bool";
        public const string TypeNameInt = "int";
        public const string TypeNameModule = "module";
        public const string TypeNameTest = "test";

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public static readonly StringComparer ResourceTypeComparer = StringComparer.OrdinalIgnoreCase;
        public static readonly StringComparison ResourceTypeComparison = StringComparison.OrdinalIgnoreCase;

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

        public static readonly TypeSymbol String = TypeFactory.CreateStringType();
        // LooseString should be regarded as equal to the 'string' type, but with different validation behavior
        public static readonly TypeSymbol LooseString = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment);
        // SecureString should be regarded as equal to the 'string' type, but with different validation behavior
        public static readonly TypeSymbol SecureString = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment | TypeSymbolValidationFlags.IsSecure);
        public static readonly TypeSymbol Object = new ObjectType(ObjectType, TypeSymbolValidationFlags.Default, Enumerable.Empty<TypeProperty>(), LanguageConstants.Any);
        public static readonly TypeSymbol SecureObject = new ObjectType(ObjectType, TypeSymbolValidationFlags.Default | TypeSymbolValidationFlags.IsSecure, Enumerable.Empty<TypeProperty>(), LanguageConstants.Any);
        public static readonly TypeSymbol Int = TypeFactory.CreateIntegerType();
        // LooseInt should be regarded as equal to the 'int' type, but with different validation behavior
        public static readonly TypeSymbol LooseInt = TypeFactory.CreateIntegerType(validationFlags: TypeSymbolValidationFlags.AllowLooseAssignment);
        public static readonly TypeSymbol Bool = TypeFactory.CreateBooleanType();
        // LooseBool should be regarded as equal to the 'bool' type, but with different validation behavior
        public static readonly TypeSymbol LooseBool = TypeFactory.CreateBooleanType(TypeSymbolValidationFlags.AllowLooseAssignment);
        public static readonly TypeSymbol True = TypeFactory.CreateBooleanLiteralType(true);
        public static readonly TypeSymbol False = TypeFactory.CreateBooleanLiteralType(false);
        public static readonly TypeSymbol Null = new NullType();
        public static readonly TypeSymbol Array = TypeFactory.CreateArrayType();

        public static readonly TypeSymbol StringFilePath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringFilePath);
        public static readonly TypeSymbol StringJsonFilePath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringFilePath | TypeSymbolValidationFlags.IsStringJsonFilePath);
        public static readonly TypeSymbol StringYamlFilePath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringFilePath | TypeSymbolValidationFlags.IsStringYamlFilePath);

        //Type for available loadTextContent encoding

        public static readonly ImmutableSortedDictionary<string, Encoding> SupportedEncodings = new SortedDictionary<string, Encoding>(IdentifierComparer)
        {
            ["us-ascii"] = Encoding.ASCII,
            ["iso-8859-1"] = Encoding.GetEncoding("iso-8859-1"),
            ["utf-8"] = Encoding.UTF8,
            ["utf-16BE"] = Encoding.BigEndianUnicode,
            ["utf-16"] = Encoding.Unicode,
        }.ToImmutableSortedDictionary(IdentifierComparer);

        public static readonly TypeSymbol LoadTextContentEncodings = TypeHelper.CreateTypeUnion(SupportedEncodings.Keys.Select(s => TypeFactory.CreateStringLiteralType(s)));

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new ObjectType(nameof(ParameterModifierMetadata), TypeSymbolValidationFlags.Default, CreateParameterModifierMetadataProperties(), Any, TypePropertyFlags.Constant);

        // types allowed to use in output and parameter declarations
        public static readonly ImmutableSortedDictionary<string, TypeSymbol> DeclarationTypes = new[] { String, Object, Int, Bool, Array }.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly ImmutableHashSet<string> ReservedTypeNames = ImmutableHashSet.Create<string>(IdentifierComparer, ResourceKeyword);

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

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant;
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
