// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class ProvidersConfigurationTests
{
    private static readonly JsonSerializerOptions DefaultDeserializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() },
    };

    [TestMethod]
    public void MyExploratoryTest()
    {
        var data = JsonElementFactory.CreateElement("""
        {
            "providers": {
                "az": {
                    "source": {
                        "registry": "mcr.microsoft.com/bicep/providers",
                        "version": "0.2.3"
                    },
                    "isDefaultImport": true
                },
                "kubernetes": {
                    "source": {
                        "builtin": true
                    }
                }
            }
        }
        """);

        var res = JsonSerializer.Deserialize<ProvidersConfigurationSection>(data, DefaultDeserializeOptions);
        res.Should().NotBeNull();
        res!.Providers.Should().NotBeNull();
        res.Providers.Count.Should().Be(2);
        res.Providers["az"].Source.Should().NotBeNull();
        // verifies that 'registry' and 'version' are valid properties for 'source'
        res.Providers["az"].Source!.Registry.Should().Be("mcr.microsoft.com/bicep/providers");
        res.Providers["az"].Source!.Version.Should().Be("0.2.3");
        // verifies the default value for 'IsDefaultImport' is true when specified
        res.Providers["az"].IsDefaultImport.Should().BeTrue();
        res.Providers["kubernetes"].Source.Should().NotBeNull();
        // verifies that 'builtin' is a valid property for 'source'
        res.Providers["kubernetes"].Source!.Builtin.Should().BeTrue();
        // verifies the default value for 'IsDefaultImport' is false when unspecified
        res.Providers["kubernetes"].IsDefaultImport.Should().BeFalse();
    }
}