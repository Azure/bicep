// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  createExtensionHostErrorStream,
  isMissingAzExtUtilsSourceMapWarning,
} from "../test-e2e/vitest/extension-host-reporter";

describe("createExtensionHostErrorStream", () => {
  test("forwards Vitest error output without its trailing newline", () => {
    const output: string[] = [];
    const stream = createExtensionHostErrorStream((value) => output.push(value));

    stream.write("AssertionError: expected true to be false\n");

    expect(output).toEqual(["AssertionError: expected true to be false"]);
  });

  test("suppresses missing vscode-azext-utils source map warnings", () => {
    const output: string[] = [];
    const stream = createExtensionHostErrorStream((value) => output.push(value));

    stream.write(missingSourceMapWarning);

    expect(output).toEqual([]);
  });
});

const missingSourceMapWarning = [
  "12:37:40 AM [vite] (ssr) Failed to load source map for f:/repo/node_modules/@microsoft/vscode-azext-utils/dist/esm/src/wizard/AzureNameStep.js.",
  "Error: An error occurred while trying to read the map file at AzureNameStep.js.map",
  "Error: ENOENT: no such file or directory, open 'f:\\repo\\node_modules\\@microsoft\\vscode-azext-utils\\dist\\esm\\src\\wizard\\AzureNameStep.js.map'",
].join("\n");

describe("isMissingAzExtUtilsSourceMapWarning", () => {
  test("returns true for a missing vscode-azext-utils ESM source map", () => {
    expect(isMissingAzExtUtilsSourceMapWarning("stderr", missingSourceMapWarning)).toBe(true);
  });

  test("returns false for stdout", () => {
    expect(isMissingAzExtUtilsSourceMapWarning("stdout", missingSourceMapWarning)).toBe(false);
  });

  test("returns false for a different package", () => {
    expect(
      isMissingAzExtUtilsSourceMapWarning("stderr", missingSourceMapWarning.replaceAll("vscode-azext-utils", "other")),
    ).toBe(false);
  });

  test("returns false for a malformed source map", () => {
    expect(
      isMissingAzExtUtilsSourceMapWarning(
        "stderr",
        missingSourceMapWarning.replace("Error: ENOENT: no such file or directory", "Error: Unexpected token"),
      ),
    ).toBe(false);
  });
});
