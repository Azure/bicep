// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Deployments.Engine.External;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Local.Deploy.Engine;

public class LocalHttpContentHandler : IHttpContentHandler
{
    public HttpResponseMessage GenerateResponseMessageFromException(HttpRequestMessage request, Exception exception)
    {
        throw new NotImplementedException();
    }

    public async Task<T> ReadAsJson<T>(HttpContent content, bool rewindContentStream = false)
    {
        using var contentStream = await content.ReadAsStreamAsync();

        return contentStream.FromJsonStream<T>();
    }

    public async Task<T?> TryReadAsJson<T>(HttpContent content, bool rewindContentStream = false)
    {
        try
        {
            return await ReadAsJson<T>(content, rewindContentStream);
        }
        catch
        {
            return default;
        }
    }

    public async Task<string?> TryReadAsString(HttpContent content, bool rewindContentStream = false)
    {
        try
        {
            return await content.ReadAsStringAsync();
        }
        catch
        {
            return default;
        }
    }
}
