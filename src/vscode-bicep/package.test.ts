// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync, readFileSync } from "fs";
import * as path from "path";
import { fileURLToPath } from "url";

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
    menus?: Record<string, Array<{ command: string; when?: string }>>;
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
  const packagePath = path.join(path.dirname(fileURLToPath(import.meta.url)), "package.json");
  return JSON.parse(readFileSync(packagePath, "utf8")) as IPackage;
}

describe("package.json", () => {
  it("offers force restore for Bicep and parameter files in context menus and keyboard shortcuts", () => {
    const contributions = getPackageJson().contributes;
    const command = "bicep.forceModulesRestore";
    const expectedCondition = "resourceLangId == bicep || resourceLangId == bicep-params";

    expect(contributions?.keybindings?.find((binding) => binding.command === command)?.when).toBe(expectedCondition);
    for (const menu of ["explorer/context", "editor/title/context", "editor/context"]) {
      expect(contributions?.menus?.[menu].find((item) => item.command === command)?.when).toBe(expectedCondition);
    }
  });

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
      .filter((resourcePath) => !existsSync(path.join(path.dirname(fileURLToPath(import.meta.url)), resourcePath)));

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
