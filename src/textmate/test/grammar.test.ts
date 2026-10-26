// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import { generateGrammar, grammarPath } from "../src/bicep.js";
import { expectFileContents } from "./utils.js";

describe('grammar tests', () => {
  it('should exist', () => {
    expect(existsSync(grammarPath)).toBeTruthy();
  });

  it('should be up-to-date', async () => {
    const generatedGrammar = await generateGrammar();

    await expectFileContents(grammarPath, generatedGrammar);
  });
});