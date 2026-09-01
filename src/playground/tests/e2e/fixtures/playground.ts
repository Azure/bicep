// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  expect,
  test as base,
  type Locator,
  type Page,
} from "@playwright/test";

export type Playground = {
  bicepEditor: Locator;
  bicepPane: Locator;
  armTemplate: Locator;
  armPane: Locator;
  error: Locator;
  copyLink: Locator;
  sampleTemplate: Locator;
  replaceEditorText(editor: Locator, content: string): Promise<void>;
  readEditorText(editor: Locator): Promise<string>;
  selectQuickstart(path: string): Promise<void>;
};

type Fixtures = {
  playground: Playground;
};

export const test = base.extend<Fixtures>({
  playground: async ({ page }, use) => {
    await page
      .context()
      .grantPermissions(["clipboard-read", "clipboard-write"]);
    await page.goto("/");

    const playground = createPlayground(page);
    await expect(playground.bicepEditor).toBeVisible();
    await use(playground);
  },
});

export { expect };

export function createPlayground(page: Page): Playground {
  const bicepEditor = page.getByRole("region", { name: "Bicep editor" });
  const bicepPane = page.getByRole("tabpanel", { name: "Bicep" });
  const armTemplate = page.getByRole("region", {
    name: "Generated ARM template editor",
  });
  const armPane = page.getByRole("tabpanel", { name: "ARM template" });

  return {
    bicepEditor,
    bicepPane,
    armTemplate,
    armPane,
    error: page.getByRole("alert", { name: "Playground error" }),
    copyLink: page.getByRole("button", { name: /Copy Link|Copied/ }),
    sampleTemplate: page.getByRole("combobox", { name: "Sample template" }),
    async replaceEditorText(editor, content) {
      await page.evaluate(
        (text) => navigator.clipboard.writeText(text),
        content,
      );
      await editor.click();
      await page.keyboard.press("ControlOrMeta+A");
      await page.keyboard.press("ControlOrMeta+V");
    },
    async readEditorText(editor) {
      await editor.click();
      await page.keyboard.press("ControlOrMeta+A");
      await page.keyboard.press("ControlOrMeta+C");

      return normalizeLineEndings(
        await page.evaluate(() => navigator.clipboard.readText()),
      );
    },
    async selectQuickstart(path) {
      await page
        .getByRole("combobox", { name: "Sample template" })
        .selectOption(path);
    },
  };
}

export async function openPlayground(page: Page): Promise<Playground> {
  await page.context().grantPermissions(["clipboard-read", "clipboard-write"]);
  await page.goto("/");

  const playground = createPlayground(page);
  await expect(playground.bicepEditor).toBeVisible();

  return playground;
}

export async function installCompilerRequestTracker(page: Page): Promise<void> {
  await page.addInitScript(() => {
    const NativeWorker = window.Worker;
    let compileRequestCount = 0;

    class TrackedWorker extends NativeWorker {
      public constructor(scriptUrl: string | URL, options?: WorkerOptions) {
        super(scriptUrl, options);

        if (scriptUrl.toString().includes("compiler.worker")) {
          const nativePostMessage = this.postMessage.bind(this);
          this.postMessage = ((message: unknown, transfer?: Transferable[]) => {
            if (
              typeof message === "object" &&
              message !== null &&
              "operation" in message &&
              message.operation === "compile"
            ) {
              ++compileRequestCount;
            }

            nativePostMessage(message, transfer ?? []);
          }) as typeof this.postMessage;
        }
      }
    }

    Object.defineProperty(window, "__playgroundCompileRequestCount", {
      get: () => compileRequestCount,
    });
    Object.defineProperty(window, "Worker", {
      configurable: true,
      value: TrackedWorker,
    });
  });
}

export async function getCompileRequestCount(page: Page): Promise<number> {
  return await page.evaluate(
    () =>
      (
        window as typeof window & {
          __playgroundCompileRequestCount: number;
        }
      ).__playgroundCompileRequestCount,
  );
}

export function withoutGeneratorField(content: string): string {
  return normalizeLineEndings(content).replace(
    /"_generator"\s*:\s*\{[^}]*\}/s,
    "",
  );
}

function normalizeLineEndings(content: string): string {
  return content.replace(/\r\n/g, "\n");
}
