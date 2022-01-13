// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Abstractions;

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
        }
    }
}
