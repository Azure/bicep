// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  createPlayground,
  expect,
  getCompileRequestCount,
  installCompilerRequestTracker,
  openPlayground,
  test,
  withoutGeneratorField,
} from "./fixtures/playground";
import { getBicepVersionLink } from "../../src/components/version-link";

const storageBicep = `param storageName string
param location string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
    name: storageName
    location: location
    kind: 'StorageV2'
    sku: {
        name: 'Standard_LRS'
    }
    properties: {
        accessTier: 'Hot'
        supportsHttpsTrafficOnly: true
        minimumTlsVersion: 'TLS1_2'
        allowBlobPublicAccess: true
    }
}`;

const expectedStorageTemplate = `{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "0.39.78.63741",
      "templateHash": "9724347989709413195"
    }
  },
  "parameters": {
    "storageName": {
      "type": "string"
    },
    "location": {
      "type": "string"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2021-02-01",
      "name": "[parameters('storageName')]",
      "location": "[parameters('location')]",
      "kind": "StorageV2",
      "sku": {
        "name": "Standard_LRS"
      },
      "properties": {
        "accessTier": "Hot",
        "supportsHttpsTrafficOnly": true,
        "minimumTlsVersion": "TLS1_2",
        "allowBlobPublicAccess": true
      }
    }
  ]
}`;

test.describe("version links", () => {
  test("links releases to tags", () => {
    expect(getBicepVersionLink("0.46.0")).toEqual({
      ariaLabel: "Bicep 0.46.0 release notes (opens in a new tab)",
      href: "https://github.com/Azure/bicep/releases/tag/v0.46.0",
      label: "Bicep 0.46.0",
    });
  });

  test("links development versions to commits", () => {
    expect(getBicepVersionLink("0.46.58-g82f38dea9d")).toEqual({
      ariaLabel: "Bicep 0.46.58-g82f38dea9d source commit (opens in a new tab)",
      href: "https://github.com/Azure/bicep/commit/82f38dea9d",
      label: "Bicep 0.46.58-g82f38dea9d",
    });
  });

  test("does not link placeholders or unrecognized versions", () => {
    expect(getBicepVersionLink("0.0.0-placeholder")).toEqual({
      label: "Bicep development",
    });
    expect(getBicepVersionLink("custom-build")).toEqual({
      label: "Bicep custom-build",
    });
  });
});

test.describe("quickstarts", () => {
  test("loads a selected template", async ({ playground }) => {
    await playground.selectQuickstart("canonical/anbox/main.bicep");

    await expect.poll(() => playground.readEditorText(playground.bicepEditor))
      .toContain(`@description('Add a dedicated disk for the LXD storage pool')
param addDedicatedDataDiskForLXD bool = true`);
    await expect(playground.sampleTemplate).toHaveValue(
      "canonical/anbox/main.bicep",
    );
  });

  test("reloads the selected template after edits", async ({
    page,
    playground,
  }) => {
    await playground.selectQuickstart("canonical/anbox/main.bicep");
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toContain("param addDedicatedDataDiskForLXD bool = true");

    await playground.replaceEditorText(
      playground.bicepEditor,
      "param replacement string",
    );
    await page.route(
      "**/quickstarts/canonical/anbox/main.bicep",
      async (route) => {
        await new Promise((resolve) => setTimeout(resolve, 500));
        await route.continue();
      },
    );
    const reloadSample = page.getByRole("button", {
      name: "Reload selected sample",
    });
    await reloadSample.click();

    await expect(reloadSample).toBeFocused();
    await expect(playground.sampleTemplate).toBeEnabled();
    await expect(page.getByRole("button", { name: "Decompile" })).toBeEnabled();
    await expect(playground.bicepEditor).toBeVisible();
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toContain("param replacement string");
    await expect(page.getByText("Loading sample template...")).toHaveCount(0);
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toContain("param addDedicatedDataDiskForLXD bool = true");
    await expect(playground.sampleTemplate).toHaveValue(
      "canonical/anbox/main.bicep",
    );
    await expect(playground.sampleTemplate).not.toBeFocused();
  });

  test("compiles a template with local modules", async ({ playground }) => {
    await playground.selectQuickstart(
      "microsoft.desktopvirtualization/azure-virtual-desktop-with-fslogix/main.bicep",
    );

    await expect
      .poll(async () =>
        withoutGeneratorField(
          await playground.readEditorText(playground.armTemplate),
        ),
      )
      .toContain('"Microsoft.DesktopVirtualization/applicationGroups"');
  });

  test("preserves editor state when a download fails", async ({
    page,
    playground,
  }) => {
    const bicep = "param preservedContent string = 'still here'";
    await playground.replaceEditorText(playground.bicepEditor, bicep);
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"defaultValue": "still here"');

    const armContent = await playground.readEditorText(playground.armTemplate);
    const cursor = page.locator('[data-pane="bicep"] .cursor').first();
    await playground.bicepEditor.click();
    await page.keyboard.press("ControlOrMeta+Home");
    await page.keyboard.press("ArrowRight");
    await page.keyboard.press("ArrowRight");
    await page.keyboard.press("ArrowRight");
    await page.keyboard.press("ArrowRight");
    await page.keyboard.press("ArrowRight");
    await page.keyboard.press("ArrowRight");
    const cursorPosition = await cursor.evaluate((element) => {
      const style = window.getComputedStyle(element);
      return { left: style.left, top: style.top };
    });

    await page.route("https://raw.githubusercontent.com/**", (route) =>
      route.fulfill({ status: 503, body: "Sample unavailable" }),
    );
    await playground.selectQuickstart("canonical/anbox/main.bicep");

    await expect(playground.error).toContainText("could not be loaded");
    await expect(playground.sampleTemplate).toBeFocused();
    await expect
      .poll(() =>
        cursor.evaluate((element) => {
          const style = window.getComputedStyle(element);
          return { left: style.left, top: style.top };
        }),
      )
      .toEqual(cursorPosition);
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toBe(armContent);
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toBe(bicep);
  });
});

test.describe("sharing", () => {
  test("restores source from a copied link", async ({ page, playground }) => {
    await playground.replaceEditorText(playground.bicepEditor, storageBicep);

    await playground.copyLink.click();
    const sharedUrl = await page.evaluate(() => navigator.clipboard.readText());
    await page.goto(sharedUrl);

    const reloadedPlayground = createPlayground(page);
    await expect
      .poll(() =>
        reloadedPlayground.readEditorText(reloadedPlayground.bicepEditor),
      )
      .toBe(storageBicep);
  });

  test("shows clipboard failures", async ({ page, playground }) => {
    await page.evaluate(() => {
      Object.defineProperty(navigator.clipboard, "writeText", {
        configurable: true,
        value: () => Promise.reject(new Error("Clipboard permission denied")),
      });
    });

    await playground.copyLink.click();

    await expect(playground.error).toContainText("Clipboard permission denied");
  });

  test("keeps source and shared-link fragments out of telemetry", async ({
    page,
  }) => {
    const telemetryPayloads: string[] = [];
    await page.route(
      "https://dc.services.visualstudio.com/**",
      async (route) => {
        telemetryPayloads.push(route.request().postData() ?? "");
        await route.fulfill({
          status: 200,
          contentType: "application/json",
          body: "{}",
        });
      },
    );

    const playground = await openPlayground(page);
    const privateMarker = "private-playground-content-marker";
    await playground.replaceEditorText(
      playground.bicepEditor,
      `param secret string = '${privateMarker}'`,
    );
    await playground.copyLink.click();
    const sharedUrl = await page.evaluate(() => navigator.clipboard.readText());

    await page.goto(sharedUrl);
    await expect(
      page.getByRole("region", { name: "Bicep editor" }),
    ).toBeVisible();
    await page.goto("about:blank");
    await expect
      .poll(() => telemetryPayloads.length, {
        timeout: 20_000,
      })
      .toBeGreaterThan(0);

    const serializedTelemetry = telemetryPayloads.join("\n");
    expect(serializedTelemetry).not.toContain(privateMarker);
    expect(serializedTelemetry).not.toContain(new URL(sharedUrl).hash);

    const telemetryItems = telemetryPayloads.flatMap(parseTelemetryPayload);
    const telemetryUrls = telemetryItems.flatMap(findTelemetryUrls);
    expect(telemetryUrls.length).toBeGreaterThan(0);
    for (const telemetryUrl of telemetryUrls) {
      const url = new URL(telemetryUrl);
      expect(url.search).toBe("");
      expect(url.hash).toBe("");
    }
  });
});

test.describe("compiler lifecycle", () => {
  test("shows startup failure and recovers on retry", async ({ page }) => {
    const compilerScript = "**/_framework/dotnet.js";
    await page.route(compilerScript, (route) => route.abort());
    await page.goto("/");

    await expect(
      page.getByRole("heading", { name: "Bicep Playground could not start" }),
    ).toBeVisible();

    await page.unroute(compilerScript);
    await page.getByRole("button", { name: "Retry" }).click();

    await expect(
      page.getByRole("region", { name: "Bicep editor" }),
    ).toBeVisible();
  });

  test("keeps the main thread responsive during compilation", async ({
    page,
    playground,
  }) => {
    await page.evaluate(() => {
      let heartbeat = 0;
      let lastHeartbeat = performance.now();
      let maximumHeartbeatGap = 0;
      const heartbeatHandle = window.setInterval(() => {
        const now = performance.now();
        maximumHeartbeatGap = Math.max(
          maximumHeartbeatGap,
          now - lastHeartbeat,
        );
        lastHeartbeat = now;
        ++heartbeat;
      }, 10);

      Object.assign(window, {
        __playgroundHeartbeat: () => heartbeat,
        __playgroundMaximumHeartbeatGap: () => maximumHeartbeatGap,
        __playgroundStopHeartbeat: () => window.clearInterval(heartbeatHandle),
      });
    });

    await playground.selectQuickstart(
      "microsoft.desktopvirtualization/azure-virtual-desktop-with-fslogix/main.bicep",
    );
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"Microsoft.DesktopVirtualization/applicationGroups"');

    const responsiveness = await page.evaluate(() => {
      const playgroundWindow = window as typeof window & {
        __playgroundHeartbeat(): number;
        __playgroundMaximumHeartbeatGap(): number;
        __playgroundStopHeartbeat(): void;
      };
      playgroundWindow.__playgroundStopHeartbeat();

      return {
        heartbeat: playgroundWindow.__playgroundHeartbeat(),
        maximumGap: playgroundWindow.__playgroundMaximumHeartbeatGap(),
      };
    });
    expect(responsiveness.heartbeat).toBeGreaterThan(5);
    expect(responsiveness.maximumGap).toBeLessThan(250);
  });

  test("does not let stale compilation overwrite newer source", async ({
    page,
  }) => {
    await installCompilerRequestTracker(page);
    const playground = await openPlayground(page);
    const slowSource = Array.from(
      { length: 1_000 },
      (_, index) => `var value${index} = ${index}`,
    ).join("\n");

    await playground.replaceEditorText(playground.bicepEditor, slowSource);
    await expect.poll(() => getCompileRequestCount(page)).toBeGreaterThan(0);
    await expect(
      playground.armPane.getByText("Compiling", { exact: true }),
    ).toBeVisible();
    await expect(
      playground.armPane.getByText("Compiling ARM template...", {
        exact: true,
      }),
    ).toBeVisible();
    await expect(playground.armPane).toHaveAttribute("aria-busy", "true");
    await playground.replaceEditorText(
      playground.bicepEditor,
      "param result string = 'latest'",
    );

    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"defaultValue": "latest"');
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate), {
        intervals: [1_000],
        timeout: 2_000,
      })
      .toContain('"defaultValue": "latest"');
    await expect(
      playground.armPane.getByText("Compiling", { exact: true }),
    ).toBeHidden();
    await expect(playground.armPane).toHaveAttribute("aria-busy", "false");
  });

  test("keeps editing and toolbar interactions responsive during compilation", async ({
    page,
  }) => {
    await installCompilerRequestTracker(page);
    const playground = await openPlayground(page);
    const slowSource = Array.from(
      { length: 1_000 },
      (_, index) => `var value${index} = ${index}`,
    ).join("\n");
    await playground.replaceEditorText(playground.bicepEditor, slowSource);
    await expect.poll(() => getCompileRequestCount(page)).toBeGreaterThan(0);

    await playground.bicepEditor.click();
    await page.keyboard.press("ControlOrMeta+End");
    await page.keyboard.insertText("\nvar responsiveMarker = true");
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor), {
        timeout: 1_000,
      })
      .toContain("var responsiveMarker = true");

    const startedAt = await page.evaluate(() => performance.now());
    await playground.copyLink.click();
    await expect(page.getByRole("button", { name: "Copied" })).toBeVisible({
      timeout: 1_000,
    });
    await expect(page.getByRole("button", { name: "Copied" })).toBeFocused();
    const elapsed = await page.evaluate(
      (start) => performance.now() - start,
      startedAt,
    );

    expect(elapsed).toBeLessThan(1_000);
  });
});

test.describe("editing", () => {
  test("compiles Bicep to the expected ARM template", async ({
    playground,
  }) => {
    await playground.replaceEditorText(playground.bicepEditor, storageBicep);

    await expect
      .poll(async () =>
        withoutGeneratorField(
          await playground.readEditorText(playground.armTemplate),
        ),
      )
      .toBe(withoutGeneratorField(expectedStorageTemplate));
  });

  test("decompiles an ARM template into Bicep", async ({
    page,
    playground,
  }) => {
    await page.getByLabel("ARM template JSON file").setInputFiles({
      name: "storage.json",
      mimeType: "application/json",
      buffer: Buffer.from(expectedStorageTemplate),
    });

    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toContain(
        "resource storage 'Microsoft.Storage/storageAccounts@2021-02-01'",
      );
  });

  test("preserves source when decompilation fails", async ({
    page,
    playground,
  }) => {
    const bicep = "param preservedContent string = 'still here'";
    await playground.replaceEditorText(playground.bicepEditor, bicep);

    await page.getByLabel("ARM template JSON file").setInputFiles({
      name: "invalid.json",
      mimeType: "application/json",
      buffer: Buffer.from("not valid JSON"),
    });

    await expect(playground.error).toContainText("Unexpected character");
    await expect
      .poll(() => playground.readEditorText(playground.bicepEditor))
      .toBe(bicep);
  });
});

test.describe("native interface", () => {
  test("exposes semantic landmarks and native document actions", async ({
    page,
    playground,
  }) => {
    await expect(page.getByRole("banner")).toContainText("Bicep Playground");
    await expect(
      page.getByRole("navigation", { name: "Playground actions" }),
    ).toBeVisible();
    await expect(
      page.getByRole("main", { name: "Bicep compilation workspace" }),
    ).toBeVisible();
    await expect(playground.bicepPane).toBeVisible();
    await expect(playground.armPane).toBeVisible();
    await expect(playground.sampleTemplate).toHaveJSProperty(
      "tagName",
      "SELECT",
    );
    await expect(page.getByRole("button", { name: "Decompile" })).toBeVisible();
    await expect(
      page.getByRole("link", {
        name: "Bicep repository on GitHub (opens in a new tab)",
      }),
    ).toHaveAttribute("href", "https://github.com/Azure/bicep");
  });

  test("switches between editor tabs without page overflow on narrow screens", async ({
    page,
    playground,
  }) => {
    await page.setViewportSize({ width: 320, height: 700 });

    await expect(playground.bicepPane).toBeVisible();
    await expect(playground.armPane).toBeHidden();
    const bicepTab = page.getByRole("tab", { name: "Bicep" });
    const armTab = page.getByRole("tab", { name: "ARM template" });
    await expect(bicepTab).toHaveAttribute("tabindex", "0");
    await expect(armTab).toHaveAttribute("tabindex", "-1");
    await bicepTab.focus();
    await page.keyboard.press("ArrowRight");
    await expect(armTab).toBeFocused();
    await expect(armTab).toHaveAttribute("aria-selected", "true");
    await expect(playground.armPane).toBeVisible();
    await expect(playground.bicepPane).toBeHidden();
    await page.keyboard.press("Home");
    await expect(bicepTab).toBeFocused();
    await expect(bicepTab).toHaveAttribute("aria-selected", "true");

    const dimensions = await page.evaluate(() => ({
      viewportWidth: document.documentElement.clientWidth,
      documentWidth: document.documentElement.scrollWidth,
    }));
    expect(dimensions.documentWidth).toBe(dimensions.viewportWidth);
  });

  test("shows, dismisses, and reopens compilation problems", async ({
    page,
    playground,
  }) => {
    await playground.replaceEditorText(
      playground.bicepEditor,
      "this is not valid bicep",
    );

    const problems = page.getByRole("region", { name: "Problems" });
    await expect(problems).toBeVisible();
    await expect(problems).toHaveCSS("min-height", "140px");
    await expect(page.getByRole("main")).toHaveCSS("padding-bottom", "0px");
    await expect(problems).toHaveCSS("margin-bottom", "4px");
    await expect(problems.locator(".problem").first()).toContainText("BCP");
    await problems.getByRole("button", { name: "Close Problems" }).click();
    await expect(problems).toBeHidden();
    await expect(page.getByRole("main")).toHaveCSS("padding-bottom", "4px");

    await page.getByRole("button", { name: /Compilation failed/ }).click();
    await expect(problems).toBeVisible();
    await problems.locator(".problem").first().click();
    await expect
      .poll(() =>
        page.evaluate(() =>
          Boolean(
            document.activeElement?.closest(
              '[role="region"][aria-label="Bicep editor"]',
            ),
          ),
        ),
      )
      .toBe(true);
  });

  test("copies and downloads only current ARM output", async ({
    page,
    playground,
  }) => {
    await playground.replaceEditorText(
      playground.bicepEditor,
      "param current string = 'yes'",
    );
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"defaultValue": "yes"');

    const copyArm = page.getByRole("button", { name: "Copy ARM template" });
    const downloadArm = page.getByRole("button", {
      name: "Download ARM template",
    });
    await expect(copyArm).toBeEnabled();
    await expect(downloadArm).toBeEnabled();

    await copyArm.click();
    await expect
      .poll(() => page.evaluate(() => navigator.clipboard.readText()))
      .toContain('"defaultValue": "yes"');

    const downloadPromise = page.waitForEvent("download");
    await downloadArm.click();
    expect((await downloadPromise).suggestedFilename()).toBe("main.json");

    await playground.replaceEditorText(
      playground.bicepEditor,
      "this is not valid bicep",
    );
    await expect(copyArm).toBeDisabled();
    await expect(downloadArm).toBeDisabled();
    const staleEditorSurface = playground.armPane.locator(".editor-surface");
    await expect(staleEditorSurface).toHaveCSS("opacity", "1");
    await expect
      .poll(() =>
        staleEditorSurface.evaluate(
          (element) =>
            getComputedStyle(element, "::after").backgroundColor !==
            "rgba(0, 0, 0, 0)",
        ),
      )
      .toBe(true);
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"defaultValue": "yes"');
  });

  test("switches the application and Monaco themes", async ({
    page,
    playground,
  }) => {
    const themeButton = page.getByRole("button", {
      name: "Switch to dark theme",
    });
    await themeButton.click();

    await expect(page.locator("html")).toHaveAttribute(
      "data-color-mode",
      "dark",
    );
    await expect(
      page.getByRole("button", { name: "Switch to light theme" }),
    ).toBeVisible();
    await expect(playground.bicepEditor.locator(".monaco-editor")).toHaveClass(
      /vs-dark/,
    );
  });
});

function parseTelemetryPayload(payload: string): unknown[] {
  const parsed = JSON.parse(payload) as unknown;
  return Array.isArray(parsed) ? parsed : [parsed];
}

function findTelemetryUrls(value: unknown): string[] {
  if (typeof value !== "object" || value === null) {
    return [];
  }

  const urls: string[] = [];
  for (const [key, child] of Object.entries(value)) {
    if (
      (key === "refUri" || key === "uri" || key === "url") &&
      typeof child === "string"
    ) {
      urls.push(child);
    } else {
      urls.push(...findTelemetryUrls(child));
    }
  }

  return urls;
}
