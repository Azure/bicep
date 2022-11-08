// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;
using Newtonsoft.Json;
using System;
using System.IO;

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
            var fileStream = CreateFileStream(outputPath);
            using (fileStream)
            {
                return ToStream(compilation, fileStream);           
            }
        }

        public EmitResult ToStream(Compilation compilation, Stream stream)
        {
            var fileKind = compilation.SourceFileGrouping.EntryPoint.FileKind;
            switch (fileKind)
            {
                case BicepSourceFileKind.BicepFile:
                    return new TemplateEmitter(compilation.GetEntrypointSemanticModel()).Emit(stream);

                case BicepSourceFileKind.ParamsFile:
                    return new ParametersEmitter(compilation.GetEntrypointSemanticModel()).EmitParamsFile(stream);

                default:
                    throw new NotImplementedException($"Unexpected file kind '{fileKind}'");
            }            
        }

        public EmitResult ToStdout(Compilation compilation)
        {
            var fileKind = compilation.SourceFileGrouping.EntryPoint.FileKind;
            var semanticModel = compilation.GetEntrypointSemanticModel();
            switch (fileKind)
            {
                case BicepSourceFileKind.BicepFile:
                    {
                        var sourceFileToTrack = semanticModel.Features.SourceMappingEnabled ? semanticModel.SourceFile : default;
                        using var writer = new SourceAwareJsonTextWriter(semanticModel.FileResolver, io.Output, sourceFileToTrack)
                        {
                            Formatting = Formatting.Indented
                        };

                        var emitter = new TemplateEmitter(semanticModel);

                        return emitter.Emit(writer);
                    }

                case BicepSourceFileKind.ParamsFile:
                    {
                        using var writer = new JsonTextWriter(io.Output)
                        {
                            Formatting = Formatting.Indented
                        };

                        return new ParametersEmitter(semanticModel).EmitParamsFile(writer);
                    }

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
