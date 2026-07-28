// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Testing.IO;
using Bicep.Testing.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class BaseParametersSymbolTests
    {
        private static TestCompiler CreateCompiler() => TestCompiler.ForInMemoryCompilation()
            .WithEmptyAzResources();

        private static async Task<Compilation> CompileParams(params (string FilePath, TestFileData FileData)[] files)
            => (await CreateCompiler().CompileWithoutRestore("main.bicepparam", files)).Compilation;

        [TestMethod]
        public async Task FileSymbol_should_include_base_parameters_symbol_when_extends_is_present()
        {
            var compilation = await CompileParams(
                ("main.bicep", """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    """),
                ("shared.bicepparam", """
                    using none
                    param three = 'param three'
                    """),
                ("main.bicepparam", """
                    using 'main.bicep'
                    extends 'shared.bicepparam'
                    param one = 'param one'
                    param two = base.three
                    """));

            var model = compilation.GetEntrypointSemanticModel();

            var baseSymbol = model.Root.Declarations.OfType<BaseParametersSymbol>().Single();

            baseSymbol.Name.Should().Be(LanguageConstants.BaseIdentifier);
            baseSymbol.ParentAssignments.Select(x => x.Name).Should().BeEquivalentTo(["three"]);
            model.Root.Declarations.Should().Contain(baseSymbol);
        }

        [TestMethod]
        public async Task FileSymbol_should_not_include_base_parameters_symbol_when_extends_is_absent()
        {
            var compilation = await CompileParams(
                ("main.bicep", """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    """),
                ("main.bicepparam", """
                    using 'main.bicep'
                    param one = 'param one'
                    param two = 'param two'
                    """));

            var model = compilation.GetEntrypointSemanticModel();

            model.Root.Declarations.OfType<BaseParametersSymbol>().Should().BeEmpty();
            model.Root.Declarations.Should().NotContain(x => x.Name == LanguageConstants.BaseIdentifier);
        }

        [TestMethod]
        public async Task Base_parameters_symbol_should_include_all_inherited_assignments()
        {
            var compilation = await CompileParams(
                ("main.bicep", """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    param four string = ''
                    """),
                ("shared.bicepparam", """
                    using none
                    param three = 'param three'
                    param four = 'param four'
                    """),
                ("main.bicepparam", """
                    using 'main.bicep'
                    extends 'shared.bicepparam'
                    param one = 'param one'
                    param two = base.three
                    """));

            var model = compilation.GetEntrypointSemanticModel();

            var baseSymbol = model.Root.Declarations.OfType<BaseParametersSymbol>().Single();

            baseSymbol.ParentAssignments.Select(x => x.Name).Should().BeEquivalentTo(["three", "four"]);
        }

        [TestMethod]
        public async Task Base_variable_access_should_have_object_type_with_read_only_parent_properties()
        {
            var compilation = await CompileParams(
                ("main.bicep", """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    param four string = ''
                    """),
                ("shared.bicepparam", """
                    using none
                    param three = 'param three'
                    param four = 'param four'
                    """),
                ("main.bicepparam", """
                    using 'main.bicep'
                    extends 'shared.bicepparam'
                    param one = 'param one'
                    param two = base.three
                    """));

            var model = compilation.GetEntrypointSemanticModel();

            var twoAssignment = model.Root.ParameterAssignments.Single(x => x.Name == "two");
            var baseAccess = ((PropertyAccessSyntax)twoAssignment.DeclaringParameterAssignment.Value).BaseExpression
                .Should().BeOfType<VariableAccessSyntax>().Subject;

            var baseType = model.GetTypeInfo(baseAccess).Should().BeOfType<ObjectType>().Subject;

            baseType.Properties.Should().ContainKeys("three", "four");
            baseType.Properties["three"].Flags.Should().HaveFlag(TypePropertyFlags.ReadOnly);
            baseType.Properties["four"].Flags.Should().HaveFlag(TypePropertyFlags.ReadOnly);
        }

        [TestMethod]
        public async Task Base_variable_access_should_not_throw_when_inherited_params_include_object_and_array_values()
        {
            var compilation = await CompileParams(
                ("main.bicep", """
                                        param one string = ''
                                        param two string = ''
                                        param three string = ''
                                        param four object = {
                                            name: 'four'
                                            value: 'four'
                                        }
                                        param five array = [
                                            {
                                                name: 'five'
                                                value: 'five'
                                            }
                                        ]
                                        """),
                ("shared.bicepparam", """
                                        using none
                                        param three = 'param three'
                                        param four = {
                                            name: 'param four'
                                        }
                                        param five = [
                                            {
                                                name: 'param five'
                                            }
                                        ]
                                        """),
                ("main.bicepparam", """
                                        using 'main.bicep'
                                        extends 'shared.bicepparam'
                                        param one = 'param one'
                                        param two = base.three
                                        param five = []
                                        """));

            var model = compilation.GetEntrypointSemanticModel();

            FluentActions.Invoking(() => model.GetAllDiagnostics().ToArray()).Should().NotThrow();

            var baseAccess = ((PropertyAccessSyntax)model.Root.ParameterAssignments
                    .Single(x => x.Name == "two")
                    .DeclaringParameterAssignment
                    .Value).BaseExpression;

            var baseType = model.GetTypeInfo(baseAccess).Should().BeOfType<ObjectType>().Subject;

            baseType.Properties.Should().ContainKeys("three", "four", "five");
            baseType.Properties["four"].Flags.Should().HaveFlag(TypePropertyFlags.ReadOnly);
            baseType.Properties["five"].Flags.Should().HaveFlag(TypePropertyFlags.ReadOnly);
        }
    }
}
