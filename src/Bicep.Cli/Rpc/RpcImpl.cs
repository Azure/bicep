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
using Grpc.Core;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Rpc;

public class RpcImpl : Bicep.Rpc.RpcBase
{
    private readonly BicepCompiler compiler;

    public RpcImpl(BicepCompiler compiler)
    {
        this.compiler = compiler;
    }

    public override async Task<Bicep.VersionResponse> Version(Bicep.VersionRequest request, ServerCallContext context)
    {
        await Task.Yield();

        return new() {
            Version = ThisAssembly.AssemblyInformationalVersion.Split('+')[0],
        };
    }

    public override async Task<Bicep.CompileResponse> Compile(Bicep.CompileRequest request, ServerCallContext context)
    {
        var model = await GetSemanticModel(compiler, request.Path);

        var writer = new StringWriter();
        var result = model.SourceFileKind == BicepSourceFileKind.BicepFile ?
            new TemplateEmitter(model).Emit(writer) :
            new ParametersEmitter(model).Emit(writer);
        var success = result.Status == EmitStatus.Succeeded;

        Bicep.CompileResponse response = new() {
            Success = success,
            Contents = success ? writer.ToString() : string.Empty,            
        };

        response.Diagnostics.AddRange(GetDiagnostics(model.Compilation));

        return response;
    }

    public override async Task<Bicep.CompileParamsResponse> CompileParams(Bicep.CompileParamsRequest request, ServerCallContext context)
    {
        var model = await GetSemanticModel(compiler, request.Path);
        if (model.SourceFile is not BicepParamFile paramFile)
        {
            throw new InvalidOperationException($"Expected a .bicepparam file");
        }

        paramFile = ParamsFileHelper.ApplyParameterOverrides(paramFile, request.ParameterOverrides.ToDictionary(x => x.Key, x => x.Value.FromJson<JToken>()));

        var workspace = new Workspace();
        workspace.UpsertSourceFile(paramFile);
        var compilation = await compiler.CreateCompilation(paramFile.FileUri, workspace);
        var paramsResult = compilation.Emitter.Parameters();

        Bicep.CompileParamsResponse response = new() {
            Success = paramsResult.Success,
            Parameters = paramsResult.Parameters,
            Template = paramsResult.Template?.Template ?? string.Empty,
            TemplateSpecId = paramsResult.TemplateSpecId ?? string.Empty,
        };

        response.Diagnostics.AddRange(GetDiagnostics(compilation));

        return response;
    }

    public override async Task<Bicep.GetFileReferencesResponse> GetFileReferences(Bicep.GetFileReferencesRequest request, ServerCallContext context)
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

        Bicep.GetFileReferencesResponse response = new();

        response.FilePaths.AddRange(fileUris.Select(x => x.LocalPath).OrderBy(x => x));

        return response;
    }

    public override async Task<Bicep.GetMetadataResponse> GetMetadata(Bicep.GetMetadataRequest request, ServerCallContext context)
    {
        var model = await GetSemanticModel(compiler, request.Path);

        Bicep.GetMetadataResponse response = new();
        response.Metadata.AddRange(GetModelMetadata(model));
        response.Parameters.AddRange(model.Root.ParameterDeclarations.Select(x => GetSymbolDefinition(model, x)));
        response.Outputs.AddRange(model.Root.OutputDeclarations.Select(x => GetSymbolDefinition(model, x)));
        response.Exports.AddRange(model.Root.Declarations.Where(x => x.IsExported()).Select(x => GetExportDefinition(model, x)));

        return response;
    }

    public override async Task<Bicep.GetDeploymentGraphResponse> GetDeploymentGraph(Bicep.GetDeploymentGraphRequest request, ServerCallContext context)
    {
        var model = await GetSemanticModel(compiler, request.Path);
        var dependenciesBySymbol = ResourceDependencyVisitor.GetResourceDependencies(model, new() { IncludeExisting = true })
            .Where(x => !x.Key.Type.IsError())
            .ToImmutableDictionary(x => x.Key, x => x.Value);

        Dictionary<DeclaredSymbol, Bicep.GetDeploymentGraphResponse.Types.Node> nodesBySymbol = new();
        foreach (var symbol in dependenciesBySymbol.Keys)
        {
            var range = GetRange(model.SourceFile, symbol.DeclaringSyntax);
            if (symbol is ResourceSymbol resourceSymbol)
            {
                var resourceType = resourceSymbol.TryGetResourceTypeReference()?.FormatType() ?? "<unknown>";
                var isExisting = resourceSymbol.DeclaringResource.IsExistingResource();
                nodesBySymbol[symbol] = new() {
                    Range = range,
                    Name = symbol.Name,
                    Type = resourceType,
                    IsExisting = isExisting,
                    RelativePath = string.Empty,
                };
            }
            if (symbol is ModuleSymbol moduleSymbol)
            {
                var modulePath = moduleSymbol.DeclaringModule.TryGetPath()?.TryGetLiteralValue();
                nodesBySymbol[symbol] = new() {
                    Range = range,
                    Name = symbol.Name,
                    Type = "<module>",
                    IsExisting = false,
                    RelativePath = modulePath,
                };
            }
        }

        List<Bicep.GetDeploymentGraphResponse.Types.Edge> edges = new();
        foreach (var (symbol, dependencies) in dependenciesBySymbol)
        {
            var source = nodesBySymbol.TryGetValue(symbol);
            foreach (var dependency in dependencies.Where(d => d.Kind == ResourceDependencyKind.Primary))
            {
                var target = nodesBySymbol.TryGetValue(dependency.Resource);
                if (source is { } && target is { })
                {
                    edges.Add(new() {
                        Source = source.Name,
                        Target = target.Name,
                    });
                }
            }
        }

        Bicep.GetDeploymentGraphResponse response = new();
        response.Nodes.AddRange(nodesBySymbol.Values.OrderBy(x => x.Name));
        response.Edges.AddRange(edges.OrderBy(x => x.Source).ThenBy(x => x.Target));

        return response;
    }

    private static Bicep.GetMetadataResponse.Types.ExportDefinition GetExportDefinition(SemanticModel model, DeclaredSymbol symbol)
    {
        Bicep.GetMetadataResponse.Types.ExportDefinition result = new() {
            Range = GetRange(model.SourceFile, symbol.DeclaringSyntax),
            Name = symbol.Name,
            Kind = symbol.Kind.ToString(),
        };

        if (symbol.TryGetDescriptionFromDecorator() is {} description) {
            result.Description = description;
        }

        return result;
    }

    private static Bicep.GetMetadataResponse.Types.SymbolDefinition GetSymbolDefinition(SemanticModel model, DeclaredSymbol symbol)
    {
        var typeSyntax = symbol switch
        {
            ParameterSymbol x => x.DeclaringParameter.Type,
            OutputSymbol x => x.DeclaringOutput.Type,
            _ => null,
        };

        Bicep.GetMetadataResponse.Types.TypeDefinition? getTypeInfo()
        {
            if (typeSyntax is { } &&
                model.GetSymbolInfo(typeSyntax) is DeclaredSymbol typeSymbol)
            {
                return new() {
                    Range = GetRange(model.SourceFile, typeSymbol.DeclaringSyntax),
                    Name = typeSymbol.Name,
                };
            }

            if (typeSyntax is { } &&
                model.GetDeclaredType(symbol.DeclaringSyntax) is { } type)
            {
                return new() {
                    Range = null,
                    Name = type.Name,
                };
            }

            return null;
        }

        Bicep.GetMetadataResponse.Types.SymbolDefinition result = new() {
            Range = GetRange(model.SourceFile, symbol.DeclaringSyntax),
            Name = symbol.Name,
            Type = getTypeInfo(),
        };

        if (symbol.TryGetDescriptionFromDecorator() is {} description) {
            result.Description = description;
        }

        return result;
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

    private static IEnumerable<Bicep.Diagnostic> GetDiagnostics(Compilation compilation)
    {
        foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
        {
            foreach (var diagnostic in diagnostics)
            {
                yield return new Bicep.Diagnostic() {
                    Source = bicepFile.FileUri.LocalPath,
                    Range = GetRange(bicepFile, diagnostic),
                    Level = diagnostic.Level.ToString(),
                    Code = diagnostic.Code,
                    Message = diagnostic.Message,
                };
            }
        }
    }

    private IEnumerable<Bicep.GetMetadataResponse.Types.MetadataDefinition> GetModelMetadata(SemanticModel model)
    {
        foreach (var metadata in model.Root.MetadataDeclarations)
        {
            if (metadata.DeclaringSyntax is MetadataDeclarationSyntax declarationSyntax &&
                declarationSyntax.Value is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is string value)
            {
                yield return new() {
                    Name = metadata.Name,
                    Value = value,
                };
            }
        }
    }

    private static Bicep.Range GetRange(BicepSourceFile file, IPositionable positionable)
    {
        var start = TextCoordinateConverter.GetPosition(file.LineStarts, positionable.GetPosition());
        var end = TextCoordinateConverter.GetPosition(file.LineStarts, positionable.GetEndPosition());

        return new() {
            Start = new() {
                Line = start.line,
                Char = start.character,
            },
            End = new() {
                Line = end.line,
                Char = end.character,
            },
        };
    }
}