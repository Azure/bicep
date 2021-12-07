// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Commands
{
    internal sealed class ValidateCommand : Command
    {
        public ValidateCommand(string name, string description)
            : base(name, description)
        {
        }

        public sealed class CommandHandler : BaseCommandHandler
        {
            public CommandHandler(IFileSystem fileSystem, ILogger<ValidateCommand> logger)
                : base(fileSystem, logger)
            {
            }

            protected override void InvokeInternal(InvocationContext context)
            {

                var mainBicepFile = MainBicepFile.ReadFromFileSystem(this.FileSystem);
                var descriptionsValidator = new DescriptionsValidator(this.FileSystem, this.Logger);

                mainBicepFile.ValidatedBy(descriptionsValidator);

                var jsonSchemaValidator = new JsonSchemaValidator();
                var diffValidator = new DiffValidator(this.FileSystem, this.Logger, mainBicepFile);

                MainArmTemplateFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator);
                MainArmTemplateParametersFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator, diffValidator);
                MetadataFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(jsonSchemaValidator);
                VersionFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator);
                ReadmeFile.ReadFromFileSystem(this.FileSystem).ValidatedBy(diffValidator);
            }
        }
    }


}
