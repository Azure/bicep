// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Features;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

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
        public const string BicepPublicMcrPathPrefix = "bicep/"; // All modules in the public bicep registry start with this prefix

        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;
        public const int MaxLiteralCharacterLimit = 131072;
        public const int MaxJsonFileCharacterLimit = 1048576; // arbitrary value of 1024*1024 characters.
                                                              // since max ARM template size is 4MB, and it's encoded in UTF that each character can be 1-4 bytes,
                                                              // we can limit maximum size of JSON file loaded to not exceed 1M characters.
                                                              // even though loading files near this limit will make user eventually hit the 4MB limit
                                                              // but it will not be hard to exceed the limit just by loading a single file.

        /// <summary>
        /// This is the maximum value that the copyIndex() function may return at run time for a resource copy loop.
        /// </summary>
        public const int MaxResourceCopyIndexValue = 800;

        /// <summary>
        /// Maximum length of a deployment (aka module) name.
        /// </summary>
        public const int MaxDeploymentNameLength = 64;

        public const string ErrorName = "<error>";
        public const string MissingName = "<missing>";

        public const string TargetScopeKeyword = "targetScope";
        public const string MetadataKeyword = "metadata";
        public const string TypeKeyword = "type";
        public const string ParameterKeyword = "param";
        public const string UsingKeyword = "using";
        public const string ExtendsKeyword = "extends";
        public const string OutputKeyword = "output";
        public const string VariableKeyword = "var";
        public const string ResourceKeyword = "resource";
        public const string ModuleKeyword = "module";
        public const string TestKeyword = "test";
        public const string FunctionKeyword = "func";
        public const string ExistingKeyword = "existing";
        public const string ImportKeyword = "import";
        public const string ExtensionKeyword = "extension";
        public const string ExtensionConfigKeyword = "extensionConfig";
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
        public const string TargetScopeTypeDesiredStateConfiguration = "desiredStateConfiguration";
        public const string TargetScopeTypeLocal = "local";

        public const string CopyLoopIdentifier = "copy";
        public const string BaseIdentifier = "base";

        public const string BicepConfigurationFileName = "bicepconfig.json";

        public const string DisableNextLineDiagnosticsKeyword = "disable-next-line";
        public const string DisableDiagnosticsKeyword = "disable-diagnostics";
        public const string RestoreDiagnosticsKeyword = "restore-diagnostics";

        public static readonly Regex ArmTemplateSchemaRegex = new(@"https?:\/\/schema\.management\.azure\.com\/schemas\/([^""\/]+\/[a-zA-Z]*[dD]eploymentTemplate\.json)#?");
        public static readonly Regex ArmParametersSchemaRegex = new(@"https?:\/\/schema\.management\.azure\.com\/schemas\/([^""\/]+\/[dD]eploymentParameters\.json)#?");

        public static readonly ImmutableSortedSet<string> DeclarationKeywords = ImmutableSortedSet.Create(
            StringComparer.Ordinal,
            [
                AssertKeyword,
                ImportKeyword,
                MetadataKeyword,
                ParameterKeyword,
                VariableKeyword,
                ResourceKeyword,
                OutputKeyword,
                ModuleKeyword,
                TypeKeyword
            ]);

        public static readonly ImmutableSortedSet<string> ContextualKeywords = DeclarationKeywords
            .Add(TargetScopeKeyword)
            .Add(IfKeyword)
            .Add(ForKeyword)
            .Add(InKeyword)
            .Add(FromKeyword)
            .Add(WithKeyword)
            .Add(AsKeyword);

        public const string TrueKeyword = "true";
        public const string FalseKeyword = "false";
        public const string NullKeyword = "null";
        public const string NoneKeyword = "none";
        public const string VoidKeyword = "void";

        public const string ListFunctionPrefix = "list";

        public static readonly ImmutableDictionary<string, TokenType> NonContextualKeywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            [TrueKeyword] = TokenType.TrueKeyword,
            [FalseKeyword] = TokenType.FalseKeyword,
            [NullKeyword] = TokenType.NullKeyword,
        }.ToImmutableDictionary();

        // Decorators
        public const string ParameterAllowedPropertyName = "allowed";
        public const string ParameterSecurePropertyName = "secure";
        public const string ParameterMinValuePropertyName = "minValue";
        public const string ParameterMaxValuePropertyName = "maxValue";
        public const string ParameterMinLengthPropertyName = "minLength";
        public const string ParameterMaxLengthPropertyName = "maxLength";
        public const string ParameterUserDefinedConstraintPropertyName = "validate";
        public const string ParameterMetadataPropertyName = "metadata";
        public const string ParameterSealedPropertyName = "sealed";
        public const string MetadataDescriptionPropertyName = "description";
        public const string MetadataResourceTypePropertyName = "resourceType";
        public const string MetadataResourceDerivedTypePropertyName = "__bicep_resource_derived_type!";
        public const string MetadataResourceDerivedTypePointerPropertyName = "source";
        public const string MetadataResourceDerivedTypeOutputFlagName = "output";
        public const string MetadataExportedPropertyName = "__bicep_export!";
        public const string MetadataImportedFromPropertyName = "__bicep_imported_from!";
        public const string TemplateMetadataExportedVariablesName = "__bicep_exported_variables!";
        public const string ImportMetadataSourceTemplatePropertyName = "sourceTemplate";
        public const string ImportMetadataOriginalIdentifierPropertyName = "originalIdentifier";
        public const string BatchSizePropertyName = "batchSize";
        public const string WaitUntilPropertyName = "waitUntil";
        public const string RetryOnPropertyName = "retryOn";
        public const string ExportPropertyName = "export";
        public const string TypeDiscriminatorDecoratorName = "discriminator";
        public const string OnlyIfNotExistsPropertyName = "onlyIfNotExists";

        // module properties
        public const string ModuleParamsPropertyName = "params";
        public const string ModuleExtensionConfigsPropertyName = "extensionConfigs";
        public const string ModuleOutputsPropertyName = "outputs";
        public const string ModuleNamePropertyName = "name";
        public const string ModuleIdentityPropertyName = "identity";

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
        public const string TypeNameAny = "any";
        public const string TypeNameModule = "module";
        public const string TypeNameTest = "test";
        public const string TypeNameResource = "resource";
        public const string TypeNameResourceInput = "resourceInput";
        public const string TypeNameResourceOutput = "resourceOutput";
        public static readonly FrozenSet<string> ResourceDerivedTypeNames = new[]
        {
            TypeNameResource,
            TypeNameResourceInput,
            TypeNameResourceOutput,
        }.ToFrozenSet(IdentifierComparer);

        // extension namespace properties
        public const string ExtensionConfigPropertyName = "config";

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public static readonly StringComparer ResourceTypeComparer = StringComparer.OrdinalIgnoreCase;
        public static readonly StringComparison ResourceTypeComparison = StringComparison.OrdinalIgnoreCase;

        public static readonly StringComparer ExtensionNameComparer = StringComparer.Ordinal;
        public static readonly StringComparison ExtensionNameComparison = StringComparison.Ordinal;

        public const string StringDelimiter = "'";
        public const string StringHoleOpen = "${";
        public const string StringHoleClose = "}";

        public const string AnyFunction = "any";
        public const string NameofFunctionName = "nameof";
        public const string ExternalInputBicepFunctionName = "externalInput";
        public const string ExternalInputsArmFunctionName = "externalInputs";
        public const string ReadCliArgBicepFunctionName = "readCliArg";
        public const string ReadEnvVarBicepFunctionName = "readEnvVar";

        public static readonly TypeSymbol Any = new AnyType();
        public static readonly TypeSymbol Never = new UnionType("never", []);

        public static readonly TypeSymbol ResourceRef = CreateResourceScopeReference(ResourceScope.Module | ResourceScope.Resource);

        public static readonly TypeSymbol Resource = CreateResourceScopeReference(ResourceScope.Resource);

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
        public static readonly TypeSymbol Object = new ObjectType(ObjectType, TypeSymbolValidationFlags.Default, [], new TypeProperty(LanguageConstants.Any));
        public static readonly TypeSymbol SecureObject = new ObjectType(ObjectType, TypeSymbolValidationFlags.Default | TypeSymbolValidationFlags.IsSecure, [], new TypeProperty(LanguageConstants.Any));
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
        public static readonly TypeSymbol StringArray = TypeFactory.CreateStringArrayType();

        public static readonly TypeSymbol StringFilePath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringFilePath);
        public static readonly TypeSymbol StringJsonFilePath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringFilePath | TypeSymbolValidationFlags.IsStringJsonFilePath);
        public static readonly TypeSymbol StringYamlFilePath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringFilePath | TypeSymbolValidationFlags.IsStringYamlFilePath);
        public static readonly TypeSymbol StringDirectoryPath = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsStringDirectoryPath);
        public static readonly TypeSymbol StringResourceIdentifier = TypeFactory.CreateStringType(validationFlags: TypeSymbolValidationFlags.IsResourceTypeIdentifier);

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
        public static readonly TypeSymbol ParameterModifierMetadata = new ObjectType(nameof(ParameterModifierMetadata), TypeSymbolValidationFlags.Default, CreateParameterModifierMetadataProperties(), new TypeProperty(Any, TypePropertyFlags.Constant));

        // types allowed to use in output and parameter declarations
        public static readonly ImmutableSortedDictionary<string, TypeSymbol> DeclarationTypes
            = new[] { String, Object, Int, Bool, Array, Any }
                .ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly ImmutableHashSet<string> ReservedTypeNames = ImmutableHashSet.Create<string>(IdentifierComparer, ResourceKeyword);

        public static readonly ImmutableArray<string> DiscriminatorPreferenceOrder = ["type", "kind"];

        private static IEnumerable<NamedTypeProperty> CreateParameterModifierMetadataProperties()
        {
            yield return new NamedTypeProperty("description", String, TypePropertyFlags.Constant);
        }

        private static readonly TypeSymbol identityTypeString =
            TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("None"),
                TypeFactory.CreateStringLiteralType("UserAssigned"));

        private static readonly IEnumerable<NamedTypeProperty> identityProperties = new[]
        {
                new NamedTypeProperty("type", identityTypeString, TypePropertyFlags.Required),
                new NamedTypeProperty("userAssignedIdentities", Object, TypePropertyFlags.None)
        };

        public static readonly TypeSymbol IdentityObject = new ObjectType("identity", TypeSymbolValidationFlags.Default, identityProperties);

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
            if (resourceScope.HasFlag(ResourceScope.DesiredStateConfiguration))
            {
                yield return "desiredStateConfiguration";
            }
            if (resourceScope.HasFlag(ResourceScope.Local))
            {
                yield return "local";
            }
        }

        public static ResourceScopeType CreateResourceScopeReference(ResourceScope resourceScope)
        {
            var scopeDescriptions = string.Join(" | ", GetResourceScopeDescriptions(resourceScope));

            return new ResourceScopeType(scopeDescriptions, resourceScope);
        }

        public static TypeSymbol CreateModuleType(IFeatureProvider features, IEnumerable<NamedTypeProperty> paramsProperties, IEnumerable<NamedTypeProperty>? extensionConfigsProperties, IEnumerable<NamedTypeProperty> outputProperties, ResourceScope moduleScope, ResourceScope containingScope, string typeName)
        {
            var paramsType = new ObjectType(ModuleParamsPropertyName, TypeSymbolValidationFlags.Default, paramsProperties, null);
            // If none of the params are required, we can allow the 'params' declaration to be omitted entirely
            var paramsRequiredFlag = paramsProperties.Any(x => x.Flags.HasFlag(TypePropertyFlags.Required)) ? TypePropertyFlags.Required : TypePropertyFlags.None;

            var outputsType = new ObjectType(ModuleOutputsPropertyName, TypeSymbolValidationFlags.Default, outputProperties, null);

            var scopePropertyFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.DisallowAny | TypePropertyFlags.LoopVariant;
            if (moduleScope != containingScope)
            {
                // If the module scope matches the parent scope, we can safely omit the scope property
                scopePropertyFlags |= TypePropertyFlags.Required;
            }

            // Module name is optional.
            var nameRequirednessFlags = TypePropertyFlags.None;
            // Taken from the official REST specs for Microsoft.Resources/deployments
            var nameType = TypeFactory.CreateStringType(minLength: 1, maxLength: 64, pattern: @"^[-\w._()]+$");

            List<NamedTypeProperty> moduleProperties =
            [
                new(ModuleNamePropertyName, nameType, nameRequirednessFlags | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.LoopVariant),
                new(ResourceScopePropertyName, CreateResourceScopeReference(moduleScope), scopePropertyFlags),
                new(ModuleParamsPropertyName, paramsType, paramsRequiredFlag | TypePropertyFlags.WriteOnly),
                new(ModuleOutputsPropertyName, outputsType, TypePropertyFlags.ReadOnly),
                new(ResourceDependsOnPropertyName, ResourceOrResourceCollectionRefArray, TypePropertyFlags.WriteOnly | TypePropertyFlags.DisallowAny),
                new(ModuleIdentityPropertyName, IdentityObject, TypePropertyFlags.DeployTimeConstant),
            ];

            if (features.ModuleExtensionConfigsEnabled)
            {
                extensionConfigsProperties ??= [];
                var extensionConfigsType = new ObjectType(ModuleExtensionConfigsPropertyName, TypeSymbolValidationFlags.Default, extensionConfigsProperties);
                var extensionConfigsRequiredFlag = extensionConfigsProperties.Any(x => x.Flags.HasFlag(TypePropertyFlags.Required)) ? TypePropertyFlags.Required : TypePropertyFlags.None;

                moduleProperties.Add(new(ModuleExtensionConfigsPropertyName, extensionConfigsType, extensionConfigsRequiredFlag | TypePropertyFlags.WriteOnly));
            }

            var moduleBody = new ObjectType(typeName, TypeSymbolValidationFlags.Default, moduleProperties, null);

            return new ModuleType(typeName, moduleScope, moduleBody);
        }

        public static TypeSymbol CreateUsingConfigType()
        {
            var optionalPropFlags = TypePropertyFlags.WriteOnly | TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.ReadableAtDeployTime | TypePropertyFlags.DisallowAny;
            var requiredPropFlags = optionalPropFlags | TypePropertyFlags.Required;

            NamedTypeProperty[] commonProps = [
                // Taken from the official REST specs for Microsoft.Resources/deployments
                new(ModuleNamePropertyName, TypeFactory.CreateStringType(minLength: 1, maxLength: 64, pattern: @"^[-\w._()]+$"), optionalPropFlags),
                // TODO model this properly as a scope, rather than a string
                new(ResourceScopePropertyName, LanguageConstants.String, requiredPropFlags),
            ];

            var deployment = new ObjectType("DeploymentConfig", TypeSymbolValidationFlags.Default, [
                ..commonProps,
                new("mode", TypeFactory.CreateStringLiteralType("deployment"), requiredPropFlags),
            ], null);

            var deleteDetachEnum = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("delete"),
                TypeFactory.CreateStringLiteralType("detach"));
            var actionOnUnmanage = new ObjectType("actionOnUnmanage", TypeSymbolValidationFlags.Default, [
                new("resources", deleteDetachEnum, requiredPropFlags, "Specifies the action that should be taken on the resource when the deployment stack is deleted. Delete will attempt to delete the resource from Azure. Detach will leave the resource in it's current state."),
                new("resourceGroups", deleteDetachEnum, optionalPropFlags, "Specifies the action that should be taken on the resource when the deployment stack is deleted. Delete will attempt to delete the resource from Azure. Detach will leave the resource in it's current state."),
                new("managementGroups", deleteDetachEnum, optionalPropFlags, "Specifies the action that should be taken on the resource when the deployment stack is deleted. Delete will attempt to delete the resource from Azure. Detach will leave the resource in it's current state."),
            ], null);

            var denySettingsModeEnum = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("denyDelete"),
                TypeFactory.CreateStringLiteralType("denyWriteAndDelete"),
                TypeFactory.CreateStringLiteralType("none"));
            var denySettings = new ObjectType("denySettings", TypeSymbolValidationFlags.Default, [
                new("applyToChildScopes", LanguageConstants.Bool, optionalPropFlags, "DenySettings will be applied to child scopes."),
                new("excludedActions", LanguageConstants.StringArray, optionalPropFlags, "List of role-based management operations that are excluded from the denySettings. Up to 200 actions are permitted. If the denySetting mode is set to 'denyWriteAndDelete', then the following actions are automatically appended to 'excludedActions': '*/read' and 'Microsoft.Authorization/locks/delete'. If the denySetting mode is set to 'denyDelete', then the following actions are automatically appended to 'excludedActions': 'Microsoft.Authorization/locks/delete'. Duplicate actions will be removed."),
                new("excludedPrincipals", LanguageConstants.StringArray, optionalPropFlags, "List of AAD principal IDs excluded from the lock. Up to 5 principals are permitted."),
                new("mode", denySettingsModeEnum, requiredPropFlags, "denySettings Mode."),
            ], null);

            var stack = new ObjectType("StackConfig", TypeSymbolValidationFlags.Default, [
                ..commonProps,
                new("description", LanguageConstants.String, optionalPropFlags, "Deployment stack description."),
                new("mode", TypeFactory.CreateStringLiteralType("stack"), requiredPropFlags),
                new("actionOnUnmanage", actionOnUnmanage, requiredPropFlags, "Defines the behavior of resources that are not managed immediately after the stack is updated."),
                new("denySettings", denySettings, requiredPropFlags, "Defines how resources deployed by the deployment stack are locked."),
            ], null);


            return new DiscriminatedObjectType("Config", TypeSymbolValidationFlags.Default, "mode", [deployment, stack]);
        }
    }
}
