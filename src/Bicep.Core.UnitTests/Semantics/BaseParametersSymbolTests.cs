// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class BaseParametersSymbolTests
    {
        [TestMethod]
        public void FileSymbol_should_include_base_parameters_symbol_when_extends_is_present()
        {
            var services = new ServiceBuilder()
                .WithEmptyAzResources()
                .WithFeatureOverrides(new(ExtendableParamFilesEnabled: true));

            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///main.bicep")] = """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    """,
                [new Uri("file:///shared.bicepparam")] = """
                    using none
                    param three = 'param three'
                    """,
                [new Uri("file:///main.bicepparam")] = """
                    using 'main.bicep'
                    extends 'shared.bicepparam'
                    param one = 'param one'
                    param two = base.three
                    """
            };

            var compilation = services.BuildCompilation(files, new Uri("file:///main.bicepparam"));
            var model = compilation.GetEntrypointSemanticModel();

            var baseSymbol = model.Root.Declarations.OfType<BaseParametersSymbol>().Single();

            baseSymbol.Name.Should().Be(LanguageConstants.BaseIdentifier);
            baseSymbol.ParentAssignments.Select(x => x.Name).Should().BeEquivalentTo(["three"]);
            model.Root.Declarations.Should().Contain(baseSymbol);
        }

        [TestMethod]
        public void FileSymbol_should_not_include_base_parameters_symbol_when_extends_is_absent()
        {
            var services = new ServiceBuilder()
                .WithEmptyAzResources()
                .WithFeatureOverrides(new(ExtendableParamFilesEnabled: true));

            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///main.bicep")] = """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    """,
                [new Uri("file:///main.bicepparam")] = """
                    using 'main.bicep'
                    param one = 'param one'
                    param two = 'param two'
                    """
            };

            var compilation = services.BuildCompilation(files, new Uri("file:///main.bicepparam"));
            var model = compilation.GetEntrypointSemanticModel();

            model.Root.Declarations.OfType<BaseParametersSymbol>().Should().BeEmpty();
            model.Root.Declarations.Should().NotContain(x => x.Name == LanguageConstants.BaseIdentifier);
        }

        [TestMethod]
        public void Base_parameters_symbol_should_include_all_inherited_assignments()
        {
            var services = new ServiceBuilder()
                .WithEmptyAzResources()
                .WithFeatureOverrides(new(ExtendableParamFilesEnabled: true));

            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///main.bicep")] = """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    param four string = ''
                    """,
                [new Uri("file:///shared.bicepparam")] = """
                    using none
                    param three = 'param three'
                    param four = 'param four'
                    """,
                [new Uri("file:///main.bicepparam")] = """
                    using 'main.bicep'
                    extends 'shared.bicepparam'
                    param one = 'param one'
                    param two = base.three
                    """
            };

            var compilation = services.BuildCompilation(files, new Uri("file:///main.bicepparam"));
            var model = compilation.GetEntrypointSemanticModel();

            var baseSymbol = model.Root.Declarations.OfType<BaseParametersSymbol>().Single();

            baseSymbol.ParentAssignments.Select(x => x.Name).Should().BeEquivalentTo(["three", "four"]);
        }

        [TestMethod]
        public void Base_variable_access_should_have_object_type_with_read_only_parent_properties()
        {
            var services = new ServiceBuilder()
                .WithEmptyAzResources()
                .WithFeatureOverrides(new(ExtendableParamFilesEnabled: true));

            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///main.bicep")] = """
                    param one string = ''
                    param two string = ''
                    param three string = ''
                    param four string = ''
                    """,
                [new Uri("file:///shared.bicepparam")] = """
                    using none
                    param three = 'param three'
                    param four = 'param four'
                    """,
                [new Uri("file:///main.bicepparam")] = """
                    using 'main.bicep'
                    extends 'shared.bicepparam'
                    param one = 'param one'
                    param two = base.three
                    """
            };

            var compilation = services.BuildCompilation(files, new Uri("file:///main.bicepparam"));
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
                public void Base_variable_access_should_not_throw_when_inherited_params_include_object_and_array_values()
                {
                        var services = new ServiceBuilder()
                                .WithEmptyAzResources()
                                .WithFeatureOverrides(new(ExtendableParamFilesEnabled: true));

                        var files = new Dictionary<Uri, string>
                        {
                                [new Uri("file:///main.bicep")] = """
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
                                        """,
                                [new Uri("file:///shared.bicepparam")] = """
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
                                        """,
                                [new Uri("file:///main.bicepparam")] = """
                                        using 'main.bicep'
                                        extends 'shared.bicepparam'
                                        param one = 'param one'
                                        param two = base.three
                                        param five = []
                                        """
                        };

                        var compilation = services.BuildCompilation(files, new Uri("file:///main.bicepparam"));
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
