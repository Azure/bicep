// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.Types.Attributes;
using FluentAssertions;

namespace Bicep.Local.Extension.UnitTests.TypesTests

{
    [BicepType(IsActive = true)]
    public class ActiveResource { }

    [BicepType(IsActive = false)]
    public class InactiveResource { }

    public class NoAttributeResource { }

    [BicepType(IsActive = true)]
    internal class InternalActiveResource { }
}

namespace Bicep.Local.Extension.UnitTests.TypesTests.TypeDefinitionBuilderTests.Types_B
{
    [BicepType(IsActive = true)]
    public class ActiveResource { }

    [BicepType(IsActive = false)]
    public class InactiveResource { }

    public class NoAttributeResource { }

    [BicepType(IsActive = true)]
    internal class InternalActiveResource { }
}

namespace Bicep.Local.Extension.UnitTests.TypesTests.TypeDefinitionBuilderTests
{
    [BicepType(IsActive = true)]
    public class ActiveResource { }

    [BicepType(IsActive = false)]
    public class InactiveResource { }

    public class NoAttributeResource { }

    [BicepType(IsActive = true)]
    internal class InternalActiveResource { }

    [TestClass]
    public class TypeProviderTests
    {
        [BicepType(IsActive = true)]
        public class NestedActiveResource { }

        [BicepType(IsActive = false)]
        public class NestedInactiveResource { }

        public class NestedNoAttributeResource { }

        [BicepType(IsActive = true)]
        private class PrivateNestedActiveResource { }


        [TestMethod]
        public void GetResourceTypes_Only_Returns_Public_Active_Types()
        {
            var provider = new TypeProvider();

            var types = provider.GetResourceTypes();

            types.Should().HaveCount(2, "only public active types should be returned");

            types.Should().Contain(typeof(ActiveResource));            
            types.Should().Contain(typeof(NestedActiveResource));

            // although these are unique types in .net for bicep this would cause
            // a type conflict resolution. To handle such scenarios
            // users will have to implement their own ITypeProvider to handle
            // such scenarios. The default TypeProvider will only accept the first
            // occurance of the class name
            types.Should().NotContain(typeof(TypesTests.TypeDefinitionBuilderTests.ActiveResource));
            types.Should().NotContain(typeof(Types_B.ActiveResource));

            types.Should().NotContain(typeof(InternalActiveResource));
            types.Should().NotContain(typeof(PrivateNestedActiveResource));

            types.Should().NotContain(typeof(InactiveResource));
            types.Should().NotContain(typeof(TypesTests.TypeDefinitionBuilderTests.InactiveResource));
            types.Should().NotContain(typeof(Types_B.InactiveResource));
            types.Should().NotContain(typeof(NestedInactiveResource));
            types.Should().NotContain(typeof(NoAttributeResource));
            types.Should().NotContain(typeof(NestedNoAttributeResource));  
            types.Should().NotContain(typeof(TypesTests.TypeDefinitionBuilderTests.ActiveResource));
            types.Should().NotContain(typeof(Types_B.ActiveResource));          
        }
    }
}

