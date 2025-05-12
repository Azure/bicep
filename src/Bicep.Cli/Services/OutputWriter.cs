// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Deployments.Engine.Definitions;
using Bicep.Cli.Models;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.Decompiler;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;

namespace Bicep.Cli.Services
{
    public class OutputWriter
    {
        private readonly IOContext io;
        private readonly IFileSystem fileSystem;

        public OutputWriter(IOContext io, IFileSystem fileSystem)
        {
            this.io = io;
            this.fileSystem = fileSystem;
        }

        public void ParametersToStdout(Compilation compilation)
        {
            var parametersResult = compilation.Emitter.Parameters();
            if (parametersResult.Parameters is null)
            {
                throw new InvalidOperationException("Failed to emit parameters");
            }

            var result = new BuildParamsStdout(
                parametersJson: parametersResult.Parameters,
                templateJson: parametersResult.Template?.Template,
                templateSpecId: parametersResult.TemplateSpecId);

            WriteToStdout(JsonConvert.SerializeObject(result));
        }

        public void ParametersToFile(Compilation compilation, Uri outputUri)
        {
            var parametersResult = compilation.Emitter.Parameters();
            if (parametersResult.Parameters is null)
            {
                throw new InvalidOperationException("Failed to emit parameters");
            }

            WriteToFile(outputUri, parametersResult.Parameters);
        }

        public void TemplateToStdout(Compilation compilation)
        {
            var templateResult = compilation.Emitter.Template();
            if (templateResult.Template is null)
            {
                throw new InvalidOperationException("Failed to emit template");
            }

            WriteToStdout(templateResult.Template);
        }

        public void TemplateToFile(Compilation compilation, Uri outputUri)
        {
            var templateResult = compilation.Emitter.Template();
            if (templateResult.Template is null)
            {
                throw new InvalidOperationException("Failed to emit template");
            }

            WriteToFile(outputUri, templateResult.Template);
        }

        public void DecompileResultToFile(DecompileResult decompilation)
        {
            foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
            {
                WriteToFile(fileUri, bicepOutput);
            }
        }

        public void DecompileResultToStdout(DecompileResult decompilation)
        {
            foreach (var (_, bicepOutput) in decompilation.FilesToSave)
            {
                WriteToStdout(bicepOutput);
            }
        }

        private void WriteToStdout(string contents)
        {
            io.Output.Write(contents);
        }

        public void WriteToFile(Uri fileUri, string contents)
        {
            try
            {
                fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(fileUri.LocalPath)!);
                using var fileStream = fileSystem.FileStream.New(fileUri.LocalPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using var sw = new StreamWriter(fileStream, TemplateEmitter.UTF8EncodingWithoutBom, 4096, leaveOpen: true);

                sw.Write(contents);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
            }
        }
    }
}
