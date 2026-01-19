// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.Types.Attributes;
using FluentAssertions;

namespace Bicep.Local.Extension.UnitTests.TypesTests.Types_A

{
    [ResourceType("ActiveResource")]
    public class ActiveResource { }

    public class NoAttributeResource { }

    [ResourceType("InternalActiveResource")]
    internal class InternalActiveResource { }
}

namespace Bicep.Local.Extension.UnitTests.TypesTests.Types_B
{
    [ResourceType("ActiveResource")]
    public class ActiveResource { }

    public class NoAttributeResource { }

    [ResourceType("InternalActiveResource")]
    internal class InternalActiveResource { }
}

namespace Bicep.Local.Extension.UnitTests.TypesTests
{
    [ResourceType("ActiveResource")]
    public class ActiveResource { }

    public class NoAttributeResource { }

    [ResourceType("InternalActiveResource")]
    internal class InternalActiveResource { }

    [ResourceType("FallbackResource")]
    public class FallbackResource { }

    public class ConfigurationType { }

    public class FallbackWithoutAttribute { }

    [TestClass]
    public class TypeProviderTests
    {
        [ResourceType("NestedActiveResource")]
        public class NestedActiveResource { }

        public class NestedNoAttributeResource { }

        [ResourceType("PrivateNestedActiveResource")]
        private class PrivateNestedActiveResource { }

        public class NestedConfigurationType { }


                [TestMethod]
                public void GetResourceTypes_Only_Returns_ClassesWithBicepTypeAttribute()
                {
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

                    var types = provider.GetResourceTypes(throwOnDuplicate: false).Select(x => x.type).ToList();

                    types.Should().HaveCount(3, "only public types in the same namespaces should be returned");

                    types.Should().Contain(typeof(ActiveResource));
                    types.Should().Contain(typeof(NestedActiveResource));
                    types.Should().Contain(typeof(FallbackResource));

                    // although these are unique types in .net for bicep this would cause
                    // a type conflict resolution. To handle such scenarios
                    // users will have to implement their own ITypeProvider to handle
                    // such scenarios. The default TypeProvider is FIFR (First In First Registered)
                    types.Should().NotContain(typeof(Types_A.ActiveResource));
                    types.Should().NotContain(typeof(Types_B.ActiveResource));

                    types.Should().NotContain(typeof(InternalActiveResource));
                    types.Should().NotContain(typeof(Types_A.InternalActiveResource));
                    types.Should().NotContain(typeof(Types_B.InternalActiveResource));

                    types.Should().NotContain(typeof(PrivateNestedActiveResource));

                    types.Should().NotContain(typeof(NoAttributeResource));
                    types.Should().NotContain(typeof(Types_A.NoAttributeResource));
                    types.Should().NotContain(typeof(Types_B.NoAttributeResource));

                    types.Should().NotContain(typeof(NestedNoAttributeResource));
                    types.Should().NotContain(typeof(FallbackWithoutAttribute));
                }


                [TestMethod]
                public void GetResourceTypes_Throws_If_DuplicateResourceTypesFound()
                {
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

                    FluentActions.Invoking(() => provider.GetResourceTypes(throwOnDuplicate: true).ToList())
                        .Should().Throw<InvalidOperationException>();
                }

                #region ConfigurationType Tests

                [TestMethod]
                public void ConfigurationType_Is_Null_When_Not_Registered()
                {
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], configurationTypeContainer: null);

                    provider.ConfigurationType.Should().BeNull("no configuration type was registered");
                }

                [TestMethod]
                public void ConfigurationType_Is_Set_When_Registered()
                {
                    var configContainer = new Types.Models.ConfigurationTypeContainer(typeof(ConfigurationType));
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], configurationTypeContainer: configContainer);

                    provider.ConfigurationType.Should().Be(typeof(ConfigurationType));
                }

                [TestMethod]
                public void ConfigurationType_Can_Be_Nested_Type()
                {
                    var configContainer = new Types.Models.ConfigurationTypeContainer(typeof(NestedConfigurationType));
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], configurationTypeContainer: configContainer);

                    provider.ConfigurationType.Should().Be(typeof(NestedConfigurationType));
                }

                #endregion

                #region FallbackType Tests

                [TestMethod]
                public void FallbackType_Is_Null_When_Not_Registered()
                {
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: null);

                    provider.FallbackType.Should().BeNull("no fallback type was registered");
                }

                [TestMethod]
                public void FallbackType_Is_Set_When_Valid_Type_Registered()
                {
                    var fallbackContainer = new Types.Models.FallbackTypeContainer(typeof(FallbackResource));
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: fallbackContainer);

                    provider.FallbackType.Should().Be(typeof(FallbackResource), 
                        "fallback type is public and decorated with ResourceTypeAttribute");
                }

                [TestMethod]
                public void FallbackType_Throws_When_Type_Missing_ResourceTypeAttribute()
                {
                    var fallbackContainer = new Types.Models.FallbackTypeContainer(typeof(FallbackWithoutAttribute));

                    FluentActions.Invoking(() => new TypeProvider(
                        [typeof(TypeProviderTests).Assembly], 
                        fallbackTypeContainer: fallbackContainer))
                        .Should().Throw<InvalidOperationException>()
                        .WithMessage("*ResourceTypeAttribute*",
                            "fallback type must be decorated with ResourceTypeAttribute");
                }

                [TestMethod]
                public void FallbackType_Throws_When_Type_Is_Not_Visible()
                {
                    var fallbackContainer = new Types.Models.FallbackTypeContainer(typeof(PrivateNestedActiveResource));

                    FluentActions.Invoking(() => new TypeProvider(
                        [typeof(TypeProviderTests).Assembly], 
                        fallbackTypeContainer: fallbackContainer))
                        .Should().Throw<InvalidOperationException>()
                        .WithMessage("*ResourceTypeAttribute*",
                            "fallback type must be publicly visible");
                }

                [TestMethod]
                public void FallbackType_Can_Be_Nested_Public_Type()
                {
                    var fallbackContainer = new Types.Models.FallbackTypeContainer(typeof(NestedActiveResource));
                    var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: fallbackContainer);

                    provider.FallbackType.Should().Be(typeof(NestedActiveResource),
                        "nested public type with ResourceTypeAttribute is valid");
                }

                #endregion
            }
        }
