// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { readFileSync } from "fs";
import * as path from "path";

// Cannot use __dirname because the .bicep files won't be copied to /out.
const examplesRoot = path.resolve(__dirname, "../../../../src/test/e2e/examples");

export function resolveExamplePath(exampleCategory: string, exampleFolder: string, exampleFile = "main.bicep"): string {
  return path.resolve(examplesRoot, exampleCategory, exampleFolder, exampleFile);
}

export function readExampleFile(exampleCategory: string, exampleFolder: string, exampleFile = "main.bicep"): string {
  const exampleFilePath = resolveExamplePath(exampleCategory, exampleFolder, exampleFile);

  return readFileSync(exampleFilePath, { encoding: "utf-8", flag: "r" });
}
