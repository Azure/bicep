// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;

namespace Bicep.Core.Parsing
{
    public interface IPositionable
    {
        TextSpan Span { get; }
    }
}
