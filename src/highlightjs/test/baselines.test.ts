// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { readdirSync, existsSync } from 'fs';
import { readFile, writeFile } from 'fs/promises';
import path, { dirname, basename, extname } from 'path';
import { env } from 'process';
import { spawnSync } from 'child_process';
import * as highlight from 'highlight.js';
import bicep, { default as bicepLanguage } from '../src/bicep';

async function writeBaseline(filePath: string) {
  const baselineBaseName = basename(filePath, extname(filePath));
  const baselineFilePath = path.join(dirname(filePath), `${baselineBaseName}.html`);

  let diffBefore = '';
  const bicepFile = await readFile(filePath, { encoding: 'utf-8' });
  try {
    diffBefore = await readFile(baselineFilePath, { encoding: 'utf-8' });
  } catch {} // ignore and create the baseline file anyway

  highlight.registerLanguage('bicep', bicepLanguage);
  const result = highlight.highlight(bicepFile, { language: 'bicep' });
  const diffAfter = `
<html>
  <head>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/10.7.2/styles/default.min.css">
  </head>
  <body>
    <pre class="hljs">
${result.value}
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
        expect(result.stderr).toBe('');
        expect(result.status).toBe(0);
      });
    }

    it('baseline matches expected', () => {
      expect(result.diffBefore).toEqual(result.diffAfter);
    });
  });
}