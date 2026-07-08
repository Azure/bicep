// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Documentation;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Core.Utils.Snapshots;
using Bicep.IO.Abstraction;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Bicep.Cli.Rpc;

public class CliJsonRpcServer(
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver,
    IEnvironment environment,
    IBicepDocumentationGenerator documentationGenerator,
    DocsGenerationOptionsResolver docsOptionsResolver,
    IFileSystem fileSystem,
    OutputWriter writer) : ICliJsonRpcProtocol
{
    public static IJsonRpcMessageHandler CreateMessageHandler(Stream inputStream, Stream outputStream)
    {
        var formatter = new JsonMessageFormatter();
        formatter.JsonSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();

        return new HeaderDelimitedMessageHandler(inputStream, outputStream, formatter);
    }

    /// <inheritdoc/>
    public async Task<VersionResponse> Version(VersionRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        return new(environment.CurrentVersion.Version);
    }

    /// <inheritdoc/>
    public async Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = GetDiagnostics(compilation).ToImmutableArray();

        var writer = new StringWriter();
        var result = model.SourceFileKind == BicepSourceFileKind.BicepFile ?
            new TemplateEmitter(model).Emit(writer) :
            new ParametersEmitter(model).Emit(writer);
        var success = result.Status == EmitStatus.Succeeded;

        return new(success, diagnostics, success ? writer.ToString() : null);
    }

    /// <inheritdoc/>
    public async Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        var model = compilation.GetEntrypointSemanticModel();
        if (model.SourceFile is not BicepParamFile paramFile)
        {
            throw new InvalidOperationException($"Expected a .bicepparam file");
        }

        paramFile = ParamsFileHelper.ApplyParameterOverrides(compilation.SourceFileFactory, paramFile, request.ParameterOverrides);

        var workspace = new ActiveSourceFileSet();
        workspace.UpsertSourceFile(paramFile);
        compilation = await compiler.CreateCompilation(paramFile.FileHandle.Uri, workspace);
        var paramsResult = compilation.Emitter.Parameters();

        return new(
            paramsResult.Success,
            [.. GetDiagnostics(compilation)],
            paramsResult.Parameters,
            paramsResult.Template?.Template,
            paramsResult.TemplateSpecId);
    }

    /// <inheritdoc/>
    public async Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        var model = compilation.GetEntrypointSemanticModel();
        var diagnostics = GetDiagnostics(compilation).ToImmutableArray();

        var fileUris = new HashSet<IOUri>();
        foreach (var otherModel in compilation.GetAllBicepModels())
        {
            fileUris.Add(otherModel.SourceFile.FileHandle.Uri);
            fileUris.UnionWith(otherModel.SourceFile.GetReferencedAuxiliaryFileUris());
            if (otherModel.Configuration.ConfigFileUri is { } configFileUri)
            {
                fileUris.Add(configFileUri);
            }
        }

        return new(
            [.. fileUris.Select(x => x.GetFilePath()).OrderBy(x => x)]);
    }

    /// <inheritdoc/>
    public async Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        var model = compilation.GetEntrypointSemanticModel();

        var metadata = GetModelMetadata(model).ToImmutableArray();
        var parameters = model.Root.ParameterDeclarations.Select(x => GetSymbolDefinition(model, x)).ToImmutableArray();
        var outputs = model.Root.OutputDeclarations.Select(x => GetSymbolDefinition(model, x)).ToImmutableArray();
        var exports = model.Root.Declarations.Where(x => x.IsExported(model)).Select(x => GetExportDefinition(model, x)).ToImmutableArray();

        return new(metadata, parameters, outputs, exports);
    }

    private static GetMetadataResponse.ExportDefinition GetExportDefinition(SemanticModel model, DeclaredSymbol symbol)
        => new(
            GetRange(model.SourceFile, symbol.DeclaringSyntax),
            symbol.Name,
            symbol.Kind.ToString(),
            symbol.TryGetDescriptionFromDecorator(model));

    private static GetMetadataResponse.SymbolDefinition GetSymbolDefinition(SemanticModel model, DeclaredSymbol symbol)
    {
        var typeSyntax = symbol switch
        {
            ParameterSymbol x => x.DeclaringParameter.Type,
            OutputSymbol x => x.DeclaringOutput.Type,
            _ => null,
        };

        GetMetadataResponse.TypeDefinition? getTypeInfo()
        {
            if (typeSyntax is { } &&
                model.GetSymbolInfo(typeSyntax) is DeclaredSymbol typeSymbol)
            {
                return new(
                    GetRange(model.SourceFile, typeSymbol.DeclaringSyntax),
                    typeSymbol.Name);
            }

            if (typeSyntax is { } &&
                model.GetDeclaredType(symbol.DeclaringSyntax) is { } type)
            {
                return new(null, type.Name);
            }

            return null;
        }

        return new(
            GetRange(model.SourceFile, symbol.DeclaringSyntax),
            symbol.Name,
            getTypeInfo(),
            symbol.TryGetDescriptionFromDecorator(model));
    }

    public async Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        var model = compilation.GetEntrypointSemanticModel();
        var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(model)
            .Where(x => !x.Key.Type.IsError())
            .ToImmutableDictionary(x => x.Key, x => x.Value);

        Dictionary<DeclaredSymbol, GetDeploymentGraphResponse.Node> nodesBySymbol = new();
        foreach (var symbol in dependenciesBySymbol.Keys)
        {
            var range = GetRange(model.SourceFile, symbol.DeclaringSyntax);
            if (symbol is ResourceSymbol resourceSymbol)
            {
                var resourceType = resourceSymbol.TryGetResourceTypeReference()?.FormatType() ?? "<unknown>";
                var isExisting = resourceSymbol.DeclaringResource.IsExistingResource();
                nodesBySymbol[symbol] = new(range, symbol.Name, resourceType, isExisting, null);
            }
            if (symbol is ModuleSymbol moduleSymbol)
            {
                var modulePath = moduleSymbol.DeclaringModule.TryGetPath()?.TryGetLiteralValue();
                nodesBySymbol[symbol] = new(range, symbol.Name, "<module>", false, modulePath);
            }
        }

        List<GetDeploymentGraphResponse.Edge> edges = new();
        foreach (var (symbol, dependencies) in dependenciesBySymbol)
        {
            var source = nodesBySymbol.TryGetValue(symbol);
            foreach (var dependency in dependencies)
            {
                var target = nodesBySymbol.TryGetValue(dependency.Resource);
                if (source is { } && target is { })
                {
                    edges.Add(new(source.Name, target.Name));
                }
            }
        }

        return new(
            [.. nodesBySymbol.Values.OrderBy(x => x.Name)],
            [.. edges.OrderBy(x => x.Source).ThenBy(x => x.Target)]);
    }

    public async Task<GetSnapshotResponse> GetSnapshot(GetSnapshotRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        if (compilation.Emitter.Parameters() is not { } result ||
            result.Template?.Template is not { } templateContent ||
            result.Parameters is not { } parametersContent)
        {
            throw new InvalidOperationException($"Compilation failed");
        }

        var externalInputs = request.ExternalInputs ?? [];

        var snapshot = await SnapshotHelper.GetSnapshot(
            targetScope: compilation.GetEntrypointSemanticModel().TargetScope,
            templateContent: templateContent,
            parametersContent: parametersContent,
            tenantId: request.Metadata.TenantId,
            managementGroupId: request.Metadata.ManagementGroupId,
            subscriptionId: request.Metadata.SubscriptionId,
            resourceGroup: request.Metadata.ResourceGroup,
            location: request.Metadata.Location,
            deploymentName: request.Metadata.DeploymentName,
            includeSymbolicNames: false,
            externalInputs: [.. externalInputs.Select(x => new SnapshotHelper.ExternalInputValue(x.Kind, x.Config, x.Value))],
            cancellationToken: cancellationToken);

        return new(SnapshotHelper.Serialize(snapshot));
    }

    /// <inheritdoc/>
    public async Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken)
    {
        var compilation = await GetCompilation(compiler, request.Path);
        var model = compilation.GetEntrypointSemanticModel();

        if (model.SourceFile is not BicepSourceFile sourceFile)
        {
            throw new InvalidOperationException($"Expected a .bicep or .bicepparam file");
        }

        string formattedContent;

        if (sourceFile.LoadFeatures().LegacyFormatterEnabled)
        {
            var v2Options = sourceFile.LoadConfiguration().Formatting.Data;
            var legacyOptions = PrettyPrintOptions.FromV2Options(v2Options);
            formattedContent = PrettyPrinter.PrintProgram(sourceFile.ProgramSyntax, legacyOptions, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);
        }
        else
        {
            var options = sourceFile.LoadConfiguration().Formatting.Data;
            var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

            using var writer = new StringWriter();
            PrettyPrinterV2.PrintTo(writer, sourceFile.ProgramSyntax, context);
            formattedContent = writer.ToString();
        }

        return new(formattedContent);
    }

    /// <inheritdoc/>
    public async Task<GenerateDocsResponse> GenerateDocs(GenerateDocsRequest request, CancellationToken cancellationToken)
    {
        var results = ImmutableArray.CreateBuilder<DocsResult>();
        var failures = new Dictionary<int, DocsResult>();
        var validTargets = new List<(int Index, string RequestedPath, IOUri InputUri)>();

        for (var index = 0; index < request.Paths.Length; index++)
        {
            var path = request.Paths[index];
            try
            {
                var inputUri = inputOutputArgumentsResolver.PathToUri(path);
                if (!inputUri.HasBicepExtension())
                {
                    failures[index] = CreateDocsFailure(path, DocsCommand.InputFailureCode, $"Invalid Bicep file path: {inputUri}");
                    continue;
                }

                if (!fileSystem.File.Exists(inputUri.GetFilePath()))
                {
                    failures[index] = CreateDocsFailure(path, DocsCommand.InputFailureCode, $"The input file \"{inputUri}\" does not exist.");
                    continue;
                }

                validTargets.Add((index, path, inputUri));
            }
            catch (Exception exception) when (exception is BicepException || exception.IsPathException())
            {
                failures[index] = CreateDocsFailure(path, DocsCommand.InputFailureCode, exception.Message);
            }
        }

        var rendered = new Dictionary<int, RenderedDocsResult>();
        var workspace = new ActiveSourceFileSet();
        foreach (var target in validTargets)
        {
            rendered[target.Index] = await RenderDocs(
                target.InputUri,
                request.TemplateFile,
                request.TemplateRoot,
                request.Custom,
                request.NoRestore,
                cancellationToken,
                workspace);
        }

        var targets = new List<DocsTarget>();
        var outputUris = new HashSet<IOUri>();
        foreach (var target in validTargets)
        {
            var renderedResult = rendered[target.Index];
            if (!renderedResult.Result.Success || renderedResult.Result.Contents is null)
            {
                continue;
            }

            try
            {
                var outputFile = request.OutputFile ??
                    renderedResult.Configuration!.Documentation.Data.Output.File;
                ValidateDocsOutputFileName(outputFile);
                var outputUri = inputOutputArgumentsResolver.PathToUri(target.InputUri.Resolve(outputFile).GetFilePath());

                if (outputUri.Equals(target.InputUri))
                {
                    failures[target.Index] = CreateDocsFailure(
                        target.RequestedPath,
                        DocsCommand.InputFailureCode,
                        "The documentation output path cannot overwrite the input Bicep file.");
                    continue;
                }

                if (outputUri.HasBicepExtension() || outputUri.HasBicepParamExtension())
                {
                    failures[target.Index] = CreateDocsFailure(
                        target.RequestedPath,
                        DocsCommand.InputFailureCode,
                        "Documentation output cannot use a Bicep source file extension.");
                    continue;
                }

                if (!outputUris.Add(outputUri))
                {
                    failures[target.Index] = CreateDocsFailure(
                        target.RequestedPath,
                        DocsCommand.InputFailureCode,
                        $"Multiple input files resolve to the output file \"{outputUri}\".");
                    continue;
                }

                targets.Add(new DocsTarget(target.Index, target.InputUri, outputUri));
            }
            catch (Exception exception) when (exception is BicepException || exception.IsPathException())
            {
                failures[target.Index] = CreateDocsFailure(
                    target.RequestedPath,
                    DocsCommand.InputFailureCode,
                    exception.Message);
            }
        }
        var targetsByIndex = targets.ToDictionary(target => target.Index);
        for (var index = 0; index < request.Paths.Length; index++)
        {
            if (failures.TryGetValue(index, out var failure))
            {
                results.Add(failure);
                continue;
            }

            var result = rendered[index].Result;
            if (!result.Success || result.Contents is null)
            {
                results.Add(result);
                continue;
            }

            var target = targetsByIndex[index];
            try
            {
                await writer.WriteToFileAsync(target.OutputUri, result.Contents);
                results.Add(result with { OutputPath = target.OutputUri.GetFilePath() });
            }
            catch (Exception exception) when (exception is BicepException || exception.IsPathException())
            {
                results.Add(AddDocsFailure(result, DocsCommand.WriteFailureCode, exception.Message));
            }
        }

        return new(results.ToImmutable());
    }

    /// <inheritdoc/>
    public async Task<OutputDocsResponse> OutputDocs(OutputDocsRequest request, CancellationToken cancellationToken)
        => new((await RenderDocs(
            request.Path,
            request.TemplateFile,
            request.TemplateRoot,
            request.Custom,
            request.NoRestore,
            cancellationToken,
            workspace: null)).Result);

    private async Task<RenderedDocsResult> RenderDocs(
        string path,
        string? templateFile,
        string? templateRoot,
        IReadOnlyDictionary<string, string>? custom,
        bool noRestore,
        CancellationToken cancellationToken,
        ActiveSourceFileSet? workspace)
    {
        IOUri inputUri;
        try
        {
            inputUri = inputOutputArgumentsResolver.PathToUri(path);
        }
        catch (Exception exception) when (exception is BicepException || exception.IsPathException())
        {
            return new(CreateDocsFailure(path, DocsCommand.InputFailureCode, exception.Message), null);
        }

        return await RenderDocs(
            inputUri,
            templateFile,
            templateRoot,
            custom,
            noRestore,
            cancellationToken,
            workspace);
    }

    private async Task<RenderedDocsResult> RenderDocs(
        IOUri inputUri,
        string? templateFile,
        string? templateRoot,
        IReadOnlyDictionary<string, string>? custom,
        bool noRestore,
        CancellationToken cancellationToken,
        ActiveSourceFileSet? workspace)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!inputUri.HasBicepExtension())
        {
            return new(
                CreateDocsFailure(inputUri.GetFilePath(), DocsCommand.InputFailureCode, $"Invalid Bicep file path: {inputUri}"),
                null);
        }

        Compilation compilation;
        try
        {
            compilation = await compiler.CreateCompilation(inputUri, workspace, skipRestore: noRestore);
            workspace?.UpsertSourceFiles(compilation.SourceFileGrouping.SourceFiles);
        }
        catch (BicepException exception)
        {
            return new(CreateDocsFailure(inputUri.GetFilePath(), DocsCommand.InputFailureCode, exception.Message), null);
        }

        var diagnostics = GetDiagnostics(compilation).ToImmutableArray();
        var model = compilation.GetEntrypointSemanticModel();

        if (model.HasErrors())
        {
            return new(
                new(inputUri.GetFilePath(), null, false, diagnostics, null),
                model.Configuration);
        }

        BicepDocumentationGenerationOptions options;
        try
        {
            options = docsOptionsResolver.Resolve(
                model.Configuration,
                templateFile,
                templateRoot,
                custom ?? ImmutableDictionary<string, string>.Empty);
        }
        catch (CommandLineException exception)
        {
            return new(
                CreateDocsFailure(inputUri.GetFilePath(), DocsCommand.InputFailureCode, exception.Message),
                model.Configuration);
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new(
                new(
                    inputUri.GetFilePath(),
                    null,
                    true,
                    diagnostics,
                    documentationGenerator.Generate(compilation, options, cancellationToken)),
                model.Configuration);
        }
        catch (BicepDocumentationException exception)
        {
            return new(
                AddDocsFailure(
                    new(inputUri.GetFilePath(), null, false, diagnostics, null),
                    DocsCommand.RenderFailureCode,
                    exception.Message),
                model.Configuration);
        }
    }

    // These codes describe CLI and RPC orchestration failures that have no source position.
    private static DocsResult AddDocsFailure(DocsResult result, string code, string message) =>
        result with
        {
            Success = false,
            OutputPath = null,
            Contents = null,
            Diagnostics = [.. result.Diagnostics, CreateDocsDiagnostic(result.Path, code, message)],
        };

    private static DocsResult CreateDocsFailure(string path, string code, string message) =>
        new(path, null, false, [CreateDocsDiagnostic(path, code, message)], null);

    private static void ValidateDocsOutputFileName(string outputFile)
    {
        if (string.IsNullOrWhiteSpace(outputFile) ||
            outputFile is "." or ".." ||
            outputFile.Contains('/') ||
            outputFile.Any(FilePathFacts.IsForbiddenPathCharacter) ||
            FilePathFacts.IsForbiddenPathTerminatorCharacter(outputFile[^1]) ||
            FilePathFacts.ContainsWindowsReservedFileName(outputFile))
        {
            throw new CommandLineException("The documentation output file must be a file name without a directory path.");
        }
    }

    private static DiagnosticDefinition CreateDocsDiagnostic(string path, string code, string message) =>
        new(path, new(new(0, 0), new(0, 0)), "Error", code, message);

    private record DocsTarget(int Index, IOUri InputUri, IOUri OutputUri);

    private record RenderedDocsResult(DocsResult Result, RootConfiguration? Configuration);

    private async Task<Compilation> GetCompilation(BicepCompiler compiler, string filePath)
    {
        var fileUri = inputOutputArgumentsResolver.PathToUri(filePath);
        if (!fileUri.HasBicepExtension() && !fileUri.HasBicepParamExtension())
        {
            throw new InvalidOperationException($"Invalid file path: {fileUri}");
        }

        var compilation = await compiler.CreateCompilation(fileUri);

        return compilation;
    }

    private static IEnumerable<DiagnosticDefinition> GetDiagnostics(Compilation compilation)
    {
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                yield return new(bicepFile.FileHandle.Uri, GetRange(bicepFile, diagnostic), diagnostic.Level.ToString(), diagnostic.Code, diagnostic.Message);
            }
        }
    }

    private IEnumerable<GetMetadataResponse.MetadataDefinition> GetModelMetadata(SemanticModel model)
    {
        foreach (var metadata in model.Root.MetadataDeclarations)
        {
            if (metadata.DeclaringSyntax is MetadataDeclarationSyntax declarationSyntax &&
                declarationSyntax.Value is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is string value)
            {
                yield return new(metadata.Name, value);
            }
        }
    }

    private static Range GetRange(BicepSourceFile file, IPositionable positionable)
    {
        var start = TextCoordinateConverter.GetPosition(file.LineStarts, positionable.GetPosition());
        var end = TextCoordinateConverter.GetPosition(file.LineStarts, positionable.GetEndPosition());

        return new(new(start.line, start.character), new(end.line, end.character));
    }
}
