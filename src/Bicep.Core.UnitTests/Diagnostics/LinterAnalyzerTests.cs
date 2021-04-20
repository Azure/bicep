// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Semantics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class LinterAnalyzerTests
    {

        [TestMethod]
        public void IsCLIInvoked()
        {
            Assert.IsFalse(LinterAnalyzer.IsCliInvoked);
        }

        [TestMethod]
        public void HasBuiltInRules()
        {
            var linter = new LinterAnalyzer();
            Assert.IsTrue(linter.GetRuleSet().Count() > 0);
        }

        [DataTestMethod]
        [DataRow("BCPL1000")]
        [DataRow("BCPL1010")]
        [DataRow("BCPL1020")]
        [DataRow("BCPL1030")]
        [DataRow("BCPL1040")]
        [DataRow("BCPL1050")]
        [DataRow("BCPL1060")]
        [DataRow("BCPL1070")]
        [DataRow("BCPL1080")]
        public void BuiltInRulesExist(string ruleCode)
        {
            var linter = new LinterAnalyzer();
            Assert.IsTrue(linter.GetRuleSet().Any(r => r.Code == ruleCode));
        }

        [TestMethod]
        public void AllRulesHaveUniqueDetails()
        {
            var ruleSet = LinterAnalyzer.CreateLinterRules().ToArray();

            var codeSet = ruleSet.Select(r => r.Code).ToHashSet();
            Assert.AreEqual(ruleSet.Length, codeSet.Count);

            var descSet = ruleSet.Select(r => r.Description).ToHashSet();
            Assert.AreEqual(ruleSet.Length, descSet.Count);

            var nameSet = ruleSet.Select(r => r.RuleName).ToHashSet();
            Assert.AreEqual(ruleSet.Length, nameSet.Count);
        }

        [TestMethod]
        public void AllRulesEnabledByDefault()
        {

            var ruleSet = LinterAnalyzer.CreateLinterRules().ToArray();
            Assert.IsTrue(ruleSet.All(r => r.EnabledForCLI || r.EnabledForEditing));
        }

        [TestMethod]
        public void AllRulesHaveDescription()
        {
            var ruleSet = LinterAnalyzer.CreateLinterRules();
            foreach (var rule in ruleSet)
            {
                Assert.IsTrue(rule.Description.Length > 0);
            }
        }

    }
}
