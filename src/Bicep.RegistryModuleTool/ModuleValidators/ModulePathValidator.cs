// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Exceptions;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public static class ModulePathValidator
    {
        public static void ValidateModulePath(IFileSystem fileSystem)
        {
            var directoryPath = fileSystem.Directory.GetCurrentDirectory();
            var modulePath = GetModulePath(fileSystem, directoryPath);

            if (modulePath.Count(x => x == fileSystem.Path.DirectorySeparatorChar) != 1)
            {
                string modulePathFormat = $"<module-folder>{fileSystem.Path.DirectorySeparatorChar}<module-name>";

                throw new InvalidModuleException($"The module path \"{modulePath}\" in the path \"{directoryPath}\" is invalid. The module path must be in the format of \"{modulePathFormat}\".");
            }

            if (modulePath.Any(char.IsUpper))
            {
                throw new InvalidModuleException($"The module path \"{modulePath}\" in the path \"{directoryPath}\" is invalid. All characters in the module path must be in lowercase.");
            }
        }

        private static string GetModulePath(IFileSystem fileSystem, string directoryPath)
        {
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

            return modulePath;
        }
    }
}
