// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as tm from "@azure-tools/tmlanguage-generator";
import path from "path";
import plist from "plist";

export const grammarPath = path.resolve(__dirname, '../bicep.tmlanguage');

type IncludeRule = tm.IncludeRule<BicepScope>;
type BeginEndRule = tm.BeginEndRule<BicepScope>;
type MatchRule = tm.MatchRule<BicepScope>;
type Grammar = tm.Grammar<BicepScope>;

type BicepScope =
  | "comment.block.bicep"
  | "comment.line.double-slash.bicep"
  | "constant.character.escape.bicep"
  | "constant.numeric.bicep"
  | "constant.language.bicep"
  | "entity.name.type.bicep"
  | "entity.name.function.bicep"
  | "keyword.control.declaration.bicep"
  | "keyword.other.bicep"
  | "string.quoted.single.bicep"
  | "string.quoted.multi.bicep"
  | "variable.name.bicep"
  | "variable.other.readwrite.bicep"
  | "variable.other.property.bicep"
  | "meta.objectliteral.bicep"
  | "meta.object.member.bicep"
  | "punctuation.definition.template-expression.begin.bicep" 
  | "punctuation.definition.template-expression.end.bicep";

const bounded = (text: string) => `\\b${text}\\b`;
const after = (regex: string) => `(?<=${regex})`;
const notAfter = (regex: string) => `(?<!${regex})`;
const before = (regex: string) => `(?=${regex})`;
const notBefore = (regex: string) => `(?!${regex})`;

const meta: typeof tm.meta = tm.meta;
const identifierStart = "[_$[:alpha:]]";
const identifierContinue = "[_$[:alnum:]]";
const identifier = bounded(`${identifierStart}${identifierContinue}*`);

// whitespace. ideally we'd tokenize in-line block comments, but that's a lot of work. For now, ignore them.
const ws = `(?:\\s|/\\*.*\\*/)*`;

const lineComment: MatchRule = {
  key: "line-comment",
  scope: "comment.line.double-slash.bicep",
  match: `//.*$`,
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

const expression: IncludeRule = {
  key: "expression",
  patterns: [
    /* placeholder filled later due to cycle*/
  ],
};

const escapeChar: MatchRule = {
  key: "escape-character",
  scope: "constant.character.escape.bicep",
  match: `\\\\(u[0-9A-Fa-f]{6}|n|r|t|\\\\|'|\\\${)`,
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
  patterns: [expression],
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
  begin: `^${notBefore(`${ws}}`)}`,
  end: `$`,
  patterns: [
    {
      key: "object-property-start",
      scope: meta,
      begin: `^${ws}`,
      end: `${ws}:`,
      patterns: [
        stringLiteral,
        objectPropertyKeyIdentifier,
      ],
    },
    {
      key: "object-property-end",
      scope: meta,
      begin: `${after(`:`)}${ws}`,
      end: `${ws}$`,
      patterns: [expression],
    },
  ],
};

const objectLiteral: BeginEndRule = {
  key: "object-literal",
  scope: meta,
  begin: `{`,
  end: `}`,
  patterns: [objectProperty],
};

const arrayProperty: BeginEndRule = {
  key: "array-property",
  scope: meta,
  begin: `^${notBefore(`${ws}]`)}`,
  end: `$`,
  patterns: [expression],
};

const arrayLiteral: BeginEndRule = {
  key: "array-literal",
  scope: meta,
  begin: `\\[${ws}${notBefore(bounded(`for`))}`,
  end: `]`,
  patterns: [arrayProperty],
};

const forExpression: BeginEndRule = {
  key: "for-expression",
  scope: meta,
  begin: `\\[${ws}(${bounded(`for`)})`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
  },
  end: `]`,
  patterns: [
    {
      key: "for-expression-in",
      scope: meta,
      begin: after(bounded(`for`)),
      end: `:`,
      patterns: [
        {
          key: "for-expression-in-start",
          scope: meta,
          begin: `${after(bounded(`for`))}${ws}`,
          end: `(${bounded(`in`)})`,
          endCaptures: {
            "1": { scope: "keyword.control.declaration.bicep" },
          },
          patterns: [expression],
        },
        {
          key: "for-expression-in-end",
          scope: meta,
          begin: `${after(bounded(`in`))}${ws}`,
          end: before(':'),
          patterns: [expression],
        },
      ],
    },
    {
      key: "for-expression-end",
      scope: meta,
      begin: `${after(`:`)}${ws}`,
      end: `${ws}${before(`]`)}`,
      patterns: [expression],
    },
  ],
};

const ifExpression: BeginEndRule = {
  key: "if-expression",
  scope: meta,
  begin: `(${bounded(`if`)})`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
  },
  end: after(`}`),
  patterns: [
    {
      key: "if-expression-start",
      scope: meta,
      begin: `${after(bounded(`if`))}${ws}\\(`,
      end: `\\)`,
      endCaptures: {
        "1": { scope: "keyword.control.declaration.bicep" },
      },
      patterns: [expression],
    },
    {
      key: "if-expression-end",
      scope: meta,
      begin: `${after(`\\)`)}${ws}${before(`{`)}`,
      end: after(`}`),
      patterns: [objectLiteral],
    },
  ],
};

const functionCall: BeginEndRule = {
  key: "function-call",
  scope: meta,
  begin: `(${identifier})${ws}\\(`,
  beginCaptures: {
    "1": { scope: "entity.name.function.bicep" },
  },
  end: `\\)`,
  patterns: [expression],
};

const targetScopeStatement: BeginEndRule = {
  key: "targetscope-statement",
  scope: meta,
  begin: `(${bounded(`targetScope`)})${ws}=${ws}`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
  },
  end: `$`,
  patterns: [expression],
};

const paramStatement: BeginEndRule = {
  key: "param-statement",
  scope: meta,
  begin: `(${bounded(`param`)})${ws}(${identifier})${ws}(${identifier})`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
    "2": { scope: "variable.name.bicep" },
    "3": { scope: "variable.name.bicep" },
  },
  end: `$`,
  patterns: [expression],
};

const resourceStatement: BeginEndRule = {
  key: "resource-statement",
  scope: meta,
  begin: `(${bounded(`resource`)})${ws}(${identifier})${ws}${before(`'`)}`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
    "2": { scope: "variable.name.bicep" },
  },
  end: `$`,
  patterns: [
    stringLiteral,
    {
      key: "resource-statement-end",
      scope: meta,
      begin: `${after(`'`)}${ws}=${ws}`,
      end: `$`,
      patterns: [expression],
    }
  ],
};

const moduleStatement: BeginEndRule = {
  key: "module-statement",
  scope: meta,
  begin: `(${bounded(`module`)})${ws}(${identifier})${ws}${before(`'`)}`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
    "2": { scope: "variable.name.bicep" },
  },
  end: `$`,
  patterns: [
    stringLiteral,
    {
      key: "module-statement-end",
      scope: meta,
      begin: `${after(`'`)}${ws}=${ws}`,
      end: `$`,
      patterns: [expression],
    },
  ],
};

const varStatement: BeginEndRule = {
  key: "var-statement",
  scope: meta,
  begin: `(${bounded(`var`)})${ws}(${identifier})${ws}=${ws}`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
    "2": { scope: "variable.name.bicep" },
  },
  end: `$`,
  patterns: [expression],
};

const outputStatement: BeginEndRule = {
  key: "output-statement",
  scope: meta,
  begin: `(${bounded(`output`)})${ws}(${identifier})${ws}(${identifier})${ws}=${ws}`,
  beginCaptures: {
    "1": { scope: "keyword.control.declaration.bicep" },
    "2": { scope: "variable.name.bicep" },
    "3": { scope: "variable.name.bicep" },
  },
  end: `$`,
  patterns: [expression],
};

const decorator: BeginEndRule = {
  key: "decorator",
  scope: meta,
  begin: `@`,
  end: `$`,
  patterns: [expression],
};

const statement: IncludeRule = {
  key: "statement",
  patterns: [
    comments,
    decorator,
    targetScopeStatement,
    paramStatement,
    resourceStatement,
    moduleStatement,
    varStatement,
    outputStatement,
  ],
};

expression.patterns = [
  identifierExpression,
  forExpression,
  ifExpression,
  stringLiteral,
  stringVerbatim,
  numericLiteral,
  namedLiteral,
  objectLiteral,
  arrayLiteral,
  functionCall,
];

const grammar: Grammar = {
  $schema: tm.schema,
  name: "Bicep",
  scopeName: "source.bicep",
  fileTypes: [".bicep"],
  patterns: [statement],
};

export async function generateGrammar() {
  const json = await tm.emitJSON(grammar);
  
  return plist.build(JSON.parse(json));
}