// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    public interface IClientCapabilitiesProvider
    {
        bool DoesClientSupportWorkspaceFolders();
        bool DoesClientSupportShowDocumentRequest();
    }
}
