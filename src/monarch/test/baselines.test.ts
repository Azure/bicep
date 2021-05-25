// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

declare const window: Record<string, unknown>;

// See https://jestjs.io/docs/manual-mocks#mocking-methods-which-are-not-implemented-in-jsdom.
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: jest.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: jest.fn(), // deprecated
    removeListener: jest.fn(), // deprecated
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
    dispatchEvent: jest.fn(),
  })),
});

import { readdirSync, existsSync } from 'fs';
import { readFile, writeFile } from 'fs/promises';
import path, { dirname, basename, extname } from 'path';
import { spawnSync } from 'child_process';
import { BicepLanguage } from '../src/bicep';
import { editor, languages } from 'monaco-editor-core';
import { escape } from 'html-escaper';

const tokenToHljsClass: Record<string, string | null> = {
  'string.bicep': 'string',
  'string.quote.bicep': 'string',
  'string.escape.bicep': 'string',
  'comment.bicep': 'comment',
  'number.bicep': 'number',
  'keyword.bicep': 'keyword',
  'identifier.bicep': 'variable',
};

async function getTokensByLine(content: string) {
  languages.register({ id: 'bicep' });
  languages.setMonarchTokensProvider('bicep', BicepLanguage);

  const tokensByLine = editor.tokenize(content, 'bicep');
  const lines = content.split(/\r\n|\r|\n/);

  return lines.map((line, i) => ({
    line,
    tokens: tokensByLine[i],
  }));
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
    for (let i = 0; i < tokens.length; i++) {
      const token = tokens[i];

      if (token.offset > currentIndex) {
        html += escape(line.substring(currentIndex, token.offset));
      }

      const tokenEnd = i + 1 < tokens.length ? tokens[i + 1].offset : line.length;
      const hljsClass = tokenToHljsClass[token.type];

      if (hljsClass) {
        html += `<span class="hljs-${hljsClass}">`;
        html += escape(line.substring(token.offset, tokenEnd));
        html += `</span>`;
      } else {
        html += escape(line.substring(token.offset, tokenEnd));
      }

      currentIndex = tokenEnd;
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

  const output = {
    diffBefore: diffBefore.replace(/\r\n/g, '\n'),
    diffAfter: diffAfter.replace(/\r\n/g, '\n'),
    baselineFilePath,
  };

  await writeFile(baselineFilePath, output.diffAfter, { encoding: 'utf-8' });

  return output;
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

        // NOTE - if stderr or status are null, this indicates we were unable to invoke the exe (missing file, or hasn't had 'chmod +x' run)
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
