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
    /// <summary>
    /// Generates a JSON parameter file from a Bicep file.
    /// </summary>
    public class PlaceholderParametersWriter
    {
        private readonly InvocationContext invocationContext;

        public PlaceholderParametersWriter(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public EmitResult ToFile(Compilation compilation, string outputPath)
        {
            var existingContent = string.Empty;
            if (File.Exists(outputPath))
            {
                existingContent = File.ReadAllText(outputPath);
            }
            using var fileStream = CreateFileStream(outputPath);
            var semanticModel = compilation.GetEntrypointSemanticModel();
            return new TemplateEmitter(semanticModel).EmitParametersFile(fileStream, existingContent);
        }

        public EmitResult ToStdout(Compilation compilation)
        {
            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };

            var semanticModel = compilation.GetEntrypointSemanticModel();
            return new TemplateEmitter(semanticModel).EmitParametersFile(writer, string.Empty);
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
