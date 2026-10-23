// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using Bicep.Core.Registry.Oci.Oras;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry.Oci.Oras;

[TestClass]
public class OrasHttpClientTests
{
    [TestMethod]
    [DataRow(OrasOciRegistryTransport.RegistryHttpClient, true)]
    [DataRow(OrasOciRegistryTransport.AuthenticationHttpClient, false)]
    public void ConfiguresRedirectsForRegistryAndAuthenticationClients(string clientName, bool allowAutoRedirect)
    {
        using var services = new ServiceCollection().AddBicepCore().BuildServiceProvider();
        var factory = services.GetRequiredService<IHttpMessageHandlerFactory>();
        var handler = factory.CreateHandler(clientName);
        while (handler is DelegatingHandler { InnerHandler: { } innerHandler })
        {
            handler = innerHandler;
        }

        var primaryHandler = handler.Should().BeOfType<HttpClientHandler>().Subject;
        primaryHandler.AllowAutoRedirect.Should().Be(allowAutoRedirect);
        if (allowAutoRedirect)
        {
            primaryHandler.MaxAutomaticRedirections.Should().Be(10);
        }
    }
}
