// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Cli.IntegrationTests
{
    public static class CliHelpers
    {
        public static IEnumerable<string> AsLines(this string output)
        {
            return Regex.Split(output, "\r?\n").Where(x => x != "");
        }
    }
}
