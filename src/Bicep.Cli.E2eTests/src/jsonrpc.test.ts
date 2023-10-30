// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep jsonrpc".
 *
 * @group CI
 */

import { MessageConnection } from "vscode-jsonrpc";
import { pathToExampleFile } from "./utils/fs";
import { compileRequestType, openConnection } from "./utils/jsonrpc";

let connection: MessageConnection;
beforeAll(async () => (connection = await openConnection()));
afterAll(() => connection.dispose());

describe("bicep jsonrpc", () => {
  it("should build a bicep file", async () => {
    const result = await compile(
      connection,
      pathToExampleFile("101", "aks.prod", "main.bicep"),
    );

    expect(result.success).toBeTruthy();
    expect(result.contents?.length).toBeGreaterThan(0);
  });

  it("should return diagnostics if the bicep file has errors", async () => {
    const result = await compile(
      connection,
      pathToExampleFile("101", "aks.prod", "flawed.bicep"),
    );

    expect(result.success).toBeFalsy();
    expect(result.contents).toBeUndefined();
    const error = result.diagnostics.filter(x => x.level === 'Error')[0];
    expect(error.code).toBe('BCP057')
    expect(error.message).toBe('The name "osDiskSizeGb" does not exist in the current context.');
  });
});

async function compile(connection: MessageConnection, bicepFile: string) {
  return await connection.sendRequest(compileRequestType, {
    path: bicepFile,
  });
}
