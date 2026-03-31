// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Security;
using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Microsoft.Extensions.Logging;

namespace Bicep.RegistryModuleTool.Commands
{
    public abstract class BaseCommandHandler
    {
        protected BaseCommandHandler(IFileSystem fileSystem, ILogger logger)
        {
            this.FileSystem = fileSystem;
            this.Logger = logger;
        }

        protected IFileSystem FileSystem { get; }

        protected ILogger Logger { get; }

        public async Task<int> InvokeAsync(IConsole console, CancellationToken cancellationToken = default)
        {
            try
            {
                // TODO: Remove this check from brm and implement it in CI.
                // The AVM team is going to add a "pattern" folder, and checking
                // for "modules" will prevent them from using brm. Removing the
                // check also allows customers to reuse brm to implement their
                // own module registry that does not necessrily enforces the same
                // module folder structure.
                this.ValidateWorkingDirectoryPath();

                return await this.InvokeInternalAsync(console, cancellationToken);
            }
            catch (Exception exception)
            {
                switch (exception)
                {
                    case BicepException:
                    case IOException:
                    case UnauthorizedAccessException:
                        this.Logger.LogDebug(exception, "Command failure.");
                        console.WriteError(exception.Message);

                        break;

                    default:
                        this.Logger.LogCritical(exception, "Unexpected exception.");
                        break;
                }

                return 1;
            }
        }

        protected abstract Task<int> InvokeInternalAsync(IConsole console, CancellationToken cancellationToken);

        private void ValidateWorkingDirectoryPath()
        {
            var directoryPath = this.FileSystem.Directory.GetCurrentDirectory();
            var modulePath = this.GetModulePath(directoryPath);

            if (modulePath.Count(x => x == this.FileSystem.Path.DirectorySeparatorChar) != 1)
            {
                string modulePathFormat = $"<module-folder>{this.FileSystem.Path.DirectorySeparatorChar}<module-name>";

                throw new BicepException($"The module path \"{modulePath}\" in the path \"{directoryPath}\" is invalid. The module path must be in the format of \"{modulePathFormat}\".");
            }

            if (modulePath.Any(char.IsUpper))
            {
                throw new BicepException($"The module path \"{modulePath}\" in the path \"{directoryPath}\" is invalid. All characters in the module path must be in lowercase.");
            }
        }

        private string GetModulePath(string directoryPath)
        {
            var directoryInfo = this.FileSystem.DirectoryInfo.New(directoryPath);
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
                    throw new BicepException($"Could not find the \"modules\" folder in the path \"{directoryPath}\".");
                }
            }
            catch (SecurityException exception)
            {
                throw new BicepException(exception.Message, exception);
            }

            var modulePath = string.Join(this.FileSystem.Path.DirectorySeparatorChar, [.. directoryStack]);

            return modulePath;
        }
    }
}
