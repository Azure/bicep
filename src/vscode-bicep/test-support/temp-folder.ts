// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import os from "os";
import path from "path";

export function createUniqueTempFolder(filenamePrefix: string): string {
  return fs.mkdtempSync(path.join(os.tmpdir(), filenamePrefix));
}
