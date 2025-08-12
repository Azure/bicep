// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions.Primitives;
using Google.Protobuf;

namespace Bicep.Local.Extension.UnitTests.Assertions;

public class IMessageAssertions : ReferenceTypeAssertions<IMessage, IMessageAssertions>
{
    public IMessageAssertions(IMessage instance)
        : base(instance)
    {
    }

    protected override string Identifier => "message";
}
