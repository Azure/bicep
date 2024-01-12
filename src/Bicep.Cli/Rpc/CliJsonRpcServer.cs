// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Tracing;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public async Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken)
    {
        var model = await GetSemanticModel(compiler, request.Path);

        var metadata = GetModelMetadata(model).ToImmutableArray();

        var parameters = model.Root.ParameterDeclarations
            .Select(x => new GetMetadataResponse.SymbolDefinition(GetRange(model.SourceFile, x.DeclaringSyntax), x.Name, x.TryGetDescriptionFromDecorator()))
            .ToImmutableArray();

        var outputs = model.Root.OutputDeclarations
            .Select(x => new GetMetadataResponse.SymbolDefinition(GetRange(model.SourceFile, x.DeclaringSyntax),x.Name, x.TryGetDescriptionFromDecorator()))
            .ToImmutableArray();

        return new(metadata, parameters, outputs);
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
                if (source is {} && target is {})
                {
                    edges.Add(new(source.Name, target.Name));
                }
            }
        }

        return new(
            nodesBySymbol.Values.OrderBy(x => x.Name).ToImmutableArray(),
            edges.OrderBy(x => x.Source).ThenBy(x => x.Target).ToImmutableArray());
    }

    private static async Task<SemanticModel> GetSemanticModel(BicepCompiler compiler, string filePath)
    {
        var fileUri = PathHelper.FilePathToFileUrl(filePath);
        if (!PathHelper.HasBicepExtension(fileUri) &&
            !PathHelper.HasBicepparamsExension(fileUri))
        {
            throw new InvalidOperationException($"Invalid file path: {fileUri}");
        }

        var compilation = await compiler.CreateCompilation(fileUri);

        return compilation.GetEntrypointSemanticModel();
    }

    private static IEnumerable<CompileResponse.DiagnosticDefinition> GetDiagnostics(Compilation compilation)
    {
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                yield return new(GetRange(bicepFile, diagnostic), diagnostic.Level.ToString(), diagnostic.Code, diagnostic.Message);
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
