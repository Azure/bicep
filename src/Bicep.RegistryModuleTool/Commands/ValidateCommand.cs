// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Bicep.RegistryModuleTool.Commands
{
    public sealed class ValidateCommand : Command
    {
        public ValidateCommand(string name, string description)
            : base(name, description)
        {
        }

        public sealed class CommandHandler : BaseCommandHandler
        {
            private readonly static IReadOnlyList<string> AllowedFileNames = new[]
            {
                MainBicepFile.FileName,
                MainArmTemplateFile.FileName,
                MainArmTemplateParametersFile.FileName,
                MetadataFile.FileName,
                ReadmeFile.FileName,
                VersionFile.FileName,
                "bicepconfig.json"
            };

            private readonly IEnvironmentProxy environmentProxy;

            private readonly IProcessProxy processProxy;

            public CommandHandler(IEnvironmentProxy environmentProxy, IProcessProxy processProxy, IFileSystem fileSystem, ILogger<ValidateCommand> logger)
                : base(fileSystem, logger)
            {
                this.environmentProxy = environmentProxy;
                this.processProxy = processProxy;
            }

            protected override void InvokeInternal(InvocationContext context)
            {
                this.Logger.LogDebug("Validating that no additional files are in the module folder...");
                this.EnsureNoAdditionalFiles();

                this.Logger.LogDebug("Validating main Bicep file...");

                var bicepCliProxy = new BicepCliProxy(this.environmentProxy, this.processProxy, this.FileSystem, this.Logger);
                var mainBicepFile = MainBicepFile.ReadFromFileSystem(this.FileSystem);

                // This also validates that the main Bicep file can be built.
                var latestMainArmTemplateFile = MainArmTemplateFile.Generate(this.FileSystem, bicepCliProxy, mainBicepFile);
                var descriptionsValidator = new DescriptionsValidator(this.Logger, latestMainArmTemplateFile);

                mainBicepFile.ValidatedBy(descriptionsValidator);

                var jsonSchemaValidator = new JsonSchemaValidator(this.Logger);
                var diffValidator = new DiffValidator(this.FileSystem, this.Logger, latestMainArmTemplateFile);

                this.Logger.LogDebug("Validating main ARM template file...");
                MainArmTemplateFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator);

                this.Logger.LogDebug("Validating main ARM template parameters file...");
                MainArmTemplateParametersFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator, diffValidator);

                this.Logger.LogDebug("Validating metadata file...");
                MetadataFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator);

                this.Logger.LogDebug("Validating README file...");
                ReadmeFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator);

                this.Logger.LogDebug("Validating version file...");
                VersionFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator);
            }

            private void EnsureNoAdditionalFiles()
            {
                var currentDirectoryPath = this.FileSystem.Directory.GetCurrentDirectory();
                var filePaths = this.FileSystem.Directory.EnumerateFiles(currentDirectoryPath, "*", SearchOption.AllDirectories);
                var allowedFilePaths = AllowedFileNames
                    .Select(fileName => this.FileSystem.Path.Combine(currentDirectoryPath, fileName))
                    .ToHashSet();

                var notAllowedFilePaths = new List<string>();

                foreach (var filePath in filePaths)
                {
                    if (allowedFilePaths.Contains(filePath))
                    {
                        continue;
                    }

                    if (filePath.EndsWith(LanguageConstants.LanguageFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        // Any Bicep file is allowed.
                        continue;
                    }

                    notAllowedFilePaths.Add(filePath);
                }

                if (notAllowedFilePaths.Any())
                {
                    var errorMessageBuilder = new StringBuilder();
                    errorMessageBuilder.AppendLine("The files below are not allowed:");

                    foreach (var filePath in notAllowedFilePaths)
                    {
                        errorMessageBuilder.Append("  - ").AppendLine(filePath);
                    }

                    throw new BicepException(errorMessageBuilder.ToString());
                }
            }
        }
    }
}
