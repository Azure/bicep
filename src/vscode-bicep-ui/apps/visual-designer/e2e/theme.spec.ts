// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

const CURATED_DARK_CANVAS = "rgb(26, 26, 26)";
const CURATED_DARK_DOCK = "rgba(38, 38, 38, 0.92)";
const MONOKAI_CANVAS = "rgb(39, 40, 34)";
// Floating chrome shares Monokai's derived card surface.
const MONOKAI_DOCK = "rgba(49, 51, 44, 0.92)";
const SOLARIZED_DARK_CANVAS = "rgb(0, 43, 54)";

/** Starts the dev playground on one of its VS Code color themes. */
async function usePlaygroundTheme(page: Page, themeId: string) {
  await page.addInitScript((id) => localStorage.setItem("vscode-playground:theme", id), themeId);
}

/** Switches the dev playground's color theme the way a user switches VS Code's. Its toolbar may be collapsed. */
async function switchPlaygroundTheme(page: Page, themeId: string) {
  await page.locator("vscode-theme-selector #theme-selector").evaluate((select, id) => {
    (select as HTMLSelectElement).value = id;
    select.dispatchEvent(new Event("change"));
  }, themeId);
}

function canvasBackground(page: Page) {
  return page.locator("#root");
}

test.describe("Color theme matching", () => {
  test("uses the curated palette for the theme kind by default", async ({ page }) => {
    await usePlaygroundTheme(page, "dark-monokai");
    await openVisualDesigner(page);

    await expect(page.locator("body")).toHaveAttribute("data-vscode-theme-id", "Monokai");
    await expect(canvasBackground(page)).toHaveCSS("background-color", CURATED_DARK_CANVAS);
    await expect(page.getByTestId("creation-dock")).toHaveCSS("background-color", CURATED_DARK_DOCK);
  });

  test("follows the active color theme, including between themes of the same kind", async ({ page }) => {
    await usePlaygroundTheme(page, "dark-monokai");
    await openVisualDesigner(page, { matchColorTheme: "true" });

    await expect(canvasBackground(page)).toHaveCSS("background-color", MONOKAI_CANVAS);
    await expect(page.getByTestId("creation-dock")).toHaveCSS("background-color", MONOKAI_DOCK);

    await switchPlaygroundTheme(page, "dark-solarized");
    await expect(page.locator("body")).toHaveAttribute("data-vscode-theme-kind", "vscode-dark");
    await expect(canvasBackground(page)).toHaveCSS("background-color", SOLARIZED_DARK_CANVAS);
  });

  test("turning the setting on and off switches without reloading", async ({ page }) => {
    await usePlaygroundTheme(page, "dark-monokai");
    await openVisualDesigner(page);
    const setting = page.getByRole("checkbox", { name: "Match color theme" });

    await setting.check();
    await expect(canvasBackground(page)).toHaveCSS("background-color", MONOKAI_CANVAS);

    await setting.uncheck();
    await expect(canvasBackground(page)).toHaveCSS("background-color", CURATED_DARK_CANVAS);
  });

  test("Current export captures the matched colors and explicit themes stay curated", async ({ page }) => {
    await usePlaygroundTheme(page, "dark-monokai");
    await openVisualDesigner(page, { matchColorTheme: "true" });
    await loadSampleGraph(page, "flat");
    await waitForStableNodePosition(page, "pip");
    await page.evaluate(() => {
      const output = window as Window & { capturedExport?: Blob };
      Object.defineProperty(window, "showSaveFilePicker", {
        configurable: true,
        value: async () => ({
          createWritable: async () => ({
            write: async (blob: Blob) => {
              output.capturedExport = blob;
            },
            close: async () => {},
          }),
        }),
      });
    });

    await page.getByTestId("control-export").click();
    const toolbar = page.getByRole("toolbar", { name: "Export settings" });
    await toolbar.getByRole("combobox").first().click();
    await page.getByRole("option", { name: "Solid" }).click();
    const cover = page.locator("[data-export-background]");
    await expect(cover).toHaveCSS("background-color", MONOKAI_CANVAS);

    await toolbar.getByRole("button", { name: "Export", exact: true }).click();
    await expect
      .poll(() => page.evaluate(() => Boolean((window as Window & { capturedExport?: Blob }).capturedExport)))
      .toBe(true);
    const corner = await page.evaluate(async () => {
      const blob = (window as Window & { capturedExport?: Blob }).capturedExport;
      if (!blob) throw new Error("Exported image not found");
      const bitmap = await createImageBitmap(blob);
      const canvas = document.createElement("canvas");
      canvas.width = bitmap.width;
      canvas.height = bitmap.height;
      const context = canvas.getContext("2d");
      if (!context) throw new Error("Could not inspect exported image");
      context.drawImage(bitmap, 0, 0);
      return [...context.getImageData(1, 1, 1, 1).data];
    });
    // Encoding can round a channel by one.
    [39, 40, 34, 255].forEach((channel, index) =>
      expect(Math.abs((corner[index] ?? -10) - channel)).toBeLessThanOrEqual(2),
    );

    await toolbar.getByRole("combobox").nth(1).click();
    await page.getByRole("option", { name: "Dark", exact: true }).click();
    await expect(cover).toHaveCSS("background-color", CURATED_DARK_CANVAS);
  });
});
