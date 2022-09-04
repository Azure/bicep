// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Proxies;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Abstractions;
using System.Linq;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public class TestValidator : IModuleFileValidator
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        private readonly BicepCliProxy bicepCliProxy;

        private readonly MainArmTemplateFile latestMainArmTemplateFile;

        public TestValidator(IFileSystem fileSystem, ILogger logger, BicepCliProxy bicepCliProxy, MainArmTemplateFile latestMainArmTemplateFile)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.bicepCliProxy = bicepCliProxy;
            this.latestMainArmTemplateFile = latestMainArmTemplateFile;
        }

        public void Validate(MainBicepTestFile file)
        {
            var tempFilePath = this.fileSystem.Path.GetTempFileName();

            try
            {
                bicepCliProxy.Build(file.Path, tempFilePath);
            }
            catch (Exception exception)
            {
                this.fileSystem.File.Delete(tempFilePath);

                if (exception is BicepException)
                {
                    // Failed to build the test file. Convert it to InvalidModuleException so that
                    // the validate command can continue validating other files.
                    throw new InvalidModuleException(exception.Message, exception);
                }

                throw;
            }

            this.logger.LogInformation("Making sure the test file contains at least one test...");

            using var tempFileStream = fileSystem.FileStream.CreateDeleteOnCloseStream(tempFilePath);
            var testTemplateElement = JsonElementFactory.CreateElement(tempFileStream);
            var testDeployments = testTemplateElement.Select($@"$..resources[?(@.type == ""Microsoft.Resources/deployments"" && @.properties.template.metadata._generator.templateHash == ""{this.latestMainArmTemplateFile.TemplateHash}"")]");

            if (testDeployments.IsEmpty())
            {
                throw new InvalidModuleException($"The file \"{file.Path}\" is invalid. Could not find tests in the file. Please make sure to add at least one module referencing the main Bicep file.");
            }
        }
    }
}
