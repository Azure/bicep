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

            var types = provider.GetResourceTypes(throwOnDuplicate: false).Select(x => x.type).ToList();

            types.Should().HaveCount(2, "only public types in the same namespaces should be returned");

            types.Should().Contain(typeof(ActiveResource));
            types.Should().Contain(typeof(NestedActiveResource));

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

        }


        [TestMethod]
        public void GetResourceTypes_Throws_If_DuplicateResourceTypesFound()
        {
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            FluentActions.Invoking(() => provider.GetResourceTypes(throwOnDuplicate: true).ToList())
                .Should().Throw<InvalidOperationException>();
        }
    }
}
