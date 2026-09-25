// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Locator, Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import {
  getGraphTransform,
  loadSampleGraph,
  nodeCount,
  openVisualDesigner,
  waitForStableNodePosition,
} from "./fixtures";

const STORAGE_TYPE = "Microsoft.Storage/storageAccounts";
const THEME_FOCUS_BORDERS = ["rgb(0, 120, 212)", "rgb(77, 166, 255)", "rgb(255, 215, 0)", "rgb(0, 0, 205)"];

async function openStorage(page: Page) {
  await page.getByRole("button", { name: "Add Resources" }).click();
  await page.getByRole("textbox", { name: "Filter resource types" }).fill("storageAccounts");
  const picker = page.getByRole("combobox", { name: `API version for ${STORAGE_TYPE}`, exact: true });
  await expect(picker).toBeVisible();
  return picker;
}

function versionOptions(page: Page) {
  return page.getByRole("listbox").getByRole("option");
}

function versionOption(page: Page, version: string) {
  return page.getByRole("option", { name: new RegExp(`^${version.replace(/[.-]/g, "\\$&")}$`) });
}

async function chooseVersion(page: Page, picker: Locator, version: string) {
  if ((await picker.getAttribute("aria-expanded")) !== "true") {
    await picker.click();
  }
  await versionOption(page, version).click();
  await expect(picker).toHaveText(version);
  await expect(page.getByRole("listbox")).toHaveCount(0);
}

function dragHandle(page: Page, fullyQualifiedType: string) {
  return page.locator(`[data-testid="resource-type-drag-handle"][data-resource-type="${fullyQualifiedType}"]`);
}

async function dragToCanvas(page: Page, fullyQualifiedType: string) {
  const handleBox = (await dragHandle(page, fullyQualifiedType).boundingBox())!;
  const canvasBox = (await page.getByTestId("graph-canvas").boundingBox())!;
  await page.mouse.move(handleBox.x + 40, handleBox.y + handleBox.height / 2);
  await page.mouse.down();
  await page.mouse.move(canvasBox.x + canvasBox.width * 0.85, canvasBox.y + canvasBox.height * 0.5, { steps: 8 });
  await page.mouse.up();
}

async function recordCreations(page: Page) {
  await page.addInitScript(() => {
    const target = window as typeof window & { creations: unknown[] };
    target.creations = [];
    window.addEventListener("dev-resource-create", (event) => {
      target.creations.push((event as CustomEvent).detail);
    });
  });
}

async function creations(page: Page) {
  return page.evaluate(() => (window as typeof window & { creations: unknown[] }).creations);
}

test("dock has keyboard-discoverable disabled tools and does not move the graph", async ({ page }) => {
  await recordCreations(page);
  await openVisualDesigner(page);
  await waitForStableNodePosition(page, "networkInterface");
  const transform = await getGraphTransform(page);
  const canvas = await page.getByTestId("graph-canvas").boundingBox();
  const dockLocator = page.getByTestId("creation-dock");
  const dock = await dockLocator.boundingBox();
  expect(dock!.height).toBe(50);
  expect(dock!.x + dock!.width / 2).toBeCloseTo(canvas!.x + canvas!.width / 2, 0);
  await expect(dockLocator).toHaveCSS("caret-color", "rgba(0, 0, 0, 0)");
  await expect(dockLocator).toHaveCSS("user-select", "none");

  for (const name of ["Modules", "Notes"]) {
    const button = page.getByRole("button", { name: `${name} - coming soon` });
    await expect(button).toHaveAttribute("aria-disabled", "true");
    await button.focus();
    await expect(page.getByRole("tooltip", { name: `${name} - coming soon` })).toBeVisible();
    await button.press("Enter");
    await button.press("Space");
    await expect(page.getByRole("complementary")).toHaveCount(0);
  }
  expect(await creations(page)).toEqual([]);
  const launcher = page.getByRole("button", { name: "Add Resources" });
  await launcher.click();
  const search = page.getByRole("textbox", { name: "Filter resource types" });
  await expect(search).toBeFocused();
  await expect(page.getByRole("complementary")).toBeVisible();
  await expect(page.getByRole("checkbox")).toHaveCount(0);
  await search.press("Escape");
  await expect(page.getByRole("complementary")).toHaveCount(0);
  await expect(launcher).toBeFocused();
  await launcher.click();
  await launcher.click();
  await expect(launcher).toBeFocused();
  expect(await getGraphTransform(page)).toBe(transform);
  expect(await page.getByTestId("graph-canvas").boundingBox()).toEqual(canvas);
  await expect(page.getByTestId("graph-canvas").getByTestId("target-scope")).toHaveCount(0);
});

test("dock groups are evenly inset from the separator and the dock edges", async ({ page }) => {
  await openVisualDesigner(page);
  const dock = (await page.getByTestId("creation-dock").boundingBox())!;
  const resources = (await page.getByRole("button", { name: "Add Resources" }).boundingBox())!;
  const modules = (await page.getByRole("button", { name: "Modules - coming soon" }).boundingBox())!;
  const notes = (await page.getByRole("button", { name: "Notes - coming soon" }).boundingBox())!;
  const separator = (await page.getByTestId("creation-dock-separator").boundingBox())!;

  const leftEdgeInset = resources.x - dock.x;
  const rightEdgeInset = dock.x + dock.width - (notes.x + notes.width);
  expect(rightEdgeInset).toBeCloseTo(leftEdgeInset, 0);
  expect(separator.x - (modules.x + modules.width)).toBeCloseTo(leftEdgeInset, 0);
  expect(notes.x - (separator.x + separator.width)).toBeCloseTo(rightEdgeInset, 0);
});

test("browse shows featured resource providers before other namespaces", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();

  const providers = page
    .getByRole("complementary", { name: "Resource Palette" })
    .getByRole("button", { name: /^Microsoft\./ });
  await expect(providers).toHaveCount(3);
  await expect(providers.nth(0)).toHaveAccessibleName("Microsoft.Network");
  await expect(providers.nth(1)).toHaveAccessibleName("Microsoft.Storage");
  await expect(providers.nth(2)).toHaveAccessibleName("Microsoft.Preview");
});

test("versions load lazily, persist across browse/search/reopen and insert the selected version", async ({ page }) => {
  await recordCreations(page);
  await openVisualDesigner(page);
  const picker = await openStorage(page);
  await expect(picker).toHaveText("2025-01-01");
  await expect(picker).toHaveAttribute("aria-expanded", "false");
  await expect(page.getByRole("listbox")).toHaveCount(0);
  await picker.click();
  await expect(picker).toHaveAttribute("aria-expanded", "true");
  await expect(versionOptions(page)).toHaveCount(3);
  await expect(versionOptions(page).first()).toHaveAccessibleName("2026-02-01-preview");
  await expect(page.getByRole("listbox").getByRole("option", { selected: true })).toHaveAccessibleName("2025-01-01");
  await chooseVersion(page, picker, "2024-01-01");
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);

  const filter = page.getByRole("textbox", { name: "Filter resource types" });
  await filter.fill("");
  await page.getByText("Microsoft.Storage", { exact: true }).click();
  await expect(picker).toHaveText("2024-01-01");
  await filter.fill("virtualNetworks");
  await expect(dragHandle(page, "Microsoft.Network/virtualNetworks")).toBeVisible();
  await filter.fill("storageAccounts");
  await expect(picker).toHaveText("2024-01-01");
  await filter.press("Escape");
  await openStorage(page);
  await expect(picker).toHaveText("2024-01-01");

  await dragToCanvas(page, STORAGE_TYPE);
  await expect.poll(() => creations(page)).toEqual([{ fullyQualifiedType: STORAGE_TYPE, apiVersion: "2024-01-01" }]);
});

test("version pill is quiet at rest, revealed on row hover, and stays emphasized when customized", async ({ page }) => {
  await openVisualDesigner(page);
  const picker = await openStorage(page);
  const transparent = "rgba(0, 0, 0, 0)";
  await page.mouse.move(0, 0);
  await expect(picker).toHaveCSS("border-top-color", transparent);
  await expect(picker).toHaveAttribute("data-customized", "false");

  await dragHandle(page, STORAGE_TYPE).hover();
  await expect(picker).not.toHaveCSS("border-top-color", transparent);

  await chooseVersion(page, picker, "2024-01-01");
  await page.mouse.move(0, 0);
  await page.getByRole("textbox", { name: "Filter resource types" }).focus();
  await expect(picker).toHaveAttribute("data-customized", "true");
  await expect(picker).not.toHaveCSS("border-top-color", transparent);
});

test("version picker is outside the drag target, keyboard operable, and preview version is sent on drop", async ({
  page,
}) => {
  await recordCreations(page);
  await openVisualDesigner(page);
  const picker = await openStorage(page);
  expect(await picker.evaluate((element) => element.closest("button"))).toBeNull();
  await picker.click();
  await expect(versionOptions(page)).toHaveCount(3);
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
  await page.keyboard.press("Escape");
  await expect(page.getByRole("listbox")).toHaveCount(0);
  await expect(page.getByRole("complementary")).toBeVisible();
  await expect(picker).toBeFocused();

  await picker.press("ArrowDown");
  await expect(page.getByRole("listbox")).toBeVisible();
  await page.keyboard.press("Home");
  await expect(picker).toHaveAttribute(
    "aria-activedescendant",
    (await versionOptions(page).first().getAttribute("id"))!,
  );
  await page.keyboard.press("Enter");
  await expect(picker).toHaveText("2026-02-01-preview");
  await expect(page.getByRole("listbox")).toHaveCount(0);
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);

  await picker.press("ArrowDown");
  await page.keyboard.press("Tab");
  await expect(page.getByRole("listbox")).toHaveCount(0);
  await expect(picker).toHaveText("2026-02-01-preview");

  const resourceBox = await dragHandle(page, STORAGE_TYPE).boundingBox();
  const canvasBox = await page.getByTestId("graph-canvas").boundingBox();
  await page.mouse.move(resourceBox!.x + 40, resourceBox!.y + resourceBox!.height / 2);
  await page.mouse.down();
  await page.mouse.move(canvasBox!.x + canvasBox!.width * 0.85, canvasBox!.y + canvasBox!.height * 0.5, { steps: 8 });
  await expect(page.getByTestId("palette-drag-preview-card")).toBeVisible();
  await page.mouse.up();
  await expect
    .poll(() => creations(page))
    .toEqual([{ fullyQualifiedType: STORAGE_TYPE, apiVersion: "2026-02-01-preview" }]);
});

test("version list is not clipped by the palette and closes when the palette scrolls", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  await page.getByRole("textbox", { name: "Filter resource types" }).fill("Microsoft");
  const pickers = page.getByTestId("api-version-picker");
  await expect(pickers).toHaveCount(3);
  const lastPicker = pickers.last();
  await lastPicker.click();
  const popup = page.getByTestId("api-version-popup");
  await expect(versionOptions(page).first()).toBeVisible();
  expect(await popup.evaluate((element) => element.matches(":popover-open"))).toBe(true);
  const popupBox = (await popup.boundingBox())!;
  const pickerBox = (await lastPicker.boundingBox())!;
  expect(popupBox.x + popupBox.width).toBeCloseTo(pickerBox.x + pickerBox.width, 0);

  await page.getByTestId("resource-palette-list").evaluate((element) => element.dispatchEvent(new Event("scroll")));
  await expect(popup).toHaveCount(0);
  await expect(page.getByRole("complementary")).toBeVisible();
});

test("version failures show an explicit retry and loading state", async ({ page }) => {
  await openVisualDesigner(page, { versionsDelay: "600", versionFailures: "1" });
  const picker = await openStorage(page);
  await picker.click();
  await expect(page.getByText("Loading API versions…", { exact: true })).toBeVisible();
  await expect(page.getByText("Failed to load API versions.", { exact: false })).toBeVisible();
  await page.getByRole("button", { name: `Retry API versions for ${STORAGE_TYPE}`, exact: true }).click();
  await expect(page.getByText("Loading API versions…", { exact: true })).toBeVisible();
  await expect(versionOptions(page)).toHaveCount(3);
  await expect(page.getByRole("button", { name: /Retry API versions/ })).toHaveCount(0);
  await expect(picker).toBeFocused();
});

test("the version list opens immediately and shows progress until versions arrive", async ({ page }) => {
  await openVisualDesigner(page, { versionsDelay: "1000" });
  const picker = await openStorage(page);
  await picker.click();
  await expect(picker).toBeFocused();
  await expect(picker).toHaveAttribute("aria-expanded", "true");
  await expect(picker).toHaveAttribute("aria-busy", "true");
  await expect(page.getByRole("status").filter({ hasText: "Loading API versions…" })).toBeVisible();
  const controlsId = (await picker.getAttribute("aria-controls"))!;
  await expect(page.locator(`[id="${controlsId}"][role="listbox"]`)).toHaveCount(1);
  await expect(page.locator(`[id="${controlsId}"]`)).toHaveAttribute("aria-busy", "true");
  await expect(versionOptions(page)).toHaveCount(3);
  await expect(picker).toHaveAttribute("aria-busy", "false");
  await expect(page.getByRole("listbox").getByRole("option", { selected: true })).toHaveAccessibleName("2025-01-01");

  await page.keyboard.press("Home");
  await page.keyboard.press("Enter");
  await expect(picker).toHaveText("2026-02-01-preview");
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
});

test("catalog changes discard saved choices and outstanding version responses", async ({ page }) => {
  await openVisualDesigner(page, { versionsDelay: "1500" });
  let picker = await openStorage(page);
  await chooseVersion(page, picker, "2024-01-01");
  const filter = page.getByRole("textbox", { name: "Filter resource types" });
  await filter.fill("virtualNetworks");
  const network = page.getByRole("combobox", {
    name: "API version for Microsoft.Network/virtualNetworks",
    exact: true,
  });
  await network.click();
  await expect(page.getByText("Loading API versions…", { exact: true })).toBeVisible();
  await page.getByRole("button", { name: "Change catalog", exact: true }).click();
  picker = await openStorage(page);
  await expect(picker).toHaveText("2026-01-01");
  await picker.click();
  await expect(versionOptions(page)).toHaveCount(2);
  await expect(versionOption(page, "2024-01-01")).toHaveCount(0);
  await expect(picker).toHaveText("2026-01-01");
});

test("palette offers only types deployable at the document's target scope, without moving the graph", async ({
  page,
}) => {
  await openVisualDesigner(page);
  await waitForStableNodePosition(page, "networkInterface");
  const transform = await getGraphTransform(page);
  for (const [value, label, icon, expectedTypes] of [
    [
      "resourceGroup",
      "Resource group",
      "Microsoft.Resources/resourceGroups-icon",
      ["Microsoft.Network/virtualNetworks", "Microsoft.Preview/widgets", STORAGE_TYPE],
    ],
    [
      "subscription",
      "Subscription",
      "Microsoft.Subscription/aliases-icon",
      ["Microsoft.Preview/widgets", "Microsoft.Resources/resourceGroups"],
    ],
    [
      "managementGroup",
      "Management group",
      "Microsoft.Management/managementGroups-icon",
      ["Microsoft.Preview/widgets"],
    ],
    ["tenant", "Tenant", "", ["Microsoft.Management/managementGroups", "Microsoft.Preview/widgets"]],
  ] as const) {
    await page.getByRole("combobox", { name: "Document target scope" }).selectOption(value);
    const indicator = page.getByTestId("target-scope");
    await expect(indicator).toHaveAccessibleName(`Target scope: ${label}`);
    await expect(indicator).toHaveCSS("text-transform", "uppercase");
    if (icon) {
      await expect(indicator.getByTestId(icon)).toBeVisible();
    }

    await page.getByRole("button", { name: "Add Resources" }).click();
    const filter = page.getByRole("textbox", { name: "Filter resource types" });
    // Searching expands every matching group, so all offered types are rendered.
    await filter.fill("Microsoft");
    await expect
      .poll(() =>
        page
          .getByTestId("resource-type-drag-handle")
          .evaluateAll((handles) => handles.map((handle) => (handle as HTMLElement).dataset.resourceType).sort()),
      )
      .toEqual([...expectedTypes].sort());
    await filter.press("Escape");
  }
  expect(await getGraphTransform(page)).toBe(transform);
  await loadSampleGraph(page, "empty");
  await expect(page.getByTestId("target-scope")).toHaveCount(0);
});

test("changing the target scope while the palette is open refreshes its types", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  await page.getByRole("textbox", { name: "Filter resource types" }).fill("Microsoft");
  await expect(dragHandle(page, STORAGE_TYPE)).toBeVisible();

  await page.getByRole("combobox", { name: "Document target scope" }).selectOption("subscription");

  // The palette stays open with the same search, and its results follow the new scope.
  await expect(page.getByRole("complementary")).toBeVisible();
  await expect(dragHandle(page, "Microsoft.Resources/resourceGroups")).toBeVisible();
  await expect(dragHandle(page, STORAGE_TYPE)).toHaveCount(0);
});

test("scope remains when the entire creation dock is disabled", async ({ page }) => {
  await openVisualDesigner(page, { resourceCreation: "false", targetScope: "tenant" });
  await expect(page.getByTestId("creation-dock")).toHaveCount(0);
  await expect(page.getByTestId("target-scope")).toHaveAccessibleName("Target scope: Tenant");
  await page.getByRole("combobox", { name: "Document target scope" }).selectOption("subscription");
  await expect(page.getByTestId("target-scope")).toHaveAccessibleName("Target scope: Subscription");
});

test("clicking a resource neither inserts it nor flashes a drag preview", async ({ page }) => {
  await recordCreations(page);
  await openVisualDesigner(page);
  await openStorage(page);
  const resourceBox = (await dragHandle(page, STORAGE_TYPE).boundingBox())!;
  const pressAt = { x: resourceBox.x + 40, y: resourceBox.y + resourceBox.height / 2 };

  await page.mouse.move(pressAt.x, pressAt.y);
  await page.mouse.down();
  // Sub-threshold jitter is still a click, not a drag.
  await page.mouse.move(pressAt.x + 2, pressAt.y + 1);
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
  await page.mouse.up();

  await page.waitForTimeout(300);
  expect(await creations(page)).toEqual([]);
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
  await expect(page.getByRole("complementary")).toBeVisible();
});

test("drag preview follows pointer movement without waiting for a React render", async ({ page }) => {
  await openVisualDesigner(page);
  await openStorage(page);
  const resourceBox = (await dragHandle(page, STORAGE_TYPE).boundingBox())!;

  await page.mouse.move(resourceBox.x + 40, resourceBox.y + resourceBox.height / 2);
  await page.mouse.down();
  await page.mouse.move(resourceBox.x + 50, resourceBox.y - 20);
  await expect(page.getByTestId("palette-drag-preview")).toBeVisible();

  const position = await page.evaluate(() => {
    const preview = document.querySelector<HTMLElement>('[data-testid="palette-drag-preview"]');
    if (!preview) {
      throw new Error("Drag preview is missing.");
    }
    const x = parseFloat(preview.style.left) + 32;
    const y = parseFloat(preview.style.top) - 16;
    window.dispatchEvent(new PointerEvent("pointermove", { pointerId: 1, clientX: x, clientY: y }));

    return { left: preview.style.left, top: preview.style.top, x, y };
  });
  expect(position.left).toBe(`${position.x}px`);
  expect(position.top).toBe(`${position.y}px`);

  await page.keyboard.press("Escape");
  await page.mouse.up();
  await expect(page.getByTestId("palette-drag-preview")).toHaveCount(0);
});

test("Escape cancels an in-progress drag without closing the palette", async ({ page }) => {
  await recordCreations(page);
  await openVisualDesigner(page);
  await openStorage(page);
  const resourceBox = (await dragHandle(page, STORAGE_TYPE).boundingBox())!;
  const canvasBox = (await page.getByTestId("graph-canvas").boundingBox())!;

  await page.mouse.move(resourceBox.x + 40, resourceBox.y + resourceBox.height / 2);
  await page.mouse.down();
  await page.mouse.move(canvasBox.x + canvasBox.width * 0.85, canvasBox.y + canvasBox.height * 0.5, { steps: 8 });
  await expect(page.getByTestId("palette-drag-preview-card")).toBeVisible();

  await page.keyboard.press("Escape");
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
  await expect(page.getByRole("complementary")).toBeVisible();

  await page.mouse.up();
  expect(await creations(page)).toEqual([]);

  // With no drag in progress, Escape dismisses the palette as before.
  await page.getByRole("textbox", { name: "Filter resource types" }).press("Escape");
  await expect(page.getByRole("complementary")).toHaveCount(0);
});

test("disabling creation during a drag removes the dock and cancels the pending drag", async ({ page }) => {
  await recordCreations(page);
  await openVisualDesigner(page);
  await openStorage(page);
  const resourceBox = await dragHandle(page, STORAGE_TYPE).boundingBox();
  await page.mouse.move(resourceBox!.x + 20, resourceBox!.y + resourceBox!.height / 2);
  await page.mouse.down();
  await page.mouse.move(resourceBox!.x + 40, resourceBox!.y - 20, { steps: 4 });
  await expect(page.getByTestId("palette-drag-preview-card")).toBeVisible();
  await page.evaluate(() => window.postMessage({ method: "resourceCreation/enablementDidChange", params: false }, "*"));
  await expect(page.getByTestId("creation-dock")).toHaveCount(0);
  await expect(page.getByRole("complementary")).toHaveCount(0);
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
  await page.mouse.move(100, 300);
  await page.mouse.up();
  expect(await creations(page)).toEqual([]);
  await page.evaluate(() => window.postMessage({ method: "resourceCreation/enablementDidChange", params: true }, "*"));
  await expect(page.getByTestId("creation-dock")).toBeVisible();
  await expect(page.getByTestId("palette-drag-preview-card")).toHaveCount(0);
  await expect(page.getByTestId("target-scope")).toBeVisible();
});

test("preview-only types use the host default and remain available at every scope", async ({ page }) => {
  await recordCreations(page);
  await openVisualDesigner(page, { targetScope: "tenant" });
  await page.getByRole("button", { name: "Add Resources" }).click();
  await page.getByRole("textbox", { name: "Filter resource types" }).fill("widgets");
  const picker = page.getByRole("combobox", { name: "API version for Microsoft.Preview/widgets", exact: true });
  await expect(picker).toHaveText("2026-01-01-preview");
  await picker.click();
  await expect(versionOptions(page)).toHaveCount(2);
  await chooseVersion(page, picker, "2025-01-01-preview");
  await dragToCanvas(page, "Microsoft.Preview/widgets");
  await expect
    .poll(() => creations(page))
    .toEqual([{ fullyQualifiedType: "Microsoft.Preview/widgets", apiVersion: "2025-01-01-preview" }]);
});

test("palette list uses an overlay scrollbar that appears only while hovering, scrolling, or dragging", async ({
  page,
}) => {
  await openVisualDesigner(page, { catalogSize: "300", catalogDelay: "0" });
  const picker = await openStorage(page);
  await picker.click();
  await expect(page.getByTestId("api-version-popup")).toHaveCSS("scrollbar-width", "none");
  await page.keyboard.press("Escape");

  await page.getByRole("textbox", { name: "Filter resource types" }).fill("Microsoft");
  const list = page.getByTestId("resource-palette-list");
  const thumb = page.getByTestId("overlay-scrollbar-thumb");
  await expect.poll(() => list.evaluate((element) => element.scrollHeight > element.clientHeight)).toBe(true);

  // No native bar, so no width is reserved and insets stay even.
  await expect(list).toHaveCSS("scrollbar-width", "none");
  expect(await list.evaluate((element) => (element as HTMLElement).offsetWidth - element.clientWidth)).toBe(0);

  // Hidden at rest, shown while hovered.
  await page.mouse.move(0, 0);
  await expect(thumb).toHaveAttribute("data-visible", "false");
  await list.hover();
  await expect(thumb).toHaveAttribute("data-visible", "true");
  await page.mouse.move(0, 0);
  await expect(thumb).toHaveAttribute("data-visible", "false");

  // Shown briefly after scrolling, even without hovering.
  await list.evaluate((element) => element.scrollBy(0, 300));
  await expect(thumb).toHaveAttribute("data-visible", "true");
  await expect(thumb).toHaveAttribute("data-visible", "false", { timeout: 3000 });

  // Wheel scrolling still works, and dragging the thumb scrolls the list.
  await list.hover();
  await page.mouse.wheel(0, 200);
  await expect.poll(() => list.evaluate((element) => element.scrollTop)).toBeGreaterThan(300);
  const before = await list.evaluate((element) => element.scrollTop);
  const box = (await thumb.boundingBox())!;
  await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
  await page.mouse.down();
  await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2 + 40, { steps: 4 });
  await expect(thumb).toHaveAttribute("data-dragging", "true");
  await page.mouse.up();
  await expect(thumb).toHaveAttribute("data-dragging", "false");
  expect(await list.evaluate((element) => element.scrollTop)).toBeGreaterThan(before);
});

test("overlay scrollbar never appears when the palette list does not overflow", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  const list = page.getByTestId("resource-palette-list");
  await expect(page.getByRole("button", { name: "Microsoft.Storage" })).toBeVisible();
  expect(await list.evaluate((element) => element.scrollHeight > element.clientHeight)).toBe(false);

  await list.hover();
  await expect(page.getByTestId("overlay-scrollbar-thumb")).toHaveAttribute("data-visible", "false");
});

test("palette and version list use a frosted-glass surface", async ({ page }) => {
  await openVisualDesigner(page);
  const picker = await openStorage(page);
  await picker.click();
  await expect(versionOptions(page).first()).toBeVisible();

  for (const surface of [page.getByRole("complementary"), page.getByTestId("api-version-popup")]) {
    await expect(surface).toHaveCSS("background-color", "rgba(244, 245, 247, 0.5)");
    await expect(surface).toHaveCSS("backdrop-filter", "blur(16px) saturate(1.8)");
  }
});

test("large catalogs render progressively and a new query starts over", async ({ page }) => {
  await openVisualDesigner(page, { catalogSize: "2312", catalogDelay: "0" });
  await page.getByRole("button", { name: "Add Resources" }).click();
  const headers = page.locator("#resource-palette button[aria-expanded]");
  const rows = page.getByTestId("resource-type-row");
  const more = page.getByTestId("resource-palette-more");
  const list = page.getByTestId("resource-palette-list");

  // Browsing: only the first batch of namespace headers is rendered.
  await expect(more).toHaveCount(1);
  expect(await headers.count()).toBeLessThanOrEqual(50);

  // Searching everything renders a bounded first batch instead of every match.
  const filter = page.getByRole("textbox", { name: "Filter resource types" });
  await filter.fill("Microsoft");
  await expect(rows.first()).toBeVisible();
  await expect(more).toHaveCount(1);
  const firstBatch = await rows.count();
  expect(firstBatch).toBeLessThanOrEqual(50);

  // Reaching the end of what is rendered renders the next batch.
  await list.evaluate((element) => element.scrollTo(0, element.scrollHeight));
  await expect.poll(() => rows.count()).toBeGreaterThan(firstBatch);

  // A new query starts from a small batch again rather than re-rendering the grown list.
  await filter.fill("resource1");
  await expect.poll(() => rows.count()).toBeLessThanOrEqual(50);
  await expect(more).toHaveCount(1);
});

test("small catalogs render completely without a load-more sentinel", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  await page.getByRole("textbox", { name: "Filter resource types" }).fill("Microsoft");
  await expect(page.getByTestId("resource-type-row")).toHaveCount(3);
  await expect(page.getByTestId("resource-palette-more")).toHaveCount(0);
});

test("popover stays above the dock, viewport-clamped with pinned search", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  const popover = page.getByRole("complementary");
  await expect(popover).toBeVisible();
  expect((await popover.boundingBox())!.width).toBe(400);
  expect((await popover.boundingBox())!.height).toBe(360);
  await page.setViewportSize({ width: 380, height: 500 });
  const filter = page.getByRole("textbox", { name: "Filter resource types" });
  await filter.fill("Microsoft");
  await expect(page.getByTestId("resource-type-row")).toHaveCount(3);
  const panel = await popover.boundingBox();
  const app = await page.getByTestId("app-root").boundingBox();
  const dock = await page.getByTestId("creation-dock").boundingBox();
  expect(panel!.width).toBeLessThanOrEqual(348);
  expect(panel!.height).toBeLessThanOrEqual(360);
  expect(panel!.x).toBeGreaterThanOrEqual(app!.x);
  expect(panel!.y).toBeGreaterThanOrEqual(app!.y);
  expect(panel!.y + panel!.height).toBeLessThan(dock!.y);
  expect(panel!.x + panel!.width / 2).toBeCloseTo(dock!.x + dock!.width / 2, 0);
  const list = page.getByTestId("resource-palette-list");
  await expect.poll(() => list.evaluate((element) => element.scrollHeight > element.clientHeight)).toBe(true);
  const before = await filter.boundingBox();
  await list.evaluate((element) => element.scrollTo(0, element.scrollHeight));
  expect(await filter.boundingBox()).toEqual(before);
  expect(await popover.evaluate((element) => element.scrollHeight > element.clientHeight)).toBe(false);
  expect(await nodeCount(page)).toBeGreaterThan(0);
});

test("popover height stays fixed for collapsed groups, expanded groups, and empty search results", async ({ page }) => {
  await page.setViewportSize({ width: 1280, height: 900 });
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  const popover = page.getByRole("complementary");
  await expect(popover).toHaveCSS("height", "360px");
  const initialBounds = await popover.boundingBox();

  await page.getByText("Microsoft.Storage", { exact: true }).click();
  await expect(page.getByTestId("resource-type-row")).toBeVisible();
  expect(await popover.boundingBox()).toEqual(initialBounds);

  const filter = page.getByRole("textbox", { name: "Filter resource types" });
  await filter.fill("no-such-resource-type");
  await expect(page.getByText("No matching resource types.", { exact: true })).toBeVisible();
  expect(await popover.boundingBox()).toEqual(initialBounds);

  await filter.fill("Microsoft");
  await expect(page.getByTestId("resource-type-row")).toHaveCount(3);
  expect(await popover.boundingBox()).toEqual(initialBounds);
});

test("search uses one outer focus border without the host's inner input outline", async ({ page }) => {
  await openVisualDesigner(page);
  await page.addStyleTag({
    content: "input:focus { outline: 1px solid orange; outline-offset: -1px; }",
  });
  await page.getByRole("button", { name: "Add Resources" }).click();
  const filter = page.getByRole("textbox", { name: "Filter resource types" });
  await expect(filter).toBeFocused();
  await expect(filter).toHaveCSS("outline-style", "none");
  await expect(filter).toHaveCSS("border-width", "0px");
  await expect(filter).toHaveCSS("padding", "0px");
  const label = filter.locator("..");
  await expect(label).toHaveCSS("border-style", "solid");
  await expect(label).toHaveCSS("border-width", "1px");
  // The border fades in over 150ms, so wait for the settled color rather than sampling mid-transition.
  await expect
    .poll(async () =>
      THEME_FOCUS_BORDERS.includes(await label.evaluate((element) => getComputedStyle(element).borderColor)),
    )
    .toBe(true);
});

test("resource rows align with their group header in browsing and search", async ({ page }) => {
  await openVisualDesigner(page);
  await page.getByRole("button", { name: "Add Resources" }).click();
  await page.getByText("Microsoft.Storage", { exact: true }).click();
  const row = page.getByTestId("resource-type-row");
  await expect(row).toBeVisible();

  const expectAligned = async () => {
    const header = page.getByRole("button", { name: /^Microsoft\.Storage/ });
    const name = (await header.getByText("Microsoft.Storage", { exact: true }).boundingBox())!;
    const chevron = (await header.getByTestId("chevron-down-codicon").boundingBox())!;
    const search = (await page.getByRole("textbox", { name: "Filter resource types" }).locator("..").boundingBox())!;
    const searchIcon = (await page.getByTestId("search-codicon").boundingBox())!;
    const icon = (await row.getByTestId(`${STORAGE_TYPE}-icon`).boundingBox())!;
    const picker = (await row.getByTestId("api-version-picker").boundingBox())!;
    expect(icon.x).toBeCloseTo(name.x, 0);
    expect(searchIcon.x).toBeCloseTo(name.x, 0);
    expect(picker.x + picker.width).toBeCloseTo(chevron.x + chevron.width, 0);
    expect(search.x + search.width - (chevron.x + chevron.width)).toBeCloseTo(name.x - search.x, 0);
  };

  await expectAligned();
  await page.getByRole("textbox", { name: "Filter resource types" }).fill("storageAccounts");
  await expect(page.locator("mark").filter({ hasText: "storageAccounts" })).toBeVisible();
  await expectAligned();
});
