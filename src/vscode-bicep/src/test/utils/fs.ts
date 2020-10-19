// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import { readFileSync } from "fs";

const examplesRoot = path.resolve(__dirname, "../../../../../docs/examples");

export function readExampleFile(
  exampleCategory: string,
  exampleName: string
): string {
  const exampleFilePath = path.join(
    examplesRoot,
    exampleCategory,
    exampleName,
    "main.bicep"
  );

  return readFileSync(exampleFilePath, { encoding: "utf-8", flag: "r" });
}
