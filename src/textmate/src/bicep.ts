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

// whitespace. ideally we'd tokenize in-line block comments, but that's a lot of work. For now, ignore them.
const ws = `(?:[ \\t\\r\\n]|\\/\\*(?:\\*(?!\\/)|[^*])*\\*\\/)*`;

const keywords = [
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
  'from',
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

const objectPropertyKeyIdentifier: MatchRule = {
  key: "object-property-key-identifier",
  scope: meta,
  match: `(${identifier})`,
  captures: {
    "1": { scope: "variable.other.property.bicep" },
  }
};

const objectProperty: BeginEndRule = {
  key: "object-property",
  scope: meta,
  begin: `${after(`^`)}${notBefore(`${ws}}`)}`,
  end: before(`$`),
  patterns: withComments([
    objectPropertyKeyIdentifier,
    stringLiteral,
    {
      key: "object-property-end",
      scope: meta,
      begin: `:(${ws})`, // wanted to use after(`:${ws}`)
      beginCaptures: {
        "1": comments
      },
      end: before(`${ws}$`),
      patterns: withComments([expression]),
    },
  ]),
};

const objectLiteral: BeginEndRule = {
  key: "object-literal",
  scope: meta,
  begin: `{`,
  end: `}`,
  patterns: withComments([objectProperty]),
};

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