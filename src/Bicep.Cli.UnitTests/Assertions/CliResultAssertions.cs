// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.UnitTests.Assertions;

public class CliResultAssertions : ReferenceTypeAssertions<CliResult, CliResultAssertions>
{
    public CliResultAssertions(CliResult instance)
        : base(instance)
    {
    }

    protected override string Identifier => "result";
}
