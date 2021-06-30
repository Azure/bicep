// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Collections.Immutable;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;

namespace Bicep.Cli.Services
{
    public class FileWriter : IWriter
    {
        private readonly InvocationContext invocationContext;

        public FileWriter(InvocationContext invocationContext) 
        {
            this.invocationContext = invocationContext;
        }

        private FileStream? CompilationFileStream { get; set; }

        public void WriteCompilation(Compilation compilation)
        {
            if (CompilationFileStream is not null)
            {
                using (CompilationFileStream)
                {
                    new TemplateEmitter(compilation.GetEntrypointSemanticModel(), invocationContext.AssemblyFileVersion).Emit(CompilationFileStream);
                }
            }
            else 
            {
                throw new Exception("Create FileStream before calling WriteCompilation");
            }
        }

        public void WriteDecompilation((Uri, ImmutableDictionary<Uri, string>) decompilation)
        {
            var (_, filesToSave) = decompilation;

            foreach (var (fileUri, bicepOutput) in filesToSave)
            {
                File.WriteAllText(fileUri.LocalPath, bicepOutput);
            }
        }


        public FileWriter CreateFileStream(string path)
        {
            try
            {
                CompilationFileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
            }

            return this;
        }
    }
}
