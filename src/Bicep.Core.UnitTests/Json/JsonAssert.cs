using JsonDiffPatchDotNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var delta = new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(expected, actual);

            throw new AssertFailedException($"The specified JSON objects are not equal.\r\nExpected:\r\n{expected}\r\nActual:\r\n{actual}\r\nDelta:{delta}");
        }

        public static JToken Diff(JToken one, JToken two) => new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple }).Diff(one, two);
    }
}
