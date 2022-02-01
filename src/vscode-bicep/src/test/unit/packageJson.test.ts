// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as fse from "fs-extra";
import * as path from "path";

type IPackageKeyBinding = {
  command: string;
  key: string;
  mac?: string;
  when?: string;
};

type IPackage = {
  contributes?: {
    keybindings?: IPackageKeyBinding[];
  };
};

function getPackageJson(): IPackage {
  const packagePath = path.join(__dirname, "../../../package.json");
  return fse.readJsonSync(packagePath);
}

describe("macShortcutKeys", () => {
  it("shortcut keys using CTRL should have a Mac equivalent using CMD", () => {
    const packageJson = getPackageJson();

    // If there are no keybindings found we're doing something wrong in the test
    const bindings: IPackageKeyBinding[] =
      packageJson.contributes?.keybindings ?? [];
    expect(bindings).not.toHaveLength(0);

    for (const binding of bindings) {
      if (binding.key.match(/ctrl/i)) {
        // See https://code.visualstudio.com/api/references/contribution-points#keybinding-example
        // Note that "ALT" in a "mac" binding is interpreted as "OPT"

        // eslint-disable-next-line jest/no-conditional-expect
        expect(binding.mac).toMatch(/cmd/i);
      }
    }
  });
});
