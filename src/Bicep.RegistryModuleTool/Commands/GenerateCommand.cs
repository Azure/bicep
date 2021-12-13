// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Commands
{
    public sealed class GenerateCommand : Command
    {
        public GenerateCommand(string name, string description)
            : base(name, description)
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

            protected override void InvokeInternal(InvocationContext context)
            {
                // Generate main ARM template file.
                this.Logger.LogDebug("Generating main ARM template file...");
                var bicepCliProxy = new BicepCliProxy(this.environmentProxy, this.processProxy, this.FileSystem, this.Logger);
                var mainBicepFile = MainBicepFile.ReadFromFileSystem(this.FileSystem);
                var mainArmTemplateFile = MainArmTemplateFile.Generate(this.FileSystem, bicepCliProxy, mainBicepFile);

                this.Logger.LogDebug("Writing main ARM template file to \"{MainArmTemplateFilePath}\"...", mainArmTemplateFile.Path);
                mainArmTemplateFile.WriteToFileSystem(FileSystem);

                // Generate main ARM template parameters file.
                this.Logger.LogDebug("Generating main ARM template parameters file...");
                var mainArmTemplateParametersFile = MainArmTemplateParametersFile.Generate(this.FileSystem, mainArmTemplateFile);

                this.Logger.LogDebug("Writing main ARM template parameters file to \"{MainArmTemplateParametersFilePath}\"...", mainArmTemplateParametersFile.Path);
                mainArmTemplateParametersFile.WriteToFileSystem(FileSystem);

                // Generate README file.
                this.Logger.LogDebug("Generating README file...");
                var metadataFile = MetadataFile.ReadFromFileSystem(this.FileSystem);
                var readmeFile = ReadmeFile.Generate(this.FileSystem, metadataFile, mainArmTemplateFile);

                this.Logger.LogDebug("Writing README file to \"{ReadmeFilePath}\"...", readmeFile.Path);
                readmeFile.WriteToFileSystem(this.FileSystem);

                // Generate version file.
                this.Logger.LogDebug("Generating version file...");
                var versionFile = VersionFile.Generate(this.FileSystem);

                this.Logger.LogDebug("Writing version file to \"{VersionFilePath}\"...", versionFile.Path);
                versionFile.WriteToFileSystem(this.FileSystem);
            }
        }
    }
}
