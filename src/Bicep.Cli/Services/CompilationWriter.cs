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
        private readonly InvocationContext invocationContext;

        public CompilationWriter(InvocationContext invocationContext) 
        {
            this.invocationContext = invocationContext;
        }

        public EmitResult ToFile(Compilation compilation, string outputPath)
        {
            var fileStream = CreateFileStream(outputPath);
            using (fileStream)
            {
                return new TemplateEmitter(compilation.GetEntrypointSemanticModel(), invocationContext.EmitterSettings).Emit(fileStream);
            }
        }

        public EmitResult ToStream(Compilation compilation, Stream stream)
        {
            return new TemplateEmitter(compilation.GetEntrypointSemanticModel(), invocationContext.EmitterSettings).Emit(stream);
        }

        public EmitResult ToStdout(Compilation compilation)
        {
            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), invocationContext.EmitterSettings);

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
