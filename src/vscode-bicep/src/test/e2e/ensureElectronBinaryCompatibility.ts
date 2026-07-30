// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as fs from "fs";
import * as path from "path";

type FileSystem = Pick<typeof fs, "existsSync" | "symlinkSync">;

export function ensureElectronBinaryCompatibility(
  vscodeExecutablePath: string,
  platform: string = process.platform,
  fileSystem: FileSystem = fs,
): void {
  if (platform !== "darwin") {
    return;
  }

  const executableDirectory = path.dirname(vscodeExecutablePath);
  const electronExecutablePath = path.join(executableDirectory, "Electron");

  if (fileSystem.existsSync(electronExecutablePath) || !fileSystem.existsSync(vscodeExecutablePath)) {
    return;
  }

  fileSystem.symlinkSync(path.basename(vscodeExecutablePath), electronExecutablePath);
}
