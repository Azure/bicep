// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { parseError } from "@microsoft/vscode-azext-utils";

//asdfg doc
export async function testScope(
  testScopeName: string,
  action: () => Promise<void> | void
): Promise<void> {
  try {
    await action();
  } catch (err) {
    throw new Error(
      `Test failure in scope "${testScopeName}":\n${parseError(err).message}`
    );
  }
}
