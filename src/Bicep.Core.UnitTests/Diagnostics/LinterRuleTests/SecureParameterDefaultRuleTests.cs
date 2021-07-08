// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SecureParameterDefaultRuleTests : LinterRuleTestsBase
    {
        [DataRow(0, @"
param password string = 'xxxx'
param o object = { a: 1 }
var sum = 1 + 3
output sub int = sum
")]
        [DataTestMethod]
        public void NotSecureParam_TestPasses(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param password string
var sum = 1 + 3
output sub int = sum
")]
        [DataRow(0, @"
@secure()
param poNoDefault object
")]
        [DataTestMethod]
        public void NoDefault_TestPasses(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param password string = ''
")]
        [DataTestMethod]
        public void EmptyString_TestPasses(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param poEmpty object = {}
")]
        [DataTestMethod]
        public void EmptyObject_TestPasses(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(0, @"
@secure()
param psNewGuid string = newGuid()
")]
        [DataRow(0, @"
@secure()
param psContainsNewGuid string = concat('${psEmpty}${newGuid()}', '')
")]
        [DataTestMethod]
        public void ExpressionContainingNewGuid_TestPasses(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(1, @"
@secure()
param param1 string
@secure()
param param2 string = 'val'
param param3 int
output sub int = sum
")]
        [DataRow(1, @"
@secure()
param param1 string = 'val1'
output sub int = sum
")]
        [DataRow(2, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
param param3 int
output sub int = sum
")]
        [DataRow(2, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
@secure()
param param3 int
output sub int = sum
")]
        [DataRow(3, @"
@secure()
param param1 string = 'val'
@secure()
param param2 string = 'val'
@secure()
param param3 int = 5
output sub int = sum
")]
        [DataRow(1, @"
@secure()
param psExpression string = resourceGroup().location
")]
        [DataTestMethod]
        public void InvalidNonEmptyDefault_TestFails(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(1, @"
@secure()
param poNotEmpty object = {
  abc: 1
}
")]
        [DataTestMethod]
        public void NonEmptySecureObject_TestFails(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

        [DataRow(2, @"
@secure()
param pi1 int = 1

@secure
param param1 string = 'val'

@secure()
param param2 string =
")]
        [DataRow(1, @"
@secure()
param pi1 int = 'wrong type'
")]
        [DataRow(1, @"
@secure()
param psWrongType string = 123
")]
        [DataRow(1, @"
@secure()
param o object = {
")]
        [DataRow(1, @"
@secure()
param o object = {
    a:
}")]
        [DataRow(0, @"
@secure()
param o object = {
    // comments
}")]
        [DataTestMethod]
        public void HandlesSyntaxErrors(int diagnosticCount, string text)
        {
            CompileAndTest(SecureParameterDefaultRule.Code, text, diagnosticCount);
        }

    }
}
