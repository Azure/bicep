// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Bicep.RegistryModuleTool.Proxies
{
    public sealed class BicepCliProxy
    {
        private readonly static Regex BicepBuildWarningRegex = new(
            @"^([^\s].*)\((\d+)(?:,\d+|,\d+,\d+)?\)\s+:\s+(Warning)\s+([a-zA-Z-\d]+):\s*(.*?)\s+\[(.*?)\]$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly static string[] LineSeperators = new[] { "\r", "\n", "\r\n" };

        private readonly IEnvironmentProxy environmentProxy;

        private readonly IProcessProxy processProxy;

        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        private readonly IConsole console;

        public BicepCliProxy(IEnvironmentProxy environmentProxy, IProcessProxy processProxy, IFileSystem fileSystem, ILogger logger, IConsole console)
        {
            this.environmentProxy = environmentProxy;
            this.processProxy = processProxy;
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.console = console;
        }

        public void Build(string bicepFilePath, string outputFilePath)
        {
            this.logger.LogInformation("Building \"{BicepFilePath}\"...", bicepFilePath);

            var bicepCliPath = this.LocateBicepCli();
            var command = $"build \"{bicepFilePath}\" --outfile \"{outputFilePath}\"";

            this.logger.LogDebug("Running Bicep CLI command: {Command}", command);
            var (exitCode, _, standardError) = this.processProxy.Start(bicepCliPath, command);

            if (exitCode is 0)
            {
                if (standardError.Length > 0)
                {
                    foreach (var warning in standardError.Split(LineSeperators, StringSplitOptions.RemoveEmptyEntries))
                    {
                        console.WriteWarning(warning);
                    }

                    console.Out.WriteLine();
                }

                return;
            }

            // Exit code is not 0. There exists errors.
            foreach (var line in standardError.Split(LineSeperators, StringSplitOptions.RemoveEmptyEntries))
            {
                if (BicepBuildWarningRegex.IsMatch(line))
                {
                    console.WriteWarning(line);
                }
                else
                {
                    console.WriteError(line);
                }
            }

            throw new BicepException($"Failed to build \"{bicepFilePath}\".");
        }

        private string LocateBicepCli()
        {
            var bicepExecutableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";

            var directoriesToSearch =
                this.environmentProxy.GetEnvironmentVariable("PATH")?.Split(this.fileSystem.Path.PathSeparator) ??
                Enumerable.Empty<string>();

            if (this.environmentProxy.GetEnvironmentVariable("AZURE_CONFIG_DIR") is { } azureConfigurationDirectory)
            {
                directoriesToSearch = directoriesToSearch.Append(this.fileSystem.Path.Combine(azureConfigurationDirectory, "bin"));
            }

            var homeDirectory = this.environmentProxy.GetHomeDirectory();
            directoriesToSearch = directoriesToSearch.Append(this.fileSystem.Path.Combine(homeDirectory, ".azure", "bin"));

            foreach (var directory in directoriesToSearch)
            {
                var bicepCliPath = this.fileSystem.Path.Combine(directory, bicepExecutableName);

                if (this.fileSystem.File.Exists(bicepCliPath))
                {
                    this.logger.LogDebug("Found Bicep CLI at \"{BicepCliPath}\".", bicepCliPath);

                    return bicepCliPath;
                }
            }

            throw new BicepException("Failed to locate Bicep CLI. Please make sure to add Bicep CLI to PATH, or install it via Azure CLI.");
        }
    }
}
