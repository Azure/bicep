// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.Core;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public class TestValidator(ILogger logger, IConsole console, BicepCompiler compiler, MainBicepFile mainBicepFile) : IModuleFileValidator
    {
        private readonly ILogger logger = logger;

        private readonly IConsole console = console;

        private readonly BicepCompiler compiler = compiler;

        private readonly MainBicepFile mainBicepFile = mainBicepFile;

        public async Task<IEnumerable<string>> ValidateAsync(MainBicepTestFile file)
        {
            this.logger.LogInformation("Making sure the test file contains at least one test...");

            var compilation = await this.compiler.CompileAsync(file.Path, this.console, this.mainBicepFile.Path);
            var hasMainBicepFileReference = compilation.SourceFileGrouping.SourceFiles
                .Any(x => x.FileUri.LocalPath.Equals(this.mainBicepFile.Path, StringComparison.Ordinal));

            if (!hasMainBicepFileReference)
            {
                return NoTestError();
            }

            return Enumerable.Empty<string>();
        }

        private static IEnumerable<string> NoTestError()
        {
            yield return "Could not find tests in the file. Please make sure to add at least one module referencing the main Bicep file.";
        }
    }
}
