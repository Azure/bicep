// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { test, expect } from "@playwright/test";
import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StreamableHTTPClientTransport } from "@modelcontextprotocol/sdk/client/streamableHttp.js";
import { spawn, type ChildProcess } from "node:child_process";
import { cpSync, existsSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { resolve, join } from "node:path";
import { createServer } from "node:net";
import type { Activity, Plan } from "../src/api";

test("matrix selects labels, predicts, explicitly applies, monitors, and inspects stacks", async ({ page }) => {
  test.setTimeout(180_000);
  const root = resolve(import.meta.dirname, "../../../..");
  const folder = mkdtempSync(join(tmpdir(), "bicep-viewer-"));
  cpSync(join(root, "docs/experimental/orchestration-samples/basic"), join(folder, "basic"), { recursive: true });
  const sourcePath = join(folder, "basic/main.bicep");
  writeFileSync(sourcePath, readFileSync(sourcePath, "utf8")
    .replace("version: version", "version: '1.0.${i}'")
    .replaceAll(/^\s*version: version\s*$/gm, "")
    .replaceAll(/^\s*deploy:.*$/gm, ""));
  const port = await new Promise<number>(done => {
    const socket = createServer();
    socket.listen(0, "127.0.0.1", () => { const p = (socket.address() as { port: number }).port; socket.close(() => done(p)); });
  });
  let host: ChildProcess | undefined;
  let mcp: Client | undefined;
  const errors: string[] = [];
  page.on("pageerror", e => errors.push(e.message));
  try {
    host = spawn("dotnet", [join(root, "src/Bicep.Orchestration.Local/bin/Debug/net10.0/Bicep.Orchestration.Local.dll"),
      "--workspace", folder, "--entry", "basic/main.bicepparam", "--state", join(folder, "state.json"),
      "--port", String(port)], { stdio: "ignore" });
    await expect.poll(async () => { try { return (await fetch(`http://127.0.0.1:${port}`)).status; } catch { return 0; } }).toBe(200);
    mcp = new Client({ name: "bicep-orchestration-browser-test", version: "1.0.0" });
    await mcp.connect(new StreamableHTTPClientTransport(new URL(`http://127.0.0.1:${port}/mcp`)));
    const toolValue = <T>(result: { isError?: boolean; structuredContent?: unknown; content?: unknown }): T => {
      expect(result.isError).not.toBe(true);
      if (result.structuredContent !== undefined) return result.structuredContent as T;
      if (!Array.isArray(result.content)) throw new Error("MCP tool returned no result.");
      const text = result.content.find((item: unknown): item is { type: "text"; text: string } =>
        typeof item === "object" && item !== null && "type" in item && item.type === "text" &&
        "text" in item && typeof item.text === "string");
      if (!text) throw new Error("MCP tool returned no text result.");
      return JSON.parse(text.text) as T;
    };
    await page.goto(`http://127.0.0.1:${port}?clawpilotTheme=dark`);
    await expect(page.locator("html")).toHaveAttribute("data-theme", "dark");
    await expect(page.locator("footer")).toHaveCount(0);
    const checkNeutralAccent = async () => {
      const colors = await page.evaluate(() => ({
        accent: getComputedStyle(document.documentElement).getPropertyValue("--cp-accent").trim(),
        link: getComputedStyle(document.documentElement).getPropertyValue("--cp-link").trim(),
        primary: getComputedStyle(document.querySelector(".primary")!).backgroundColor,
        navigation: getComputedStyle(document.querySelector('nav button[aria-pressed="true"]')!).color,
      }));
      expect(colors.accent).toBe(colors.link);
      expect(colors.primary).toBe(colors.navigation);
    };
    await checkNeutralAccent();
    const navigation = page.getByRole("navigation", { name: "Views" });
    const deployView = navigation.getByRole("button", { name: "Deploy", exact: true });
    const approvalsView = navigation.getByRole("button", { name: "Approvals", exact: true });
    const deploymentView = navigation.getByRole("button", { name: "Active Deployments", exact: true });
    const healthView = navigation.getByRole("button", { name: "Service Health", exact: true });
    await expect(navigation.getByRole("button")).toHaveText(["Deploy", "Approvals", "Active Deployments", "Service Health"]);
    await approvalsView.click();
    await expect(page.getByRole("region", { name: "Pending approvals", exact: true })).toContainText("No plans pending approval.");
    await deployView.click();
    await expect(page.locator("header nav")).toBeVisible();
    await expect(page.getByRole("button", { name: /Target:|^Simulated$|^Refresh$/ })).toHaveCount(0);
    await expect(deployView).toHaveAttribute("aria-pressed", "true");
    await expect(page.getByText("1 · Source in VS Code", { exact: true })).toHaveCount(0);
    await expect(page.locator(".timeline")).toHaveCount(0);
    await expect(page.getByLabel("Access token")).toHaveCount(0);
    await expect(page.getByText("JSON SIMULATION", { exact: true })).toHaveCount(0);
    await expect(page.locator("button.stack")).toHaveCount(5);
    await expect(page.locator(".topology-matrix thead th")).toHaveText(["Region", "cluster[*]", "clusterApp[*]", "global"]);
    await expect(page.locator(".topology-matrix tbody tr")).toHaveCount(2);
    await expect(page.locator(".topology-matrix tbody tr").first().locator("button.stack")).toHaveCount(3);
    const checkSelection = async () => {
      const cell = page.locator("button.stack.selected");
      await expect(cell).toHaveCount(1);
      await expect(cell).toHaveCSS("outline-width", "2px");
      await expect(cell).toHaveAttribute("aria-pressed", "true");
    };
    const firstCell = page.locator('button.stack[data-stack="cluster[0]"]');
    await expect(firstCell.locator(".stack-version")).toHaveText("Version: Not deployed");
    const inspector = page.getByRole("complementary", { name: "Property inspector" });
    const selectors = page.getByRole("group", { name: "Label selectors", exact: true });
    const filterChips = selectors.locator(".filter-chip > button:first-child");
    const filterEditor = page.getByRole("dialog", { name: /label filter/ });
    await selectors.getByRole("button", { name: "Add filter" }).click();
    await expect(filterEditor.getByRole("button", { name: "Apply filter" })).toBeDisabled();
    await expect(filterEditor.getByRole("combobox", { name: "Label", exact: true }).locator("option")).toHaveText(["Select a label", "kind", "region"]);
    await filterEditor.getByRole("combobox", { name: "Label", exact: true }).selectOption("region");
    await filterEditor.getByRole("textbox", { name: "Values" }).fill("EAST");
    await expect(filterEditor.getByRole("checkbox")).toHaveCount(1);
    await filterEditor.getByRole("checkbox", { name: "eastus", exact: true }).check();
    await filterEditor.getByRole("textbox", { name: "Values" }).fill("not-a-region");
    await expect(filterEditor).toContainText("No matching values.");
    await filterEditor.getByRole("textbox", { name: "Values" }).clear();
    await filterEditor.getByRole("checkbox", { name: "westus", exact: true }).check();
    await filterEditor.getByRole("button", { name: "Apply filter" }).click();
    await expect(filterChips).toHaveText(["region equals eastus or westus"]);
    await selectors.getByRole("button", { name: "Add filter" }).click();
    await expect(filterEditor.getByRole("combobox", { name: "Label", exact: true }).locator("option")).toHaveText(["Select a label", "kind"]);
    await filterEditor.getByRole("combobox", { name: "Label", exact: true }).selectOption("kind");
    await filterEditor.getByRole("checkbox", { name: "global", exact: true }).check();
    await filterEditor.getByRole("button", { name: "Apply filter" }).click();
    await expect(filterChips).toHaveText(["region equals eastus or westus", "kind equals global"]);
    await expect(selectors.getByRole("button", { name: "Add filter" })).toBeDisabled();
    await selectors.getByRole("button", { name: "Remove filter kind" }).click();
    await page.getByRole("button", { name: "Predict changes" }).click();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled({ timeout: 60_000 });
    const multiValuePlan = toolValue<{ plan: { selectors: string[]; stacks: { name: string }[] } }>(
      await mcp.callTool({ name: "get_activity", arguments: {} }));
    expect(multiValuePlan.plan.selectors).toEqual(["region=eastus,westus"]);
    expect(multiValuePlan.plan.stacks).toHaveLength(5);
    await selectors.getByRole("button", { name: "Edit filter region" }).click();
    await expect(filterEditor.getByRole("checkbox", { name: "eastus", exact: true })).toBeChecked();
    await filterEditor.getByRole("checkbox", { name: "westus", exact: true }).uncheck();
    await filterEditor.getByRole("button", { name: "Cancel" }).click();
    await expect(filterChips).toHaveText(["region equals eastus or westus"]);
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled();
    await selectors.getByRole("button", { name: "Edit filter region" }).click();
    await filterEditor.getByRole("checkbox", { name: "westus", exact: true }).uncheck();
    await filterEditor.getByRole("button", { name: "Apply filter" }).click();
    await expect(filterChips).toHaveText(["region equals eastus"]);
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeDisabled();
    await selectors.getByRole("button", { name: "Remove filter region" }).click();
    await expect(filterChips).toHaveCount(0);
    await selectors.getByRole("button", { name: "Add filter" }).click();
    await filterEditor.press("Escape");
    await expect(filterEditor).toHaveCount(0);
    await expect(selectors.getByRole("button", { name: "Add filter" })).toBeFocused();
    await selectors.getByRole("button", { name: "Add filter" }).click();
    await page.getByRole("heading", { name: "Bicep Orchestration", exact: true }).click();
    await expect(filterEditor).toHaveCount(0);
    await expect(inspector).toHaveCount(0);
    await firstCell.click();
    await expect(inspector).toBeVisible();
    await inspector.getByRole("button", { name: "Close inspector" }).click();
    await expect(inspector).toHaveCount(0);
    await expect(page.locator(".workspace")).not.toHaveClass(/with-inspector/);
    await expect(firstCell).toHaveAttribute("aria-pressed", "true");
    await page.getByRole("button", { name: "Inspect selected stack" }).click();
    const regionLabel = inspector.getByRole("button", { name: "region=eastus", exact: true });
    await regionLabel.click();
    await inspector.getByRole("button", { name: "kind=cluster", exact: true }).click();
    await expect(filterChips).toHaveText(["region equals eastus", "kind equals cluster"]);
    await deploymentView.click();
    await expect(deploymentView).toHaveAttribute("aria-pressed", "true");
    await expect(selectors).toHaveCount(0);
    await expect(page.getByRole("button", { name: "Predict changes" })).toHaveCount(0);
    await expect(page.locator(".timeline")).toContainText("No deployment has been started");
    const matrix = page.getByRole("region", { name: "Stack topology matrix", exact: true });
    await expect(matrix).toBeVisible();
    await expect(matrix.locator("button.stack")).toHaveCount(5);
    await expect(firstCell).toHaveAttribute("aria-pressed", "true");
    await page.locator('button.stack[data-stack="clusterApp[0]"]').click();
    await expect(inspector.getByRole("heading", { name: "clusterApp[0]", exact: true })).toBeVisible();
    await inspector.getByRole("button", { name: "cluster[0]", exact: true }).click();
    await expect(firstCell).toHaveAttribute("aria-pressed", "true");
    await deployView.click();
    await expect(filterChips).toHaveText(["region equals eastus", "kind equals cluster"]);
    await expect(firstCell).toHaveAttribute("aria-pressed", "true");
    await page.getByRole("button", { name: "Predict changes" }).click();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled({ timeout: 60_000 });
    await expect(page.locator('button.stack[data-stack="cluster[1]"]')).toContainText("Outside plan");
    await inspector.getByRole("button", { name: "kind=cluster", exact: true }).click();
    await expect(filterChips).toHaveText(["region equals eastus", "kind equals cluster"]);
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeDisabled();
    await page.locator('button.stack[data-stack="clusterApp[0]"]').click();
    await inspector.getByRole("button", { name: "kind=app", exact: true }).click();
    await expect(filterChips).toHaveText(["region equals eastus", "kind equals app"]);
    await page.getByRole("button", { name: "Predict changes" }).click();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled({ timeout: 60_000 });
    const selectedPlan = toolValue<{ plan: { selectors: string[]; stacks: { name: string }[] } }>(
      await mcp.callTool({ name: "get_activity", arguments: {} }));
    expect(selectedPlan.plan.selectors).toEqual(["region=eastus", "kind=app"]);
    expect(selectedPlan.plan.stacks.map(s => s.name).sort()).toEqual(["clusterApp[0]", "cluster[0]"]);
    await expect(firstCell).not.toContainText("Outside plan");
    await selectors.getByRole("button", { name: "Clear filters" }).click();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeDisabled();
    await page.getByRole("button", { name: "Predict changes" }).click();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled({ timeout: 60_000 });
    await expect(page.locator(".badge.Create").first()).toBeVisible();
    await firstCell.click();
    await checkSelection();
    await expect(inspector.getByRole("heading", { name: "Desired dependencies" })).toBeVisible();
    await expect(inspector.getByText("No prerequisites.", { exact: true })).toBeVisible();
    const appCell = page.locator('button.stack[data-stack="clusterApp[0]"]');
    await appCell.click();
    await expect(firstCell).toHaveAttribute("aria-pressed", "false");
    await checkSelection();
    const dependencyBadges = inspector.getByRole("list", { name: "Desired dependencies" }).locator("button.badge");
    await expect(dependencyBadges).toHaveText(["cluster[0]"]);
    await page.locator('button.stack[data-stack="global"]').click();
    await expect(dependencyBadges).toHaveText(["clusterApp[0]", "clusterApp[1]"]);
    await appCell.click();
    await dependencyBadges.click();
    await expect(firstCell).toHaveAttribute("aria-pressed", "true");
    await expect(page.getByRole("heading", { name: "Plan · property changes" })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Managed resources" })).toBeVisible();
    const diff = page.getByLabel("What-if changes");
    await expect(diff.locator(".wi-resource .wi-symbol.wi-Create").first()).toHaveText("+");
    await expect(diff.locator(".wi-summary")).toContainText(/Resource changes: \d+ to create\./);
    await expect(diff).not.toContainText("\"changeType\"");
    const draft = toolValue<Activity>(await mcp.callTool({ name: "get_activity", arguments: {} })).plan!;
    const unsubmitted = await mcp.callTool({ name: "approve_deployment", arguments: { planId: draft.id, approved: true } });
    expect(unsubmitted.isError).toBe(true);
    expect(JSON.stringify(unsubmitted.content)).toContain("Submit it first");
    await page.getByRole("button", { name: "Submit deployment" }).click();
    await expect(approvalsView).toHaveAttribute("aria-pressed", "true");
    const approvals = page.getByRole("region", { name: "Pending approvals", exact: true });
    await expect(approvals.locator(".approval-list > li")).toHaveCount(1);
    await expect(approvals.getByRole("region", { name: "Plan review" })).toContainText(draft.id);
    const submitted = toolValue<Activity>(await mcp.callTool({ name: "get_activity", arguments: {} }));
    expect(submitted.pendingApprovals.map(item => item.plan.id)).toEqual([draft.id]);
    expect(submitted.operation ?? null).toBeNull();
    expect(existsSync(join(folder, "state.json"))).toBe(false);
    const duplicateSubmission = await mcp.callTool({ name: "submit_deployment", arguments: { planId: draft.id } });
    expect(duplicateSubmission.isError).toBe(true);
    expect(JSON.stringify(duplicateSubmission.content)).toContain("already awaiting approval");

    // Independent predictions remain reviewable alongside the submitted plan.
    const anotherPlan = toolValue<Plan>(await mcp.callTool({ name: "plan_deployment", arguments: { selectors: ["kind=cluster", "region=eastus"] } }));
    await mcp.callTool({ name: "submit_deployment", arguments: { planId: anotherPlan.id } });
    await expect(approvals.locator(".approval-list > li")).toHaveCount(2);
    await approvals.getByRole("button", { name: `Review plan ${anotherPlan.id.slice(0, 8)}` }).click();
    const originalSource = readFileSync(sourcePath, "utf8");
    writeFileSync(sourcePath, `${originalSource}\n// Changed after submission.\n`);
    await approvals.getByRole("button", { name: "Approve & deploy" }).click();
    await expect(page.getByRole("alert")).toContainText("Stale plan");
    expect(toolValue<Activity>(await mcp.callTool({ name: "get_activity", arguments: {} })).operation ?? null).toBeNull();
    expect(existsSync(join(folder, "state.json"))).toBe(false);
    writeFileSync(sourcePath, originalSource);
    await approvals.getByRole("button", { name: "Reject plan" }).click();
    await expect(approvals.locator(".approval-list > li")).toHaveCount(1);
    expect((await mcp.callTool({ name: "approve_deployment", arguments: { planId: anotherPlan.id, approved: true } })).isError).toBe(true);
    const notApproved = await mcp.callTool({ name: "approve_deployment", arguments: { planId: draft.id, approved: false } });
    expect(notApproved.isError).toBe(true);
    expect(JSON.stringify(notApproved.content)).toContain("Explicit approval is required");
    await approvals.getByRole("button", { name: "Approve & deploy" }).click();
    await expect(deploymentView).toHaveAttribute("aria-pressed", "true");
    await deploymentView.click();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toHaveCount(0);
    await expect(page.getByLabel("What-if changes")).toHaveCount(0);
    await expect(page.locator(".stack-status.Running").first()).toBeVisible();
    await expect(appCell.locator(".stack-status")).toHaveText("Queued");
    const activeOperation = toolValue<{ operation: { planId: string } }>(
      await mcp.callTool({ name: "get_activity", arguments: {} }));
    const duplicate = await mcp.callTool({ name: "approve_deployment", arguments: { planId: activeOperation.operation.planId, approved: true } });
    expect(duplicate.isError).toBe(true);
    expect(JSON.stringify(duplicate.content)).toContain("An operation is already in progress.");
    await expect(page.locator(".timeline h2")).toContainText("Succeeded", { timeout: 60_000 });
    expect(toolValue<Activity>(await mcp.callTool({ name: "get_activity", arguments: {} })).pendingApprovals).toHaveLength(0);
    expect((await mcp.callTool({ name: "approve_deployment", arguments: { planId: draft.id, approved: true } })).isError).toBe(true);
    await expect(page.locator(".timeline li").filter({ hasText: "Succeeded" })).toHaveCount(6);
    const finished = toolValue<{ operation: { events: { stack?: string; status: string; time: string }[] } }>(
      await mcp.callTool({ name: "get_activity", arguments: {} }));
    for (const name of ["cluster[0]", "cluster[1]", "clusterApp[0]", "clusterApp[1]", "global"]) {
      const events = finished.operation.events.filter(event => event.stack === name);
      expect(events.map(event => event.status)).toEqual(["Queued", "Running", "Succeeded"]);
      expect(Date.parse(events[2].time) - Date.parse(events[1].time)).toBeGreaterThanOrEqual(5000);
      expect(Date.parse(events[2].time) - Date.parse(events[1].time)).toBeLessThan(15000);
    }
    await expect(firstCell).toContainText("Succeeded");
    await expect(firstCell.locator(".stack-version")).toHaveText("Version: 1.0.0");
    await expect(inspector).toContainText("Current version: 1.0.0");
    await expect(matrix.locator("button.stack")).toHaveCount(5);
    await expect(page.getByRole("checkbox", { name: "Changes overlay" })).toHaveCount(0);
    await healthView.click();
    await expect(healthView).toHaveAttribute("aria-pressed", "true");
    await expect(page.getByRole("region", { name: "Health monitoring limitations" })).toContainText("not health checks");
    await expect(page.getByRole("region", { name: "Health monitoring limitations" })).toContainText("Showing simulated state");
    await expect(page.getByText("Health not assessed", { exact: true })).toBeVisible();
    await expect(page.locator(".timeline")).toHaveCount(0);
    await expect(matrix.locator("button.stack")).toHaveCount(5);
    await expect(firstCell.locator(".stack-status")).toHaveText("1 observed resource");
    await expect(firstCell.locator(".stack-version")).toHaveText("Version: 1.0.0");
    await expect(appCell.locator(".stack-version")).toHaveText("Version: Unknown");
    await expect(firstCell).not.toContainText("Succeeded");
    await appCell.focus();
    await page.keyboard.press("Enter");
    await expect(inspector.getByRole("heading", { name: "clusterApp[0]", exact: true })).toBeVisible();
    await firstCell.click();
    await expect(page.getByLabel("Managed resource IDs").locator("li")).toHaveCount(1);
    await expect(page.getByLabel("Managed resource IDs")).not.toContainText("/deploymentStacks/");
    const regionFilter = matrix.getByRole("textbox", { name: "Filter regions" });
    await expect(page.getByRole("combobox", { name: "Region" })).toHaveCount(0);
    await regionFilter.fill("  WEST  ");
    await expect(matrix.locator("button.stack")).toHaveCount(2);
    await expect(matrix.locator("tbody tr")).toHaveCount(1);
    await regionFilter.fill("no-such-region");
    await expect(matrix.locator("tbody tr")).toHaveCount(0);
    await regionFilter.fill("us");
    await expect(matrix.locator("button.stack")).toHaveCount(5);
    await regionFilter.clear();
    await expect(matrix.locator("button.stack")).toHaveCount(5);
    await deployView.click();
    await expect(deployView).toHaveAttribute("aria-pressed", "true");
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeDisabled();
    await page.locator('button.stack[data-stack="cluster[1]"]').click();
    await checkSelection();
    await firstCell.click();
    await expect(page.getByLabel("Managed resource IDs").locator("li")).toHaveCount(1);
    await expect(page.getByLabel("Managed resource IDs")).toContainText("/subscriptions/");
    await expect(page.getByLabel("Managed resource IDs")).not.toContainText('"properties"');
    // Exercise the completion-refresh race: prediction must wait behind observation reads,
    // while still requiring explicit approval of the newly returned, unconsumed plan.
    const refreshes = Promise.all(Array.from({ length: 8 }, () => mcp!.callTool({ name: "get_topology", arguments: {} })));
    await page.getByRole("button", { name: "Predict changes" }).click();
    expect((await refreshes).every(response => !response.isError)).toBe(true);
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled({ timeout: 60_000 });
    await expect(page.getByRole("alert")).toHaveCount(0);
    await expect(page.locator(".badge.Create")).toHaveCount(0);
    await expect(page.locator("button.unchanged")).toHaveCount(5);
    await checkSelection();
    await regionFilter.fill("west");
    await expect(page.locator("button.stack")).toHaveCount(2);
    await expect(page.locator(".topology-matrix tbody tr")).toHaveCount(1);
    await deploymentView.click();
    await expect(regionFilter).toHaveValue("west");
    await expect(matrix.locator("button.stack")).toHaveCount(2);
    await deployView.click();
    await regionFilter.clear();
    await expect(page.getByRole("button", { name: "Submit deployment" })).toBeEnabled();
    await expect(page.getByRole("checkbox", { name: "Changes overlay" })).toHaveCount(0);
    await expect(page.locator("button.unchanged")).toHaveCount(5);
    await expect(page.locator(".cell-changes")).toHaveCount(5);
    await page.setViewportSize({ width: 900, height: 900 });
    await expect(page.locator("button.stack")).toHaveCount(5);
    await expect(page.getByRole("heading", { name: "Bicep Orchestration" })).toBeVisible();
    writeFileSync(sourcePath, readFileSync(sourcePath, "utf8").replace("version: '1.0.${i}'", "version: '2.0.${i}'"));
    await expect(firstCell.locator(".stack-version")).toHaveText("Version: 1.0.0");
    // Another actor (e.g. MCP/Copilot) uses the same service; browser picks up its plan and operation.
    await selectors.getByRole("button", { name: "Add filter" }).click();
    const externalPlan = toolValue<Plan>(await mcp!.callTool({
      name: "plan_deployment",
      arguments: { selectors: ["kind=cluster", "region=eastus", "region=eastus"] },
    }));
    expect(externalPlan.stacks).toHaveLength(1);
    const resourceChanges = externalPlan.stacks[0].changes;
    expect(resourceChanges.length).toBeGreaterThan(0);
    expect(resourceChanges.every(change => change.changeType === "NoChange")).toBe(true);
    expect(resourceChanges.some(change => change.resourceId.toLowerCase().includes("/providers/microsoft.resources/deploymentstacks/"))).toBe(false);
    await expect(page.getByText(`Plan ${externalPlan.id.slice(0, 8)}`, { exact: true })).toBeVisible();
    await expect(filterEditor).toHaveCount(0);
    await expect(filterChips).toHaveText(["kind equals cluster", "region equals eastus"]);
    await firstCell.click();
    await expect(page.getByLabel("What-if changes")).not.toContainText("tags.version");
    await expect(firstCell).toHaveClass(/unchanged/);
    await expect(firstCell.locator(".stack-version")).toHaveText("Version: 1.0.0");
    const versionBeforeApproval = readFileSync(join(folder, "state.json"), "utf8");
    await mcp.callTool({ name: "submit_deployment", arguments: { planId: externalPlan.id } });
    await approvalsView.click();
    await expect(approvals.locator(".approval-list > li")).toHaveCount(1);
    await expect(approvals.getByRole("region", { name: "Plan review" })).toContainText(externalPlan.id);
    expect(readFileSync(join(folder, "state.json"), "utf8")).toBe(versionBeforeApproval);
    const externalOperation = toolValue<{ id: string }>(await mcp!.callTool({
      name: "approve_deployment",
      arguments: { planId: externalPlan.id, approved: true },
    }));
    await deploymentView.click();
    await expect(page.locator(".timeline code")).toHaveText(externalOperation.id);
    await expect(page.locator(".timeline h2")).toContainText("Succeeded", { timeout: 60_000 });
    await expect(firstCell.locator(".stack-version")).toHaveText("Version: 2.0.0");
    await expect(page.locator('button.stack[data-stack="cluster[1]"] .stack-version')).toHaveText("Version: 1.0.1");
    await expect(matrix.locator("button.stack")).toHaveCount(5);
    await page.goto(`http://127.0.0.1:${port}?clawpilotTheme=light`);
    await expect(page.locator("html")).toHaveAttribute("data-theme", "light");
    await checkNeutralAccent();
    await expect(page.locator("button.stack")).toHaveCount(5);
    await page.locator('button.stack[data-stack="cluster[1]"]').click();
    await checkSelection();
    await expect(inspector.getByText("No prerequisites.", { exact: true })).toBeVisible();
    await page.locator('button.stack[data-stack="global"]').click();
    await expect(dependencyBadges).toHaveText(["clusterApp[0]", "clusterApp[1]"]);
    // Multiple instances in one region/declaration must remain individually selectable.
    const parametersPath = join(folder, "basic/main.bicepparam");
    writeFileSync(parametersPath, readFileSync(parametersPath, "utf8").replace("regions: ['eastus', 'westus']", "regions: ['eastus', 'eastus']"));
    await expect(page.locator(".topology-matrix tbody tr")).toHaveCount(1);
    await expect(page.locator(".topology-matrix tbody td").first().locator("button.stack")).toHaveCount(2);
    await expect(page.locator("button.stack")).toHaveCount(5);
    await page.locator('button.stack[data-stack="cluster[1]"]').click();
    await expect(inspector.getByRole("heading", { name: "cluster[1]", exact: true })).toBeVisible();
    expect(errors).toEqual([]);
  } finally {
    await mcp?.close();
    if (host && host.exitCode === null) {
      host.kill("SIGTERM");
      await new Promise<void>(done => {
        const timer = setTimeout(() => { host?.kill("SIGKILL"); }, 15_000);
        host!.once("exit", () => { clearTimeout(timer); done(); });
      });
    }
    rmSync(folder, { recursive: true, force: true });
  }
});
