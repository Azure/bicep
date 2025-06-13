// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Snapshot;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Bicep.Cli.Rpc;

public class CliJsonRpcServer : ICliJsonRpcProtocol
{
    public static IJsonRpcMessageHandler CreateMessageHandler(Stream inputStream, Stream outputStream)
    {
        var formatter = new JsonMessageFormatter();
        formatter.JsonSerializer.ContractResolver = new CamelCasePropertyNamesContractResolver();

        return new HeaderDelimitedMessageHandler(inputStream, outputStream, formatter);
    }

    private readonly BicepCompiler compiler;

    public CliJsonRpcServer(BicepCompiler compiler)
    {
        this.compiler = compiler;
    }

    /// <inheritdoc/>
    public async Task<VersionResponse> Version(VersionRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        return new(
            ThisAssembly.AssemblyInformationalVersion.Split('+')[0]);
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

        var workspace = new Workspace();
        workspace.UpsertSourceFile(paramFile);
        compilation = await compiler.CreateCompilation(paramFile.Uri, workspace);
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

        var fileUris = new HashSet<Uri>();
        foreach (var otherModel in compilation.GetAllBicepModels())
        {
            fileUris.Add(otherModel.SourceFile.Uri);
            fileUris.UnionWith(otherModel.SourceFile.GetReferencedAuxiliaryFileUris().Select(ioUri => ioUri.ToUri()));
            if (otherModel.Configuration.ConfigFileUri is { } configFileIdentifier)
            {
                var uri = new UriBuilder
                {
                    Scheme = configFileIdentifier.Scheme,
                    Host = configFileIdentifier.Authority,
                    Path = configFileIdentifier.Path,
                }.Uri;

                fileUris.Add(uri);
            }
        }

        return new(
            [.. fileUris.Select(x => x.LocalPath).OrderBy(x => x)]);
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

    private static async Task<Compilation> GetCompilation(BicepCompiler compiler, string filePath)
    {
        var fileUri = PathHelper.FilePathToFileUrl(filePath);
        if (!PathHelper.HasBicepExtension(fileUri) &&
            !PathHelper.HasBicepparamsExtension(fileUri))
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
