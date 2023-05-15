// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
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


        public EmitResult ToStdout(SemanticModel bicepModel, SemanticModel paramsModel)
        {
                //emit template
                var templateOutputBuffer = new StringBuilder();
                using var templateOutputWriter = new StringWriter(templateOutputBuffer);

                var templateEmitter = new TemplateEmitter(bicepModel);
                var templateResult = templateEmitter.Emit(templateOutputWriter);

                templateOutputWriter.Flush();
                var templateOutput = templateOutputBuffer.ToString();

                //emit parameters
                var paramsOutputBuffer = new StringBuilder();
                using var paramsOutputWriter = new StringWriter(paramsOutputBuffer);

                var paramsEmitter = new ParametersEmitter(paramsModel);
                var paramsResult = paramsEmitter.Emit(paramsOutputWriter);

                paramsOutputWriter.Flush();
                var paramsOutput = paramsOutputBuffer.ToString();


                //emit combined output
                using var wrapperWriter = new JsonTextWriter(io.Output)
                {
                    Formatting = Formatting.Indented
                };

                wrapperWriter.WriteStartObject();
                wrapperWriter.WritePropertyName("templateJson");
                wrapperWriter.WriteValue(templateOutput);

                wrapperWriter.WritePropertyName("parametersJson");
                wrapperWriter.WriteValue(paramsOutput);
                wrapperWriter.WriteEndObject();

                var combinedDiagnostics = templateResult.Diagnostics.Concat(paramsResult.Diagnostics);

                if(templateResult.Status == EmitStatus.Failed || paramsResult.Status == EmitStatus.Failed)
                {
                    return new EmitResult(EmitStatus.Failed, combinedDiagnostics);
                }

                return new EmitResult(EmitStatus.Succeeded, combinedDiagnostics, templateResult.SourceMap);
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
