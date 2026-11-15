// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { test, expect } from "@playwright/test";
import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StreamableHTTPClientTransport } from "@modelcontextprotocol/sdk/client/streamableHttp.js";
import { spawn, spawnSync, type ChildProcess } from "node:child_process";
import { cpSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { resolve, join } from "node:path";
import { createServer } from "node:net";

test("simulation-only viewer refreshes every second, serializes slow reads, and recovers", async ({ page }) => {
  const root = resolve(import.meta.dirname, "../../../..");
  const folder = mkdtempSync(join(tmpdir(), "bicep-polling-"));
  cpSync(join(root, "docs/experimental/orchestration-samples/basic"), join(folder, "basic"), { recursive: true });
  const port = await new Promise<number>(done => {
    const socket = createServer();
    socket.listen(0, "127.0.0.1", () => { const p = (socket.address() as { port: number }).port; socket.close(() => done(p)); });
  });
  const args = [join(root, "src/Bicep.Orchestration.Local/bin/Debug/net10.0/Bicep.Orchestration.Local.dll"),
    "--workspace", folder, "--entry", "basic/main.bicepparam", "--state", join(folder, "state.json"), "--port", String(port)];
  let host: ChildProcess | undefined;
  let mcp: Client | undefined;
  let calls = 0;
  let failNext = false;
  let slow = false;
  let concurrent = 0;
  let maxConcurrent = 0;
  try {
    const real = spawnSync("dotnet", [...args, "--real"], { encoding: "utf8" });
    expect(real.status).not.toBe(0);
    expect(real.stderr).toContain("supports JSON simulation only");
    host = spawn("dotnet", args, { stdio: "ignore" });
    await expect.poll(async () => { try { return (await fetch(`http://127.0.0.1:${port}`)).status; } catch { return 0; } }).toBe(200);
    mcp = new Client({ name: "bicep-polling-test", version: "1.0.0" });
    await mcp.connect(new StreamableHTTPClientTransport(new URL(`http://127.0.0.1:${port}/mcp`)));
    expect((await mcp.listTools()).tools.map(tool => tool.name)).not.toContain("set_target");
    const stale = await mcp.callTool({ name: "get_topology", arguments: { expectedTargetInstance: "previous-host", expectedTargetRevision: 0 } });
    expect(stale.isError).toBe(true);

    await page.route("**/mcp", async route => {
      const request = route.request();
      const body = request.method() === "POST" ? request.postDataJSON() : undefined;
      if (body?.method !== "tools/call" || body.params?.name !== "get_topology") {
        await route.continue();
        return;
      }
      calls++;
      concurrent++;
      maxConcurrent = Math.max(maxConcurrent, concurrent);
      try {
        if (failNext) {
          failNext = false;
          await route.fulfill({ contentType: "application/json", body: JSON.stringify({
            jsonrpc: "2.0", id: body.id, result: { isError: true, content: [{ type: "text", text: "Temporary simulation read failure." }] },
          }) });
        } else {
          const response = await route.fetch();
          if (slow) await new Promise(done => setTimeout(done, 1500));
          await route.fulfill({ response });
        }
      } finally { concurrent--; }
    });
    await page.goto(`http://127.0.0.1:${port}`);
    await expect(page.locator("button.stack")).toHaveCount(5);
    await expect.poll(() => calls, { timeout: 5000 }).toBeGreaterThanOrEqual(3);
    for (const view of ["Approvals", "Active Deployments", "Service Health"]) {
      const before = calls;
      await page.getByRole("navigation", { name: "Views" }).getByRole("button", { name: view, exact: true }).click();
      await expect.poll(() => calls).toBeGreaterThan(before);
    }
    const parameters = join(folder, "basic/main.bicepparam");
    writeFileSync(parameters, readFileSync(parameters, "utf8").replace("regions: ['eastus', 'westus']", "regions: ['eastus', 'eastus']"));
    await expect(page.locator(".topology-matrix tbody tr")).toHaveCount(1, { timeout: 10000 });
    failNext = true;
    await expect(page.getByRole("alert")).toContainText("Temporary simulation read failure.");
    await expect(page.getByRole("alert")).toHaveCount(0, { timeout: 10000 });
    slow = true;
    const before = calls;
    await expect.poll(() => calls, { timeout: 10000 }).toBeGreaterThanOrEqual(before + 3);
    expect(maxConcurrent).toBe(1);
  } finally {
    await page.unrouteAll({ behavior: "wait" });
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
