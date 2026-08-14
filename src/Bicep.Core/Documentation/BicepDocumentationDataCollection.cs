// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Documentation;

/// <summary>
/// Describes a module's telemetry behavior.
/// </summary>
public record BicepDocumentationDataCollection(bool Enabled, string Note);
