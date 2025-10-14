// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;

namespace Bicep.Core.Highlighting;

public record SemanticToken(
    IPositionable Positionable,
    SemanticTokenType TokenType);

public enum SemanticTokenType
{
    Comment,
    EnumMember,
    Event,
    Modifier,
    Label,
    Parameter,
    Variable,
    Property,
    Function,
    TypeParameter,
    Macro,
    Interface,
    Enum,
    String,
    Number,
    Regexp,
    Operator,
    Keyword,
    Type,
    Struct,
    Class,
    Namespace,
    Decorator,
}