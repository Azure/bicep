// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";

const examplesRoot = path.resolve(__dirname, "../../../../../docs/examples");

export function getExampleBicepFilePath(
  exampleCategory: string,
  exampleName: string
): string {
  return path.join(examplesRoot, exampleCategory, exampleName, "main.bicep");
}
