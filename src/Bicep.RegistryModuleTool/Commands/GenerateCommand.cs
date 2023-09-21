// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Microsoft.Extensions.Logging;

namespace Bicep.RegistryModuleTool.Commands
{
    public sealed class GenerateCommand : Command
    {
        public GenerateCommand()
            : base("generate", "Generate files for the Bicep registry module")
        {
        }

        public sealed class CommandHandler : BaseCommandHandler
        {
            private readonly BicepCompiler compiler;

            public CommandHandler(IFileSystem fileSystem, ILogger<GenerateCommand> logger, BicepCompiler compiler)
                : base(fileSystem, logger)
            {
                this.compiler = compiler;
            }

            protected override async Task<int> InvokeInternalAsync(InvocationContext context)
            {
                var mainBicepFile = await this.GenerateAndLogAsync(
                    MainBicepFile.FileName, () => MainBicepFile.GenerateAsync(this.FileSystem, this.compiler, context.Console));

                await this.GenerateAndLogAsync(
                    MainArmTemplateFile.FileName, () => MainArmTemplateFile.GenerateAsync(this.FileSystem, mainBicepFile));

                await this.GenerateAndLogAsync(
                    ReadmeFile.FileName, () => ReadmeFile.GenerateAsync(this.FileSystem, mainBicepFile));

                await this.GenerateAndLogAsync(
                    MainBicepTestFile.FileName, () => MainBicepTestFile.GenerateAsync(this.FileSystem));

                await this.GenerateAndLogAsync(
                    VersionFile.FileName, () => VersionFile.GenerateAsync(this.FileSystem));

                return 0;
            }

            private async Task<T> GenerateAndLogAsync<T>(string fileName, Func<Task<T>> fileGenerator) where T : ModuleFile
            {
                this.Logger.LogInformation("Generating {FileName}...", fileName);
                var file = await fileGenerator();
                this.Logger.LogInformation(@"Generation succeeded. File path: ""{FilePath}"".", file.Path);

                return file;
            }
        }
    }
}
