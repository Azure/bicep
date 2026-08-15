// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as path from "path";
import * as fse from "fs-extra";

type IPackageKeyBinding = {
  command: string;
  key: string;
  mac?: string;
  when?: string;
};

type IPackage = {
  icon?: string;
  contributes?: {
    grammars?: Array<{ path: string }>;
    jsonValidation?: Array<{ url: string }>;
    keybindings?: IPackageKeyBinding[];
    languages?: Array<{
      configuration?: string;
      icon?: { dark: string; light: string };
    }>;
    snippets?: Array<{ path: string }>;
    walkthroughs?: Array<{
      steps: Array<{ media: { image?: string; markdown?: string } }>;
    }>;
  };
};

function getPackageJson(): IPackage {
  const packagePath = path.join(__dirname, "package.json");
  return fse.readJsonSync(packagePath);
}

describe("package.json", () => {
  it("provides Cmd-based Mac equivalents for Ctrl-based shortcuts", () => {
    const packageJson = getPackageJson();
    const bindings = packageJson.contributes?.keybindings ?? [];
    expect(bindings).not.toHaveLength(0);

    const invalidBindings = bindings.filter((binding) => {
      if (!/ctrl/i.test(binding.key)) {
        return false;
      }

      const macKey = binding.mac ?? "";
      return !/cmd/i.test(macKey) || getNonPlatformKeys(binding.key) !== getNonPlatformKeys(macKey);
    });

    expect(invalidBindings).toEqual([]);
  });

  it("references existing static resources", () => {
    const packageJson = getPackageJson();
    const generatedResources = new Set([
      "resources/language/bicep.tmlanguage",
      "resources/language/language-configuration.json",
    ]);
    const missingResources = getResourcePaths(packageJson)
      .filter((resourcePath) => !generatedResources.has(resourcePath))
      .filter((resourcePath) => !fse.existsSync(path.join(__dirname, resourcePath)));

    expect(missingResources).toEqual([]);
  });
});

function getResourcePaths(packageJson: IPackage): string[] {
  const contributions = packageJson.contributes;
  return [
    packageJson.icon,
    ...(contributions?.jsonValidation?.map(({ url }) => url) ?? []),
    ...(contributions?.languages?.flatMap(({ configuration, icon }) => [configuration, icon?.dark, icon?.light]) ?? []),
    ...(contributions?.grammars?.map(({ path }) => path) ?? []),
    ...(contributions?.snippets?.map(({ path }) => path) ?? []),
    ...(contributions?.walkthroughs?.flatMap(({ steps }) =>
      steps.flatMap(({ media }) => [media.image, media.markdown]),
    ) ?? []),
  ]
    .filter((resourcePath): resourcePath is string => resourcePath !== undefined)
    .map((resourcePath) => resourcePath.replace(/^\.\//, ""));
}

function getNonPlatformKeys(keybinding: string): string {
  return keybinding
    .toLowerCase()
    .replace(/\b(?:alt|cmd|ctrl|shift)\+/g, "")
    .replace(/\s+/g, " ")
    .trim();
}
