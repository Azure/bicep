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
    [BicepType]
    public class ActiveResource { }

    public class NoAttributeResource { }

    [BicepType]
    internal class InternalActiveResource { }
}

namespace Bicep.Local.Extension.UnitTests.TypesTests.Types_B
{
    [BicepType]
    public class ActiveResource { }

    public class NoAttributeResource { }

    [BicepType]
    internal class InternalActiveResource { }
}

namespace Bicep.Local.Extension.UnitTests.TypesTests
{
    [BicepType]
    public class ActiveResource { }

    public class NoAttributeResource { }

    [BicepType]
    internal class InternalActiveResource { }

    [TestClass]
    public class TypeProviderTests
    {
        [BicepType]
        public class NestedActiveResource { }

        public class NestedNoAttributeResource { }

        [BicepType]
        private class PrivateNestedActiveResource { }


        [TestMethod]
        public void GetResourceTypes_Only_Returns_Public_Active_Types()
        {
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            var types = provider.GetResourceTypes();

            types.Should().HaveCount(2, "only public types in the same namespaces should be returned");

            types.Should().Contain(typeof(ActiveResource));
            types.Should().Contain(typeof(NestedActiveResource));

            // although these are unique types in .net for bicep this would cause
            // a type conflict resolution. To handle such scenarios
            // users will have to implement their own ITypeProvider to handle
            // such scenarios. The default TypeProvider will only accept the first
            // occurrence of the class name
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
    }
}

