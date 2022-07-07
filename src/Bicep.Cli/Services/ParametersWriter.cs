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
    public class ParametersWriter
    {
        private readonly InvocationContext invocationContext;

        public ParametersWriter(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public EmitResult ToFile(ProgramSyntax syntax, string outputPath)
        {
            using var fileStream = CreateFileStream(outputPath);
            return new ParametersEmitter(syntax, invocationContext.EmitterSettings).EmitParamsFile(fileStream);
        }

        public EmitResult ToStdout(ProgramSyntax syntax)
        {
            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };new ParametersEmitter(syntax, invocationContext.EmitterSettings).EmitParamsFile(writer);

            return new ParametersEmitter(syntax, invocationContext.EmitterSettings).EmitParamsFile(writer);
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
