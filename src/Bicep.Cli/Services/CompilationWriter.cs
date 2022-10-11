// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
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
                var semanticModel = compilation.GetEntrypointSemanticModel();
                return new TemplateEmitter(semanticModel).Emit(fileStream);
            }
        }

        public EmitResult ToStream(Compilation compilation, Stream stream)
        {
            var semanticModel = compilation.GetEntrypointSemanticModel();
            return new TemplateEmitter(semanticModel).Emit(stream);
        }

        public EmitResult ToStdout(Compilation compilation)
        {
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var sourceFileToTrack = semanticModel.Features.SourceMappingEnabled ? semanticModel.SourceFile : null;
            using var writer = new SourceAwareJsonTextWriter(semanticModel.FileResolver, io.Output, sourceFileToTrack)
            {
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateEmitter(semanticModel);

            return emitter.Emit(writer);
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
