// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using FluentAssertions.Primitives;

namespace Bicep.Core.UnitTests.Assertions;

public class IOUriAssertions : ReferenceTypeAssertions<IOUri, IOUriAssertions>
{
    public IOUriAssertions(IOUri instance) : base(instance) { }

    protected override string Identifier => "uri";
}
