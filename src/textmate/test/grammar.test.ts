// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import { readFile } from "fs/promises";
import { generateGrammar, grammarPath } from "../src/bicep";

describe('grammar tests', () => {
  it('should exist', () => {
    expect(existsSync(grammarPath)).toBeTruthy();
  });

  it('should be up-to-date', async () => {
    const generatedGrammar = await generateGrammar();
    const savedGrammar = await readFile(grammarPath, { encoding: 'utf8' });

    expect(generatedGrammar).toEqual(savedGrammar);
  });
});