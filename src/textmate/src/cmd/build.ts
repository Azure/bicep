// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { writeFile } from "fs/promises";
import { generateGrammar, grammarPath } from "../bicep";

async function main() {
  const grammar = await generateGrammar();
  await writeFile(grammarPath, grammar);
}

main().catch((err) => {
  console.log(err.stack);
  process.exit(1);
});