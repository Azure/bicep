// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { readdirSync, existsSync } from 'fs';
import { readFile } from 'fs/promises';
import path, { dirname, basename, extname } from 'path';
import { env } from 'process';
import { spawnSync } from 'child_process';
import hljs from 'highlight.js';
import { default as bicepLanguage } from '../src/bicep';
import { expectFileContents, baselineRecordEnabled } from './utils';

async function generateBaseline(filePath: string) {
  const baselineBaseName = basename(filePath, extname(filePath));
  const baselineFilePath = path.join(dirname(filePath), `${baselineBaseName}.html`);

  const bicepFile = await readFile(filePath, { encoding: 'utf-8' });

  hljs.registerLanguage('bicep', bicepLanguage);
  const result = hljs.highlight(bicepFile, { language: 'bicep' });
  const expected = `
<!--
  Preview this file by prepending http://htmlpreview.github.io/? to its URL
  e.g. http://htmlpreview.github.io/?https://raw.githubusercontent.com/Azure/bicep/main/src/highlightjs/test/baselines/${baselineBaseName}.html
-->
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

  return {
    expected: expected.replace(/\r\n/g, '\n'),
    baselineFilePath,
  };
}

const baselinesDir = `${__dirname}/baselines`;

const baselineFiles = readdirSync(baselinesDir)
  .filter(p => extname(p) === '.bicep' || extname(p) === '.bicepparam')
  .map(p => path.join(baselinesDir, p));

for (const filePath of baselineFiles) {
  describe(filePath, () => {
    if (!basename(filePath).startsWith('invalid_')) {
      // skip the invalid files - we don't expect them to compile

      it('can be compiled', async () => {
        const cliCsproj = `${__dirname}/../../Bicep.Cli/Bicep.Cli.csproj`;

        if (!existsSync(cliCsproj)) {
          fail(`Unable to find '${cliCsproj}'`);
          return;
        }
        
        const subCommand = extname(filePath) === '.bicepparam' ? 'build-params' : 'build';
        const result = spawnSync(`dotnet`, ['run', '-p', cliCsproj, subCommand, '--stdout', filePath], {
          encoding: 'utf-8',
          env,
        });

        // NOTE - if stderr or status are null, this indicates we were unable to invoke the exe (missing file, or hasn't had 'chmod +x' run)
        expect(result.error).toBeUndefined();
        expect(result.stderr).not.toContain(') : Error ')
        expect(result.status).toBe(0);
      });
    }

    it('baseline matches expected', async () => {
      const { expected, baselineFilePath } = await generateBaseline(filePath);

      await expectFileContents(baselineFilePath, expected);
    });
  });
}

describe('Test suite', () => {
  it('should not succeed if BASELINE_RECORD is set to true', () => {
    // This test just ensures the suite doesn't pass in 'record' mode
    expect(baselineRecordEnabled).toBeFalsy();
  });
});