// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Snapshot;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Bicep.Cli.Rpc;

public class CliJsonRpcServer(
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver,
    IEnvironment environment) : ICliJsonRpcProtocol
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
            subscriptionId: request.Metadata.SubscriptionId,
            resourceGroup: request.Metadata.ResourceGroup,
            location: request.Metadata.Location,
            deploymentName: request.Metadata.DeploymentName,
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

        if (sourceFile.Features.LegacyFormatterEnabled)
        {
            var v2Options = sourceFile.Configuration.Formatting.Data;
            var legacyOptions = PrettyPrintOptions.FromV2Options(v2Options);
            formattedContent = PrettyPrinter.PrintProgram(sourceFile.ProgramSyntax, legacyOptions, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);
        }
        else
        {
            var options = sourceFile.Configuration.Formatting.Data;
            var context = PrettyPrinterV2Context.Create(options, sourceFile.LexingErrorLookup, sourceFile.ParsingErrorLookup);

            using var writer = new StringWriter();
            PrettyPrinterV2.PrintTo(writer, sourceFile.ProgramSyntax, context);
            formattedContent = writer.ToString();
        }

        return new(formattedContent);
    }

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
