// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

global using Bicep.Testing;
global using Bicep.Testing.Assertions.Json;
global using Bicep.Testing.IO;
global using EmbeddedFile = Azure.Deployments.Testing.Utilities.EmbeddedFile;
global using TestEmbeddedFilesAttribute = Azure.Deployments.Testing.Utilities.EmbeddedFilesTestDataAttribute;
global using BaselineFile = Azure.Deployments.Testing.Utilities.BaselineFile;
global using static Azure.Deployments.Testing.Utilities.BaselineAssertionsExtensions;
global using static Azure.Deployments.Testing.Utilities.JTokenAssertionsExtensions;
global using static Azure.Deployments.Testing.Utilities.StringAssertionsExtensions;
global using static Azure.Deployments.Testing.Utilities.TestContextExtensions;
