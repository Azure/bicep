// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// TODO: Test with different configs
namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoHardcodedEnvironmentUrlsRuleTests : LinterRuleTestsBase
    {

        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = '${p1}azuredatalakestore.net${p2}'
        ")]
        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = '${p1} azuredatalakestore.net${p2}'
        ")]
        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = '${p1} azuredatalakestore.net$ {p2}'
        ")]
        [DataTestMethod]
        public void InsideStringInterpolation(int diagnosticCount, string text)
        {
            CompileAndTest(NoHardcodedEnvironmentUrlsRule.Code, text, diagnosticCount);
        }

        [DataRow(1, @"var a = 'azuredatalakestore.net' + 1")]
        [DataRow(2, @"var a = 'azuredatalakestore.net azuredatalakestore.net' + 1")]
        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = concat('${p1}${'azuredatalakestore.net'}${p2}', 'foo')
        ")]
        [DataRow(0, @"
        param p1 string
        param p2 string
        var a = concat('${p1}${'https://schema.management.azure.com'}${p2}', 'foo')
        ")]
        [DataRow(2, @"
        param p1 string
        param p2 string
        var a = concat('${p1}${'azuredatalakestore.net'}${p2}${'management.azure.com'}-${'schema.management.azure.com'}', 'foo')
        ")]
        [DataTestMethod]
        public void InsideExpressions(int diagnosticCount, string text)
        {
            CompileAndTest(NoHardcodedEnvironmentUrlsRule.Code, text, diagnosticCount);
        }
    }
}

