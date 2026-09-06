// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.Emit.Options;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Services
{
    /// <summary>
    /// Generates a JSON parameter file from a Bicep file.
    /// </summary>
    public class PlaceholderParametersWriter
    {
        private readonly IOContext io;

        public PlaceholderParametersWriter(IOContext io)
        {
            this.io = io;
        }

        public EmitResult ToFile(Compilation compilation, IFileHandle outputFile, OutputFormatOption outputFormat, IncludeParamsOption includeParams)
        {
            var existingContent = string.Empty;
            if (outputFile.Exists())
            {
                existingContent = outputFile.ReadAllText();
            }
            using var fileStream = outputFile.OpenWrite();
            var semanticModel = compilation.GetEntrypointSemanticModel();
            return new TemplateEmitter(semanticModel).EmitTemplateGeneratedParameterFile(fileStream, existingContent, outputFormat, includeParams);
        }

        public EmitResult ToStdout(Compilation compilation, OutputFormatOption outputFormat, IncludeParamsOption includeParams)
        {
            var semanticModel = compilation.GetEntrypointSemanticModel();
            return new TemplateEmitter(semanticModel).EmitTemplateGeneratedParameterFile(io.Output.Writer, string.Empty, outputFormat, includeParams);
        }
    }
}
