// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Utils;
using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Json
{
    public static class JsonAssert
    {
        public static void AreEqual(JToken expected, JToken actual)
        {
            if (JToken.DeepEquals(expected, actual))
            {
                return;
            }

            var delta = GetDelta(expected, actual);
            
            throw new AssertFailedException($"The specified JSON objects are not equal.\r\nExpected:\r\n{expected}\r\nActual:\r\n{actual}\r\nDelta:{delta}");
        }

        public static void AreEqual(JToken expected, JToken actual, TestContext testContext, string deltaFileName)
        {
            if (JToken.DeepEquals(expected, actual))
            {
                return;
            }

            var delta = GetDelta(expected, actual);
            var filePath = FileHelper.SaveResultFile(testContext, deltaFileName, delta.ToString(Formatting.Indented));

            throw new AssertFailedException($"The specified JSON objects are not equal. See results file '{filePath}' for more details.");
        }

        private static JToken GetDelta(JToken expected, JToken actual)
        {
            return new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(expected, actual);
        }

        public static JToken Diff(JToken one, JToken two) => new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(one, two);
    }
}
