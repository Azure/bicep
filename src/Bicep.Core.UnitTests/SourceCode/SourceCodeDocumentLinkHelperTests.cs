// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class SourceCodeDocumentLinkHelperTests : TestBase
{
    private Uri GetUri(string relativePath)
    {
        return new Uri(InMemoryFileResolver.GetFileUri("/path/to/"), relativePath);
    }

    [TestMethod]
    public void Single()
    {
        var result = CompilationHelper.Compile(
            [
                    ("createVM.bicep", ""),
                    ("main.bicep", """
                        // A comment
                        module creatingVM 'createVM.bicep' = { // 'createVM.bicep' = [1:18]-[1:34]
                          name: 'creatingVM'
                          params: {
                            _artifactsLocation: 'my artifactsLocation'
                            _artifactsLocationSasToken: 'my artifactsLocationSasToken'
                            adminPasswordOrKey: 'adminPasswordOrKey'
                            adminUsername: 'adminUsername'
                          }
                        }
                        """)]);

        var links = SourceCodeDocumentLinkHelper.GetAllModuleDocumentLinks(result.Compilation.SourceFileGrouping);

        links.Should().BeEquivalentTo(new Dictionary<Uri, SourceCodeDocumentUriLink[]>()
        {
            {
                GetUri("main.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(1, 18, 1, 34), GetUri("createVM.bicep")),
                }
            },
        });
    }

    [TestMethod]
    public void StartingWithDotDot()
    {
        var result = CompilationHelper.Compile(
            [
                    ("createVM.bicep", ""),
                    ("main.bicep", """
                        // A comment
                        module creatingVM '../createVM.bicep' = { // '../createVM.bicep' = [1:18]-[1:37]
                        }
                        """)]);

        var links = SourceCodeDocumentLinkHelper.GetAllModuleDocumentLinks(result.Compilation.SourceFileGrouping);

        links.Should().BeEquivalentTo(new Dictionary<Uri, SourceCodeDocumentUriLink[]>()
        {
            {
                GetUri("main.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(1, 18, 1, 37), GetUri("../createVM.bicep")),
                }
            },
        });
    }

    [TestMethod]
    public void Multiple()
    {
        var result = CompilationHelper.Compile(
            [
                    ("createVM.bicep", """
                        module main '../../whatever.bicep' = {
                        }
                        """),
                    ("main.bicep", """
                        module bing 'modules/ai/bing-resources.json' = {
                        }
                        module creatingVM 'createVM.bicep' = {
                        }
                        """),
                    ("../../whatever.bicep", ""),
                    ("modules/ai/bing-resources.json", """
                        {
                          "$schema": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                          "contentVersion": "1.0.0.0",
                          "languageVersion": ""2.0",
                          "resources": {},
                          "parameters": {
                            "objectParameter": {
                              "type": "object",
                              "properties": {
                                "foo": { "type": "string" },
                                "bar": { "type": "int" },
                                "baz": { "type": "bool" }
                              }
                            }
                          }
                        }
                        """),
            ]);

        var links = SourceCodeDocumentLinkHelper.GetAllModuleDocumentLinks(result.Compilation.SourceFileGrouping);

        links.Should().BeEquivalentTo(new Dictionary<Uri, SourceCodeDocumentUriLink[]>()
        {
            {
                GetUri("createVM.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(0, 12, 0, 34), GetUri("../../whatever.bicep")),
                }
            },
            {
                GetUri("main.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(0, 12, 0, 44), GetUri("modules/ai/bing-resources.json")),
                    new(new SourceCodeRange(2, 18, 2, 34), GetUri("createVM.bicep")),
                }
            },
        });
    }

    [TestMethod]
    public void IsButsAndArrays()
    {
        var result = CompilationHelper.Compile(
            [
                    ("main.bicep", """
                        module keyVault_accessPolicies 'access-policy/main.bicep' = if (!empty(accessPolicies)) {
                        name: '${uniqueString(deployment().name, location)}-KeyVault-AccessPolicies'
                          params: {
                            keyVaultName: keyVault.name
                            accessPolicies: accessPolicies
                          }
                        }

                        module keyVault_secrets 'secret/main.bicep' = [for (secret, index) in secretList: {
                          name: '${uniqueString(deployment().name, location)}-KeyVault-Secret-${index}'
                          params: {
                            name: secret.name
                            value: secret.value
                            keyVaultName: keyVault.name
                            attributesEnabled: secret.?attributesEnabled ?? true
                            attributesExp: secret.?attributesExp
                            attributesNbf: secret.?attributesNbf
                            contentType: secret.?contentType
                            tags: secret.?tags ?? tags
                            roleAssignments: secret.?roleAssignments
                          }
                        }]

                        module keyVault_keys 'key/main.bicep' = [for (key, index) in (keys ?? []): {
                        name: '${uniqueString(deployment().name, location)}-KeyVault-Key-${index}'
                          params: {
                            name: key.name
                            keyVaultName: keyVault.name
                            attributesEnabled: key.? attributesEnabled ?? true
                            attributesExp: key.? attributesExp
                            attributesNbf: key.? attributesNbf
                            curveName: key.? curveName ?? 'P-256'
                            keyOps: key.? keyOps
                            keySize: key.? keySize
                            kty: key.? kty ?? 'EC'
                            tags: key.? tags ?? tags
                            roleAssignments: key.? roleAssignments
                            rotationPolicy: key.? rotationPolicy
                          }
                        }]
                        """),
            ]);

        var links = SourceCodeDocumentLinkHelper.GetAllModuleDocumentLinks(result.Compilation.SourceFileGrouping);

        links.Should().BeEquivalentTo(new Dictionary<Uri, SourceCodeDocumentUriLink[]>()
        {
            {
                GetUri("main.bicep"),
                new SourceCodeDocumentUriLink[]
                {
                    new(new SourceCodeRange(0, 31, 0, 31+26), GetUri("access-policy/main.bicep")),
                    new(new SourceCodeRange(8, 24, 8, 24+19), GetUri("secret/main.bicep")),
                    new(new SourceCodeRange(23, 21, 23, 21+16), GetUri("key/main.bicep")),
                }
            },
        });
    }
}
