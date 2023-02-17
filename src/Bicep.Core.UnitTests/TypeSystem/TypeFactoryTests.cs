// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem;

[TestClass]
public class TypeFactoryTests
{
    [TestMethod]
    public void Factory_reuses_immutable_integer_instances_for_identical_parameters()
    {
        TypeFactory.CreateIntegerType(0, 10).Should().BeSameAs(TypeFactory.CreateIntegerType(0, 10));
    }
}
