// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Client } from "@modelcontextprotocol/sdk/client/index.js";

export type Resource = { id: string; type: string; name?: string; tags?: Record<string, string>; properties?: unknown };
export type SourceLocation = { uri: string; line: number };
export type StackSource = { declaration: SourceLocation; requires?: SourceLocation | null };
export type Stack = { name: string; region: string; stage?: string; stageOrder?: number; labels: Record<string, string>; dependencies: string[]; observed: Resource[]; source?: StackSource | null };
export type Target = { target: "JSON SIMULATION"; targetInstance: string; targetRevision: number };
export type Topology = Target & { stacks: Stack[] };
export type Change = { resourceId: string; changeType: string; delta?: unknown; before?: unknown; after?: unknown };
export type PlannedStack = { name: string; changes: Change[]; selectionReason?: "Selected" | "Prerequisite"; requiredBy?: string[] };
export type Plan = Target & { id: string; fingerprint: string; source: string; sourceFingerprint: string; parametersFingerprint: string; observedFingerprint: string; selectors: string[]; stacks: PlannedStack[] };
export type Progress = { sequence: number; time: string; stack: string | null; status: string };
export type Operation = Target & { id: string; planId: string; status: string; stacks?: string[]; events: Progress[] };
export type PendingApproval = { plan: Plan; submittedAt: string };
export type Activity = Target & { plan?: Plan | null; operation?: Operation | null; pendingApprovals: PendingApproval[] };

let connected: Client | undefined;
let connecting: Promise<Client> | undefined;

async function getClient(): Promise<Client> {
  if (connected) return connected;
  if (!connecting) {
    const connect = async () => {
      const [{ Client: McpClient }, { StreamableHTTPClientTransport }] = await Promise.all([
        import("@modelcontextprotocol/sdk/client/index.js"),
        import("@modelcontextprotocol/sdk/client/streamableHttp.js"),
      ]);
      const client = new McpClient({ name: "bicep-orchestration-viewer", version: "1.0.0" });
      try {
        await client.connect(new StreamableHTTPClientTransport(new URL("/mcp", window.location.origin)));
        connected = client;
        return client;
      } catch (error) {
        await client.close().catch(() => {});
        throw error;
      }
    };
    connecting = connect().finally(() => { connecting = undefined; });
  }
  return connecting;
}

async function tool<T>(name: string, args: Record<string, unknown> = {}): Promise<T> {
  const client = await getClient();
  let result;
  try {
    result = await client.callTool({ name, arguments: args });
  } catch (error) {
    if (connected === client) {
      connected = undefined;
      await client.close().catch(() => {});
    }
    throw error;
  }
  const content = "content" in result ? result.content : undefined;
  if (!Array.isArray(content)) throw new Error(`MCP tool '${name}' returned no result.`);
  const textItems = content.filter((item: unknown): item is { type: "text"; text: string } =>
    typeof item === "object" && item !== null && "type" in item && item.type === "text" && "text" in item && typeof item.text === "string");
  if ("isError" in result && result.isError === true) {
    throw new Error(textItems.map(item => item.text).join("\n") || `MCP tool '${name}' failed.`);
  }
  if ("structuredContent" in result && result.structuredContent !== undefined) {
    return result.structuredContent as T;
  }
  const text = textItems[0]?.text;
  if (text === undefined) throw new Error(`MCP tool '${name}' returned no result.`);
  try {
    return JSON.parse(text) as T;
  } catch {
    throw new Error(`MCP tool '${name}' returned invalid JSON.`);
  }
}

export function client() {
  const expected = (target: Target) => ({ expectedTargetInstance: target.targetInstance, expectedTargetRevision: target.targetRevision });
  return {
    topology: (target: Target) => tool<Topology>("get_topology", expected(target)),
    activity: () => tool<Activity>("get_activity"),
    plan: (selectors: string[], target: Target) => tool<Plan>("plan_deployment", { selectors, ...expected(target) }),
    submit: (planId: string) => tool<PendingApproval>("submit_deployment", { planId }),
    approve: (planId: string) => tool<Operation>("approve_deployment", { planId, approved: true }),
    reject: (planId: string) => tool<PendingApproval>("reject_deployment", { planId }),
  };
}
