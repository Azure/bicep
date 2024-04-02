// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import { generateGrammar, grammarPath } from "../src/bicep";
import { expectFileContents } from "./utils";

describe('grammar tests', () => {
  it('should exist', () => {
    expect(existsSync(grammarPath)).toBeTruthy();
  });

  it('should be up-to-date', async () => {
    const generatedGrammar = await generateGrammar();

    await expectFileContents(grammarPath, generatedGrammar);
  });
});