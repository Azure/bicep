// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bicep.RegistryModuleTool.ModuleFiles;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public interface IModuleFileValidator
    {
        Task<IEnumerable<string>> ValidateAsync(VersionFile file) => throw new NotImplementedException();

        Task<IEnumerable<string>> ValidateAsync(MainBicepFile file) => throw new NotImplementedException();

        Task<IEnumerable<string>> ValidateAsync(MainBicepTestFile file) => throw new NotImplementedException();

        Task<IEnumerable<string>> ValidateAsync(MainArmTemplateFile file) => throw new NotImplementedException();

        Task<IEnumerable<string>> ValidateAsync(ReadmeFile file) => throw new NotImplementedException();
    }
}
