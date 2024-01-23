// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    public class CompletionDataHelper
    {
        public static IEnumerable<object[]> GetSnippetCompletionData()
        {
            Assembly languageServerAssembly = Assembly.Load("Bicep.LangServer");
            IEnumerable<string> manifestResourceNames = languageServerAssembly.GetManifestResourceNames()
                .Where(p => p.EndsWith(".bicep", StringComparison.Ordinal));

            foreach (var manifestResourceName in manifestResourceNames)
            {
                Stream? stream = languageServerAssembly.GetManifestResourceStream(manifestResourceName);
                StreamReader streamReader = new(stream!);

                string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
                CompletionData completionData = new(prefix, streamReader.ReadToEnd());

                yield return new object[] { completionData };
            }
        }
    }
}
