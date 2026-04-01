// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Microsoft.Extensions.Logging;

namespace Bicep.Core.Registry.Sessions;

public sealed class RegistryProviderContext
{
    public required ILogger Logger { get; init; }

    public required ICredentialChain CredentialChain { get; init; }

    public required CloudConfiguration Cloud { get; init; }

    public CancellationToken CancellationToken { get; init; }
}
