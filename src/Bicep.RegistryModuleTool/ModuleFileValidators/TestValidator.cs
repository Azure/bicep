// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Core;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public class TestValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        private readonly IConsole console;

        private readonly BicepCompiler compiler;

        private readonly MainBicepFile mainBicepFile;

        public TestValidator(ILogger logger, IConsole console, BicepCompiler compiler, MainBicepFile mainBicepFile)
        {
            this.logger = logger;
            this.console = console;
            this.compiler = compiler;
            this.mainBicepFile = mainBicepFile;
        }

        public async Task<IEnumerable<string>> ValidateAsync(MainBicepTestFile file)
        {
            this.logger.LogInformation("Making sure the test file contains at least one test...");

            var compilation = await this.compiler.CompileAsync(file.Path, this.console, this.mainBicepFile.Path);
            var hasMainBicepFileReference = compilation.SourceFileGrouping.SourceFiles
                .Any(x => x.FileHandle.Uri.GetFilePath().Equals(this.mainBicepFile.Path, StringComparison.Ordinal));

            if (!hasMainBicepFileReference)
            {
                return NoTestError();
            }

            return [];
        }

        private static IEnumerable<string> NoTestError()
        {
            yield return "Could not find tests in the file. Please make sure to add at least one module referencing the main Bicep file.";
        }
    }
}
