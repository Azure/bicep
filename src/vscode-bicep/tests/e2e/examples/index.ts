// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { cpSync, existsSync, readFileSync } from "fs";
import * as path from "path";
import { fileURLToPath } from "url";
import { withTempDirectory } from "../utils/temp-directory";

const examplesRoot = path.join(findExtensionRoot(path.dirname(fileURLToPath(import.meta.url))), "tests/e2e/examples");

export function resolveExamplePath(exampleCategory: string, exampleFolder: string, exampleFile = "main.bicep"): string {
  return path.resolve(examplesRoot, exampleCategory, exampleFolder, exampleFile);
}

export function readExampleFile(exampleCategory: string, exampleFolder: string, exampleFile = "main.bicep"): string {
  const exampleFilePath = resolveExamplePath(exampleCategory, exampleFolder, exampleFile);

  return readFileSync(exampleFilePath, { encoding: "utf-8", flag: "r" });
}

export async function withWritableExampleDirectory<T>(
  exampleCategory: string,
  exampleFolder: string,
  action: (directory: string) => Promise<T>,
): Promise<T> {
  const sourceDirectory = path.dirname(resolveExamplePath(exampleCategory, exampleFolder));
  return await withTempDirectory("bicep-e2e-example-", async (directory) => {
    cpSync(sourceDirectory, directory, { recursive: true });
    return await action(directory);
  });
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
