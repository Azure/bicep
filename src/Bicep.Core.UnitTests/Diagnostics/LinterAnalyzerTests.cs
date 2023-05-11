// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bicep.Core.Analyzers;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class LinterAnalyzerTests
    {
        [TestMethod]
        public void HasBuiltInRules()
        {
            var linter = new LinterAnalyzer();
            linter.GetRuleSet().Should().NotBeEmpty();
        }


        /////////////////////////


        /*

        
            - aka link: https://aka.ms/bicep/linter/my-wonderful-rule
            - doc link: https://learn.microsoft.com/azure/azure-resource-manager/bicep/linter-rule-my-wonderful-rule            

        States:

            Rule under development:
                aka link: does not exist (or is disabled)
                help link: does not work

            Rule merged, help under development:
                aka link: does not exist (or is disabled)
                help link: does not work

            Rule merged, help merged, aka link not yet created/enabled:
                aka link: does not exist (or is disabled)
                help link: works (full URI)

            Rule merged, help merged, aka link created
                aka link: works
                help link: works

            Rule merged, help merged, aka link broken or help link broken
                aka link: may be broken
                help link: may be broken

         */


        [TestMethod]
        [DynamicData(nameof(GetRules))]
        public async Task AllLinterHelpLinksAreCorrect(string analyzerName, string ruleCode, Uri? helpUri)
        {
            // eg bicep/linter/max-params
            // eg https://aka.ms/bicep/linter/max-params
            // eg https://learn.microsoft.com/azure/azure-resource-manager/bicep/linter-rule-max-parameters

            // eg https://aka.ms/bicep/tests/surveytests/active
            // eg https://aka.ms/bicep/tests/surveytests/inactive

            // eg https://aka.ms/bicep/linter/simplify-json-null
            // eg https://learn.microsoft.com/azure/azure-resource-manager/bicep/linter-rule-simplify-json-null

#pragma warning disable RS0030 // Do not use banned APIs
            Console.WriteLine(analyzerName);
            Console.WriteLine(ruleCode);
            Console.WriteLine(helpUri?.AbsolutePath ?? "null");
            if (helpUri is null)
            {
                throw new Exception("helpUri should not be null");
            }

            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(5); //asdfg
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(helpUri);
                Console.WriteLine(response.StatusCode);
            }
            catch (TimeoutException)
            {
                throw new Exception($"Timed out doing get on {helpUri.AbsoluteUri}");
            }
            var helpContents = await response.Content.ReadAsStringAsync();
            if (!helpContents.Contains("Linter rule"))
            {
                throw new Exception("Help documentation link does not appear to be pointed to the correct help page (expected it to contain \"Linter rule\" in the http contents)");
            }

            //var handler = new HttpClientHandler()
            //{
            //    AllowAutoRedirect = false,
            //    CheckCertificateRevocationList = true,
            //};
            //using HttpClient client2 = new HttpClient(handler);
            //var b = await client2.GetAsync(helpUri);
            //var c = await b.Content.ReadAsStringAsync();
            //Console.WriteLine(c);

#pragma warning restore RS0030 // Do not use banned APIs
        }


        /////////////////////////


        // No need to add new rules here, just checking a few known ones
        [DataTestMethod]
        [DataRow(AdminUsernameShouldNotBeLiteralRule.Code)]
        [DataRow(ExplicitValuesForLocationParamsRule.Code)]
        [DataRow(NoHardcodedEnvironmentUrlsRule.Code)]
        [DataRow(NoHardcodedLocationRule.Code)]
        public void BuiltInRulesExistSanityCheck(string ruleCode)

        {
            var linter = new LinterAnalyzer();
            linter.GetRuleSet().Should().Contain(r => r.Code == ruleCode);
        }

        [TestMethod]
        public void AllDefinedRulesAreListedInLinterRulesProvider()
        {
            var linter = new LinterAnalyzer();
            var ruleTypes = linter.GetRuleSet().Select(r => r.GetType()).ToArray();

            var expectedRuleTypes = typeof(LinterAnalyzer).Assembly
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && t.IsClass
                            && t.IsPublic
                            && t.GetConstructor(Type.EmptyTypes) != null);

            var actualTypeNames = ruleTypes.Select(t => t.FullName ?? throw new ArgumentNullException("bad type"));
            var expectedTypeNames = expectedRuleTypes.Select(t => t.FullName ?? throw new ArgumentNullException("bad type"));

            actualTypeNames.Should().BeEquivalentTo(expectedTypeNames, "Please verify that the {nameof(LinterRuleTypeGenerator)} source generator is working correctly");
        }

        [TestMethod]
        public void AllRulesHaveUniqueDetails()
        {
            var analyzer = new LinterAnalyzer();
            var ruleSet = analyzer.GetRuleSet();

            var codeSet = ruleSet.Select(r => r.Code).ToHashSet();
            codeSet.Should().HaveSameCount(ruleSet);

            var descSet = ruleSet.Select(r => r.Description).ToHashSet();
            descSet.Should().HaveSameCount(ruleSet);
        }

        [TestMethod]
        public void MostRulesEnabledByDefault()
        {
            var analyzer = new LinterAnalyzer();
            var ruleSet = analyzer.GetRuleSet();
            var numberEnabled = ruleSet.Where(r => r.DefaultDiagnosticLevel != DiagnosticLevel.Off).Count();
            numberEnabled.Should().BeGreaterThan(ruleSet.Count() / 2, "most rules should probably be enabled by default");
        }

        [TestMethod]
        public void AllRulesHaveDescription()
        {
            var analyzer = new LinterAnalyzer();
            var ruleSet = analyzer.GetRuleSet();
            ruleSet.Should().OnlyContain(r => r.Description.Length > 0);
        }

        public static IEnumerable<object?[]> GetRules
        {
            get
            {
                var analyzer = new LinterAnalyzer();
                var ruleSet = analyzer.GetRuleSet();
                return ruleSet.Select(r => new object?[] { r.AnalyzerName, r.Code, r.HelpUri });
            }
        }

        public class LinterThrowsTestRule : LinterRuleBase
        {
            public LinterThrowsTestRule() : base("ThrowsRule", "Throws an exception when used", null, DiagnosticLevel.Warning) { }

            public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                // Have a yield return to force this method to return an iterator like the real rules
                yield return new AnalyzerDiagnostic(this.AnalyzerName,
                                                    TextSpan.TextDocumentStart,
                                                    diagnosticLevel,
                                                    "fakeRule",
                                                    "Fake Rule",
                                                    null);
                // Now throw an exception
                throw new System.ArgumentOutOfRangeException();
            }
        }

        [TestMethod]
        public void TestRuleThrowingException()
        {
            var text = @"
@secure()
param param1 string = 'val'";
            var compilationResult = CompilationHelper.Compile(text);
            var semanticModel = compilationResult.Compilation.GetSemanticModel(compilationResult.BicepFile);

            var throwRule = new LinterThrowsTestRule();
            var test = () => throwRule.Analyze(semanticModel).ToArray();
            test.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
