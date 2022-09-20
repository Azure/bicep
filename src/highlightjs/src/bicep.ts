// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Mode, Language, HLJSApi } from 'highlight.js';

const bounded = (text: string) => `\\b${text}\\b`;
const after = (regex: string) => `(?<=${regex})`;
const notAfter = (regex: string) => `(?<!${regex})`;
const before = (regex: string) => `(?=${regex})`;
const notBefore = (regex: string) => `(?!${regex})`;

const identifierStart = "[_a-zA-Z]";
const identifierContinue = "[_a-zA-Z0-9]";
const identifier = bounded(`${identifierStart}${identifierContinue}*`);
const directive = bounded(`[_a-zA-Z-0-9]+`);

// whitespace. ideally we'd tokenize in-line block comments, but that's a lot of work. For now, ignore them.
const ws = `(?:[ \\t\\r\\n]|\\/\\*(?:\\*(?!\\/)|[^*])*\\*\\/)*`;

const KEYWORDS = {
  $pattern: '[A-Za-z$_][0-9A-Za-z$_]*',
  keyword: [
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
  ],
  literal: [
    "true",
    "false",
    "null",
  ],
  built_in: [
    'az',
    'sys',
  ]
};

const lineComment: Mode = {
  className: 'comment',
  match: `//.*${before(`$`)}`,
};

const blockComment: Mode = {
  className: 'comment',
  begin: `/\\*`,
  end: `\\*/`,
};

const comments: Mode = {
  variants: [lineComment, blockComment],
};

function withComments(input: Mode[]): Mode[] {
  return [...input, comments]
}

const expression: Mode = {
  variants: [
    /* placeholder filled later due to cycle*/
  ],
};

const escapeChar: Mode = {
  match: `\\\\(u{[0-9A-Fa-f]+}|n|r|t|\\\\|'|\\\${)`,
};

const stringVerbatim: Mode = {
  className: 'string',
  begin: `'''`,
  end: `'''`,
}

const stringSubstitution: Mode = {
  className: 'subst',
  begin: `${notAfter(`\\\\`)}(\\\${)`,
  end: `(})`,
  contains: withComments([expression]),
};

const stringLiteral: Mode = {
  className: 'string',
  begin: `'${notBefore(`''`)}`,
  end: `'`,
  contains: [
    escapeChar,
    stringSubstitution
  ],
};

const numericLiteral: Mode = {
  className: "number",
  match: `[0-9]+`,
};

const namedLiteral: Mode = {
  className: 'literal',
  match: bounded(`(true|false|null)`),
  relevance: 0,
};

const identifierExpression: Mode = {
  className: "variable",
  match: `${identifier}${notBefore(`${ws}\\(`)}`,
  keywords: KEYWORDS,
};

const objectLiteral: Mode = {
  begin: `{`,
  end: `}`,
  contains: withComments([
    {
      className: "property",
      match: `${identifier}${before(`${ws}:`)}`,
      relevance: 0,
    },
    expression
  ]),
}

const arrayLiteral: Mode = {
  begin: `\\[${notBefore(`${ws}${bounded(`for`)}`)}`,
  end: `]`,
  contains: withComments([expression]),
};

const functionCall: Mode = {
  className: 'function',
  begin: `(${identifier})${ws}\\(`,
  end: `\\)`,
  contains: withComments([expression]),
};

const decorator: Mode = {
  className: 'meta',
  begin: `@${ws}${before(identifier)}`,
  end: ``,
  contains: withComments([functionCall]),
};

const lambdaStart = `(` +
  `\\(${ws}${identifier}${ws}(,${ws}${identifier}${ws})*\\)|` +
  `\\(${ws}\\)|` +
  `${ws}${identifier}${ws}` +
`)${before(`${ws}=>`)}`;

const lambda: Mode = {
  begin: lambdaStart,
  returnBegin: true,
  end: `${ws}=>`,
  contains: withComments([
    identifierExpression,
  ]),
};

const directiveStatement: Mode = {
  begin: `${after(`^${ws}`)}#${directive}`,
  end: `$`,
  className: 'meta',
  contains: withComments([
    {
      className: 'variable',
      match: directive,
    },
  ]),
};

expression.variants = [
  stringLiteral,
  stringVerbatim,
  numericLiteral,
  namedLiteral,
  objectLiteral,
  arrayLiteral,
  identifierExpression,
  functionCall,
  decorator,
  lambda,
  directiveStatement,
];

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export default function(hljs?: HLJSApi): Language {
  return {
    aliases: ['bicep'],
    case_insensitive: true,
    keywords: KEYWORDS,
    contains: withComments([expression]),
  }
}
