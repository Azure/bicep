// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
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
using System.IO.Abstractions;
using System.Linq;
using System.Security;

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

                this.Logger.LogInformation("Validting that the module path is in lowercase...");
                valid &= Validate(context.Console, () => ValidateModulePathInLowercase(this.FileSystem));

                this.Logger.LogInformation("Validating main Bicep file...");

                var bicepCliProxy = new BicepCliProxy(this.environmentProxy, this.processProxy, this.FileSystem, this.Logger, context.Console);
                var mainBicepFile = MainBicepFile.ReadFromFileSystem(this.FileSystem);

                // This also validates that the main Bicep file can be built without errors.
                var latestMainArmTemplateFile = MainArmTemplateFile.Generate(this.FileSystem, bicepCliProxy, mainBicepFile);
                var descriptionsValidator = new DescriptionsValidator(this.Logger, latestMainArmTemplateFile);

                valid &= Validate(context.Console, () => mainBicepFile.ValidatedBy(descriptionsValidator));

                var testValidator = new TestValidator(this.FileSystem, this.Logger, bicepCliProxy, latestMainArmTemplateFile);
                var jsonSchemaValidator = new JsonSchemaValidator(this.Logger);
                var diffValidator = new DiffValidator(this.FileSystem, this.Logger, latestMainArmTemplateFile);

                this.Logger.LogInformation("Validating main Bicep test file...");
                valid &= Validate(context.Console, () => MainBicepTestFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(testValidator));

                this.Logger.LogInformation("Validating main ARM template file...");
                valid &= Validate(context.Console, () => MainArmTemplateFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator));

                this.Logger.LogInformation("Validating metadata file...");
                valid &= Validate(context.Console, () => MetadataFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator));

                this.Logger.LogInformation("Validating README file...");
                valid &= Validate(context.Console, () => ReadmeFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator));

                this.Logger.LogInformation("Validating version file...");
                valid &= Validate(context.Console, () => VersionFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator));

                return valid ? 0 : 1;
            }

            private static void ValidateModulePathInLowercase(IFileSystem fileSystem)
            {
                var directoryPath = fileSystem.Directory.GetCurrentDirectory();
                var directoryInfo = fileSystem.DirectoryInfo.FromDirectoryName(directoryPath);
                var directoryStack = new Stack<string>();

                try
                {

                    while (directoryInfo is not null && !directoryInfo.Name.Equals("modules", StringComparison.OrdinalIgnoreCase))
                    {
                        directoryStack.Push(directoryInfo.Name);

                        directoryInfo = directoryInfo.Parent;
                    }

                    if (directoryInfo is null)
                    {
                        throw new InvalidModuleException($"Could not find the \"modules\" folder in the path \"{directoryPath}\".");
                    }
                }
                catch (SecurityException exception)
                {
                    throw new BicepException(exception.Message, exception);
                }

                var modulePath = string.Join(fileSystem.Path.DirectorySeparatorChar, directoryStack.ToArray());

                if (modulePath.Any(char.IsUpper))
                {
                    throw new InvalidModuleException($"The module path \"{modulePath}\" in the path \"{directoryPath}\" must be in lowercase.");
                }
            }

            private static bool Validate(IConsole console, Action validateAction)
            {
                try
                {
                    validateAction();
                }
                catch (InvalidModuleException exception)
                {
                    // Normalize the error message to make it always end with a new line.
                    var normalizedErrorMessage = exception.Message.ReplaceLineEndings();

                    if (!normalizedErrorMessage.EndsWith(Environment.NewLine))
                    {
                        normalizedErrorMessage = $"{normalizedErrorMessage}{Environment.NewLine}";
                    }

                    console.WriteError(normalizedErrorMessage);

                    return false;
                }

                return true;
            }
        }
    }
}
