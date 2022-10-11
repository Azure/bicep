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
    /// Generates a Bicep param file from a JSON parameter file.
    /// </summary>
    public class ParametersWriter
    {
        private readonly InvocationContext invocationContext;

        public ParametersWriter(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public EmitResult ToFile(ParamsSemanticModel paramSemanticModel, string outputPath)
        {
            using var fileStream = CreateFileStream(outputPath);
            return new ParametersEmitter(paramSemanticModel).EmitParamsFile(fileStream);
        }

        public EmitResult ToStdout(ParamsSemanticModel paramSemanticModel)
        {
            using var writer = new JsonTextWriter(invocationContext.OutputWriter)
            {
                Formatting = Formatting.Indented
            };

            return new ParametersEmitter(paramSemanticModel).EmitParamsFile(writer);
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
