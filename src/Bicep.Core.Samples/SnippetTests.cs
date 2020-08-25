// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
            [JsonConstructor]
            public SnippetModel(string prefix, IEnumerable<string> body, string description)
            {
                Prefix = prefix;
                Body = body;
                Description = description;
            }

            public string Prefix { get; }

            public IEnumerable<string> Body { get; }

            public string Description { get; }
        }

        public class NamedSnippetModel
        {
            public NamedSnippetModel(string name, SnippetModel model)
            {
                Name = name;
                Model = model;
            }

            public string Name { get; }

            public SnippetModel Model { get; }

            public static string GetDisplayName(MethodInfo info, object[] data) => ((NamedSnippetModel)data[0]).Name;
        }

        public class SnippetValidation
        {
            public SnippetValidation(string expectedPrefix, Action<string> validateAction)
            {
                ExpectedPrefix = expectedPrefix;
                ValidateAction = validateAction;
            }

            public string ExpectedPrefix { get; }

            public Action<string> ValidateAction { get; }
        }

        private static IDictionary<string, SnippetValidation> SnippetValidations = new Dictionary<string, SnippetValidation>
        {
            ["ResourceWithDefaults"] = new SnippetValidation(
                "resource",
                body => ValidateSnippet(body, "myResource", "myProvider", "myType", "2020-01-01", "'parent'", "'West US'", "prop1: 'val1'")
            ),
            ["ResourceChildWithDefaults"] = new SnippetValidation(
                "resource",
                body => ValidateSnippet(body, "myResource", "myProvider", "myType", "myChildType", "2020-01-01", "'parent/child'", "prop1: 'val1'")
            ),
            ["ResourceWithoutDefaults"] = new SnippetValidation(
                "resource",
                body => ValidateSnippet(body, "myResource", "myProvider", "myType", "2020-01-01", "'parent'", "properties: {\nprop1: 'val1'\n}")
            ),
            ["ResourceChildWithoutDefaults"] = new SnippetValidation(
                "resource",
                body => ValidateSnippet(body, "myResource", "myProvider", "myType", "myChildType", "2020-01-01", "'parent/child'", "properties: {\nprop1: 'val1'\n}")
            ),
            ["Variable"] = new SnippetValidation(
                "var",
                body => ValidateSnippet(body, "myVariable", "'stringVal'")
            ),
            ["Parameter"] = new SnippetValidation(
                "param",
                body => ValidateSnippet(body, "myParam", "string")
            ),
            ["ParameterWithInlineDefault"] = new SnippetValidation(
                "param",
                body => ValidateSnippet(body, "myParam", "string", "'myDefault'")
            ),
            ["ParameterWithDefaultAndAllowedValues"] = new SnippetValidation(
                "param",
                body => ValidateSnippet(body, "myParam", "string", "'myDefault'", "'val1'\n'val2'")
            ),
            ["ParameterWithOptions"] = new SnippetValidation(
                "param",
                body => ValidateSnippet(body, "myParam", "string", "default: 'myDefault'\nsecure: true")
            ),
            ["ParameterSecureString"] = new SnippetValidation(
                "param",
                body => ValidateSnippet(body, "myParam")
            ),
            ["Output"] = new SnippetValidation(
                "output",
                body => ValidateSnippet(body, "myOutput", "string", "'stringVal'")
            ),
        };

        private static IEnumerable<object[]> GetSnippets()
        {
            var snippetFileContents = JsonConvert.DeserializeObject<Dictionary<string, SnippetModel>>(DataSet.ReadFile("vscode-bicep.snippets.bicep.json"));
            var testData = snippetFileContents.Select(x => new NamedSnippetModel(x.Key, x.Value));
            
            return testData.Select(model => new object[] { model });
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(NamedSnippetModel), DynamicDataDisplayName = nameof(NamedSnippetModel.GetDisplayName))]
        public void SnippetIsValid(NamedSnippetModel snippet)
        {
            snippet.Model.Prefix.Should().NotBeEmpty();
            snippet.Model.Description.Should().NotBeEmpty();
            var snippetBody = string.Join('\n', snippet.Model.Body);

            SnippetValidations.Should().ContainKey(snippet.Name, "validation has not been defined for this snippet");
            var validation = SnippetValidations[snippet.Name];

            snippet.Model.Prefix.Should().Be(validation.ExpectedPrefix);
            validation.ValidateAction(snippetBody);
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

            var program = SyntaxFactory.CreateFromText(body);
            program.GetParseDiagnostics().Should().BeEmpty($"compilation failed: {body}");
        }
    }
}

