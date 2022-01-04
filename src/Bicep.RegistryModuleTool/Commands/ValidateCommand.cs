// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
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
        public ValidateCommand()
            : base("validate", "Validate files for the Bicep registry module")
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

            protected override int Invoke(InvocationContext context)
            {
                var valid = true;

                this.Logger.LogInformation("Validating that no additional files are in the module folder...");
                valid &= Validate(context.Console, () => this.ValidateNoAdditionalFiles());

                this.Logger.LogInformation("Validating main Bicep file...");

                var bicepCliProxy = new BicepCliProxy(this.environmentProxy, this.processProxy, this.FileSystem, this.Logger, context.Console);
                var mainBicepFile = MainBicepFile.ReadFromFileSystem(this.FileSystem);

                // This also validates that the main Bicep file can be built without errors.
                var latestMainArmTemplateFile = MainArmTemplateFile.Generate(this.FileSystem, bicepCliProxy, mainBicepFile);
                var descriptionsValidator = new DescriptionsValidator(this.Logger, latestMainArmTemplateFile);

                valid &= Validate(context.Console, () => mainBicepFile.ValidatedBy(descriptionsValidator));

                var jsonSchemaValidator = new JsonSchemaValidator( this.Logger);
                var diffValidator = new DiffValidator(this.FileSystem, this.Logger, latestMainArmTemplateFile);

                this.Logger.LogInformation("Validating main ARM template file...");
                valid &= Validate(context.Console, () => MainArmTemplateFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator));

                this.Logger.LogInformation("Validating main ARM template parameters file...");
                valid &= Validate(context.Console, () => MainArmTemplateParametersFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator, diffValidator));

                this.Logger.LogInformation("Validating metadata file...");
                valid &= Validate(context.Console, () => MetadataFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator));

                this.Logger.LogInformation("Validating README file...");
                valid &= Validate(context.Console, () => ReadmeFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator));

                this.Logger.LogInformation("Validating version file...");
                valid &= Validate(context.Console, () => VersionFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator));

                return valid ? 0 : 1;
            }

            private static bool Validate(IConsole console, Action validateAction)
            {
                try
                {
                    validateAction();
                }
                catch (InvalidModuleFileException exception)
                {
                    console.WriteError(exception.Message);

                    return false;
                }

                return true;
            }

            private void ValidateNoAdditionalFiles()
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
                    errorMessageBuilder.AppendLine("The files below are not allowed in the module:");

                    foreach (var filePath in notAllowedFilePaths)
                    {
                        errorMessageBuilder.Append("  - ").AppendLine(filePath);
                    }

                    throw new InvalidModuleFileException(errorMessageBuilder.ToString());
                }
            }
        }
    }
}
