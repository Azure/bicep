// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { readdirSync, existsSync } from 'fs';
import { readFile, writeFile } from 'fs/promises';
import { IOnigLib, IToken, parseRawGrammar, Registry, StackElement } from 'vscode-textmate';
import { createOnigScanner, createOnigString, loadWASM } from 'vscode-oniguruma';
import path, { dirname, basename, extname } from 'path';
import { grammarPath, BicepScope } from '../src/bicep';
import { spawnSync } from 'child_process';
import { escape } from 'html-escaper';

async function createOnigLib(): Promise<IOnigLib> {
  const onigWasm = await readFile(`${path.dirname(require.resolve('vscode-oniguruma'))}/onig.wasm`);

  await loadWASM(onigWasm.buffer);

  return {
    createOnigScanner: sources => createOnigScanner(sources),
    createOnigString,
  };
}

const registry = new Registry({
  onigLib: createOnigLib(),
  loadGrammar: async scopeName => {
    const grammar = await readFile(grammarPath, { encoding: 'utf-8' });

    return parseRawGrammar(grammar);
  }
});

const tokenToHljsClass: Record<BicepScope, string | null> = {
  'comment.block.bicep': 'comment',
  'comment.line.double-slash.bicep': 'comment',
  'constant.character.escape.bicep': null,
  'constant.numeric.bicep': 'number',
  'constant.language.bicep': 'literal',
  'entity.name.function.bicep': 'function',
  'keyword.control.declaration.bicep': 'keyword',
  'string.quoted.single.bicep': 'string',
  'string.quoted.multi.bicep': 'string',
  'variable.other.readwrite.bicep': 'variable',
  'variable.other.property.bicep': 'property',
  'punctuation.definition.template-expression.begin.bicep': 'subst',
  'punctuation.definition.template-expression.end.bicep': 'subst',
};

function getTokenPriority(scope: BicepScope) {
  switch (scope) {
    // a bit of a hack to make changes easier to review; if there are multiple tokens, layer them on top of the string.
    // this basically emulates what VSCode does.
    case 'string.quoted.single.bicep':
      return 0;
    default:
      return 1;
  }
}

async function getTokensByLine(content: string) {
  const grammar = await registry.loadGrammar('source.bicep');
  if (!grammar) {
    throw `Grammar initialization failed!`;
  }

  const lines = content.split(/\r\n|\r|\n/);
  const tokensByLine: IToken[][] = [];

  let ruleStack: StackElement | null = null;
  for (const line of lines) {
    const result = grammar.tokenizeLine(line, ruleStack);

    ruleStack = result.ruleStack
    tokensByLine.push(result.tokens);
  }

  return lines.map((line, i) => ({
    line,
    tokens: tokensByLine[i],
  }));
}

function hasOverlap(first: IToken, second: IToken) {
  if (first.endIndex < second.startIndex) {
    return false;
  }

  if (first.scopes.length !== second.scopes.length) {
    return false;
  }

  for (let i = 0; i < first.scopes.length; i++) {
    if (first.scopes[i] !== second.scopes[i]) {
      return false;
    }
  }

  return true;
}

async function writeBaseline(filePath: string) {
  const baselineBaseName = basename(filePath, extname(filePath));
  const baselineFilePath = path.join(dirname(filePath), `${baselineBaseName}.html`);

  let diffBefore = '';
  const bicepFile = await readFile(filePath, { encoding: 'utf-8' });
  try {
    diffBefore = await readFile(baselineFilePath, { encoding: 'utf-8' });
  } catch {} // ignore and create the baseline file anyway

  let html = '';
  const tokensByLine = await getTokensByLine(bicepFile);

  for (const { line, tokens } of tokensByLine) {
    let currentIndex = 0;
    for (const token of tokens) {
      const visibleScopes = token.scopes
        .filter(x => !!(tokenToHljsClass as any)[x]) as BicepScope[];

      visibleScopes.sort((a, b) => getTokenPriority(b) - getTokenPriority(a));

      if (token.startIndex > currentIndex) {
        html += escape(line.substring(currentIndex, token.startIndex));
      }

      if (visibleScopes.length > 0) {
        html += `<span class="hljs-${tokenToHljsClass[visibleScopes[0]]}">`;
        html += escape(line.substring(token.startIndex, token.endIndex));
        html += `</span>`;
      } else {
        html += escape(line.substring(token.startIndex, token.endIndex));
      }

      currentIndex = token.endIndex;
    }

    if (line.length > currentIndex) {
      html += escape(line.substring(currentIndex, line.length));
    }

    html += '\n';
  }

  const diffAfter = `
<html>
  <head>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/10.7.2/styles/default.min.css">
  </head>
  <body>
    <pre class="hljs">
${html}
    </pre>
  </body>
</html>`;
  await writeFile(baselineFilePath, diffAfter, { encoding: 'utf-8' });

  return {
    diffBefore,
    diffAfter,
    baselineFilePath,
  };
}

const baselinesDir = `${__dirname}/baselines`;

const baselineFiles = readdirSync(baselinesDir)
  .filter(p => extname(p) === '.bicep')
  .map(p => path.join(baselinesDir, p));

for (const filePath of baselineFiles) {
  describe(filePath, () => {
    let result = {
      baselineFilePath: '',
      diffBefore: '',
      diffAfter: ''
    };

    beforeAll(async () => {
      result = await writeBaseline(filePath);
    });

    if (!basename(filePath).startsWith('bad_')) {
      // skip the invalid files - we don't expect them to compile

      it('can be compiled', async () => {
        const cliCsproj = `${__dirname}/../../Bicep.Cli/Bicep.Cli.csproj`;

        if (!existsSync(cliCsproj)) {
          fail(`Unable to find '${cliCsproj}'`);
          return;
        }

        const result = spawnSync(`dotnet`, ['run', '-p', cliCsproj, 'build', '--stdout', filePath], { encoding: 'utf-8' });

        expect(result.error).toBeUndefined();
        expect(result.stderr).not.toContain(') : Error ')
        expect(result.status).toBe(0);
      });
    }

    it('baseline matches expected', () => {
      expect(result.diffBefore).toEqual(result.diffAfter);
    });
  });
}
