// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { loadSampleGraph, nodeCount, openVisualDesigner } from "./fixtures";

test.describe("resource creation", () => {
  test("hides the Resource Palette when the experimental setting is disabled", async ({ page }) => {
    await page.goto("/?resourceCreation=false");
    await expect(page.getByTestId("app-root")).toBeVisible();
    await expect(page.getByRole("button", { name: "Add Resources" })).toHaveCount(0);
  });

  test("opens the Resource Palette and keeps zoomed preview and pending nodes aligned", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
    await page.waitForTimeout(1_000);
    await page.getByTestId("control-zoom-in").click();

    await page.getByRole("button", { name: "Add Resources" }).click();
    await expect(page.getByRole("complementary")).toBeVisible();
    await page.getByText("Microsoft.Storage", { exact: true }).click();

    const resourceType = page.getByText("storageAccounts", { exact: true });
    const resourceBox = await resourceType.boundingBox();
    const canvasBox = await page.getByTestId("graph-canvas").boundingBox();
    expect(resourceBox).not.toBeNull();
    expect(canvasBox).not.toBeNull();

    const initialCount = await nodeCount(page);
    const dropPoint = {
      x: canvasBox!.x + canvasBox!.width * 0.75,
      y: canvasBox!.y + canvasBox!.height * 0.7,
    };

    await page.mouse.move(resourceBox!.x + resourceBox!.width / 2, resourceBox!.y + resourceBox!.height / 2);
    await page.mouse.down();
    await expect(page.getByText("storageAccounts", { exact: true }).last()).toBeVisible();
    await page.mouse.move(dropPoint.x, dropPoint.y, { steps: 10 });
    const previewBox = await page.getByTestId("palette-drag-preview-card").boundingBox();
    expect(previewBox).not.toBeNull();
    expect(previewBox!.width).toBeGreaterThan(140);
    await expect(page.getByTestId("palette-drag-preview-card")).toHaveCSS("opacity", "1");
    await expect(page.getByTestId("palette-drag-preview-card").getByText("2025-01-01")).toHaveCount(0);
    expect(previewBox!.x + previewBox!.width / 2).toBeCloseTo(dropPoint.x, 0);
    expect(previewBox!.y + previewBox!.height / 2).toBeCloseTo(dropPoint.y, 0);
    await page.mouse.up();

    const pendingNode = page.getByTestId("pending-resource-node");
    await expect(pendingNode).toBeVisible();
    const pendingBox = await page.getByTestId("pending-resource-card").boundingBox();
    expect(pendingBox).not.toBeNull();
    await expect(page.getByTestId("pending-resource-card")).toHaveCSS("opacity", "1");
    await expect(page.getByTestId("pending-resource-card").getByText("2025-01-01")).toHaveCount(0);
    expect(pendingBox!.x + pendingBox!.width / 2).toBeCloseTo(dropPoint.x, 0);
    expect(pendingBox!.y + pendingBox!.height / 2).toBeCloseTo(dropPoint.y, 0);
    expect(pendingBox!.width).toBeCloseTo(previewBox!.width, 0);
    expect(pendingBox!.height).toBeCloseTo(previewBox!.height, 0);

    await expect(page.getByTestId("graph-node")).toHaveCount(initialCount + 1);
    await expect(pendingNode).toHaveCount(0);
  });

  test("collapses the Resource Palette without changing canvas size", async ({ page }) => {
    await openVisualDesigner(page);
    const canvasBefore = await page.getByTestId("graph-canvas").boundingBox();

    await page.getByRole("button", { name: "Add Resources" }).click();
    await page.getByRole("button", { name: "Close Resource Palette" }).click();

    const canvasAfter = await page.getByTestId("graph-canvas").boundingBox();
    expect(canvasAfter).toEqual(canvasBefore);
  });

  test("searches all resource namespaces without expanding them first", async ({ page }) => {
    await openVisualDesigner(page);
    await page.getByRole("button", { name: "Add Resources" }).click();

    const filter = page.getByRole("textbox", { name: "Filter resource types" });
    await filter.fill("storageAccounts");
    const progress = page.getByTestId("resource-palette-progress");
    await expect(progress).toBeVisible();
    const progressAnimationName = await progress.evaluate(
      (element) => getComputedStyle(element.shadowRoot!.querySelector(".indicator")!).animationName,
    );
    expect(progressAnimationName).not.toBe("none");
    const initialProgressLeft = await progress.evaluate(
      (element) => element.shadowRoot!.querySelector(".indicator")!.getBoundingClientRect().left,
    );
    await page.waitForTimeout(120);
    const nextProgressLeft = await progress.evaluate(
      (element) => element.shadowRoot!.querySelector(".indicator")!.getBoundingClientRect().left,
    );
    expect(Math.abs(nextProgressLeft - initialProgressLeft)).toBeGreaterThan(1);
    await expect(page.getByRole("button", { name: /storageAccounts/ })).toBeVisible();
    await expect(page.locator("mark").filter({ hasText: "storageAccounts" })).toBeVisible();
    await expect(page.getByRole("button", { name: /Microsoft\.Storage/ })).toHaveAttribute("aria-expanded", "true");

    await filter.fill("virtualNetworks");
    await expect(page.getByRole("button", { name: /virtualNetworks/ })).toBeVisible();
    await expect(page.getByTestId("resource-palette-progress")).toHaveCount(0);
  });

  test("does not create a resource when dropped over the Resource Palette", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
    const initialCount = await nodeCount(page);

    await page.getByRole("button", { name: "Add Resources" }).click();
    await page.getByText("Microsoft.Storage", { exact: true }).click();

    const resourceBox = await page.getByText("storageAccounts", { exact: true }).boundingBox();
    const paletteBox = await page.getByRole("complementary").boundingBox();
    expect(resourceBox).not.toBeNull();
    expect(paletteBox).not.toBeNull();

    await page.mouse.move(resourceBox!.x + resourceBox!.width / 2, resourceBox!.y + resourceBox!.height / 2);
    await page.mouse.down();
    await page.mouse.move(paletteBox!.x + paletteBox!.width / 2, paletteBox!.y + 20);
    await page.mouse.up();
    await page.waitForTimeout(400);

    await expect(page.getByTestId("graph-node")).toHaveCount(initialCount);
    await expect(page.getByTestId("pending-resource-node")).toHaveCount(0);
  });

  test("places a keyboard-activated resource at the canvas center", async ({ page }) => {
    await openVisualDesigner(page);
    const initialCount = await nodeCount(page);
    const canvasBox = await page.getByTestId("graph-canvas").boundingBox();
    expect(canvasBox).not.toBeNull();

    await page.getByRole("button", { name: "Add Resources" }).click();
    await page.getByText("Microsoft.Storage", { exact: true }).click();
    const resourceButton = page.getByRole("button", { name: /storageAccounts/ });
    await resourceButton.focus();
    await resourceButton.press("Enter");

    await expect(page.getByTestId("graph-node")).toHaveCount(initialCount + 1);
    const createdBox = await page.locator('[data-node-id="storageAccount"]').boundingBox();
    expect(createdBox).not.toBeNull();
    expect(createdBox!.x + createdBox!.width / 2).toBeCloseTo(canvasBox!.x + canvasBox!.width / 2, 0);
    expect(createdBox!.y + createdBox!.height / 2).toBeCloseTo(canvasBox!.y + canvasBox!.height / 2, 0);
  });
});
