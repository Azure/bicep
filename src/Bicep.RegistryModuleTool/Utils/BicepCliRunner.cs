// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;

namespace Bicep.RegistryModuleTool.Utils
{
    internal class BicepCliRunner
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        public BicepCliRunner(IFileSystem fileSystem, ILogger logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void BuildBicepFile(string bicepFilePath, string? outputFilePath = null)
        {
            this.logger.LogDebug("Building \"{BicepFilePath}\"...", bicepFilePath);

            var command = outputFilePath is null
                ? $"build \"{bicepFilePath}\""
                : $"build \"{bicepFilePath}\" --outfile \"{outputFilePath}\"";

            var (exitCode, standardOutput, standardError) = RunBicepCommand(command);

            if (exitCode == 0 && !string.IsNullOrEmpty(standardOutput))
            {
                this.logger.LogWarning("{BicepBuildWarnings}", standardOutput);

                return;
            }

            if (exitCode != 0)
            {
                if (!string.IsNullOrEmpty(standardError))
                {
                    // TODO: log warnings separately.
                    this.logger.LogError("{BicepBuildErrors}", standardError);
                }

                throw new BicepException($"Failed to build \"{bicepFilePath}\"");
            }
        }

        private (int exitCode, string standardOutput, string standardError) RunBicepCommand(string arguments)
        {
            var process = new Process();

            process.StartInfo.FileName = this.LocateBicepCli();
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var standardError = process.StandardError.ReadToEnd();
            var standardOutput = process.StandardOutput.ReadToEnd();

            process.WaitForExit();


            return (process.ExitCode, standardOutput, standardError);
        }

        private string LocateBicepCli()
        {
            var bicepExecutableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";

            var directoriesToSearch =
                Environment.GetEnvironmentVariable("PATH")?.Split(this.fileSystem.Path.PathSeparator) ??
                Enumerable.Empty<string>();

            if (Environment.GetEnvironmentVariable("AZURE_CONFIG_DIR") is { } azureConfigurationDirectory)
            {
                directoriesToSearch = directoriesToSearch.Append(this.fileSystem.Path.Combine(azureConfigurationDirectory, "bin"));
            }

            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            directoriesToSearch = directoriesToSearch.Append(this.fileSystem.Path.Combine(homeDirectory, ".azure", "bin"));

            foreach (var directory in directoriesToSearch)
            {
                var bicepCliPath = this.fileSystem.Path.Combine(directory, bicepExecutableName);

                if (this.fileSystem.File.Exists(bicepCliPath))
                {
                    return bicepCliPath;
                }
            }

            throw new BicepException("Failed to locate Bicep CLI. Please make sure to add Bicep CLI to PATH, or install it via Azure CLI.");
        }
    }
}
