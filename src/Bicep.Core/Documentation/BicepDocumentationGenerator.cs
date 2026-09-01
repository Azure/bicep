// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Reflection;
using Bicep.Core.Configuration;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.Abstraction;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;

namespace Bicep.Core.Documentation;

public class BicepDocumentationGenerator(
    IFileExplorer fileExplorer,
    IFileSystem? fileSystem = null) : IBicepDocumentationGenerator
{
    private const string BuiltInTemplateResourceName = "Bicep.Core.Documentation.Templates.Markdown.scriban";

    private const string MetadataNamePropertyName = "name";

    private static readonly Lazy<string> BuiltInTemplateSource = new(LoadBuiltInTemplateSource);

    private static readonly Lazy<Template> BuiltInTemplate = new(() =>
        ParseTemplate(BuiltInTemplateSource.Value, BuiltInTemplateResourceName));

    private static readonly ImmutableDictionary<ResourceScope, string> TargetScopeNames =
        new Dictionary<ResourceScope, string>
        {
            [ResourceScope.Tenant] = LanguageConstants.TargetScopeTypeTenant,
            [ResourceScope.ManagementGroup] = LanguageConstants.TargetScopeTypeManagementGroup,
            [ResourceScope.Subscription] = LanguageConstants.TargetScopeTypeSubscription,
            [ResourceScope.Local] = LanguageConstants.TargetScopeTypeLocal,
        }.ToImmutableDictionary();

    public BicepDocumentationModel BuildModel(
        Compilation compilation,
        IReadOnlyDictionary<string, string>? customValues = null,
        CancellationToken cancellationToken = default) =>
        BuildModel(compilation, customValues, new(), cancellationToken);

    /// <inheritdoc/>
    public BicepDocumentationModel BuildModelWithOptions(
        Compilation compilation,
        BicepDocumentationGenerationOptions options,
        CancellationToken cancellationToken = default) =>
        BuildModel(compilation, options.CustomValues, options.Examples, cancellationToken);

    private BicepDocumentationModel BuildModel(
        Compilation compilation,
        IReadOnlyDictionary<string, string>? customValues,
        DocumentationExamples examples,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var semanticModel = compilation.GetEntrypointSemanticModel();
        var typeAnalyzer = new BicepDocumentationTypeAnalyzer(cancellationToken);

        if (semanticModel.HasErrors())
        {
            throw new BicepDocumentationException("Cannot generate documentation for a module that has compilation errors.");
        }

        var entryFile = semanticModel.SourceFile.FileHandle;
        var moduleRoot = entryFile.GetParent();

        return new BicepDocumentationModel(
            Name: GetModuleName(semanticModel, moduleRoot),
            Description: NormalizeText(DescriptionHelper.TryGetFromSemanticModel(semanticModel)),
            Path: entryFile.Uri.GetFilePath(),
            TargetScope: GetTargetScopeName(semanticModel.TargetScope),
            Custom: BuildCustom(customValues),
            ResourceTypes: BuildResourceTypes(semanticModel),
            Parameters: BuildParameters(semanticModel, typeAnalyzer),
            Outputs: BuildOutputs(semanticModel, typeAnalyzer),
            ExportedTypes: BuildExportedTypes(semanticModel, typeAnalyzer),
            ExportedVariables: BuildExportedVariables(semanticModel, typeAnalyzer),
            ExportedFunctions: BuildExportedFunctions(semanticModel, typeAnalyzer),
            References: BuildReferences(semanticModel),
            UsageExamples: BicepDocumentationExampleDiscovery.Discover(
                moduleRoot,
                examples,
                fileSystem is null ? null : uri => IsReparsePoint(fileSystem, uri)));
    }

    public string Render(
        BicepDocumentationModel model,
        BicepDocumentationGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        options ??= BicepDocumentationGenerationOptions.Default;

        var (template, templateSourcePath) = GetTemplate(options);

        var scriptObject = BicepDocumentationScriptModelFactory.Create(ApplyCustomValues(model, options.CustomValues));
        var context = new TemplateContext
        {
            TemplateLoader = new BicepDocumentationTemplateLoader(fileExplorer, GetIncludeRoot(options, model)),
            LimitToString = 0,
            LoopLimit = 100_000,
        };
        context.PushGlobal(scriptObject);

        string rendered;
        try
        {
            rendered = template.Render(context);
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (ScriptRuntimeException ex)
        {
            throw new BicepDocumentationException($"Failed to render the documentation template '{templateSourcePath}': {ex.Message}", ex);
        }

        return NormalizeOutput(rendered);
    }

    private static ImmutableSortedDictionary<string, string> BuildCustom(IReadOnlyDictionary<string, string>? customValues) =>
        customValues is null
            ? ImmutableSortedDictionary.Create<string, string>(StringComparer.Ordinal)
            : customValues.ToImmutableSortedDictionary(StringComparer.Ordinal);

    private (Template Template, string SourcePath) GetTemplate(BicepDocumentationGenerationOptions options)
    {
        var (templateSource, templateSourcePath) = GetTemplateSource(options);
        return options.TemplateFile is null
            ? (BuiltInTemplate.Value, templateSourcePath)
            : (ParseTemplate(templateSource, templateSourcePath), templateSourcePath);
    }

    private static Template ParseTemplate(string source, string sourcePath)
    {
        var template = Template.Parse(source, sourcePath);
        if (template.HasErrors)
        {
            throw new BicepDocumentationException(
                $"Failed to parse the documentation template '{sourcePath}':{System.Environment.NewLine}{template.Messages}");
        }

        return template;
    }

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
        catch (Exception ex) when (ex.IsFileSystemException())
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

    private static ImmutableArray<BicepDocumentationParameter> BuildParameters(
        SemanticModel semanticModel,
        BicepDocumentationTypeAnalyzer typeAnalyzer)
    {
        var parameters = semanticModel.Root.ParameterDeclarations
            .Select(symbol =>
            {
                var metadata = semanticModel.Parameters[symbol.Name];
                var defaultValue = symbol.DeclaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax
                    ? SyntaxStringifier.Stringify(defaultValueSyntax.DefaultValue)
                    : null;

                return typeAnalyzer.BuildParameter(
                    symbol.Name,
                    metadata.TypeReference.Type,
                    metadata.IsRequired,
                    NormalizeText(metadata.Description),
                    defaultValue);
            })
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(parameters, p => p.Name);
    }

    private static ImmutableArray<BicepDocumentationOutput> BuildOutputs(
        SemanticModel semanticModel,
        BicepDocumentationTypeAnalyzer typeAnalyzer)
    {
        var outputs = semanticModel.Outputs
            .Select(output => new BicepDocumentationOutput(
                output.Name,
                typeAnalyzer.GetTypeName(output.TypeReference.Type),
                output.TypeReference.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
                NormalizeText(output.Description)))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(outputs, o => o.Name);
    }

    private static ImmutableArray<BicepDocumentationFunction> BuildExportedFunctions(
        SemanticModel semanticModel,
        BicepDocumentationTypeAnalyzer typeAnalyzer)
    {
        var functions = semanticModel.Exports.Values
            .OfType<ExportedFunctionMetadata>()
            .Select(function => new BicepDocumentationFunction(
                function.Name,
                function.Parameters
                    .Select(parameter => new BicepDocumentationFunctionParameter(
                        parameter.Name,
                        typeAnalyzer.GetTypeName(parameter.TypeReference.Type),
                        NormalizeText(parameter.Description)))
                    .ToImmutableArray(),
                typeAnalyzer.GetTypeName(function.Return.TypeReference.Type),
                NormalizeText(function.Description)))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(functions, f => f.Name);
    }

    private static ImmutableArray<BicepDocumentationExport> BuildExportedTypes(
        SemanticModel semanticModel,
        BicepDocumentationTypeAnalyzer typeAnalyzer)
    {
        var types = semanticModel.Exports.Values
            .OfType<ExportedTypeMetadata>()
            .Select(type => typeAnalyzer.BuildExport(
                type.Name,
                ((TypeType)type.TypeReference.Type).Unwrapped,
                NormalizeText(type.Description)))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(types, type => type.Name);
    }

    private static ImmutableArray<BicepDocumentationExport> BuildExportedVariables(
        SemanticModel semanticModel,
        BicepDocumentationTypeAnalyzer typeAnalyzer)
    {
        var variables = semanticModel.Exports.Values
            .OfType<ExportedVariableMetadata>()
            .Select(variable => typeAnalyzer.BuildExport(
                variable.Name,
                variable.TypeReference.Type,
                NormalizeText(variable.Description)))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(variables, variable => variable.Name);
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
                    description = NormalizeText(DescriptionHelper.TryGetFromSemanticModel(referencedModel));
                }

                return new BicepDocumentationReference(module.Name, path, description);
            })
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(references, r => r.SymbolicName);
    }

    private static string? NormalizeText(string? value) => value?.Trim();

    private static bool IsReparsePoint(IFileSystem fileSystem, IOUri uri) =>
        fileSystem.File.GetAttributes(uri.GetFilePath()).HasFlag(FileAttributes.ReparsePoint);
}
