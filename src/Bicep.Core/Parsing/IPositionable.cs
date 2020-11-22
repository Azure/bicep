// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Parsing
{
    public interface IPositionable
    {
        TextSpan Span { get; }
    }
}
