// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Artifacts.LegacyModuleAliasMocks;

/// <summary>
/// <c>moduleAliasesMock</c> is the only deletable reference feature (see the REP 0022 implementation plan).
/// This test keeps it isolated so it can be removed without touching shared code.
/// </summary>
[TestClass]
public class ModuleAliasMockIsolationTests
{
    private static readonly Regex MockReferencePattern = new("ModuleAliasesMock|ModuleAliasMock|LegacyModuleAliasMocks", RegexOptions.CultureInvariant);

    private static readonly string[] AllowedPathPrefixes =
    [
        // The mock implementation.
        "Bicep.Core/Artifacts/LegacyModuleAliasMocks/",
        // The single call site, at step 3 of reference resolution.
        "Bicep.Core/Registry/ArtifactReferenceResolver.cs",
        // Configuration binding and the declaring-URI annotation.
        "Bicep.Core/Configuration/BicepConfiguration.cs",
        "Bicep.Core/Configuration/BicepConfigurationExtensions.cs",
        "Bicep.Core/Configuration/BicepConfigurationManager.cs",
        "Bicep.Core/Configuration/IBicepConfiguration.cs",
        "Bicep.Core/Configuration/IBicepModuleAliasesMockConfiguration.cs",
        "Bicep.Core/Configuration/ModuleAliasesMockConfiguration.cs",
    ];

    [TestMethod]
    public void Module_alias_mocks_are_only_referenced_from_allowed_product_files()
    {
        var srcRoot = BaselineHelper.GetAbsolutePathRelativeToRepoRoot("src");

        var productProjectDirectories = Directory.GetFiles(srcRoot, "*.csproj", SearchOption.AllDirectories)
            .Select(Path.GetDirectoryName)
            .OfType<string>()
            .Where(directory => !Path.GetFileName(directory).Contains("Test", StringComparison.Ordinal) &&
                !Path.GetFileName(directory).Contains("Samples", StringComparison.Ordinal))
            .ToArray();

        productProjectDirectories.Should().Contain(directory => Path.GetFileName(directory) == "Bicep.Core");

        var violations = productProjectDirectories
            .SelectMany(directory => Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories))
            .Select(file => Path.GetRelativePath(srcRoot, file).Replace('\\', '/'))
            .Where(relativePath => !relativePath.Contains("/obj/", StringComparison.Ordinal) && !relativePath.Contains("/bin/", StringComparison.Ordinal))
            .Where(relativePath => !AllowedPathPrefixes.Any(prefix => relativePath.StartsWith(prefix, StringComparison.Ordinal)))
            .Where(relativePath => MockReferencePattern.IsMatch(File.ReadAllText(Path.Combine(srcRoot, relativePath))))
            .ToArray();

        violations.Should().BeEmpty("moduleAliasesMock must stay isolated in the LegacyModuleAliasMocks folder and its allowed call sites");
    }
}
