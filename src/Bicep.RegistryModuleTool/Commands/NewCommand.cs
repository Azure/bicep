// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.Commands
{
    public sealed class NewCommand : Command
    {
        public NewCommand(string name, string description)
            : base(name, description)
        {
        }

        public sealed class CommandHandler : BaseCommandHandler
        {
            public CommandHandler(IFileSystem fileSystem, ILogger<NewCommand> logger)
                : base(fileSystem, logger)
            {
            }

            protected override void InvokeInternal(InvocationContext context)
            {
                MainBicepFile.CreateInFileSystem(this.FileSystem);
                MetadataFile.CreateInFileSystem(this.FileSystem);
            }
        }
    }

}
