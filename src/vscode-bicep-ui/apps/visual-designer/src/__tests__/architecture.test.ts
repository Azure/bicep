// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ESLint } from "eslint";
import { beforeAll, describe, expect, it } from "vitest";

const eslint = new ESLint();

beforeAll(async () => {
  await eslint.calculateConfigForFile("src/app/boundary-check.ts");
}, 30_000);

async function boundaryMessages(filePath: string, dependency: string) {
  const results = await eslint.lintText(
    `// Copyright (c) Microsoft Corporation.\n// Licensed under the MIT License.\n\nimport "${dependency}";\n`,
    { filePath },
  );
  return results.flatMap((result) => result.messages.filter((message) => message.ruleId === "no-restricted-imports"));
}

describe("public module boundaries", () => {
  it.each(["@/core", "@/features/export", "@/lib/graph", "@/ui", "@/ui/theme"])(
    "allows the public entry point %s",
    async (dependency) => {
      expect(await boundaryMessages("src/app/boundary-check.ts", dependency)).toEqual([]);
    },
  );

  it.each([
    "@/core/atoms",
    "@/features/export/atoms",
    "@/lib/graph/atoms",
    "@/ui/components/IconButton",
    "@/ui/theme/atoms",
    "@/devtools/fakes/fake-graph",
  ])("rejects the private subpath %s", async (dependency) => {
    expect(await boundaryMessages("src/app/boundary-check.ts", dependency)).not.toEqual([]);
  });

  it("allows relative imports inside a feature", async () => {
    expect(await boundaryMessages("src/features/export/components/boundary-check.ts", "../atoms")).toEqual([]);
  });

  it.each(["@/core", "../../../../core/api", "@vscode-bicep-ui/messaging"])(
    "keeps the rendering engine independent of %s",
    async (dependency) => {
      expect(await boundaryMessages("src/lib/graph/components/boundary-check.ts", dependency)).not.toEqual([]);
    },
  );

  it("keeps core independent of product features", async () => {
    expect(await boundaryMessages("src/core/boundary-check.ts", "@/features/export")).not.toEqual([]);
  });
});

describe("feature dependency direction", () => {
  it.each([
    ["canvas", ["export"]],
    ["controls", ["export"]],
    ["dock", ["palette"]],
    ["export", []],
    ["palette", ["canvas"]],
    ["status", []],
  ] as const)("allows only the declared dependencies of %s", async (feature, allowed) => {
    for (const other of ["canvas", "controls", "dock", "export", "palette", "status"]) {
      if (other === feature) {
        continue;
      }
      const messages = await boundaryMessages(`src/features/${feature}/boundary-check.ts`, `@/features/${other}`);
      expect(messages.length === 0, `${feature} -> ${other}`).toBe(allowed.some((dependency) => dependency === other));
    }
  });
});
