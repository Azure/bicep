// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Models;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Bicep.Cli.Services
{
    public class CompilationWriter
    {
        private readonly IOContext io;

        public CompilationWriter(IOContext io)
        {
            this.io = io;
        }

        public EmitResult ToFile(Compilation compilation, string outputPath)
        {
            return ToFile(compilation.GetEntrypointSemanticModel(), outputPath);
        }

        public EmitResult ToFile(SemanticModel model, string outputPath)
        {
            var fileStream = CreateFileStream(outputPath);
            using (fileStream)
            {
                return ToStream(model, fileStream);
            }
        }

        public EmitResult ToStream(Compilation compilation, Stream stream)
        {
            return ToStream(compilation.GetEntrypointSemanticModel(), stream);
        }

        public EmitResult ToStream(SemanticModel model, Stream stream)
        {
            var fileKind = model.SourceFileKind;
            switch (fileKind)
            {
                case BicepSourceFileKind.BicepFile:
                    return new TemplateEmitter(model).Emit(stream);

                case BicepSourceFileKind.ParamsFile:
                    return new ParametersEmitter(model).Emit(stream);

                default:
                    throw new NotImplementedException($"Unexpected file kind '{fileKind}'");
            }
        }

        public EmitResult ToStdout(SemanticModel paramsModel, ISemanticModel usingModel)
        {
            var (templateResult, templateJson) = EmitTemplate(paramsModel, usingModel);
            var (paramsResult, parametersJson) = EmitParameters(paramsModel);

            var combinedDiagnostics = templateResult.Diagnostics.Concat(paramsResult.Diagnostics);
            if (templateResult.Status == EmitStatus.Failed || paramsResult.Status == EmitStatus.Failed)
            {
                return new EmitResult(EmitStatus.Failed, combinedDiagnostics);
            }

            var result = new BuildParamsStdout(
                parametersJson: parametersJson,
                templateJson:   (usingModel is not TemplateSpecSemanticModel) ? templateJson : null,
                templateSpecId: (usingModel as TemplateSpecSemanticModel)?.SourceFile.TemplateSpecId);

            io.Output.Write(JsonConvert.SerializeObject(result));

            return new EmitResult(EmitStatus.Succeeded, combinedDiagnostics, templateResult.SourceMap);
        }

        private static (EmitResult result, string output) EmitTemplate(SemanticModel paramsModel, ISemanticModel usingModel)
        {
            var stringBuilder = new StringBuilder();
            using var stringWriter = new StringWriter(stringBuilder) { NewLine = "\n" };

            var emitter = new TemplateEmitter(paramsModel.Compilation, usingModel);
            var result = emitter.Emit(stringWriter);

            return (result, stringBuilder.ToString());
        }

        private static (EmitResult result, string output) EmitParameters(SemanticModel paramsModel)
        {
            var stringBuilder = new StringBuilder();
            using var stringWriter = new StringWriter(stringBuilder) { NewLine = "\n" };

            var emitter = new ParametersEmitter(paramsModel);
            var result = emitter.Emit(stringWriter);

            return (result, stringBuilder.ToString());
        }

        public EmitResult ToStdout(Compilation compilation)
        {
            var fileKind = compilation.SourceFileGrouping.EntryPoint.FileKind;
            var semanticModel = compilation.GetEntrypointSemanticModel();
            return ToStdout(semanticModel, fileKind);
        }

        public EmitResult ToStdout(SemanticModel semanticModel, BicepSourceFileKind fileKind)
        {
            switch (fileKind)
            {
                case BicepSourceFileKind.BicepFile:
                    return new TemplateEmitter(semanticModel).Emit(io.Output);

                case BicepSourceFileKind.ParamsFile:
                    return new ParametersEmitter(semanticModel).Emit(io.Output);

                default:
                    throw new NotImplementedException($"Unexpected file kind '{fileKind}'");
            }
        }

        private static FileStream CreateFileStream(string path)
        {
            try
            {
                return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
            }
        }
    }
}
