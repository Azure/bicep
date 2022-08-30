// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleValidators;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Abstractions;

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
            private readonly IEnvironmentProxy environmentProxy;

            private readonly IProcessProxy processProxy;

            public CommandHandler(IEnvironmentProxy environmentProxy, IProcessProxy processProxy, IFileSystem fileSystem, ILogger<GenerateCommand> logger)
                : base(fileSystem, logger)
            {
                this.environmentProxy = environmentProxy;
                this.processProxy = processProxy;
            }

            protected override int InvokeInternal(InvocationContext context)
            {
                try
                {
                    ModulePathValidator.ValidateModulePath(this.FileSystem);
                }
                catch (InvalidModuleException exception)
                {
                    context.Console.WriteError(exception.Message);

                    return 1;
                }

                // Read or create main Bicep file.
                this.Logger.LogInformation("Ensuring {MainBicepFile} exists...", "main Bicep file");
                var mainBicepFile = MainBicepFile.EnsureInFileSystem(this.FileSystem);

                // Create main Bicep test file if it doesn't exist.
                this.Logger.LogInformation("Ensuring {MainBicepTestFile} exists...", "main Bicep test file");
                MainBicepTestFile.EnsureInFileSystem(this.FileSystem);

                // Generate main ARM template file.
                var bicepCliProxy = new BicepCliProxy(this.environmentProxy, this.processProxy, this.FileSystem, this.Logger, context.Console);
                var mainArmTemplateFile = this.GenerateFileAndLogInformation("main ARM template file", () => MainArmTemplateFile
                    .Generate(this.FileSystem, bicepCliProxy, mainBicepFile)
                    .WriteToFileSystem(FileSystem));

                // Read or create metadata file.
                this.Logger.LogInformation("Ensuring {MetadataFile} exists...", "metadata file");
                var metadataFile = MetadataFile.EnsureInFileSystem(this.FileSystem);

                // Generate README file.
                this.GenerateFileAndLogInformation("README file", () => ReadmeFile
                    .Generate(this.FileSystem, metadataFile, mainArmTemplateFile)
                    .WriteToFileSystem(this.FileSystem));

                // Generate version file.
                this.GenerateFileAndLogInformation("version file", () => VersionFile
                    .Generate(this.FileSystem)
                    .WriteToFileSystem(this.FileSystem));

                return 0;
            }

            private T GenerateFileAndLogInformation<T>(string fileFriendlyName, Func<T> fileGenerator) where T : ModuleFile
            {
                this.Logger.LogInformation("Generating {FileFriendlyName}..", fileFriendlyName);
                var file = fileGenerator();
                this.Logger.LogInformation("Wrote {FileFriendlyName} to \"{FilePath}\".", fileFriendlyName, file.Path);

                return file;
            }
        }
    }
}
