// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;

namespace Bicep.Testing;

public record TestResultFile(string FileName, string Contents, Encoding? Encoding = null);
