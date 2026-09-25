// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualResourceCreationServiceTests
{
    private static readonly ImmutableArray<ResourceTypeComponents> CatalogFixture =
    [
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2020-01-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2021-01-01-preview", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/beta", "2020-06-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/gamma", "2019-01-01", TypeSymbolValidationFlags.Default),
    ];

    #region Catalog

    [TestMethod]
    public void GetResourceTypeNamespaces_ReturnsSortedNamespacesWithCountsAndStableCatalogId()
    {
        var fixture = CatalogFixture.Add(
            TestTypeHelper.CreateCustomResourceType("Other.Rp/delta", "2022-01-01", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);
        var service = new VisualResourceCreationService();

        var first = service.GetResourceTypeNamespaces(model);
        var second = service.GetResourceTypeNamespaces(model);

        first.CatalogId.Should().Be(second.CatalogId);
        first.Namespaces.Should().Equal(
            new VisualResourceTypeNamespace("Other.Rp", 1),
            new VisualResourceTypeNamespace("Test.Rp", 3));
    }

    [TestMethod]
    public void GetResourceTypes_ProviderNamespace_LoadsOnlyThatNamespace()
    {
        var fixture = CatalogFixture.Add(
            TestTypeHelper.CreateCustomResourceType("Other.Rp/delta", "2022-01-01", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(
            model,
            providerNamespace: "other.rp",
            query: null,
            pageSize: 50,
            continuationToken: null);

        result.Items.Should().ContainSingle();
        result.Items[0].FullyQualifiedType.Should().Be("Other.Rp/delta");
    }

    [TestMethod]
    public void GetResourceTypes_PrefersStableVersionWithSameDate()
    {
        var fixture = CatalogFixture
            .Add(TestTypeHelper.CreateCustomResourceType("Other.Rp/delta", "2022-01-01-preview", TypeSymbolValidationFlags.Default))
            .Add(TestTypeHelper.CreateCustomResourceType("Other.Rp/delta", "2022-01-01", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(
            model,
            providerNamespace: "Other.Rp",
            query: null,
            pageSize: 50,
            continuationToken: null);

        result.Items.Should().ContainSingle();
        result.Items[0].ApiVersion.Should().Be("2022-01-01");
    }

    [TestMethod]
    public void GetResourceTypes_ReturnsLatestStableApiVersionForEachType()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, providerNamespace: null, query: null, pageSize: 50, continuationToken: null);

        result.Items.Select(entry => (entry.FullyQualifiedType, entry.ApiVersion)).Should().Equal(
            ("Test.Rp/alpha", "2020-01-01"),
            ("Test.Rp/beta", "2020-06-01"),
            ("Test.Rp/gamma", "2019-01-01"));
        result.Items.Should().OnlyContain(entry => !entry.IsPreview);
        result.ContinuationToken.Should().BeNull();
    }

    [TestMethod]
    public void GetResourceTypes_QueryFilter_MatchesSubstringCaseInsensitively()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, providerNamespace: null, query: "ALPHA", pageSize: 50, continuationToken: null);

        result.Items.Should().ContainSingle();
        result.Items.Should().OnlyContain(entry => entry.FullyQualifiedType == "Test.Rp/alpha");
    }

    [TestMethod]
    public void GetResourceTypes_PagesResults_UntilContinuationTokenIsExhausted()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var firstPage = service.GetResourceTypes(model, providerNamespace: null, query: null, pageSize: 2, continuationToken: null);
        firstPage.Items.Should().HaveCount(2);
        firstPage.ContinuationToken.Should().Be("2");

        var secondPage = service.GetResourceTypes(model, providerNamespace: null, query: null, pageSize: 2, continuationToken: firstPage.ContinuationToken);
        secondPage.Items.Should().ContainSingle();
        secondPage.ContinuationToken.Should().BeNull();

        firstPage.Items.Concat(secondPage.Items)
            .Select(entry => (entry.FullyQualifiedType, entry.ApiVersion))
            .Should().Equal(
                ("Test.Rp/alpha", "2020-01-01"),
                ("Test.Rp/beta", "2020-06-01"),
                ("Test.Rp/gamma", "2019-01-01"));
    }

    [TestMethod]
    public void GetResourceTypes_NonPositivePageSize_FallsBackToDefaultPageSize()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, providerNamespace: null, query: null, pageSize: 0, continuationToken: null);

        result.Items.Should().HaveCount(3);
        result.ContinuationToken.Should().BeNull();
    }

    [TestMethod]
    public void GetResourceTypes_PageSizeAboveMax_IsClampedRatherThanRejected()
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        var result = service.GetResourceTypes(model, providerNamespace: null, query: null, pageSize: 10_000, continuationToken: null);

        result.Items.Should().HaveCount(3);
        result.ContinuationToken.Should().BeNull();
    }

    [TestMethod]
    public void GetResourceTypes_PreviewOnlyType_ReturnsNewestPreview()
    {
        var fixture = CatalogFixture
            .Add(TestTypeHelper.CreateCustomResourceType("Test.Rp/previewOnly", "2022-01-01-preview", TypeSymbolValidationFlags.Default))
            .Add(TestTypeHelper.CreateCustomResourceType("Test.Rp/previewOnly", "2023-01-01-preview", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);
        var service = new VisualResourceCreationService();

        var catalog = service.GetResourceTypes(model, providerNamespace: null, query: "previewOnly", 50, null);

        catalog.Items.Should().Equal(new VisualResourceTypeCatalogEntry("Test.Rp/previewOnly", "2023-01-01-preview", true));
        service.GetResourceTypeNamespaces(model).Namespaces.Should().ContainSingle()
            .Which.ResourceTypeCount.Should().Be(4);
    }

    [TestMethod]
    public void GetResourceTypeVersions_ReturnsAllVersionsNewestFirstWithMatchingCatalogId()
    {
        var fixture = CatalogFixture
            .Add(TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2021-01-01", TypeSymbolValidationFlags.Default));
        var model = CreateModel(fixture, string.Empty);
        var service = new VisualResourceCreationService();

        var versions = service.GetResourceTypeVersions(model, "test.rp/ALPHA");

        versions.ApiVersions.Should().Equal("2021-01-01", "2021-01-01-preview", "2020-01-01");
        versions.CatalogId.Should().Be(service.GetResourceTypeNamespaces(model).CatalogId);
        versions.CatalogId.Should().Be(service.GetResourceTypes(model, providerNamespace: null, query: null, 50, null).CatalogId);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("Test.Rp/missing")]
    public void GetResourceTypeVersions_UnknownType_ReportsFailure(string resourceType)
    {
        var model = CreateModel(CatalogFixture, string.Empty);
        var service = new VisualResourceCreationService();

        Action act = () => service.GetResourceTypeVersions(model, resourceType);

        act.Should().Throw<VisualResourceCreationException>().WithMessage("*was not found.");
    }

    private static readonly ImmutableArray<ResourceTypeComponents> ScopedFixture =
    [
        CreateScopedType("Scope.Rp/resourceGroupOnly", ResourceScope.ResourceGroup),
        CreateScopedType("Scope.Rp/subscriptionOnly", ResourceScope.Subscription),
        CreateScopedType("Scope.Rp/managementGroupOnly", ResourceScope.ManagementGroup),
        CreateScopedType("Scope.Rp/tenantOnly", ResourceScope.Tenant),
        CreateScopedType("Scope.Rp/extensionOnly", ResourceScope.Resource),
        CreateScopedType("Scope.Rp/everywhere", ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource),
        // Readable at the resource group (usable with `existing`) but only deployable at the subscription.
        CreateScopedType("Scope.Rp/readOnlyAtResourceGroup", ResourceScope.ResourceGroup | ResourceScope.Subscription, readOnlyScopes: ResourceScope.ResourceGroup),
        CreateScopedType("Other.Rp/tenantOnly", ResourceScope.Tenant),
    ];

    [DataTestMethod]
    [DataRow("resourceGroup", new[] { "Scope.Rp/everywhere", "Scope.Rp/resourceGroupOnly" })]
    [DataRow("subscription", new[] { "Scope.Rp/everywhere", "Scope.Rp/readOnlyAtResourceGroup", "Scope.Rp/subscriptionOnly" })]
    [DataRow("managementGroup", new[] { "Scope.Rp/everywhere", "Scope.Rp/managementGroupOnly" })]
    [DataRow("tenant", new[] { "Other.Rp/tenantOnly", "Scope.Rp/everywhere", "Scope.Rp/tenantOnly" })]
    public void ResourceCatalog_OffersOnlyTypesDeployableAtTheDocumentScope(string targetScope, string[] expectedTypes)
    {
        var model = CreateModel(ScopedFixture, $"targetScope = '{targetScope}'");
        var service = new VisualResourceCreationService();

        service.GetResourceTypes(model, providerNamespace: null, query: null, 50, null).Items
            .Select(entry => entry.FullyQualifiedType).Should().Equal(expectedTypes);

        var expectedNamespaces = expectedTypes
            .GroupBy(type => type[..type.IndexOf('/')])
            .Select(group => new VisualResourceTypeNamespace(group.Key, group.Count()));
        service.GetResourceTypeNamespaces(model).Namespaces.Should().Equal(expectedNamespaces);

        service.GetResourceTypes(model, providerNamespace: "Scope.Rp", query: null, 50, null).Items
            .Select(entry => entry.FullyQualifiedType).Should().Equal(expectedTypes.Where(type => type.StartsWith("Scope.Rp/")));
        service.GetResourceTypes(model, providerNamespace: null, query: "only", 50, null).Items
            .Select(entry => entry.FullyQualifiedType).Should().Equal(expectedTypes.Where(type => type.Contains("Only")));
    }

    [TestMethod]
    public void ResourceCatalog_ExcludesExtensionOnlyTypesThatRequireAScopeProperty()
    {
        var model = CreateModel(ScopedFixture, string.Empty);
        var service = new VisualResourceCreationService();

        service.GetResourceTypes(model, providerNamespace: null, query: "extensionOnly", 50, null).Items.Should().BeEmpty();
    }

    [TestMethod]
    public void CatalogId_IsStableWithinAScopeAndChangesWithTheScope()
    {
        var service = new VisualResourceCreationService();
        var resourceGroupModel = CreateModel(ScopedFixture, "targetScope = 'resourceGroup'");
        var subscriptionModel = CreateModel(ScopedFixture, "targetScope = 'subscription'");

        var resourceGroupId = service.GetResourceTypeNamespaces(resourceGroupModel).CatalogId;

        service.GetResourceTypeNamespaces(resourceGroupModel).CatalogId.Should().Be(resourceGroupId);
        service.GetResourceTypes(resourceGroupModel, providerNamespace: null, query: null, 50, null).CatalogId.Should().Be(resourceGroupId);
        service.GetResourceTypeVersions(resourceGroupModel, "Scope.Rp/everywhere").CatalogId.Should().Be(resourceGroupId);
        service.GetResourceTypeNamespaces(subscriptionModel).CatalogId.Should().NotBe(resourceGroupId);
    }

    [TestMethod]
    public void ResourceCatalog_VersionsAreNotFilteredByScope()
    {
        var fixture = ImmutableArray.Create(
            CreateScopedType("Scope.Rp/widgets", ResourceScope.ResourceGroup, "2024-01-01"),
            CreateScopedType("Scope.Rp/widgets", ResourceScope.Subscription, "2020-01-01"));
        var model = CreateModel(fixture, string.Empty);

        new VisualResourceCreationService().GetResourceTypeVersions(model, "Scope.Rp/widgets").ApiVersions
            .Should().Equal("2024-01-01", "2020-01-01");
    }

    #endregion

    #region PrepareResource

    [DataTestMethod]
    [DataRow("2020-01-01")]
    [DataRow("2021-01-01-preview")]
    public void CreateResourceDeclarationInsertion_UsesTheExplicitlySelectedVersion(string apiVersion)
    {
        var (compiler, result) = CompileWithResourceTypes(CatalogFixture, string.Empty);
        var context = new CompilationContext(result.Compilation);
        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "selected-version",
            new VisualResourceTypeIdentifier("Test.Rp/alpha", apiVersion));

        var response = new VisualResourceCreationService().CreateResourceDeclarationInsertion(compiler, context, request);

        ApplyEdit(string.Empty, context.LineStarts, response.Edit).Should().Contain($"'Test.Rp/alpha@{apiVersion}'");
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_GeneratesValidTopLevelDeclarationAndVersionedEdit()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 7 },
            "operation-1",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.CreateResourceDeclarationInsertion(compiler, context, request);

        response.OperationId.Should().Be("operation-1");
        response.SymbolicName.Should().Be("basicTest");
        response.ExpectedNodeId.Should().Be("basicTest");
        response.UnresolvedRequiredProperties.Should().BeEmpty();

        var textDocumentEdit = response.Edit.DocumentChanges.Should().ContainSingle().Subject.TextDocumentEdit;
        textDocumentEdit.Should().NotBeNull();
        textDocumentEdit!.TextDocument.Uri.Should().Be(request.TextDocument.Uri);
        textDocumentEdit.TextDocument.Version.Should().Be(request.TextDocument.Version);

        var updatedContent = ApplyEdit(string.Empty, context.LineStarts, response.Edit);
        updatedContent.ReplaceLineEndings("\n").Should().Be("""
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name: 'basicTest'
            }
            """);
        var (_, updatedResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, updatedContent);
        updatedResult.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_SymbolicNameCollision_GeneratesUniqueSuffixedName()
    {
        var content = """
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name: 'existing'
            }
            """;

        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        compilationResult.Should().NotHaveAnyDiagnostics();
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-2",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.CreateResourceDeclarationInsertion(compiler, context, request);

        response.SymbolicName.Should().Be("basicTest1");

        var updatedContent = ApplyEdit(content, context.LineStarts, response.Edit);
        updatedContent.ReplaceLineEndings("\n").Should().Contain(
            "resource basicTest1 'Test.Rp/basicTests@2020-01-01' = {\n  name: 'basicTest1'\n}");
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_WithExistingResources_InsertsAfterLastResourceAndBeforeOutput()
    {
        var content = """
            resource first 'Test.Rp/basicTests@2020-01-01' = {
              name: 'first'
            }

            resource second 'Test.Rp/basicTests@2020-01-01' = {
              name: 'second'
            }

            output result string = second.name
            """;

        var updatedContent = PrepareAndApply(content);

        updatedContent.IndexOf("resource second", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal));
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
        updatedContent.ReplaceLineEndings("\n").Should().Contain("}\n\nresource basicTest");
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_WithoutExistingResource_InsertsAfterParametersAndVariablesAndBeforeOutputs()
    {
        var content = """
            param prefix string
            var resourceName = '${prefix}-resource'

            output result string = resourceName
            """;

        var updatedContent = PrepareAndApply(content);

        updatedContent.IndexOf("var resourceName", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal));
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_WithOnlyOutputs_InsertsBeforeFirstOutput()
    {
        var content = "output result string = 'value'";

        var updatedContent = PrepareAndApply(content);

        updatedContent.Should().StartWith("resource basicTest");
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_ReadWriteType_ReportsUnresolvedRequiredProperties()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-3",
            new VisualResourceTypeIdentifier("Test.Rp/readWriteTests", "2020-01-01"));

        var response = service.CreateResourceDeclarationInsertion(compiler, context, request);

        response.UnresolvedRequiredProperties.Should().Equal("properties");
        ApplyEdit(string.Empty, context.LineStarts, response.Edit).ReplaceLineEndings("\n").Should().Be("""
            resource readWriteTest 'Test.Rp/readWriteTests@2020-01-01' = {
              name: 'readWriteTest'
              properties: {
                required:
              }
            }
            """);
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_UsesConfiguredFormattingForIncompleteValues()
    {
        var bicepConfig = """
            {
              "formatting": {
                "indentKind": "Tab",
                "newlineKind": "CRLF"
              }
            }
            """;
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty, bicepConfig);
        var context = new CompilationContext(compilationResult.Compilation);
        var request = CreateRequest("Test.Rp/readWriteTests", "2020-01-01");

        var response = new VisualResourceCreationService().CreateResourceDeclarationInsertion(compiler, context, request);

        ApplyEdit(string.Empty, context.LineStarts, response.Edit).Should().Be(
            "resource readWriteTest 'Test.Rp/readWriteTests@2020-01-01' = {\r\n" +
            "\tname: 'readWriteTest'\r\n" +
            "\tproperties: {\r\n" +
            "\t\trequired:\r\n" +
            "\t}\r\n" +
            "}");
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_DiscriminatedType_ReportsDiscriminatorKeyAsUnresolved()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-4",
            new VisualResourceTypeIdentifier("Test.Rp/discriminatorTests", "2020-01-01"));

        var response = service.CreateResourceDeclarationInsertion(compiler, context, request);

        response.UnresolvedRequiredProperties.Should().Equal("kind");
        ApplyEdit(string.Empty, context.LineStarts, response.Edit).ReplaceLineEndings("\n").Should().Be("""
            resource discriminatorTest 'Test.Rp/discriminatorTests@2020-01-01' = {
              kind:
            }
            """);
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_VersionNotDeployableAtTheDocumentScope_Throws()
    {
        var fixture = ImmutableArray.Create(
            CreateScopedType("Scope.Rp/widgets", ResourceScope.ResourceGroup, "2024-01-01"),
            CreateScopedType("Scope.Rp/widgets", ResourceScope.Subscription, "2020-01-01"));
        var (compiler, result) = CompileWithResourceTypes(fixture, string.Empty);
        var context = new CompilationContext(result.Compilation);
        var service = new VisualResourceCreationService();

        CreateRequest("Scope.Rp/widgets", "2024-01-01").Invoking(request => service.CreateResourceDeclarationInsertion(compiler, context, request))
            .Should().NotThrow();
        CreateRequest("Scope.Rp/widgets", "2020-01-01").Invoking(request => service.CreateResourceDeclarationInsertion(compiler, context, request))
            .Should().Throw<VisualResourceCreationException>()
            .WithMessage("Resource type \"Scope.Rp/widgets@2020-01-01\" cannot be deployed at the \"resourceGroup\" scope.");
    }

    [TestMethod]
    public void CreateResourceDeclarationInsertion_UnknownResourceType_ThrowsVisualResourceCreationException()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();

        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-5",
            new VisualResourceTypeIdentifier("Test.Rp/doesNotExist", "2020-01-01"));

        Action act = () => service.CreateResourceDeclarationInsertion(compiler, context, request);

        act.Should().Throw<VisualResourceCreationException>()
            .WithMessage("Resource type \"Test.Rp/doesNotExist@2020-01-01\" was not found.");
    }

    #endregion

    private static ResourceTypeComponents CreateScopedType(
        string fullyQualifiedType,
        ResourceScope scopes,
        string apiVersion = "2024-01-01",
        ResourceScope readOnlyScopes = ResourceScope.None) =>
        TestTypeHelper.CreateCustomResourceType(
            fullyQualifiedType, apiVersion, TypeSymbolValidationFlags.Default, scopes, readOnlyScopes, ResourceFlags.None);

    private static CreateResourceDeclarationInsertionParams CreateRequest(string fullyQualifiedType, string apiVersion) =>
        new(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "scope-check",
            new VisualResourceTypeIdentifier(fullyQualifiedType, apiVersion));

    private static SemanticModel CreateModel(IEnumerable<ResourceTypeComponents> resourceTypes, string content) =>
        CompilationHelper.Compile(new ServiceBuilder().WithAzResources(resourceTypes), content).Compilation.GetEntrypointSemanticModel();

    // Mirrors CompilationHelper.Compile's internal implementation, but also returns the BicepCompiler used to
    // build the compilation. The service needs a compiler built from the exact same registrations as the
    // supplied CompilationContext so its internal self-validation recompile of the generated resource
    // declaration succeeds.
    private static (BicepCompiler Compiler, CompilationHelper.CompilationResult Result) CompileWithResourceTypes(
        IEnumerable<ResourceTypeComponents> resourceTypes, string content, string? bicepConfig = null)
    {
        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", content);
        if (bicepConfig is not null)
        {
            fileSet.AddFile("bicepconfig.json", bicepConfig);
        }

        var compiler = new ServiceBuilder()
            .WithAzResources(resourceTypes)
            .WithFileExplorer(fileSet.FileExplorer)
            .WithFileSystem(fileSet.FileSystem)
            .Build()
            .GetCompiler();

        var compilation = compiler.CreateCompilationWithoutRestore(fileSet.GetUri("main.bicep"));
        return (compiler, CompilationHelper.GetCompilationResult(compilation));
    }

    private static string PrepareAndApply(string content)
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceCreationService();
        var request = new CreateResourceDeclarationInsertionParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.CreateResourceDeclarationInsertion(compiler, context, request);
        return ApplyEdit(content, context.LineStarts, response.Edit);
    }

    // The generated code replacement is always a zero-length insertion, so applying it is a plain string insertion.
    private static string ApplyEdit(string content, ImmutableArray<int> lineStarts, WorkspaceEdit edit)
    {
        var textDocumentEdit = edit.DocumentChanges!.Single().TextDocumentEdit!;
        var textEdit = textDocumentEdit.Edits.Single();
        var offset = PositionHelper.GetOffset(lineStarts, textEdit.Range.Start);

        return content.Insert(offset, textEdit.NewText);
    }
}
