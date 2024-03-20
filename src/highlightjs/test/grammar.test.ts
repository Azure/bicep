// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import { readFile, rm } from "fs/promises";
import { spawnSync } from "child_process";
import { expectFileContents } from "./utils";

const root = `${__dirname}/..`;

const grammarPath = `${root}/dist/bicep.min.js`;
async function generateGrammar() {
  spawnSync(`webpack`, {
    cwd: root,
    stdio: 'inherit',
    encoding: 'utf8'
  });

  return await readFile(`${root}/out/usage.min.js`, { encoding: 'utf8' });
}

// Invoking webpack can take some time
const webpackTestTimeout = 60000

describe('grammar tests', () => {
  it('should exist', () => {
    expect(existsSync(grammarPath)).toBeTruthy();
  });

  it('should be up-to-date', async () => {
    const generatedGrammar = await generateGrammar();

    await expectFileContents(grammarPath, generatedGrammar);
  }, webpackTestTimeout);
});
