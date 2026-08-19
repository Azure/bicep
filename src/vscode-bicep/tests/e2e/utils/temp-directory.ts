// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import os from "os";
import path from "path";

export async function withTempDirectory<T>(
  filenamePrefix: string,
  action: (directory: string) => Promise<T>,
): Promise<T> {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), filenamePrefix));

  try {
    return await action(directory);
  } finally {
    try {
      fs.rmSync(directory, {
        force: true,
        recursive: true,
        maxRetries: 5,
        retryDelay: 1000,
      });
    } catch {
      // Cleanup is best-effort because VS Code can briefly retain file handles on Windows.
    }
  }
}
