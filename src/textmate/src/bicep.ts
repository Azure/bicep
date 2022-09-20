// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as tm from "tmlanguage-generator";
import path from "path";
import plist from "plist";

export const grammarPath = path.resolve(__dirname, '../bicep.tmlanguage');

type Rule = tm.Rule<BicepScope>;
type IncludeRule = tm.IncludeRule<BicepScope>;
type BeginEndRule = tm.BeginEndRule<BicepScope>;
type MatchRule = tm.MatchRule<BicepScope>;
type Grammar = tm.Grammar<BicepScope>;

export type BicepScope =
  | "comment.block.bicep"
  | "comment.line.double-slash.bicep"
  | "constant.character.escape.bicep"
  | "constant.numeric.bicep"
  | "constant.language.bicep"
  | "entity.name.function.bicep"
  | "keyword.control.declaration.bicep"
  | "string.quoted.single.bicep"
  | "string.quoted.multi.bicep"
  | "variable.other.readwrite.bicep"
  | "variable.other.property.bicep"
  | "punctuation.definition.template-expression.begin.bicep"
  | "punctuation.definition.template-expression.end.bicep";

const bounded = (text: string) => `\\b${text}\\b`;
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const after = (regex: string) => `(?<=${regex})`;
const notAfter = (regex: string) => `(?<!${regex})`;
const before = (regex: string) => `(?=${regex})`;
const notBefore = (regex: string) => `(?!${regex})`;

const meta: typeof tm.meta = tm.meta;
const identifierStart = "[_$[:alpha:]]";
const identifierContinue = "[_$[:alnum:]]";
const identifier = bounded(`${identifierStart}${identifierContinue}*`);
const directive = bounded(`[_a-zA-Z-0-9]+`);

// whitespace. ideally we'd tokenize in-line block comments, but that's a lot of work. For now, ignore them.
const ws = `(?:[ \\t\\r\\n]|\\/\\*(?:\\*(?!\\/)|[^*])*\\*\\/)*`;

const keywords = [
  'metadata',
  'targetScope',
  'resource',
  'module',
  'param',
  'var',
  'output',
  'for',
  'in',
  'if',
  'existing',
  'import',
  'as',
];

const keywordExpression: MatchRule = {
  key: 'keyword',
  scope: 'keyword.control.declaration.bicep',
  match: bounded(`(${keywords.join('|')})`),
};

const lineComment: MatchRule = {
  key: "line-comment",
  scope: "comment.line.double-slash.bicep",
  match: `//.*${before(`$`)}`,
};

const blockComment: BeginEndRule = {
  key: "block-comment",
  scope: "comment.block.bicep",
  begin: `/\\*`,
  end: `\\*/`,
};

const comments: IncludeRule = {
  key: 'comments',
  patterns: [lineComment, blockComment],
};

function withComments(input: Rule[]): Rule[] {
  return [...input, comments];
}

const expression: IncludeRule = {
  key: "expression",
  patterns: [
    /* placeholder filled later due to cycle*/
  ],
};

const escapeChar: MatchRule = {
  key: "escape-character",
  scope: "constant.character.escape.bicep",
  match: `\\\\(u{[0-9A-Fa-f]+}|n|r|t|\\\\|'|\\\${)`,
};

const stringVerbatim: BeginEndRule = {
  key: "string-verbatim",
  scope: "string.quoted.multi.bicep",
  begin: `'''`,
  end: `'''`,
  patterns: [],
}

const stringSubstitution: BeginEndRule = {
  key: "string-literal-subst",
  scope: meta,
  begin: `${notAfter(`\\\\`)}(\\\${)`,
  beginCaptures: {
    "1": { scope: "punctuation.definition.template-expression.begin.bicep" },
  },
  end: `(})`,
  endCaptures: {
    "1": { scope: "punctuation.definition.template-expression.end.bicep" },
  },
  patterns: withComments([expression]),
};

const stringLiteral: BeginEndRule = {
  key: "string-literal",
  scope: "string.quoted.single.bicep",
  begin: `'${notBefore(`''`)}`,
  end: `'`,
  patterns: [
    escapeChar,
    stringSubstitution
  ],
};

const numericLiteral: MatchRule = {
  key: "numeric-literal",
  scope: "constant.numeric.bicep",
  match: `[0-9]+`,
};

const namedLiteral: MatchRule = {
  key: "named-literal",
  scope: "constant.language.bicep",
  match: bounded(`(true|false|null)`),
};

const identifierExpression: MatchRule = {
  key: "identifier",
  scope: "variable.other.readwrite.bicep",
  match: `${identifier}${notBefore(`${ws}\\(`)}`,
};

const objectLiteral: BeginEndRule = {
  key: "object-literal",
  scope: meta,
  begin: `{`,
  end: `}`,
  patterns: withComments([
    {
      key: "object-property-key",
      scope: "variable.other.property.bicep",
      match: `${identifier}${before(`${ws}:`)}`,
    },
    expression
  ]),
}

const arrayLiteral: BeginEndRule = {
  key: "array-literal",
  scope: meta,
  begin: `\\[${notBefore(`${ws}${bounded(`for`)}`)}`,
  end: `]`,
  patterns: withComments([expression]),
};

const functionCall: BeginEndRule = {
  key: "function-call",
  scope: meta,
  begin: `(${identifier})${ws}\\(`,
  beginCaptures: {
    "1": { scope: "entity.name.function.bicep" },
  },
  end: `\\)`,
  patterns: withComments([expression]),
};

const decorator: BeginEndRule = {
  key: "decorator",
  scope: meta,
  begin: `@${ws}${before(identifier)}`,
  end: ``,
  patterns: withComments([expression]),
};

const lambdaStart = `(` +
  `\\(${ws}${identifier}${ws}(,${ws}${identifier}${ws})*\\)|` +
  `\\(${ws}\\)|` +
  `${ws}${identifier}${ws}` +
`)${before(`${ws}=>`)}`;

const lambda: BeginEndRule = {
  key: "lambda-start",
  scope: meta,
  begin: lambdaStart,
  beginCaptures: {
    "1": {
      scope: meta,
      patterns: withComments([
        identifierExpression,
      ])
    }
  },
  end: `${ws}=>`,
};

const directiveStatement: BeginEndRule = {
  key: "directive",
  scope: meta,
  begin: `#${directive}`,
  end: `$`,
  patterns: withComments([
    {
      key: 'directive-variable',
      scope: 'keyword.control.declaration.bicep',
      match: directive,
    },
  ]),
};

expression.patterns = [
  stringLiteral,
  stringVerbatim,
  numericLiteral,
  namedLiteral,
  objectLiteral,
  arrayLiteral,
  keywordExpression,
  identifierExpression,
  functionCall,
  decorator,
  lambda,
  directiveStatement,
];

const grammar: Grammar = {
  $schema: tm.schema,
  name: "Bicep",
  scopeName: "source.bicep",
  fileTypes: [".bicep"],
  patterns: withComments([expression]),
};

export async function generateGrammar(): Promise<string> {
  const json = await tm.emitJSON(grammar);

  return plist.build(JSON.parse(json));
}
