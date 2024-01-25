// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
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
    public void ProviderConfigurationDeserializationSucceeds()
    {
        var data = JsonElementFactory.CreateElement("""
        {
            "providers": {
                "az": {
                    "registry": "mcr.microsoft.com/bicep/providers",
                    "version": "0.2.3"
                },
                "kubernetes": {
                    "builtin": true
                }
            }
        }
        """);

        var res = JsonSerializer.Deserialize<ProvidersConfigurationSection>(data, DefaultDeserializeOptions);
        res.Should().NotBeNull();
        res!.Providers.Should().NotBeNull();
        res.Providers.Count.Should().Be(2);

        res.Providers["az"].Should().NotBeNull();
        // verifies that 'registry' and 'version' are valid properties for 'source'
        res.Providers["az"].Registry.Should().Be("mcr.microsoft.com/bicep/providers");
        res.Providers["az"].Version.Should().Be("0.2.3");

        res.Providers["kubernetes"].Should().NotBeNull();
        // verifies that 'builtin' is a valid property for 'source'
        res.Providers["kubernetes"].Builtin.Should().BeTrue();
    }
}