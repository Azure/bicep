// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Aliased rather than importing Azure.Deployments.Testing.Utilities, which would make JToken.Should() and
// TestContext extension methods ambiguous with the Bicep.Testing equivalents.
global using EmbeddedFile = Azure.Deployments.Testing.Utilities.EmbeddedFile;
