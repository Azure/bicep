// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import { readFile, rm } from "fs/promises";
import { spawnSync } from "child_process";
import { expectFileContents } from "./utils";
import { build } from 'esbuild';

const root = `${__dirname}/..`;

const iifePath = `${root}/dist/bicep.min.js`;
const esPath = `${root}/dist/bicep.es.min.js`;

async function generateGrammar() {
  const iifeTempPath = `${__dirname}/out/bicep.min.js`;
  const iifeHeader = '// https://github.com/Azure/bicep/blob/main/src/highlightjs/dist/bicep.min.js'
  await build({
    entryPoints: [`${root}/src/usage.ts`],
    outfile: iifeTempPath,
    minify: true,
    bundle: true,
    external: ['highlight.js'],
  });

  const esTempPath = `${__dirname}/out/bicep.es.min.js`;
  const esHeader = '// https://github.com/Azure/bicep/blob/main/src/highlightjs/dist/bicep.es.min.js'
  await build({
    entryPoints: [`${root}/src/bicep.ts`],
    outfile: esTempPath,
    minify: true,
    bundle: false,
  });

  return {
    'bicep.min.js': `${iifeHeader}\n${await readFile(iifeTempPath, { encoding: 'utf8' })}`,
    'bicep.es.min.js': `${esHeader}\n${await readFile(esTempPath, { encoding: 'utf8' })}`,
  };
}

describe('grammar tests', () => {
  it('should exist', () => {
    expect(existsSync(iifePath)).toBeTruthy();
    expect(existsSync(esPath)).toBeTruthy();
  });

  it('should be up-to-date', async () => {

    const files = await generateGrammar();

    await expectFileContents(iifePath, files['bicep.min.js']);
    await expectFileContents(esPath, files['bicep.es.min.js']);
  });
});
