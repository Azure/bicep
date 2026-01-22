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

    // Add versioned resources for testing
    [ResourceType("VersionedResource", "v1")]
    public class VersionedResourceV1 { }

    [ResourceType("VersionedResource", "v2")]
    public class VersionedResourceV2 { }

    [ResourceType("VersionedResource", "v3")]
    public class VersionedResourceV3 { }
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

            // Updated count: 1 ActiveResource (FIFO) + 3 VersionedResource (v1,v2,v3) + 1 NestedActiveResource + 1 FallbackResource = 6
            types.Should().HaveCount(6, "only public types with ResourceTypeAttribute should be returned, duplicates resolved by FIFO");

            // One ActiveResource (first discovered, duplicates excluded)
            types.Should().Contain(typeof(ActiveResource));
            types.Should().Contain(typeof(NestedActiveResource));
            types.Should().Contain(typeof(FallbackResource));
            
            // All three versioned resources (unique FullNames)
            types.Should().Contain(typeof(Types_A.VersionedResourceV1), "v1 is a unique version");
            types.Should().Contain(typeof(Types_A.VersionedResourceV2), "v2 is a unique version");
            types.Should().Contain(typeof(Types_A.VersionedResourceV3), "v3 is a unique version");

            // although these are unique types in .net for bicep this would cause
            // a type conflict resolution. To handle such scenarios
            // users will have to implement their own ITypeProvider to handle
            // such scenarios. The default TypeProvider is FIFO (First In First Registered)
            types.Should().NotContain(typeof(Types_A.ActiveResource), "duplicate ActiveResource filtered by FIFO");
            types.Should().NotContain(typeof(Types_B.ActiveResource), "duplicate ActiveResource filtered by FIFO");

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

        #region Versioning Tests

        [TestMethod]
        public void GetResourceTypes_Treats_Different_Versions_As_Unique_Types()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act - use throwOnDuplicate: false to avoid ActiveResource duplicates
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Assert - should find all three versions
            var versionedTypes = types.Where(x => x.attribute.Name == "VersionedResource").ToList();

            versionedTypes.Should().HaveCount(3,
                "different versions of the same resource name should be treated as unique types");

            versionedTypes.Should().Contain(x => x.attribute.FullName == "VersionedResource@v1");
            versionedTypes.Should().Contain(x => x.attribute.FullName == "VersionedResource@v2");
            versionedTypes.Should().Contain(x => x.attribute.FullName == "VersionedResource@v3");

            // Verify they map to correct types
            versionedTypes.Should().Contain(x => x.type == typeof(Types_A.VersionedResourceV1));
            versionedTypes.Should().Contain(x => x.type == typeof(Types_A.VersionedResourceV2));
            versionedTypes.Should().Contain(x => x.type == typeof(Types_A.VersionedResourceV3));
        }

        [TestMethod]
        public void GetResourceTypes_Versioned_Types_Do_Not_Conflict_With_ThrowOnDuplicate()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act - get only versioned types by filtering
            var allTypes = provider.GetResourceTypes(throwOnDuplicate: false).ToList();
            var versionedTypes = allTypes.Where(x => x.attribute.Name == "VersionedResource").ToList();

            // Assert - versioned types should not be considered duplicates
            versionedTypes.Should().HaveCount(3, "versioned resources should all be present");

            // Verify each has unique FullName
            var fullNames = versionedTypes.Select(x => x.attribute.FullName).ToList();
            fullNames.Should().OnlyHaveUniqueItems("each version should have a unique FullName");
        }

        [TestMethod]
        public void GetResourceTypes_VersionedResource_FullName_Includes_Version()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();
            var versionedType = types.FirstOrDefault(x => x.type == typeof(Types_A.VersionedResourceV1));

            // Assert
            versionedType.Should().NotBeNull();
            versionedType!.attribute.FullName.Should().Be("VersionedResource@v1",
                "FullName should include version in format Name@Version");
            versionedType.attribute.Name.Should().Be("VersionedResource");
            versionedType.attribute.ApiVersion.Should().Be("v1");
        }

        [TestMethod]
        public void GetResourceTypes_Resource_Without_Version_Has_FullName_Equal_To_Name()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();
            var fallbackType = types.FirstOrDefault(x => x.type == typeof(FallbackResource));

            // Assert
            fallbackType.Should().NotBeNull();
            fallbackType!.attribute.FullName.Should().Be("FallbackResource",
                "FullName should equal Name when no version is specified");
            fallbackType.attribute.ApiVersion.Should().BeNull();
        }

        #endregion

        #region Duplicate Detection Tests

        [TestMethod]
        public void GetResourceTypes_Throws_When_Duplicate_FullNames_Exist_And_ThrowOnDuplicate_Is_True()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act & Assert - should throw because ActiveResource appears 3 times with same FullName
            FluentActions.Invoking(() => provider.GetResourceTypes(throwOnDuplicate: true).ToList())
                .Should().Throw<InvalidOperationException>()
                .WithMessage("*Duplicate resource type*")
                .WithMessage("*ActiveResource*",
                    "error should identify the duplicate resource type name");
        }

        [TestMethod]
        public void GetResourceTypes_Returns_First_Occurrence_When_Duplicates_Exist_And_ThrowOnDuplicate_Is_False()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act - throwOnDuplicate: false should use FIFO (First In First Out)
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Assert - should return only ONE ActiveResource (the first one discovered)
            var activeResources = types.Where(x => x.attribute.FullName == "ActiveResource").ToList();
            activeResources.Should().HaveCount(1,
                "when duplicates exist with throwOnDuplicate=false, only first occurrence should be returned (FIFO)");
        }

        [TestMethod]
        public void GetResourceTypes_Duplicate_Exception_Includes_Conflicting_Type_Names()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act & Assert
            var exception = FluentActions.Invoking(() => provider.GetResourceTypes(throwOnDuplicate: true).ToList())
                .Should().Throw<InvalidOperationException>()
                .Which;

            // Verify exception message contains useful debugging information
            exception.Message.Should().Contain("ActiveResource", "should show the duplicate resource type name");
            exception.Message.Should().MatchRegex(@"Types:.*TypesTests",
                "should include .NET type names for debugging");
        }

        [TestMethod]
        public void GetResourceTypes_Does_Not_Consider_Versioned_Resources_As_Duplicates()
        {
            // Arrange - create a provider and get all types
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);
            var allTypes = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Act - check that versioned resources are not grouped as duplicates
            var versionedResourceGroups = allTypes
                .Where(x => x.attribute.Name == "VersionedResource")
                .GroupBy(x => x.attribute.FullName)
                .ToList();

            // Assert - each version should have its own unique group
            versionedResourceGroups.Should().HaveCount(3,
                "each version should be treated as a separate resource type");

            versionedResourceGroups.Should().OnlyContain(g => g.Count() == 1,
                "no versioned resource should have duplicates");
        }

        [TestMethod]
        public void GetResourceTypes_Detects_Duplicates_By_FullName_Not_By_DotNet_Type()
        {
            // Arrange
            var provider = new TypeProvider([typeof(TypeProviderTests).Assembly]);

            // Act
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Assert - even though there are 3 different .NET types with [ResourceType("ActiveResource")],
            // they all have the same FullName, so only one should be returned
            var activeResourceTypes = types.Where(x => x.attribute.FullName == "ActiveResource").ToList();
            activeResourceTypes.Should().HaveCount(1,
                "duplicate detection is based on FullName (Name@Version), not .NET type identity");

            // The .NET types are different, but Bicep FullName is the same
            var allActiveResourceDotNetTypes = new[]
            {
                        typeof(ActiveResource),
                        typeof(Types_A.ActiveResource),
                        typeof(Types_B.ActiveResource)
                    };

            // Only one of these should be in the result
            var matchingTypes = types.Where(x => allActiveResourceDotNetTypes.Contains(x.type)).ToList();
            matchingTypes.Should().HaveCount(1,
                "only the first discovered ActiveResource should be returned");
        }

        #endregion

        #region Assembly Handling Tests

        [TestMethod]
        public void TypeProvider_Deduplicates_Duplicate_Assemblies()
        {
            // Arrange - register the same assembly multiple times
            var assembly = typeof(TypeProviderTests).Assembly;
            var duplicateAssemblies = new[] { assembly, assembly, assembly };

            // Act
            var provider = new TypeProvider(duplicateAssemblies);
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Assert - should get same results as single registration
            // Should not triple-count types
            types.Should().Contain(x => x.type == typeof(FallbackResource));

            var fallbackCount = types.Count(x => x.type == typeof(FallbackResource));
            fallbackCount.Should().Be(1, "duplicate assemblies should be deduplicated internally");
        }

        [TestMethod]
        public void TypeProvider_Treats_Empty_Assembly_List_As_Default()
        {
            // Arrange
            var emptyAssemblies = Array.Empty<Assembly>();

            // Act
            var provider = new TypeProvider(emptyAssemblies);
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Assert - should use default assemblies (executing + entry + referenced)
            types.Should().NotBeEmpty("empty assembly list should trigger default assembly discovery");
        }

        [TestMethod]
        public void TypeProvider_Handles_Null_Assembly_List()
        {
            // Arrange & Act
            var provider = new TypeProvider(assemblies: null);
            var types = provider.GetResourceTypes(throwOnDuplicate: false).ToList();

            // Assert - should use default assemblies
            types.Should().NotBeEmpty("null assembly list should trigger default assembly discovery");
        }
    }
}

        #endregion
