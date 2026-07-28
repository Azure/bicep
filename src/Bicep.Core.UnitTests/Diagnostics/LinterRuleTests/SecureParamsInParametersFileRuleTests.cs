// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.Abstraction;
using Bicep.Testing.IO;
using Bicep.Testing.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SecureParamsInParametersFileRuleTests : LinterRuleTestsBase
    {
        private static readonly IOUri MainUri = TestFileUri.FromInMemoryPath("main.bicep");
        private static readonly IOUri ParamsUri = TestFileUri.FromInMemoryPath("main.bicepparam");

        private static TestCompiler CreateCompiler() => TestCompiler.ForInMemoryCompilation()
            .WithConfiguration(BicepTestConstants.BuiltInConfigurationWithStableAnalyzers)
            .WithEmptyAzResources();

        private static async Task<Compilation> Compile(string mainBicep, string paramsBicep)
        {
            var result = await CreateCompiler().CompileWithoutRestore(
                "main.bicepparam",
                ("main.bicep", mainBicep),
                ("main.bicepparam", paramsBicep));

            return result.Compilation;
        }

        [TestMethod]
        public async Task InsecureParamAssignedSecureParamValue_IsFlagged()
        {
            var compilation = await Compile(
                """
                @secure()
                param secureParam string

                param insecureParam string
                """,
                """
                using 'main.bicep'

                param secureParam = 'MYSECRET'
                param insecureParam = secureParam
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().ContainSingleDiagnostic(
                SecureParamsInParametersFileRule.Code,
                DiagnosticLevel.Warning,
                "Insecure parameter 'insecureParam' is assigned a value that references secure parameter(s) 'secureParam', which could expose their values in deployment history.");
        }

        [TestMethod]
        public async Task SecureParamAssignedSecureParamValue_IsNotFlagged()
        {
            var compilation = await Compile(
                """
                @secure()
                param secureParam string

                @secure()
                param anotherSecureParam string
                """,
                """
                using 'main.bicep'

                param secureParam = 'MYSECRET'
                param anotherSecureParam = secureParam
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().NotContainDiagnostic(SecureParamsInParametersFileRule.Code);
        }

        [TestMethod]
        public async Task InsecureParamAssignedLiteralValue_IsNotFlagged()
        {
            var compilation = await Compile(
                """
                @secure()
                param secureParam string

                param insecureParam string
                """,
                """
                using 'main.bicep'

                param secureParam = 'MYSECRET'
                param insecureParam = 'not a secret'
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().NotContainDiagnostic(SecureParamsInParametersFileRule.Code);
        }

        [TestMethod]
        public async Task InsecureParamAssignedInsecureParamValue_IsNotFlagged()
        {
            var compilation = await Compile(
                """
                param firstParam string

                param secondParam string
                """,
                """
                using 'main.bicep'

                param firstParam = 'value'
                param secondParam = firstParam
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().NotContainDiagnostic(SecureParamsInParametersFileRule.Code);
        }

        [TestMethod]
        public async Task InsecureParamTransitivelyReferencingSecureParam_IsFlagged()
        {
            var compilation = await Compile(
                """
                @secure()
                param secureParam string

                param insecureParam string
                """,
                """
                using 'main.bicep'

                param secureParam = 'MYSECRET'
                var derived = secureParam
                param insecureParam = derived
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().ContainSingleDiagnostic(
                SecureParamsInParametersFileRule.Code,
                DiagnosticLevel.Warning,
                "Insecure parameter 'insecureParam' is assigned a value that references secure parameter(s) 'secureParam', which could expose their values in deployment history.");
        }

        [TestMethod]
        public async Task InsecureParamAssignedSecureParamInInterpolation_IsFlagged()
        {
            var compilation = await Compile(
                """
                @secure()
                param secureParam string

                param insecureParam string
                """,
                """
                using 'main.bicep'

                param secureParam = 'MYSECRET'
                param insecureParam = 'prefix-${secureParam}'
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().ContainSingleDiagnostic(
                SecureParamsInParametersFileRule.Code,
                DiagnosticLevel.Warning,
                "Insecure parameter 'insecureParam' is assigned a value that references secure parameter(s) 'secureParam', which could expose their values in deployment history.");
        }

        [TestMethod]
        public async Task SecureObjectParam_ReferencedByInsecureParam_IsFlagged()
        {
            var compilation = await Compile(
                """
                @secure()
                param secureObj object

                param insecureParam object
                """,
                """
                using 'main.bicep'

                param secureObj = { key: 'MYSECRET' }
                param insecureParam = secureObj
                """);

            compilation.GetSourceFileDiagnostics(ParamsUri).Should().ContainSingleDiagnostic(
                SecureParamsInParametersFileRule.Code,
                DiagnosticLevel.Warning,
                "Insecure parameter 'insecureParam' is assigned a value that references secure parameter(s) 'secureObj', which could expose their values in deployment history.");
        }

        [TestMethod]
        public async Task Rule_DoesNotRunOnBicepFiles()
        {
            var compilation = (await CreateCompiler().CompileWithoutRestore(
                """
                    @secure()
                    param secureParam string

                    var insecureVar = secureParam

                    output insecureOutput string = insecureVar
            """)).Compilation;

            compilation.GetSourceFileDiagnostics(MainUri).Should().NotContainDiagnostic(SecureParamsInParametersFileRule.Code);
        }
    }
}
