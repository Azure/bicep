// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;

namespace Bicep.Core.Documentation;

public class BicepDocumentationGenerator(IFileExplorer fileExplorer) : IBicepDocumentationGenerator
{
    private const string BuiltInTemplateResourceName = "Bicep.Core.Documentation.Templates.Markdown.scriban";

    private const string EnableTelemetryParameterName = "enableTelemetry";

    private const string DataCollectionNote =
        "This module uses the `enableTelemetry` parameter to report anonymized module usage to Microsoft, " +
        "in support of continued investment in the Bicep and Azure Verified Modules ecosystems. No resource-specific data is collected.";

    private const string MetadataNamePropertyName = "name";

    private static readonly Lazy<string> BuiltInTemplateSource = new(LoadBuiltInTemplateSource);

    private static readonly ImmutableDictionary<ResourceScope, string> TargetScopeNames =
        new Dictionary<ResourceScope, string>
        {
            [ResourceScope.Tenant] = LanguageConstants.TargetScopeTypeTenant,
            [ResourceScope.ManagementGroup] = LanguageConstants.TargetScopeTypeManagementGroup,
            [ResourceScope.Subscription] = LanguageConstants.TargetScopeTypeSubscription,
            [ResourceScope.Local] = LanguageConstants.TargetScopeTypeLocal,
        }.ToImmutableDictionary();

    public BicepDocumentationModel BuildModel(Compilation compilation, IReadOnlyDictionary<string, string>? customValues = null)
    {
        var semanticModel = compilation.GetEntrypointSemanticModel();

        if (semanticModel.HasErrors())
        {
            throw new BicepDocumentationException("Cannot generate documentation for a module that has compilation errors.");
        }

        var entryFile = semanticModel.SourceFile.FileHandle;
        var moduleRoot = entryFile.GetParent();

        return new BicepDocumentationModel(
            Name: GetModuleName(semanticModel, moduleRoot),
            Description: DescriptionHelper.TryGetFromSemanticModel(semanticModel),
            Path: entryFile.Uri.GetFilePath(),
            TargetScope: GetTargetScopeName(semanticModel.TargetScope),
            Custom: BuildCustom(customValues),
            ResourceTypes: BuildResourceTypes(semanticModel),
            Parameters: BuildParameters(semanticModel),
            Outputs: BuildOutputs(semanticModel),
            ExportedFunctions: BuildExportedFunctions(semanticModel),
            References: BuildReferences(semanticModel),
            UsageExamples: BicepDocumentationExampleDiscovery.Discover(moduleRoot),
            DataCollection: BuildDataCollection(semanticModel));
    }

    public string Render(BicepDocumentationModel model, BicepDocumentationGenerationOptions? options = null)
    {
        options ??= BicepDocumentationGenerationOptions.Default;

        if (!Enum.IsDefined(options.Preset))
        {
            throw new BicepDocumentationException($"The documentation preset '{options.Preset}' is not supported.");
        }

        var (templateSource, templateSourcePath) = GetTemplateSource(options);

        var template = Template.Parse(templateSource, templateSourcePath);

        if (template.HasErrors)
        {
            throw new BicepDocumentationException($"Failed to parse the documentation template '{templateSourcePath}':{System.Environment.NewLine}{template.Messages}");
        }

        var scriptObject = BicepDocumentationScriptModelFactory.Create(ApplyCustomValues(model, options.CustomValues));
        var context = new TemplateContext
        {
            TemplateLoader = new BicepDocumentationTemplateLoader(fileExplorer, GetIncludeRoot(options, model)),
        };
        context.PushGlobal(scriptObject);

        string rendered;
        try
        {
            rendered = template.Render(context);
        }
        catch (ScriptRuntimeException ex)
        {
            throw new BicepDocumentationException($"Failed to render the documentation template '{templateSourcePath}': {ex.Message}", ex);
        }

        return NormalizeOutput(rendered);
    }

    public string Generate(Compilation compilation, BicepDocumentationGenerationOptions? options = null)
    {
        var model = BuildModel(compilation, options?.CustomValues);

        return Render(model, options);
    }

    private static ImmutableSortedDictionary<string, string> BuildCustom(IReadOnlyDictionary<string, string>? customValues) =>
        customValues is null
            ? ImmutableSortedDictionary.Create<string, string>(StringComparer.Ordinal)
            : customValues.ToImmutableSortedDictionary(StringComparer.Ordinal);

    private static BicepDocumentationModel ApplyCustomValues(
        BicepDocumentationModel model,
        IReadOnlyDictionary<string, string>? customValues)
    {
        if (customValues is null)
        {
            return model;
        }

        var merged = model.Custom.ToBuilder();
        foreach (var (key, value) in customValues)
        {
            merged[key] = value;
        }

        return model with { Custom = merged.ToImmutable() };
    }

    private IOUri GetIncludeRoot(BicepDocumentationGenerationOptions options, BicepDocumentationModel model)
    {
        if (options.TemplateRoot is { } templateRoot)
        {
            return fileExplorer.GetDirectory(templateRoot).Uri;
        }

        try
        {
            return IOUri.FromFilePath(model.Path).Resolve(".");
        }
        catch (IOException ex)
        {
            throw new BicepDocumentationException($"Unable to resolve an include root from module path '{model.Path}': {ex.Message}", ex);
        }
    }

    private (string source, string sourcePath) GetTemplateSource(BicepDocumentationGenerationOptions options)
    {
        if (options.TemplateFile is not { } templateFileUri)
        {
            return (BuiltInTemplateSource.Value, BuiltInTemplateResourceName);
        }

        var file = fileExplorer.GetFile(templateFileUri);
        if (!file.Exists())
        {
            throw new BicepDocumentationException($"The template file '{templateFileUri}' does not exist.");
        }

        string contents;
        try
        {
            contents = file.ReadAllText();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new BicepDocumentationException($"Unable to read template file '{templateFileUri}': {ex.Message}", ex);
        }

        return (contents, templateFileUri.ToString());
    }

    private static string LoadBuiltInTemplateSource() =>
        LoadTemplateSource(Assembly.GetExecutingAssembly(), BuiltInTemplateResourceName);

    internal static string LoadTemplateSource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("Could not get manifest resource stream for the built-in documentation template.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private static string NormalizeOutput(string rendered)
    {
        var normalized = rendered.ReplaceLineEndings("\n").TrimEnd('\n');

        return normalized + "\n";
    }

    private static string GetModuleName(SemanticModel semanticModel, IDirectoryHandle moduleRoot)
    {
        var nameMetadata = semanticModel.Root.MetadataDeclarations
            .FirstOrDefault(metadata => LanguageConstants.IdentifierComparer.Equals(metadata.Name, MetadataNamePropertyName));

        if (nameMetadata?.Value is StringSyntax nameSyntax && nameSyntax.TryGetLiteralValue() is { } literalName)
        {
            return literalName;
        }

        return GetFallbackModuleName(moduleRoot.Uri, semanticModel.SourceFile.FileHandle.Uri);
    }

    internal static string GetFallbackModuleName(IOUri moduleRootUri, IOUri entryFileUri)
    {
        var directoryName = moduleRootUri.GetFileName();
        return directoryName.Length > 0
            ? directoryName
            : entryFileUri.GetFileNameWithoutExtension().ToString();
    }

    private static string GetTargetScopeName(ResourceScope targetScope)
    {
        return TargetScopeNames.TryGetValue(targetScope, out var name)
            ? name
            : LanguageConstants.TargetScopeTypeResourceGroup;
    }

    private static ImmutableArray<BicepDocumentationResourceType> BuildResourceTypes(SemanticModel semanticModel)
    {
        var resourceTypes = semanticModel.DeclaredResources
            .Select(resource => new BicepDocumentationResourceType(resource.Type.TypeReference.FormatName(), resource.IsExistingResource))
            .Distinct()
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(resourceTypes, r => r.Type);
    }

    private static ImmutableArray<BicepDocumentationParameter> BuildParameters(SemanticModel semanticModel)
    {
        var parameters = semanticModel.Root.ParameterDeclarations
            .Select(symbol =>
            {
                var metadata = semanticModel.Parameters[symbol.Name];
                var defaultValue = symbol.DeclaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax
                    ? SyntaxStringifier.Stringify(defaultValueSyntax.DefaultValue)
                    : null;

                return BicepDocumentationTypeAnalyzer.BuildParameter(symbol.Name, metadata.TypeReference.Type, metadata.IsRequired, metadata.Description, defaultValue);
            })
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(parameters, p => p.Name);
    }

    private static ImmutableArray<BicepDocumentationOutput> BuildOutputs(SemanticModel semanticModel)
    {
        var outputs = semanticModel.Outputs
            .Select(output => new BicepDocumentationOutput(
                output.Name,
                BicepDocumentationTypeAnalyzer.GetTypeName(output.TypeReference.Type),
                output.TypeReference.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
                output.Description))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(outputs, o => o.Name);
    }

    private static ImmutableArray<BicepDocumentationFunction> BuildExportedFunctions(SemanticModel semanticModel)
    {
        var functions = semanticModel.Exports.Values
            .OfType<ExportedFunctionMetadata>()
            .Select(function => new BicepDocumentationFunction(
                function.Name,
                function.Parameters
                    .Select(parameter => new BicepDocumentationFunctionParameter(
                        parameter.Name,
                        BicepDocumentationTypeAnalyzer.GetTypeName(parameter.TypeReference.Type),
                        parameter.Description))
                    .ToImmutableArray(),
                BicepDocumentationTypeAnalyzer.GetTypeName(function.Return.TypeReference.Type),
                function.Description))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(functions, f => f.Name);
    }

    private static ImmutableArray<BicepDocumentationReference> BuildReferences(SemanticModel semanticModel)
    {
        var references = semanticModel.Root.ModuleDeclarations
            .Select(module =>
            {
                var path = ((StringSyntax)module.DeclaringModule.Path).TryGetLiteralValue();
                string? description = null;

                if (module.TryGetSemanticModel().IsSuccess(out var referencedModel))
                {
                    description = DescriptionHelper.TryGetFromSemanticModel(referencedModel);
                }

                return new BicepDocumentationReference(module.Name, path, description);
            })
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(references, r => r.SymbolicName);
    }

    private static BicepDocumentationDataCollection? BuildDataCollection(SemanticModel semanticModel)
    {
        var enableTelemetryParameter = semanticModel.Root.ParameterDeclarations
            .FirstOrDefault(symbol => LanguageConstants.IdentifierComparer.Equals(symbol.Name, EnableTelemetryParameterName));

        if (enableTelemetryParameter is null)
        {
            return null;
        }

        var enabledByDefault = enableTelemetryParameter.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax { DefaultValue: BooleanLiteralSyntax { Value: false } };

        return new BicepDocumentationDataCollection(enabledByDefault, DataCollectionNote);
    }
}
