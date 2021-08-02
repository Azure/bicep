// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.UnitTests.Modules
{
    [TestClass]
    public class ModuleReferenceTests
    {
        [TestMethod]
        public void ModuleReferenceShouldBeDerivedAtLeastOnce()
        {
            var subClasses = GetModuleRefSubClasses().ToList();

            subClasses.Should().HaveCountGreaterOrEqualTo(1);
            subClasses.Should().Contain(item => (Type)item[0] == typeof(LocalModuleReference));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetModuleRefSubClasses), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet))]
        public void ModuleRefSubClassesShouldOverrideEqualsAndHashCode(Type type)
        {
            /* 
             * The module reference subclasses must implement Equals() and GetHashCode() so reference deduping works correctly.
             */
            static MethodInfo? GetDeclaredMethod(Type type, string name) => type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);

            var equals = GetDeclaredMethod(type, nameof(object.Equals));
            equals.Should().NotBeNull();
            equals!.ReturnType.Should().Be(typeof(bool));
            equals.GetParameters().Should().SatisfyRespectively(x => x.ParameterType.Should().Be(typeof(object)));

            var getHashCode = GetDeclaredMethod(type, nameof(object.GetHashCode));
            getHashCode.Should().NotBeNull();
            getHashCode!.ReturnType.Should().Be(typeof(int));
            getHashCode.GetParameters().Should().BeEmpty();
        }

        private static IEnumerable<object[]> GetModuleRefSubClasses() => typeof(ModuleReference).Assembly
            .GetTypes()
            .Where(type => type.IsClass && type.IsSubclassOf(typeof(ModuleReference)))
            .Select(type => new[] { type });
    }
}
