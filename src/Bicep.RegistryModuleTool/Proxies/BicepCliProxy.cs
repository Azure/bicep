// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public BicepCliProxy(IEnvironmentProxy environmentProxy, IProcessProxy processProxy, IFileSystem fileSystem, ILogger logger)
        {
            this.environmentProxy = environmentProxy;
            this.processProxy = processProxy;
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void Build(string bicepFilePath, string outputFilePath)
        {
            this.logger.LogDebug("Building \"{BicepFilePath}\"...", bicepFilePath);

            var bicepCliPath = this.LocateBicepCli();
            var command = $"build \"{bicepFilePath}\" --outfile \"{outputFilePath}\"";

            var (exitCode, _, standardError) = this.processProxy.Start(bicepCliPath, command);

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
