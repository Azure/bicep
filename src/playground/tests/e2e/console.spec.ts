// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createPlayground, test, expect } from "./fixtures/playground";
import type { Page } from "@playwright/test";
import { deflate, inflate } from "pako";

function consoleShareHash(commands: unknown): string {
  const data = deflate(new TextEncoder().encode(JSON.stringify(commands)));
  return `#console/${Buffer.from(data).toString("base64")}`;
}

async function copyConsoleHistory(
  page: Page,
): Promise<{ link: string; commands: unknown }> {
  await page.getByRole("button", { name: "Copy Link", exact: true }).click();
  await expect(
    page.getByRole("button", { name: "Copied", exact: true }),
  ).toBeVisible();
  const link = await page.evaluate(() => navigator.clipboard.readText());
  const hash = new URL(link).hash;
  expect(hash).toMatch(/^#console\//);
  const data = Buffer.from(hash.substring("#console/".length), "base64");
  return {
    link,
    commands: JSON.parse(new TextDecoder().decode(inflate(data))),
  };
}

function getConsoleEditor(page: Page) {
  return page.getByRole("region", { name: "Bicep console input", exact: true });
}

async function replaceConsoleInput(page: Page, content: string) {
  await createPlayground(page).replaceEditorText(
    getConsoleEditor(page),
    content,
  );
}

async function expectConsoleInput(page: Page, content: string) {
  if (content === "") {
    await expect(getConsoleEditor(page).locator(".view-lines")).toHaveText("");
    return;
  }
  await expect
    .poll(() => createPlayground(page).readEditorText(getConsoleEditor(page)))
    .toBe(content);
  await page.keyboard.press("ArrowRight");
}

test.describe("console UI with mocked .NET exports", () => {
  test.beforeEach(async ({ page }) => {
    await page
      .context()
      .grantPermissions(["clipboard-read", "clipboard-write"]);
    await page.route("**/_framework/dotnet.js", (route) =>
      route.fulfill({
        contentType: "text/javascript",
        body: `
          let session = 0;
          const methods = {
            ResetConsole: () => JSON.stringify(String(++session)),
            EvaluateConsole: (content, sessionId) => {
              if (sessionId !== String(session)) throw new Error('Session lost');
              if (content === 'fail') throw new Error('Evaluation transport failed');
              return JSON.stringify({ output: JSON.stringify(content), diagnostics: [] });
            },
            CompileAndEmitDiagnostics: async () => JSON.stringify({
              template: '{}', diagnostics: [], error: null,
            }),
            GetSemanticTokensLegend: () => JSON.stringify({
              tokenTypes: [], tokenModifiers: [],
            }),
            GetSemanticTokens: async () => JSON.stringify({ data: [] }),
          };
          export const dotnet = {
            create: async () => ({
              setModuleImports() {},
              getConfig: () => ({ mainAssemblyName: 'Bicep.Wasm' }),
              getAssemblyExports: async () => ({
                Bicep: { Wasm: { WorkerInterop: methods } },
              }),
            }),
          };
        `,
      }),
    );
    await page.goto("/");
    await page.getByRole("tab", { name: "Console", exact: true }).click();
  });

  test("highlights live input, submitted Bicep, and JSON results in both themes", async ({
    page,
  }) => {
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    const command = "var greeting = 'hello'";
    await replaceConsoleInput(page, command);
    const highlight = page.locator(".console-input .view-lines");
    await expect(highlight.locator("span[class^='mtk']")).not.toHaveCount(0);
    await expect(highlight).toContainText(command);
    const tokenColors = () =>
      highlight
        .locator("span[class^='mtk']")
        .evaluateAll((tokens) => [
          ...new Set(tokens.map((token) => getComputedStyle(token).color)),
        ]);
    await expect
      .poll(async () => (await tokenColors()).length)
      .toBeGreaterThan(1);
    const lightColors = await tokenColors();
    await page.emulateMedia({ colorScheme: "dark" });
    await expect.poll(tokenColors).not.toEqual(lightColors);
    await expectConsoleInput(page, command);
    await input.press("Enter");
    const entry = page.locator(".console-entry").last();
    await expect(
      entry.locator(".console-input-echo span[class^='mtk']"),
    ).not.toHaveCount(0);
    await expect(
      entry.locator("pre").nth(1).locator("span[class^='mtk']"),
    ).not.toHaveCount(0);
    await expect(input).toBeFocused();
    await input.press("ArrowUp");
    await expectConsoleInput(page, command);
    await expect(highlight).toContainText(command);

    await replaceConsoleInput(page, "'first'\n'second'\n'last'");
    await input.press("ControlOrMeta+End");
    await expect(highlight.getByText("'last'", { exact: true })).toBeVisible();
    await page.emulateMedia({ forcedColors: "active" });
    await expect(highlight).toBeVisible();
    await expect(highlight.locator("span[class^='mtk']").last()).not.toHaveCSS(
      "color",
      "rgba(0, 0, 0, 0)",
    );
  });

  test("matches workspace tabs to toolbar buttons in both themes", async ({
    page,
  }) => {
    const header = page.getByRole("banner");
    const editorTab = header.getByRole("tab", {
      name: "Editor",
      exact: true,
    });
    const consoleTab = header.getByRole("tab", {
      name: "Console",
      exact: true,
    });
    const decompile = header.getByRole("button", {
      name: "Decompile",
      exact: true,
    });
    await editorTab.click();
    for (const colorScheme of ["light", "dark"] as const) {
      await page.emulateMedia({ colorScheme });
      for (const property of [
        "height",
        "border-radius",
        "border-width",
        "padding",
        "font-family",
        "font-size",
        "background-color",
        "border-color",
      ]) {
        const expected = await decompile.evaluate(
          (element, property) =>
            getComputedStyle(element).getPropertyValue(property),
          property,
        );
        await expect(consoleTab).toHaveCSS(property, expected);
      }
      const inactiveBorder = await consoleTab.evaluate(
        (element) => getComputedStyle(element).borderColor,
      );
      await expect(editorTab).not.toHaveCSS("border-color", inactiveBorder);
      await expect(editorTab).toHaveCSS("font-weight", "600");
      await consoleTab.click();
      await expect(consoleTab).toHaveAttribute("aria-selected", "true");
      await expect(
        page.getByRole("button", { name: "Copy Link", exact: true }),
      ).toBeVisible();
      await editorTab.click();
    }
  });

  test("keeps workspace tabs in the header and shows editor actions only in Editor mode", async ({
    page,
  }) => {
    const header = page.getByRole("banner");
    const editorTab = header.getByRole("tab", {
      name: "Editor",
      exact: true,
    });
    const consoleTab = header.getByRole("tab", {
      name: "Console",
      exact: true,
    });
    const sample = header.getByRole("combobox", { name: "Sample template" });
    const decompile = header.getByRole("button", {
      name: "Decompile",
      exact: true,
    });
    const copy = header.getByRole("button", { name: /Copy Link|Copied/ });
    for (const width of [1024, 768, 681, 375, 320]) {
      await page.setViewportSize({ width, height: 812 });
      await consoleTab.click();
      await expect(sample).toBeHidden();
      await expect(decompile).toBeHidden();
      await expect(copy).toBeVisible();
      await expect(
        page.getByRole("contentinfo").getByRole("status"),
      ).toHaveCount(0);
      await expect(page.locator(".status-details")).toHaveCount(0);
      await expect(
        page.getByRole("link", {
          name: "Bicep repository on GitHub (opens in a new tab)",
        }),
      ).toBeVisible();
      await expect(
        header.getByRole("button", { name: /Switch to .* theme/ }),
      ).toBeVisible();
      const editorBounds = await editorTab.boundingBox();
      const consoleBounds = await consoleTab.boundingBox();
      expect(editorBounds!.y).toBe(consoleBounds!.y);
      expect((await header.boundingBox())!.height).toBeLessThan(50);
      await editorTab.click();
      await expect(sample).toBeVisible();
      await expect(decompile).toBeVisible();
      await expect(copy).toBeVisible();
      await expect(
        page.getByRole("contentinfo", { name: "Compilation status" }),
      ).toBeVisible();
      await expect(page.locator(".status-section")).toBeVisible();
      await expect(page.locator(".status-details")).toHaveCount(1);
      const actionsBounds = await copy.boundingBox();
      expect(actionsBounds!.x + actionsBounds!.width).toBeLessThanOrEqual(
        width,
      );
      expect(
        await page.evaluate(() => document.documentElement.scrollWidth),
      ).toBeLessThanOrEqual(width);
    }
  });

  test("routes workspace tabs through the URL and preserves state across Back and Forward", async ({
    page,
  }) => {
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    await expect(page).toHaveURL(/#console$/);
    await replaceConsoleInput(page, "saved draft");
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await expect(page).toHaveURL(/#editor$/);
    await page.goBack();
    await expect(
      page.getByRole("tab", { name: "Console", exact: true }),
    ).toHaveAttribute("aria-selected", "true");
    await expectConsoleInput(page, "saved draft");
    await page.goForward();
    await expect(
      page.getByRole("tab", { name: "Editor", exact: true }),
    ).toHaveAttribute("aria-selected", "true");
    await page.goto("/#console");
    await expect(input).toBeVisible();
    await expect(page).toHaveURL(/#console$/);
    await page.reload();
    await expect(input).toBeVisible();
    await expect(page).toHaveURL(/#console$/);
    await page.goto("/#editor");
    await expect(
      page.getByRole("tabpanel", { name: "Editor", exact: true }),
    ).toBeVisible();
    await expect(page).toHaveURL(/#editor$/);
  });

  test("opens legacy shared code in Editor and copies links without duplicate hashes", async ({
    page,
  }) => {
    const content = "output shared string = 'preserved'";
    const hash = `#${Buffer.from(deflate(new TextEncoder().encode(content))).toString("base64")}`;
    await page.goto(`/${hash}`);
    await expect(
      page.getByRole("tab", { name: "Editor", exact: true }),
    ).toHaveAttribute("aria-selected", "true");
    await expect(page).toHaveURL(/#editor$/);
    await expect(page.locator(".view-lines").first()).toContainText(
      "preserved",
    );
    await page
      .context()
      .grantPermissions(["clipboard-read", "clipboard-write"]);
    await page.getByRole("button", { name: "Copy Link", exact: true }).click();
    const copied = await page.evaluate(() => navigator.clipboard.readText());
    expect(copied.split("#")).toHaveLength(2);
    expect(new URL(copied).hash).toMatch(/^#editor\//);
    expect(copied).not.toContain("#editor#");
    await page.goto(copied);
    await expect(
      page.getByRole("tab", { name: "Editor", exact: true }),
    ).toHaveAttribute("aria-selected", "true");
    await expect(page.locator(".view-lines").first()).toContainText(
      "preserved",
    );
  });

  test("uses native keyboard activation for full workspace tabs", async ({
    page,
  }) => {
    const consoleTab = page.getByRole("tab", { name: "Console", exact: true });
    const editorTab = page.getByRole("tab", { name: "Editor", exact: true });
    const consoleWorkspace = page.getByRole("tabpanel", {
      name: "Console",
      exact: true,
    });
    const editorWorkspace = page.getByRole("tabpanel", {
      name: "Editor",
      exact: true,
    });
    await expect(consoleTab).toHaveAttribute("aria-selected", "true");
    await expect(editorWorkspace).toBeHidden();
    const bounds = await consoleWorkspace.boundingBox();
    expect(bounds!.height).toBeGreaterThan(500);
    await consoleTab.focus();
    await consoleTab.press("Shift+Tab");
    await expect(editorTab).toBeFocused();
    await editorTab.press("Enter");
    await expect(editorTab).toHaveAttribute("aria-selected", "true");
    await expect(consoleWorkspace).toBeHidden();
    await expect(editorWorkspace).toBeVisible();
    await editorTab.press("Tab");
    await expect(consoleTab).toBeFocused();
    await consoleTab.press("Space");
    await expect(consoleWorkspace).toBeVisible();
    await consoleTab.press("Shift+Tab");
    await expect(editorTab).toBeFocused();
    await editorTab.press("Enter");
    await expect(editorWorkspace).toBeVisible();
  });

  test("renders output safely, surfaces transport errors, and preserves panel state", async ({
    page,
  }) => {
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    const output = page.getByRole("log", { name: "Console output" });
    await expect(input).toBeFocused();
    await replaceConsoleInput(page, "<script>throw 'not executed'</script>");
    await input.press("Enter");
    await expect(output.locator(".console-entry").last()).toContainText(
      '"<script>',
    );
    await expect(output.locator("script")).toHaveCount(0);
    await replaceConsoleInput(page, "fail");
    await input.press("Enter");
    await expect(output).toContainText("Evaluation transport failed");
    await replaceConsoleInput(page, "draft");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "fail");
    await input.press("ArrowDown");
    await expectConsoleInput(page, "draft");
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await expect(input).toBeHidden();
    await page.getByRole("tab", { name: "Console", exact: true }).click();
    await expectConsoleInput(page, "draft");
    await expect(output).toContainText("Evaluation transport failed");
    await page.getByRole("button", { name: "Clear output" }).click();
    await expect(output.locator(".console-entry")).toHaveCount(0);
    await input.press("ArrowUp");
    await expectConsoleInput(page, "fail");
    await page.getByRole("button", { name: "Reset session" }).click();
    await expectConsoleInput(page, "");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "");
    await replaceConsoleInput(page, "recovered");
    await input.press("Enter");
    await expect(output).toContainText('"recovered"');
  });

  test("supports multiline input and narrow layouts in both themes", async ({
    page,
  }) => {
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    await page.setViewportSize({ width: 320, height: 640 });
    await replaceConsoleInput(page, "[");
    await input.press("Shift+Enter");
    await input.pressSequentially("1]");
    await expectConsoleInput(page, "[\n1]");
    await expect(page.locator(".console-entry")).toHaveCount(0);
    await page.getByRole("button", { name: "Run", exact: true }).click();
    await expect(
      page.getByRole("log", { name: "Console output" }),
    ).toContainText('"[\\n1]"');
    for (const colorScheme of ["light", "dark"] as const) {
      await page.emulateMedia({ colorScheme });
      await expect(input).toBeVisible();
      const panel = page.getByRole("region", {
        name: "Bicep console",
        exact: true,
      });
      const bounds = await panel.boundingBox();
      expect(bounds).not.toBeNull();
      expect(bounds!.x).toBeGreaterThanOrEqual(0);
      expect(bounds!.x + bounds!.width).toBeLessThanOrEqual(320);
      expect(
        await page.evaluate(() => document.documentElement.scrollWidth),
      ).toBeLessThanOrEqual(320);
    }
  });

  test("keeps history at line boundaries, leaves selections alone, and allows Tab to exit", async ({
    page,
  }) => {
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    await replaceConsoleInput(page, "previous");
    await input.press("Enter");
    await expect(
      page.getByRole("log", { name: "Console output" }),
    ).toContainText('"previous"');
    await replaceConsoleInput(page, "'first'\n'last'");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "'first'\n'last'");
    await input.press("ControlOrMeta+Home");
    await input.press("Shift+ArrowRight");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "'first'\n'last'");
    await input.press("ControlOrMeta+Home");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "previous");
    await input.press("ArrowDown");
    await expectConsoleInput(page, "'first'\n'last'");
    await input.press("Tab");
    await expect(
      page.getByRole("button", { name: "Run", exact: true }),
    ).toBeFocused();
  });

  test("copies Unicode and multiline history and replays shared commands in order", async ({
    page,
  }) => {
    const commands = [
      "var greeting = 'Hello 🌍'",
      "{\n  name: '日本語'\n}",
      "greeting",
    ];
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    for (const command of commands) {
      await replaceConsoleInput(page, command);
      await input.press("Enter");
      await expect(
        page.getByRole("button", { name: "Evaluating..." }),
      ).toHaveCount(0);
    }
    await replaceConsoleInput(page, "unsubmitted draft");
    const shared = await copyConsoleHistory(page);
    expect(shared.commands).toEqual(commands);
    expect(shared.link.split("#")).toHaveLength(2);
    await page.goto(shared.link);
    await expect(page).toHaveURL(/#console$/);
    await expect(page.locator(".console-entry")).toHaveCount(commands.length);
    const echoes = page.locator(".console-input-echo");
    for (let index = 0; index < commands.length; index++) {
      await expect(echoes.nth(index)).toContainText(commands[index], {
        useInnerText: true,
      });
    }
    await expectConsoleInput(page, "");
    await input.press("ArrowUp");
    await expectConsoleInput(page, commands[commands.length - 1]);
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await page.goBack();
    await expect(page.locator(".console-entry")).toHaveCount(commands.length);
    await page.goForward();
    await page.goBack();
    await expect(page.locator(".console-entry")).toHaveCount(commands.length);
  });

  test("shares cleared history but excludes reset history and unsubmitted input", async ({
    page,
  }) => {
    await replaceConsoleInput(page, "submitted");
    await page.getByRole("button", { name: "Run", exact: true }).click();
    await expect(page.locator(".console-entry")).toContainText('"submitted"');
    await page.getByRole("button", { name: "Clear output" }).click();
    await replaceConsoleInput(page, "draft");
    expect((await copyConsoleHistory(page)).commands).toEqual(["submitted"]);
    await page.getByRole("button", { name: "Reset session" }).click();
    await expectConsoleInput(page, "");
    const shared = await copyConsoleHistory(page);
    expect(shared.commands).toEqual([]);
    await page.goto(shared.link);
    await expect(page).toHaveURL(/#console$/);
    await expect(
      page.getByRole("button", { name: "Copy Link", exact: true }),
    ).toBeEnabled();
    await expect(page.locator(".console-entry")).toHaveCount(0);
  });

  test("restores a new shared session in place without changing editor content", async ({
    page,
  }) => {
    const playground = createPlayground(page);
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await playground.replaceEditorText(
      playground.bicepEditor,
      "output unchanged int = 42",
    );
    await page.evaluate(
      (hash) => {
        window.location.hash = hash;
      },
      consoleShareHash(["first", "second"]),
    );
    await expect(page.locator(".console-entry")).toHaveCount(2);
    await expect(page.locator(".console-entry").last()).toContainText(
      '"second"',
    );
    await page.evaluate(
      (hash) => {
        window.location.hash = hash;
      },
      consoleShareHash(["replacement"]),
    );
    await expect(page.locator(".console-entry")).toHaveCount(1);
    await expect(page.locator(".console-entry")).toContainText('"replacement"');
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toBe("output unchanged int = 42");
  });

  test("reports invalid console links without discarding the current session", async ({
    page,
  }) => {
    await replaceConsoleInput(page, "saved draft");
    const invalidHashes = [
      "#console/not-base64!",
      "#console/",
      consoleShareHash({ commands: [] }),
      consoleShareHash([1]),
      consoleShareHash([" "]),
      `#console/${Buffer.from(deflate(new Uint8Array([0xff]))).toString("base64")}`,
    ];
    for (const hash of invalidHashes) {
      await page.evaluate((hash) => {
        window.location.hash = hash;
      }, hash);
      await expect(
        page.getByRole("alert", { name: "Playground error" }),
      ).toContainText("shared console history is invalid");
      await expect(page).toHaveURL(/#console$/);
      await expectConsoleInput(page, "saved draft");
      await page.getByRole("button", { name: "Dismiss error" }).click();
    }
    await page.goto("/#console/broken");
    await expect(
      page.getByRole("alert", { name: "Playground error" }),
    ).toContainText("shared console history is invalid");
    await expect(getConsoleEditor(page)).toBeVisible();
  });

  test("opens editor share links in place and reports invalid editor payloads", async ({
    page,
  }) => {
    const playground = createPlayground(page);
    for (const prefix of ["editor/", ""]) {
      const source = `output shared string = '${prefix || "legacy"}'`;
      const payload = Buffer.from(
        deflate(new TextEncoder().encode(source)),
      ).toString("base64");
      await page.evaluate((hash) => {
        window.location.hash = hash;
      }, `#${prefix}${payload}`);
      await expect(page).toHaveURL(/#editor$/);
      await expect
        .poll(() => playground.readEditorText(playground.bicepEditor))
        .toBe(source);
    }
    await page.evaluate(() => {
      window.location.hash = "#editor/not-base64!";
    });
    await expect(playground.error).toContainText(
      "shared editor source is invalid",
    );
    await expect(page).toHaveURL(/#editor$/);
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toBe("output shared string = 'legacy'");
  });

  test("keeps history and share links consistent after resubmitting a recalled command", async ({
    page,
  }) => {
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    for (const command of ["first", "second"]) {
      await replaceConsoleInput(page, command);
      await input.press("Enter");
      await expect(
        page.getByRole("button", { name: "Evaluating..." }),
      ).toHaveCount(0);
    }
    await input.press("ArrowUp");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "first");
    await input.press("Enter");
    await expect(page.locator(".console-entry")).toHaveCount(3);
    await replaceConsoleInput(page, "draft");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "first");
    await input.press("ArrowDown");
    await expectConsoleInput(page, "draft");
    expect((await copyConsoleHistory(page)).commands).toEqual([
      "first",
      "second",
      "first",
    ]);
  });

  test("reports console clipboard failures and warns that commands are shared", async ({
    page,
  }) => {
    const copy = page.getByRole("button", { name: "Copy Link", exact: true });
    await expect(copy).toHaveAttribute(
      "title",
      /all submitted console commands/,
    );
    await expect(page.getByRole("log")).toContainText("Do not share secrets.");
    await page.evaluate(() => {
      Object.defineProperty(navigator.clipboard, "writeText", {
        configurable: true,
        value: () => Promise.reject(new Error("Clipboard permission denied")),
      });
    });
    await copy.click();
    await expect(
      page.getByRole("alert", { name: "Playground error" }),
    ).toContainText("Clipboard permission denied");
    await expect(copy).toBeVisible();
  });

  test("disables sharing during replay and ignores superseded replay results", async ({
    page,
  }) => {
    await page.addInitScript(() => {
      const state = window as Window & {
        __releaseConsoleRequest?: () => void;
        __supersededConsoleResponse?: boolean;
      };
      const NativeWorker = window.Worker;
      class DelayedWorker extends NativeWorker {
        constructor(url: string | URL, options?: WorkerOptions) {
          super(url, options);
          const send = this.postMessage.bind(this);
          let heldRequestId: unknown;
          this.addEventListener("message", (event) => {
            if (
              heldRequestId !== undefined &&
              event.data?.requestId === heldRequestId
            ) {
              state.__supersededConsoleResponse = true;
            }
          });
          this.postMessage = ((message: unknown, transfer?: Transferable[]) => {
            if (
              typeof message === "object" &&
              message !== null &&
              "operation" in message &&
              message.operation === "evaluateConsole" &&
              "content" in message &&
              message.content === "held" &&
              "requestId" in message
            ) {
              heldRequestId = message.requestId;
              state.__releaseConsoleRequest = () =>
                send(message, transfer ?? []);
            } else {
              send(message, transfer ?? []);
            }
          }) as typeof this.postMessage;
        }
      }
      Object.defineProperty(window, "Worker", {
        configurable: true,
        value: DelayedWorker,
      });
    });
    await page.reload();
    await page.goto(`/${consoleShareHash(["held", "stale"])}`);
    await expect
      .poll(() =>
        page.evaluate(() =>
          Boolean(
            (window as Window & { __releaseConsoleRequest?: () => void })
              .__releaseConsoleRequest,
          ),
        ),
      )
      .toBe(true);
    await expect(
      page.getByRole("button", { name: "Copy Link", exact: true }),
    ).toBeDisabled();
    await page.evaluate(
      (hash) => {
        window.location.hash = hash;
      },
      consoleShareHash(["fresh"]),
    );
    await expect(page.locator(".console-entry")).toHaveCount(1);
    await expect(page.locator(".console-entry")).toContainText('"fresh"');
    await page.evaluate(() => {
      (
        window as Window & { __releaseConsoleRequest?: () => void }
      ).__releaseConsoleRequest?.();
    });
    await expect
      .poll(() =>
        page.evaluate(
          () =>
            (window as Window & { __supersededConsoleResponse?: boolean })
              .__supersededConsoleResponse,
        ),
      )
      .toBe(true);
    await expect(page.locator(".console-entry")).toHaveCount(1);
    await expect(page.locator(".console-entry")).toContainText('"fresh"');
    await expect(page.getByRole("log").getByRole("alert")).toHaveCount(0);
    expect((await copyConsoleHistory(page)).commands).toEqual(["fresh"]);
  });

  test("stops history replay on transport failure and permits an explicit reset", async ({
    page,
  }) => {
    await page.goto(`/${consoleShareHash(["first", "fail", "not executed"])}`);
    await expect(page.locator(".console-entry")).toHaveCount(2);
    await expect(page.getByRole("log")).toContainText(
      "Evaluation transport failed",
    );
    await expect(page.getByRole("log").getByRole("alert")).toContainText(
      "history replay stopped",
    );
    await expect(page.getByRole("log")).not.toContainText("not executed");
    await page.getByRole("button", { name: "Reset session" }).click();
    await expect(page.locator(".console-entry")).toHaveCount(0);
    await expect(page.getByRole("log").getByRole("alert")).toHaveCount(0);
  });
});

test.describe("interactive console", () => {
  test("restores declarations, functions, diagnostics and Unicode from a copied console link", async ({
    page,
    playground,
  }) => {
    test.setTimeout(120_000);
    void playground;
    await page.getByRole("tab", { name: "Console", exact: true }).click();
    const commands = [
      "type Person = { name: string }",
      "func greet(person Person) string => 'Hello ${person.name}'",
      "var userPerson = { name: '🌍' }",
      "var count = 1",
      "var count = unknownName",
      "greet(userPerson)",
      "[\n  count\n  2\n]",
    ];
    for (const command of commands) {
      await replaceConsoleInput(page, command);
      await page.getByRole("button", { name: "Run", exact: true }).click();
      await expect(
        page.getByRole("button", { name: "Evaluating..." }),
      ).toHaveCount(0);
    }
    const shared = await copyConsoleHistory(page);
    expect(shared.commands).toEqual(commands);
    await page.goto(shared.link);
    await expect(page).toHaveURL(/#console$/);
    const output = page.getByRole("log", { name: "Console output" });
    await expect(output.locator(".console-entry")).toHaveCount(commands.length);
    await expect(output).toContainText('"Hello 🌍"');
    await expect(output).toContainText("BCP057");
    await expect(output.locator(".console-entry").last()).toContainText(
      "[\n  1,\n  2\n]",
      { useInnerText: true },
    );
    await expect(output.getByRole("alert")).toHaveCount(0);
    await replaceConsoleInput(page, "greet(userPerson)");
    await page.getByRole("button", { name: "Run", exact: true }).click();
    await expect(output.locator(".console-entry").last()).toContainText(
      '"Hello 🌍"',
    );
  });

  test("evaluates expressions, declarations, and errors without changing the editor", async ({
    page,
    playground,
  }) => {
    await playground.replaceEditorText(
      playground.bicepEditor,
      "output unchanged int = 42",
    );
    await page.getByRole("tab", { name: "Console", exact: true }).click();
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    const output = page.getByRole("log", { name: "Console output" });
    await expect(input).toBeFocused();

    async function run(command: string) {
      await replaceConsoleInput(page, command);
      await input.press("Enter");
      await expect(
        page.getByRole("button", { name: "Evaluating..." }),
      ).toHaveCount(0);
    }

    await run("var greeting = 'hello'");
    await run("toUpper(greeting)");
    await expect(output).toContainText('"HELLO"');
    await run("var greeting = missingName");
    await expect(output).toContainText("BCP057");
    await run("greeting");
    await expect(output.locator(".console-entry").last()).toContainText(
      '"hello"',
    );
    await run("1 / 0");
    await expect(output.locator(".console-entry").last()).toContainText(
      "Error",
    );
    await run("2 + 3");
    await expect(output.locator(".console-entry").last()).toContainText("5");
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"value": 42');
  });

  test("supports multiline input, history, clear, switching tabs, and reset", async ({
    page,
    playground,
  }) => {
    void playground;
    await page.getByRole("tab", { name: "Console", exact: true }).click();
    const input = page.getByRole("textbox", { name: "Bicep console input" });
    const output = page.getByRole("log", { name: "Console output" });
    await replaceConsoleInput(page, "var values = [");
    await input.press("Shift+Enter");
    await input.pressSequentially("  10");
    await input.press("Shift+Enter");
    await input.pressSequentially("]");
    await expect(output.locator(".console-entry")).toHaveCount(0);
    await input.press("Enter");
    await expect(
      page.getByRole("button", { name: "Evaluating..." }),
    ).toHaveCount(0);
    await replaceConsoleInput(page, "values[0]");
    await page.getByRole("button", { name: "Run", exact: true }).click();
    await expect(output.locator(".console-entry").last()).toContainText("10");
    await replaceConsoleInput(page, "draft");
    await input.press("ArrowUp");
    await expectConsoleInput(page, "values[0]");
    await input.press("ArrowDown");
    await expectConsoleInput(page, "draft");
    await page.getByRole("button", { name: "Clear output" }).click();
    await expect(output.locator(".console-entry")).toHaveCount(0);
    await page.getByRole("tab", { name: "Editor", exact: true }).click();
    await expect(input).toBeHidden();
    await page.getByRole("tab", { name: "Console", exact: true }).click();
    await expectConsoleInput(page, "draft");
    await replaceConsoleInput(page, "values[0]");
    await input.press("Enter");
    await expect(output).toContainText("10");
    await page.getByRole("button", { name: "Reset session" }).click();
    await expect(output.locator(".console-entry")).toHaveCount(0);
    await replaceConsoleInput(page, "values");
    await input.press("Enter");
    await expect(output).toContainText("BCP057");
  });

  test("is usable on a narrow viewport", async ({ page, playground }) => {
    void playground;
    await page.setViewportSize({ width: 375, height: 812 });
    await page.getByRole("tab", { name: "Console", exact: true }).click();
    await replaceConsoleInput(page, "toUpper('mobile')");
    await page.getByRole("button", { name: "Run", exact: true }).click();
    await expect(
      page.getByRole("log", { name: "Console output" }),
    ).toContainText('"MOBILE"');
    expect(
      await page.evaluate(() => document.documentElement.scrollWidth),
    ).toBeLessThanOrEqual(375);
  });
});
