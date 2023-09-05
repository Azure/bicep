// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.Workspaces;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public class TestValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        private readonly IConsole console;

        private readonly BicepCompiler compiler;

        private readonly MainBicepFile latestMainBicepFile;

        public TestValidator(ILogger logger, IConsole console, BicepCompiler compiler, MainBicepFile latestMainBicepFile)
        {
            this.logger = logger;
            this.console = console;
            this.compiler = compiler;
            this.latestMainBicepFile = latestMainBicepFile;
        }

        public async Task<IEnumerable<string>> ValidateAsync(MainBicepTestFile file)
        {
            this.logger.LogInformation("Making sure the test file contains at least one test...");

            // Skip writting diagnostics to avoid printing duplicated diagnostics for main.bicep.
            var compilation = await this.compiler.CompileAsync(file.Path, this.console, writeDiagnostics: false);
            var hasMainBicepFileReference = compilation.SourceFileGrouping.SourceFiles
                .Any(x => x.FileUri.LocalPath.Equals(this.latestMainBicepFile.Path, StringComparison.Ordinal));

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
