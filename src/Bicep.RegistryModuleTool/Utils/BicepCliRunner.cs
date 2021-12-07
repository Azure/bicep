// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Bicep.RegistryModuleTool.Utils
{
    internal sealed class BicepCliRunner
    {
        private readonly static Regex BicepBuildWarningRegex = new(
            @"^([^\s].*)\((\d+)(?:,\d+|,\d+,\d+)?\)\s+:\s+(Warning)\s+([a-zA-Z-\d]+):\s*(.*?)\s+\[(.*?)\]$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly static string[] LineSeperators = new[] { "\r", "\n", "\r\n" };

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

            var (exitCode, _, standardError) = RunBicepCommand(command);

            if (exitCode == 0 && !string.IsNullOrEmpty(standardError))
            {
                this.logger.LogWarning("{BicepBuildWarnings}", standardError);

                return;
            }

            if (exitCode != 0)
            {
                if (!string.IsNullOrEmpty(standardError))
                {
                    var warnings = new List<string>();
                    var errors = new List<string>();

                    foreach (var line in standardError.Split(LineSeperators, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (BicepBuildWarningRegex.IsMatch(line))
                        {
                            warnings.Add(line);
                        }
                        else
                        {
                            errors.Add(line);
                        }
                    }

                    if (warnings.Count > 0)
                    {
                        this.logger.LogWarning("{BicepBuildWarnings}", string.Join(Environment.NewLine, warnings));
                    }


                    this.logger.LogError("{BicepBuildErrors}", string.Join(Environment.NewLine, errors));
                }

                throw new BicepException($"Failed to build \"{bicepFilePath}\".");
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

            // TODO: fix deadlock.
            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();

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
