// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { existsSync, readFileSync } from "fs";
import * as path from "path";

const examplesRoot = path.join(findExtensionRoot(__dirname), "test-e2e/examples");

export function resolveExamplePath(exampleCategory: string, exampleFolder: string, exampleFile = "main.bicep"): string {
  return path.resolve(examplesRoot, exampleCategory, exampleFolder, exampleFile);
}

export function readExampleFile(exampleCategory: string, exampleFolder: string, exampleFile = "main.bicep"): string {
  const exampleFilePath = resolveExamplePath(exampleCategory, exampleFolder, exampleFile);

  return readFileSync(exampleFilePath, { encoding: "utf-8", flag: "r" });
}

function findExtensionRoot(startPath: string): string {
  let candidate = startPath;

  while (true) {
    if (existsSync(path.join(candidate, "package.json"))) {
      return candidate;
    }

    const parent = path.dirname(candidate);
    if (parent === candidate) {
      throw new Error(`Could not find the extension package root from '${startPath}'.`);
    }

    candidate = parent;
  }
}
