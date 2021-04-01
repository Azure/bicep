// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { readdirSync, existsSync } from 'fs';
import { readFile, writeFile } from 'fs/promises';
import { IOnigLib, IToken, parseRawGrammar, Registry, StackElement } from 'vscode-textmate';
import { createOnigScanner, createOnigString, loadWASM } from 'vscode-oniguruma';
import path, { dirname, basename, extname } from 'path';
import plist from 'plist';
import { grammarPath } from '../src/grammar';
import { env } from 'process';
import { spawnSync } from 'child_process';

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
  const baselineFilePath = path.join(dirname(filePath), `${baselineBaseName}.tokens`);

  let diffBefore = '';
  const bicepFile = await readFile(filePath, { encoding: 'utf-8' });
  try {
    diffBefore = await readFile(baselineFilePath, { encoding: 'utf-8' });
  } catch {} // ignore and create the baseline file anyway

  let baseline = '';
  const tokensByLine = await getTokensByLine(bicepFile);
  const maxLineLength = Math.max(...tokensByLine.map(x => x.line.length));

  for (const { line, tokens } of tokensByLine) {
    baseline += `  ${line}\n`;

    const filteredTokens = tokens.map(token => ({
      ...token,
      // only interested in non-metadata scopes
      scopes: token.scopes.filter(x => !x.startsWith('meta.') && x !== 'source.bicep'),
    }))
      .filter(x => x.scopes.length > 0)
      .reduce<IToken[]>((acc, curr) => {
        const prevIndex = acc.length - 1;
        if (acc.length > 1 && hasOverlap(acc[prevIndex], curr)) {
          acc[prevIndex] = {
            ...acc[prevIndex],
            endIndex: curr.endIndex,
          };
        } else {
          acc.push(curr);
        }

        return acc;
      }, []);

    for (const token of filteredTokens) {
      baseline += '//';
      baseline += ' '.repeat(token.startIndex);
      baseline += '~'.repeat(token.endIndex - token.startIndex);
      baseline += ' '.repeat(maxLineLength + 2 - token.endIndex);
      baseline += token.scopes.join(', ');
      baseline += '\n';
    }
  }

  const diffAfter = baseline;
  await writeFile(baselineFilePath, baseline, { encoding: 'utf-8' });

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
        const bicepExePathVariable = 'BICEP_CLI_EXECUTABLE';
        const bicepExePath = env[bicepExePathVariable];
        if (!bicepExePath) {
          fail(`Unable to find '${bicepExePathVariable}' env variable`);
          return;
        }
  
        if (!existsSync(bicepExePath)) {
          fail(`Unable to find '${bicepExePath}' specified in '${bicepExePathVariable}' env variable`);
          return;
        }
  
        const result = spawnSync(bicepExePath, ['build', '--stdout', filePath], { encoding: 'utf-8' });
  
        // NOTE - if stderr or status are null, this indicates we were unable to invoke the exe (missing file, or hasn't had 'chmod +x' run)
        expect(result.stderr).toBe('');
        expect(result.status).toBe(0);
      });
    }

    it('baseline matches expected', () => {
      expect(result.diffBefore).toEqual(result.diffAfter);
    });
  });
}