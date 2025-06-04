// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import { readFile, rm } from "fs/promises";
import { spawnSync } from "child_process";
import { expectFileContents } from "./utils";

const root = `${__dirname}/..`;

const iifePath = `${root}/dist/bicep.min.js`;
const esPath = `${root}/dist/bicep.es.min.js`;

async function generateGrammar() {
  spawnSync(`webpack`, {
    cwd: root,
    stdio: 'inherit',
    encoding: 'utf8'
  });

  return {
    'bicep.min.js': await readFile(`${root}/out/bicep.min.js`, { encoding: 'utf8' }),
    'bicep.es.min.js': await readFile(`${root}/out/bicep.es.min.js`, { encoding: 'utf8' }),
  };
}

// Invoking webpack can take some time
const webpackTestTimeout = 60000

describe('grammar tests', () => {
  it('should exist', () => {
    expect(existsSync(iifePath)).toBeTruthy();
    expect(existsSync(esPath)).toBeTruthy();
  });

  it('should be up-to-date', async () => {
    const generatedGrammar = await generateGrammar();

    await expectFileContents(iifePath, generatedGrammar['bicep.min.js']);
    await expectFileContents(esPath, generatedGrammar['bicep.es.min.js']);
  }, webpackTestTimeout);
});
