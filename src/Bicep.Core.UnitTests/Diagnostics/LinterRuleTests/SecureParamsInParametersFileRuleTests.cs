// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SecureParamsInParametersFileRuleTests : LinterRuleTestsBase
    {
        private static readonly Uri MainUri = new("file:///main.bicep");
        private static readonly Uri ParamsUri = new("file:///main.bicepparam");

        private static ServiceBuilder ServiceBuilder => new ServiceBuilder()
            .WithConfiguration(BicepTestConstants.BuiltInConfigurationWithStableAnalyzers)
            .WithEmptyAzResources();

        private static Compilation Compile(string mainBicep, string paramsBicep)
        {
            var files = new Dictionary<Uri, string>
            {
                [MainUri] = mainBicep,
                [ParamsUri] = paramsBicep,
            };

            return ServiceBuilder.BuildCompilation(files, ParamsUri);
        }

        [TestMethod]
        public void InsecureParamAssignedSecureParamValue_IsFlagged()
        {
            var compilation = Compile(
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
        public void SecureParamAssignedSecureParamValue_IsNotFlagged()
        {
            var compilation = Compile(
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
        public void InsecureParamAssignedLiteralValue_IsNotFlagged()
        {
            var compilation = Compile(
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
        public void InsecureParamAssignedInsecureParamValue_IsNotFlagged()
        {
            var compilation = Compile(
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
        public void InsecureParamTransitivelyReferencingSecureParam_IsFlagged()
        {
            var compilation = Compile(
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
        public void InsecureParamAssignedSecureParamInInterpolation_IsFlagged()
        {
            var compilation = Compile(
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
        public void SecureObjectParam_ReferencedByInsecureParam_IsFlagged()
        {
            var compilation = Compile(
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
        public void Rule_DoesNotRunOnBicepFiles()
        {
            var compilation = ServiceBuilder.BuildCompilation(
                new Dictionary<Uri, string>
                {
                    [MainUri] = """
                        @secure()
                        param secureParam string

                        var insecureVar = secureParam

                        output insecureOutput string = insecureVar
                        """,
                },
                MainUri);

            compilation.GetSourceFileDiagnostics(MainUri).Should().NotContainDiagnostic(SecureParamsInParametersFileRule.Code);
        }
    }
}
