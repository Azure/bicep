using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Bicep.Core.Samples
{
    [TestClass]
    public class SnippetTests
    {
        public class SnippetModel
        {
            public string? Prefix { get; set; }

            public IEnumerable<string>? Body { get; set; }

            public static string GetDisplayName(MethodInfo info, object[] data) => ((SnippetModel)data[0]).Prefix!;
        }

        private static IDictionary<string, Action<string>> SnippetValidations = new Dictionary<string, Action<string>>
        {
            ["resource"] = body => ValidateSnippet(body, "myResource", "myProvider", "myType", "2020-01-01", "name: 'myResource'"),
            ["variable"] = body => ValidateSnippet(body, "myVariable", "'stringVal'"),
            ["parameter"] = body => ValidateSnippet(body, "myParam", "string"),
            ["output"] = body => ValidateSnippet(body, "myOutput", "string", "'stringVal'"),
        };

        private static IEnumerable<object[]> GetSnippets()
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, SnippetModel>>(DataSet.ReadFile("vscode-bicep.snippets.bicep.json"));
            
            return data.Values.Select(snippet => new object[] { snippet });
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(SnippetModel), DynamicDataDisplayName = nameof(SnippetModel.GetDisplayName))]
        public void SnippetIsValid(SnippetModel snippet)
        {
            var snippetBody = string.Join('\n', snippet.Body!);
            var prefix = snippet.Prefix!;

            SnippetValidations.Should().ContainKey(prefix, "validation has not been defined for this snippet");
            SnippetValidations[prefix].Invoke(snippetBody);
        }
        
        private static void ValidateSnippet(string body, params string[] replacements)
        {
            var holes = new Dictionary<int, Match>();
            var currentMatch = Regex.Match(body, @"\$({(?<index>\d+):\w+}|(?<index>\d+))");
            while (currentMatch.Success)
            {
                var index = int.Parse(currentMatch.Groups["index"].Value);

                holes.Should().NotContainKey(index, "there should only be one entry per index");
                holes[index] = currentMatch;

                currentMatch = currentMatch.NextMatch();
            }

            holes.Should().HaveCount(replacements.Length, "the number of replacements should match the number of holes");
            if (holes.ContainsKey(0))
            {
                holes.Should().HaveCount(holes.Keys.Max() + 1, "there should be a consecutive range of numbered holes");
                holes.Keys.Min().Should().Be(0, "the numbered holes should start at 0");
            }
            else if (holes.Any())
            {
                holes.Should().HaveCount(holes.Keys.Max(), "there should be a consecutive range of numbered holes");
                holes.Keys.Min().Should().Be(1, "the numbered holes should start at 1");
            }

            var orderedKeys = holes.Keys.OrderBy(i => i > 0 ? i : int.MaxValue).ToArray(); // VSCode puts $0 (if present) at the end, hence the strange ordering.
            var replacementPairs = orderedKeys.Select((holeIndex, i) => (holes[holeIndex], replacements[i]));

            // replace backwards so we don't have to recompute the index each iteration
            foreach (var (match, replacement) in replacementPairs.OrderByDescending(t => t.Item1.Index))
            {
                body = body.Substring(0, match.Index) + replacement + body.Substring(match.Index + match.Length);
            }

            var program = SyntaxFactory.CreateFromText(body + "\n");
            program.GetParseDiagnostics().Should().BeEmpty($"compilation failed: {body}");
        }
    }
}
