// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;

namespace Bicep.RegistryModuleTool.Proxies
{
    internal sealed class ProcessProxy : IProcessProxy
    {
        public (int exitCode, string standardOutput, string standardError) Start(string executablePath, string arguments)
        {
            var process = new Process();

            process.StartInfo.FileName = executablePath;
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
    }
}
