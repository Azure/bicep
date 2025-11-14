// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Newtonsoft.Json;

namespace Bicep.Core.Emit;

public record ParametersResult(
    bool Success,
    ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> Diagnostics,
    string? Parameters,
    string? TemplateSpecId,
    TemplateResult? Template);

public record TemplateResult(
    bool Success,
    ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> Diagnostics,
    string? Template,
    string? SourceMap);

public interface ICompilationEmitter
{
    TemplateResult Template();

    ParametersResult Parameters();
}

public class CompilationEmitter : ICompilationEmitter
{
    private readonly Compilation compilation;

    public CompilationEmitter(Compilation compilation)
    {
        this.compilation = compilation;
    }

    public ParametersResult Parameters()
    {
        var model = compilation.GetEntrypointSemanticModel();
        if (model.SourceFileKind != BicepSourceFileKind.ParamsFile)
        {
            throw new InvalidOperationException($"Entry-point {model.SourceFile.FileHandle.Uri} is not a parameters file");
        }

        var diagnostics = compilation.GetAllDiagnosticsByBicepFile();

        using var writer = new StringWriter { NewLine = "\n" };
        var result = new ParametersEmitter(model).Emit(writer);
        if (result.Status != EmitStatus.Succeeded)
        {
            return new(false, diagnostics, null, null, null);
        }

        var parametersData = writer.ToString();
        if (!model.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
        {
            throw new InvalidOperationException($"Failed to find linked bicep file for parameters file {model.SourceFile.FileHandle.Uri}");
        }

        switch (usingModel)
        {
            case SemanticModel bicepModel:
                {
                    var templateResult = Template(bicepModel);
                    return new ParametersResult(true, diagnostics, parametersData, null, templateResult);
                }
            case ArmTemplateSemanticModel armTemplateModel:
                {
                    var template = armTemplateModel.SourceFile.Text;
                    var templateResult = new TemplateResult(true, [], template, null);

                    return new ParametersResult(true, diagnostics, parametersData, null, templateResult);
                }
            case TemplateSpecSemanticModel templateSpecModel:
                {
                    return new ParametersResult(true, diagnostics, parametersData, templateSpecModel.SourceFile.TemplateSpecId, null);
                }
            case EmptySemanticModel _:
                {
                    return new ParametersResult(true, diagnostics, parametersData, null, null);
                }
        }

        throw new InvalidOperationException($"Invalid semantic model of type {usingModel.GetType()}");
    }

    public TemplateResult Template()
    {
        var model = this.compilation.GetEntrypointSemanticModel();
        if (model.SourceFileKind != SourceGraph.BicepSourceFileKind.BicepFile)
        {
            throw new InvalidOperationException($"Entry-point {model.SourceFile.FileHandle.Uri} is not a bicep file");
        }

        return Template(model);
    }

    private TemplateResult Template(SemanticModel model)
    {
        var diagnostics = compilation.GetAllDiagnosticsByBicepFile();

        using var writer = new StringWriter { NewLine = "\n" };
        var result = new TemplateEmitter(model).Emit(writer);
        if (result.Status != EmitStatus.Succeeded)
        {
            return new(false, diagnostics, null, null);
        }

        var template = writer.ToString();
        var sourceMap = result.SourceMap is { } ? JsonConvert.SerializeObject(result.SourceMap) : null;

        return new(true, diagnostics, template, sourceMap);
    }
}
