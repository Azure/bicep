// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Cli.UnitTests;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;

namespace Bicep.Cli.IntegrationTests
{
    public static class CliHelper
    {
        public static (IEnumerable<string> outputLines, IEnumerable<string> errorLines, int result) ExecuteProgram(params string[] args)
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((outputWriter, errorWriter) =>
            {
                var program = new Program(TestTypeHelper.CreateEmptyProvider(), outputWriter, errorWriter, BicepTestConstants.DevAssemblyFileVersion);

                return program.Run(args);
            });

            return (
                Regex.Split(output, "\r?\n").Where(x => x != ""),
                Regex.Split(error, "\r?\n").Where(x => x != ""),
                result);
        }
    }
}