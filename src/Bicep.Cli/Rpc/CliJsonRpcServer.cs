// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
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

    public async Task<VersionResponse> Version(VersionRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        return new(
            ThisAssembly.AssemblyInformationalVersion.Split('+')[0]);
    }

    public async Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken)
    {
        var model = await GetSemanticModel(compiler, request.Path);
        var diagnostics = GetDiagnostics(model.Compilation).ToImmutableArray();

        var writer = new StringWriter();
        var result = model.SourceFileKind == BicepSourceFileKind.BicepFile ?
            new TemplateEmitter(model).Emit(writer) :
            new ParametersEmitter(model).Emit(writer);
        var success = result.Status == EmitStatus.Succeeded;

        return new(success, diagnostics, success ? writer.ToString() : null);
    }

    public async Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken)
    {
        var model = await GetSemanticModel(compiler, request.Path);
        if (model.SourceFile is not BicepParamFile paramFile)
        {
            throw new InvalidOperationException($"Expected a .bicepparam file");
        }

        paramFile = ParamsFileHelper.ApplyParameterOverrides(paramFile, request.ParameterOverrides);

        var workspace = new Workspace();
        workspace.UpsertSourceFile(paramFile);
        var compilation = await compiler.CreateCompilation(paramFile.FileUri, workspace);
        var paramsResult = compilation.Emitter.Parameters();

        return new(
            paramsResult.Success,
            GetDiagnostics(compilation).ToImmutableArray(),
            paramsResult.Parameters,
            paramsResult.Template?.Template,
            paramsResult.TemplateSpecId);
    }

    public async Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken)
    {
        var model = await GetSemanticModel(compiler, request.Path);
        var diagnostics = GetDiagnostics(model.Compilation).ToImmutableArray();

        var fileUris = new HashSet<Uri>();
        foreach (var otherModel in model.Compilation.GetAllBicepModels())
        {
            fileUris.Add(otherModel.SourceFile.FileUri);
            fileUris.UnionWith(otherModel.GetAuxiliaryFileReferences());
            if (otherModel.Configuration.ConfigFileUri is { } configFileUri)
            {
                fileUris.Add(configFileUri);
            }
        }

        return new(
            [.. fileUris.Select(x => x.LocalPath).OrderBy(x => x)]);
    }

    public async Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken)
    {
        var model = await GetSemanticModel(compiler, request.Path);

        var metadata = GetModelMetadata(model).ToImmutableArray();
        var parameters = model.Root.ParameterDeclarations.Select(x => GetSymbolDefinition(model, x)).ToImmutableArray();
        var outputs = model.Root.OutputDeclarations.Select(x => GetSymbolDefinition(model, x)).ToImmutableArray();
        var exports = model.Root.Declarations.Where(x => x.IsExported()).Select(x => GetExportDefinition(model, x)).ToImmutableArray();

        return new(metadata, parameters, outputs, exports);
    }

    private static GetMetadataResponse.ExportDefinition GetExportDefinition(SemanticModel model, DeclaredSymbol symbol)
        => new(
            GetRange(model.SourceFile, symbol.DeclaringSyntax),
            symbol.Name,
            symbol.Kind.ToString(),
            symbol.TryGetDescriptionFromDecorator());

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
            symbol.TryGetDescriptionFromDecorator());
    }

    public async Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken)
    {
        var model = await GetSemanticModel(compiler, request.Path);
        var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(model, new() { IncludeExisting = true })
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
            foreach (var dependency in dependencies.Where(d => d.Kind == ResourceDependencyKind.Primary))
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

    private static async Task<SemanticModel> GetSemanticModel(BicepCompiler compiler, string filePath)
    {
        var fileUri = PathHelper.FilePathToFileUrl(filePath);
        if (!PathHelper.HasBicepExtension(fileUri) &&
            !PathHelper.HasBicepparamsExtension(fileUri))
        {
            throw new InvalidOperationException($"Invalid file path: {fileUri}");
        }

        var compilation = await compiler.CreateCompilation(fileUri);

        return compilation.GetEntrypointSemanticModel();
    }

    private static IEnumerable<DiagnosticDefinition> GetDiagnostics(Compilation compilation)
    {
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                yield return new(bicepFile.FileUri.LocalPath, GetRange(bicepFile, diagnostic), diagnostic.Level.ToString(), diagnostic.Code, diagnostic.Message);
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
