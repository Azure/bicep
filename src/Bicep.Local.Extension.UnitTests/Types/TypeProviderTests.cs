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
using Bicep.Local.Extension.Types.Models;
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

    public class FallbackWithoutAttribute { }

    [TestClass]
    public class TypeProviderTests
    {
        [ResourceType("NestedActiveResource")]
        public class NestedActiveResource { }

        public class NestedNoAttributeResource { }

        [ResourceType("PrivateNestedActiveResource")]
        private class PrivateNestedActiveResource { }


        [TestMethod]
        public void GetResourceTypes_Only_Returns_ClassesWithBicepTypeAttribute()
        {
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            var types = provider.GetResourceTypes(throwOnDuplicate: false).Select(x => x.Type).ToList();

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

        [TestMethod]
        public void GetFallbackType_Returns_Null_When_No_FallbackType_Registered()
        {
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: null);

            var fallbackType = provider.GetFallbackType();

            fallbackType.Should().BeNull("no fallback type was registered");
        }

        [TestMethod]
        public void GetFallbackType_Returns_RegisteredType_When_Type_Has_ResourceTypeAttribute()
        {
            var fallbackRegistration = new FallbackTypeRegistration(typeof(FallbackResource));
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: fallbackRegistration);

            var fallbackType = provider.GetFallbackType();

            fallbackType.Should().NotBeNull("a valid fallback type was registered");
            fallbackType!.Type.Should().Be(typeof(FallbackResource));
            fallbackType.Attribute.Should().NotBeNull();
            fallbackType.Attribute.FullName.Should().Be("FallbackResource");
        }

        [TestMethod]
        public void GetFallbackType_Returns_Null_When_RegisteredType_Missing_ResourceTypeAttribute()
        {
            var fallbackRegistration = new FallbackTypeRegistration(typeof(FallbackWithoutAttribute));
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: fallbackRegistration);

            var fallbackType = provider.GetFallbackType();

            fallbackType.Should().BeNull("fallback type does not have ResourceTypeAttribute");
        }

        [TestMethod]
        public void GetFallbackType_Returns_Null_When_RegisteredType_Is_Not_Visible()
        {
            var fallbackRegistration = new FallbackTypeRegistration(typeof(PrivateNestedActiveResource));
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly], fallbackTypeContainer: fallbackRegistration);

            var fallbackType = provider.GetFallbackType();

            fallbackType.Should().BeNull("fallback type is not publicly visible");
        }
    }
}
