// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions.Primitives;

namespace Bicep.Cli.UnitTests.Assertions;

public class CliResultAssertions(CliResult instance) : ReferenceTypeAssertions<CliResult, CliResultAssertions>(instance)
{
    protected override string Identifier => "result";
}
