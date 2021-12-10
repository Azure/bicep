// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Threading;

namespace Bicep.RegistryModuleTool.Proxies
{
    public sealed class ProcessProxy : IProcessProxy
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

            string standardOutput = "";
            string standardError = "";

            Thread readStandardOutputThread = new(() => { standardOutput = process.StandardOutput.ReadToEnd(); });
            Thread readStandardErrorThread = new(() => { standardError = process.StandardError.ReadToEnd(); });

            readStandardOutputThread.Start();
            readStandardErrorThread.Start();

            process.WaitForExit();
            readStandardOutputThread.Join();
            readStandardErrorThread.Join();


            return (process.ExitCode, standardOutput, standardError);
        }
    }
}
