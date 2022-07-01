// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Bicep.Cli.Services
{
    public class ParamsFileWriter
    {
        private readonly InvocationContext invocationContext;

        public ParamsFileWriter(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public EmitResult ToFile(ProgramSyntax syntax, string outputPath)
        {
            // var existingContent = string.Empty;
            // if (File.Exists(outputPath))
            // {
            //     existingContent = File.ReadAllText(outputPath);
            // }
            using var fileStream = CreateFileStream(outputPath);
            return new ParamsEmitter(syntax, invocationContext.EmitterSettings).EmitParamsFile(fileStream);
        }

        public EmitResult ToStdout(ProgramSyntax syntax)
        {
            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };new ParamsEmitter(syntax, invocationContext.EmitterSettings).EmitParamsFile(writer);

            return new ParamsEmitter(syntax, invocationContext.EmitterSettings).EmitParamsFile(writer);
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
