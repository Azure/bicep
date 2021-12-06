// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    internal interface IModuleFileValidator
    {
        void Validate(VersionFile file) { }

        void Validate(MetadataFile file) { }

        void Validate(MainBicepFile file) { }

        void Validate(MainArmTemplateFile file) { }

        void Validate(MainArmTemplateParametersFile file) { }

        void Validate(ReadmeFile file) { }
    }
}
