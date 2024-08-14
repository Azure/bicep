// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as os from "os";
import * as path from "path";
import * as fse from "fs-extra";

export function createUniqueTempFolder(filenamePrefix: string): string {
  const tempFolder = os.tmpdir();
  if (!fse.existsSync(tempFolder)) {
    fse.mkdirSync(tempFolder, { recursive: true });
  }

  return fse.mkdtempSync(path.join(tempFolder, filenamePrefix));
}
