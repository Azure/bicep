// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Cli.Services
{
    public class ConsoleWriter : IWriter
    {
        private readonly InvocationContext invocationContext;

        public ConsoleWriter(InvocationContext invocationContext) 
        {
            this.invocationContext = invocationContext;
        }

        public void WriteCompilation(Compilation compilation)
        {
            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), invocationContext.AssemblyFileVersion);

            emitter.Emit(writer);
        }

        public void WriteDecompilation((Uri, ImmutableDictionary<Uri, string>) decompilation)
        {
            var (_, filesToSave) = decompilation;

            foreach (var (_, bicepOutput) in filesToSave)
            {
                invocationContext.OutputWriter.Write(bicepOutput);
            }
        }
    }
}
