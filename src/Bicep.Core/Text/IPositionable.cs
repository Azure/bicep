// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Text
{
    public interface IPositionable
    {
        TextSpan Span { get; }
    }
}
