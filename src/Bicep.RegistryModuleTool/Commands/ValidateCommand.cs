// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
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
    public sealed class ValidateCommand : Command
    {
        public ValidateCommand()
            : base("validate", "Validate files for the Bicep registry module")
        {
        }

        public sealed class CommandHandler : BaseCommandHandler
        {
            private readonly BicepCompiler compiler;

            public CommandHandler(IFileSystem fileSystem, BicepCompiler compiler, ILogger<ValidateCommand> logger)
                : base(fileSystem, logger)
            {
                this.compiler = compiler;
            }

            protected override async Task<int> InvokeInternalAsync(InvocationContext context)
            {
                var valid = true;

                this.Logger.LogInformation("Validating main Bicep file...");

                var mainBicepFile = await MainBicepFile.OpenAsync(this.FileSystem, this.compiler, context.Console);
                var descriptionsValidator = new DescriptionsValidator(this.Logger);
                var metadataValidator = new BicepMetadataValidator(this.Logger);
                valid &= await ValidateFileAsync(context.Console, () => mainBicepFile.ValidatedByAsync(descriptionsValidator, metadataValidator));

                this.Logger.LogInformation("Validating main Bicep test file...");

                var mainBicepTestFile = MainBicepTestFile.Open(this.FileSystem);
                var testValidator = new TestValidator(this.Logger, context.Console, this.compiler, mainBicepFile);
                valid &= await ValidateFileAsync(context.Console, () => mainBicepTestFile.ValidatedByAsync(testValidator));

                this.Logger.LogInformation("Validating main ARM template file...");

                var diffValidator = new DiffValidator(this.FileSystem, this.Logger, mainBicepFile);
                var mainArmTemplateFile = await MainArmTemplateFile.OpenAsync(this.FileSystem);
                valid &= await ValidateFileAsync(context.Console, () => mainArmTemplateFile.ValidatedByAsync(diffValidator));

                this.Logger.LogInformation("Validating README file...");

                var readmeFile = await ReadmeFile.OpenAsync(this.FileSystem);
                valid &= await ValidateFileAsync(context.Console, () => readmeFile.ValidatedByAsync(diffValidator));

                this.Logger.LogInformation("Validating version file...");

                var versionFile = await VersionFile.OpenAsync(this.FileSystem);
                var jsonSchemaValidator = new JsonSchemaValidator(this.Logger);
                valid &= await ValidateFileAsync(context.Console, () => versionFile.ValidatedByAsync(jsonSchemaValidator, diffValidator));

                return valid ? 0 : 1;
            }

            private static async Task<bool> ValidateFileAsync(IConsole console, Func<Task> fileValidator)
            {
                try
                {
                    await fileValidator();
                }
                catch (InvalidModuleFileException exception)
                {
                    console.WriteError(exception.Message);

                    return false;
                }

                return true;
            }
        }
    }
}
