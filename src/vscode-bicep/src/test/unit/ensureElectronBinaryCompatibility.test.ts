// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { PathLike } from "fs";
import { ensureElectronBinaryCompatibility } from "../e2e/ensureElectronBinaryCompatibility";

describe("ensureElectronBinaryCompatibility", () => {
  it("creates Electron symlink on macOS when Electron executable is missing", () => {
    const existsSync = jest.fn((path: PathLike) => path.toString().endsWith("/Visual Studio Code"));
    const symlinkSync = jest.fn();

    ensureElectronBinaryCompatibility(
      "/tmp/vscode/Visual Studio Code.app/Contents/MacOS/Visual Studio Code",
      "darwin",
      { existsSync, symlinkSync },
    );

    expect(existsSync).toHaveBeenCalledWith("/tmp/vscode/Visual Studio Code.app/Contents/MacOS/Electron");
    expect(symlinkSync).toHaveBeenCalledWith(
      "Visual Studio Code",
      "/tmp/vscode/Visual Studio Code.app/Contents/MacOS/Electron",
    );
  });

  it("does nothing on non-macOS platforms", () => {
    const existsSync = jest.fn();
    const symlinkSync = jest.fn();

    ensureElectronBinaryCompatibility("/tmp/vscode/Code", "linux", { existsSync, symlinkSync });

    expect(existsSync).not.toHaveBeenCalled();
    expect(symlinkSync).not.toHaveBeenCalled();
  });
});
