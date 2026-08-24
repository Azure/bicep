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

test.describe("quickstarts", () => {
  test("loads a selected template", async ({ playground }) => {
    await playground.selectQuickstart("canonical/anbox/main.bicep");

    await expect.poll(() => playground.readEditorText(playground.bicepEditor))
      .toContain(`@description('Add a dedicated disk for the LXD storage pool')
param addDedicatedDataDiskForLXD bool = true`);
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
    const cursor = page.locator(".playground-editorpane .cursor").first();
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
      page.getByRole("status", { name: "Compiling ARM template..." }),
    ).toBeVisible();
    await expect(playground.armTemplate.locator("..")).toHaveAttribute(
      "aria-busy",
      "true",
    );
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
      page.getByRole("status", { name: "Compiling ARM template..." }),
    ).toBeHidden();
    await expect(playground.armTemplate.locator("..")).toHaveAttribute(
      "aria-busy",
      "false",
    );
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

  test("preserves source when decompilation fails", async ({
    page,
    playground,
  }) => {
    const bicep = "param preservedContent string = 'still here'";
    await playground.replaceEditorText(playground.bicepEditor, bicep);

    await page.getByLabel("Upload ARM template").setInputFiles({
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
