// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Parser
{
    public interface IPositionable
    {
        TextSpan Span { get; }
    }
}
