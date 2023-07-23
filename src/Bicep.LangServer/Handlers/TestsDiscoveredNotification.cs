// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers;

public record TestEntry(
    string Id,
    Range Range);

[Method("bicep/testsDiscovered", Direction.ServerToClient)]
public record TestsDiscoveredNotification(
    TextDocumentIdentifier TextDocument,
    Container<TestEntry> Tests);
