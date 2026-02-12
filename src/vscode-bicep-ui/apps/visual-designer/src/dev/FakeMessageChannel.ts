// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WebviewNotificationCallback, WebviewNotificationMessage } from "@vscode-bicep-ui/messaging";

import {
  DEPLOYMENT_GRAPH_NOTIFICATION,
  READY_NOTIFICATION,
  type DeploymentGraphPayload,
  type DeploymentGraph,
} from "../messages";

const FAKE_FILE_PATH = "file:///main.bicep";

const ZERO_RANGE = {
  start: { line: 0, character: 0 },
  end: { line: 0, character: 0 },
};

// ─── Sample graphs ───────────────────────────────────────────────────────────

/**
 * A module with two child resources, plus two standalone resources
 * and cross-module edges.
 */
const MODULE_GRAPH: DeploymentGraph = {
  nodes: [
    {
      id: "myModule",
      type: "<module>",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: true,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "myModule::vmResource",
      type: "Microsoft.Compute/virtualMachines",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "myModule::storageAccount",
      type: "Microsoft.Storage/storageAccounts",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "networkInterface",
      type: "Microsoft.Network/networkInterfaces",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "publicIp",
      type: "Microsoft.Network/publicIPAddresses",
      isCollection: true,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
  ],
  edges: [
    { sourceId: "myModule::vmResource", targetId: "networkInterface" },
    { sourceId: "networkInterface", targetId: "publicIp" },
    { sourceId: "myModule::storageAccount", targetId: "networkInterface" },
  ],
  errorCount: 0,
};

/** Flat graph with no modules — just four standalone resources in a chain. */
const FLAT_GRAPH: DeploymentGraph = {
  nodes: [
    {
      id: "vnet",
      type: "Microsoft.Network/virtualNetworks",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "subnet",
      type: "Microsoft.Network/virtualNetworks/subnets",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "nsg",
      type: "Microsoft.Network/networkSecurityGroups",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "pip",
      type: "Microsoft.Network/publicIPAddresses",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
  ],
  edges: [
    { sourceId: "subnet", targetId: "vnet" },
    { sourceId: "nsg", targetId: "subnet" },
    { sourceId: "pip", targetId: "nsg" },
  ],
  errorCount: 0,
};

/** Graph containing nodes with errors and a collection. */
const ERROR_GRAPH: DeploymentGraph = {
  nodes: [
    {
      id: "brokenStorage",
      type: "Microsoft.Storage/storageAccounts",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: true,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "webApps",
      type: "Microsoft.Web/sites",
      isCollection: true,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: false,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "badModule",
      type: "<module>",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: true,
      hasError: true,
      filePath: FAKE_FILE_PATH,
    },
    {
      id: "badModule::db",
      type: "Microsoft.Sql/servers",
      isCollection: false,
      range: ZERO_RANGE,
      hasChildren: false,
      hasError: true,
      filePath: FAKE_FILE_PATH,
    },
  ],
  edges: [{ sourceId: "webApps", targetId: "brokenStorage" }],
  errorCount: 3,
};

/**
 * Complex graph modeled after modules-vwan-to-vnet-s2s-with-fw Bicep sample.
 * 2 resource groups, 13 modules with child resources, and rich inter-module dependencies.
 */
const COMPLEX_GRAPH: DeploymentGraph = {
  nodes: [
    // ── Top-level resources ──────────────────────────────────────────────
    { id: "hubrg", type: "Microsoft.Resources/resourceGroups", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vwanrg", type: "Microsoft.Resources/resourceGroups", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vnet module (scope: hubrg) ───────────────────────────────────────
    { id: "vnet", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vnet::servernsg", type: "Microsoft.Network/networkSecurityGroups", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vnet::bastionnsg", type: "Microsoft.Network/networkSecurityGroups", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vnet::vnet", type: "Microsoft.Network/virtualNetworks", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vpngw module (scope: hubrg, depends on: vnet) ────────────────────
    { id: "vpngw", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vpngw::vpngwpip", type: "Microsoft.Network/publicIPAddresses", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vpngw::vpngw", type: "Microsoft.Network/virtualNetworkGateways", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── fwpolicy module (scope: hubrg) ───────────────────────────────────
    { id: "fwpolicy", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "fwpolicy::policy", type: "Microsoft.Network/firewallPolicies", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "fwpolicy::platformrcgroup", type: "Microsoft.Network/firewallPolicies/ruleCollectionGroups", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── fwpip module (scope: hubrg) ──────────────────────────────────────
    { id: "fwpip", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "fwpip::fwipprefix", type: "Microsoft.Network/publicIPPrefixes", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "fwpip::fwip", type: "Microsoft.Network/publicIPAddresses", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── fw module (scope: hubrg, depends on: fwpolicy, fwpip, vnet) ──────
    { id: "fw", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "fw::firewall", type: "Microsoft.Network/azureFirewalls", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vwan module (scope: vwanrg) ──────────────────────────────────────
    { id: "vwan", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vwan::wan", type: "Microsoft.Network/virtualWans", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vhub module (scope: vwanrg, depends on: vwan) ────────────────────
    { id: "vhub", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vhub::hub", type: "Microsoft.Network/virtualHubs", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vhubfwpolicy module (scope: vwanrg) ──────────────────────────────
    { id: "vhubfwpolicy", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vhubfwpolicy::policy", type: "Microsoft.Network/firewallPolicies", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vhubfwpolicy::platformrcgroup", type: "Microsoft.Network/firewallPolicies/ruleCollectionGroups", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vhubfw module (scope: vwanrg, depends on: vhub, vhubfwpolicy) ────
    { id: "vhubfw", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vhubfw::firewall", type: "Microsoft.Network/azureFirewalls", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vhubvpngw module (scope: vwanrg, depends on: vhub) ──────────────
    { id: "vhubvpngw", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vhubvpngw::hubvpngw", type: "Microsoft.Network/vpnGateways", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vwanvpnsite module (scope: vwanrg, depends on: vnet, vpngw, vwan)
    { id: "vwanvpnsite", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vwanvpnsite::vpnsite", type: "Microsoft.Network/vpnSites", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vhubs2s module (scope: vwanrg, depends on: vhubvpngw, vwanvpnsite)
    { id: "vhubs2s", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vhubs2s::hubvpnconnection", type: "Microsoft.Network/vpnGateways/vpnConnections", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },

    // ── vnets2s module (scope: hubrg, depends on: vhub, vhubvpngw, vpngw)
    { id: "vnets2s", type: "<module>", isCollection: false, range: ZERO_RANGE, hasChildren: true, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vnets2s::localnetworkgw", type: "Microsoft.Network/localNetworkGateways", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
    { id: "vnets2s::s2sconnection", type: "Microsoft.Network/connections", isCollection: false, range: ZERO_RANGE, hasChildren: false, hasError: false, filePath: FAKE_FILE_PATH },
  ],
  edges: [
    // Module → resource group (scope dependencies)
    { sourceId: "vnet", targetId: "hubrg" },
    { sourceId: "vpngw", targetId: "hubrg" },
    { sourceId: "fwpolicy", targetId: "hubrg" },
    { sourceId: "fwpip", targetId: "hubrg" },
    { sourceId: "fw", targetId: "hubrg" },
    { sourceId: "vnets2s", targetId: "hubrg" },
    { sourceId: "vwan", targetId: "vwanrg" },
    { sourceId: "vhub", targetId: "vwanrg" },
    { sourceId: "vhubfwpolicy", targetId: "vwanrg" },
    { sourceId: "vhubfw", targetId: "vwanrg" },
    { sourceId: "vhubvpngw", targetId: "vwanrg" },
    { sourceId: "vwanvpnsite", targetId: "vwanrg" },
    { sourceId: "vhubs2s", targetId: "vwanrg" },

    // Inter-module dependencies
    { sourceId: "vpngw", targetId: "vnet" },
    { sourceId: "fw", targetId: "fwpolicy" },
    { sourceId: "fw", targetId: "fwpip" },
    { sourceId: "fw", targetId: "vnet" },
    { sourceId: "vhub", targetId: "vwan" },
    { sourceId: "vhubfw", targetId: "vhub" },
    { sourceId: "vhubfw", targetId: "vhubfwpolicy" },
    { sourceId: "vhubvpngw", targetId: "vhub" },
    { sourceId: "vwanvpnsite", targetId: "vnet" },
    { sourceId: "vwanvpnsite", targetId: "vpngw" },
    { sourceId: "vwanvpnsite", targetId: "vwan" },
    { sourceId: "vhubs2s", targetId: "vhubvpngw" },
    { sourceId: "vhubs2s", targetId: "vwanvpnsite" },
    { sourceId: "vnets2s", targetId: "vhub" },
    { sourceId: "vnets2s", targetId: "vhubvpngw" },
    { sourceId: "vnets2s", targetId: "vpngw" },
  ],
  errorCount: 0,
};

/**
 * Named sample graphs available in the dev toolbar.
 */
export const SAMPLE_GRAPHS: Record<string, DeploymentGraph | null> = {
  "Module graph": MODULE_GRAPH,
  "Flat graph": FLAT_GRAPH,
  "Error graph": ERROR_GRAPH,
  "Complex graph": COMPLEX_GRAPH,
  "Empty (null)": null,
};

// ─── Graph mutations ─────────────────────────────────────────────────────────

export interface GraphMutation {
  label: string;
  description: string;
  apply: (graph: DeploymentGraph) => DeploymentGraph;
}

/** All available mutations for testing incremental updates. */
export const GRAPH_MUTATIONS: GraphMutation[] = [
  {
    label: "+ Add node",
    description: "Append a new resource node and an edge to the first existing node",
    apply: (graph) => {
      const index = graph.nodes.filter((n) => !n.hasChildren).length + 1;
      const newId = `addedResource${index}`;
      const firstNodeId = graph.nodes[0]?.id;
      return {
        ...graph,
        nodes: [
          ...graph.nodes,
          {
            id: newId,
            type: "Microsoft.Web/sites",
            isCollection: false,
            range: ZERO_RANGE,
            hasChildren: false,
            hasError: false,
            filePath: FAKE_FILE_PATH,
          },
        ],
        edges: firstNodeId
          ? [...graph.edges, { sourceId: newId, targetId: firstNodeId }]
          : graph.edges,
      };
    },
  },
  {
    label: "+ Add module",
    description: "Add a new module with a child resource",
    apply: (graph) => {
      const index = graph.nodes.filter((n) => n.type === "<module>").length + 1;
      const moduleId = `newModule${index}`;
      const childId = `${moduleId}::childResource`;
      return {
        ...graph,
        nodes: [
          ...graph.nodes,
          {
            id: moduleId,
            type: "<module>",
            isCollection: false,
            range: ZERO_RANGE,
            hasChildren: true,
            hasError: false,
            filePath: FAKE_FILE_PATH,
          },
          {
            id: childId,
            type: "Microsoft.Storage/storageAccounts",
            isCollection: false,
            range: ZERO_RANGE,
            hasChildren: false,
            hasError: false,
            filePath: FAKE_FILE_PATH,
          },
        ],
      };
    },
  },
  {
    label: "− Remove last node",
    description: "Remove the last atomic node and any edges referencing it",
    apply: (graph) => {
      const atomicNodes = graph.nodes.filter((n) => !n.hasChildren);
      const target = atomicNodes[atomicNodes.length - 1];
      if (!target) return graph;
      return {
        ...graph,
        nodes: graph.nodes.filter((n) => n.id !== target.id),
        edges: graph.edges.filter(
          (e) => e.sourceId !== target.id && e.targetId !== target.id,
        ),
      };
    },
  },
  {
    label: "− Remove first node",
    description: "Remove the first atomic node and any edges referencing it",
    apply: (graph) => {
      const target = graph.nodes.find((n) => !n.hasChildren);
      if (!target) return graph;
      return {
        ...graph,
        nodes: graph.nodes.filter((n) => n.id !== target.id),
        edges: graph.edges.filter(
          (e) => e.sourceId !== target.id && e.targetId !== target.id,
        ),
      };
    },
  },
  {
    label: "− Remove module",
    description: "Remove the last module and all its children, plus any edges referencing them",
    apply: (graph) => {
      // Find the last module node
      const modules = graph.nodes.filter((n) => n.type === "<module>");
      const target = modules[modules.length - 1];
      if (!target) return graph;
      // Collect IDs to remove: the module itself + any node whose ID starts with "moduleId::"
      const removedIds = new Set(
        graph.nodes
          .filter((n) => n.id === target.id || n.id.startsWith(`${target.id}::`))
          .map((n) => n.id),
      );
      return {
        ...graph,
        nodes: graph.nodes.filter((n) => !removedIds.has(n.id)),
        edges: graph.edges.filter(
          (e) => !removedIds.has(e.sourceId) && !removedIds.has(e.targetId),
        ),
      };
    },
  },
  {
    label: "Rename node",
    description: "Rename the first atomic node's ID (simulating a symbolic name change)",
    apply: (graph) => {
      const target = graph.nodes.find((n) => !n.hasChildren);
      if (!target) return graph;
      const newId = `${target.id}_renamed`;
      return {
        ...graph,
        nodes: graph.nodes.map((n) =>
          n.id === target.id ? { ...n, id: newId } : n,
        ),
        edges: graph.edges.map((e) => ({
          sourceId: e.sourceId === target.id ? newId : e.sourceId,
          targetId: e.targetId === target.id ? newId : e.targetId,
        })),
      };
    },
  },
  {
    label: "Toggle error",
    description: "Toggle hasError on the first atomic node",
    apply: (graph) => {
      const target = graph.nodes.find((n) => !n.hasChildren);
      if (!target) return graph;
      return {
        ...graph,
        nodes: graph.nodes.map((n) =>
          n.id === target.id ? { ...n, hasError: !n.hasError } : n,
        ),
        errorCount: target.hasError
          ? Math.max(0, graph.errorCount - 1)
          : graph.errorCount + 1,
      };
    },
  },
  {
    label: "Toggle collection",
    description: "Toggle isCollection on the first atomic node",
    apply: (graph) => {
      const target = graph.nodes.find((n) => !n.hasChildren);
      if (!target) return graph;
      return {
        ...graph,
        nodes: graph.nodes.map((n) =>
          n.id === target.id ? { ...n, isCollection: !n.isCollection } : n,
        ),
      };
    },
  },
  {
    label: "+ Add edge",
    description: "Add an edge between two random unconnected atomic nodes",
    apply: (graph) => {
      const atomicIds = graph.nodes
        .filter((n) => !n.hasChildren)
        .map((n) => n.id);
      const existingEdgeKeys = new Set(
        graph.edges.map((e) => `${e.sourceId}->${e.targetId}`),
      );
      for (const src of atomicIds) {
        for (const tgt of atomicIds) {
          if (src !== tgt && !existingEdgeKeys.has(`${src}->${tgt}`)) {
            return {
              ...graph,
              edges: [...graph.edges, { sourceId: src, targetId: tgt }],
            };
          }
        }
      }
      return graph;
    },
  },
  {
    label: "− Remove edge",
    description: "Remove the last edge",
    apply: (graph) => ({
      ...graph,
      edges: graph.edges.slice(0, -1),
    }),
  },
];

/**
 * A fake message channel that simulates the VS Code extension host
 * for dev-mode usage. When the webview sends the "ready" notification,
 * it replies asynchronously with a sample deployment graph — the same
 * flow that happens in production.
 *
 * Also exposes {@link pushGraph} so dev toolbar buttons can simulate
 * the extension host pushing new graphs at any time.
 */
export class FakeMessageChannel {
  private readonly notificationSubscriptions: Record<string, Set<WebviewNotificationCallback>> = {};

  revive() {
    // no-op
  }

  dispose() {
    // no-op
  }

  sendRequest<T>(): Promise<T> {
    return Promise.reject(new Error("FakeMessageChannel does not support requests."));
  }

  /** The last graph pushed, so mutations can build on top of it. */
  private currentGraph: DeploymentGraph | null = null;

  sendNotification(notificationMessage: WebviewNotificationMessage) {
    if (notificationMessage.method === READY_NOTIFICATION) {
      // Simulate async response from the extension host:
      // after a short delay, push the sample deployment graph.
      setTimeout(() => {
        this.pushGraph(MODULE_GRAPH);
      }, 50);
    }
  }

  /** Returns the most recently pushed graph (for mutations). */
  getCurrentGraph(): DeploymentGraph | null {
    return this.currentGraph;
  }

  /**
   * Simulate the extension host sending a new deployment graph
   * notification to the webview.
   */
  pushGraph(graph: DeploymentGraph | null) {
    this.currentGraph = graph;
    this.dispatchNotification(DEPLOYMENT_GRAPH_NOTIFICATION, {
      documentPath: FAKE_FILE_PATH,
      deploymentGraph: graph,
    } satisfies DeploymentGraphPayload);
  }

  subscribeToNotification(method: string, callback: WebviewNotificationCallback) {
    this.notificationSubscriptions[method] ??= new Set();
    this.notificationSubscriptions[method].add(callback);
  }

  unsubscribeFromNotification(method: string, callback: WebviewNotificationCallback) {
    this.notificationSubscriptions[method]?.delete(callback);
  }

  private dispatchNotification(method: string, params: unknown) {
    const callbacks = this.notificationSubscriptions[method];
    if (callbacks) {
      for (const callback of callbacks) {
        callback(params);
      }
    }
  }
}
