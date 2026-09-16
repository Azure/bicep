// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type {
  MessageArgs,
  NotificationDescriptor,
  RequestDescriptor,
  WebviewMessageChannelApi,
  WebviewNotificationCallback,
  WebviewNotificationMessage,
} from "@vscode-bicep-ui/messaging";
import type {
  CreateResourceParams,
  CreateResourceResult,
  GetGraphLayoutParams,
  GetGraphLayoutResult,
  GetGraphUpdateParams,
  GetGraphUpdateResult,
} from "@/features/canvas";
import type { SampleGraph } from "./sample-graph";

// The fake host implements the whole protocol, so it is the one legitimate consumer of every
// feature's `api` surface.
import { createResource, getGraphLayout, getGraphUpdate, revealNodeSource } from "@/features/canvas";
import { getResourceCreationEnablement, getResourceTypeNamespaces, loadResourceTypeCatalog } from "@/features/palette";
import { showProblemsPanel } from "@/features/status";
import { documentDidChange, getMotionPolicy, ready } from "@/hooks";
import { diffGraph, layoutGraph } from "./fake-graph-differ";

const FAKE_FILE_PATH = "file:///main.bicep";

// ─── Sample graphs ───────────────────────────────────────────────────────────

/**
 * A module with two child resources, plus two standalone resources.
 * Edges only connect nodes within the same scope (no cross-boundary edges).
 */
const MODULE_GRAPH: SampleGraph = {
  nodes: [
    {
      id: "myModule",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "myModule::vmResource",
      type: "Microsoft.Compute/virtualMachines",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "myModule::storageAccount",
      type: "Microsoft.Storage/storageAccounts",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "networkInterface",
      type: "Microsoft.Network/networkInterfaces",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "publicIp",
      type: "Microsoft.Network/publicIPAddresses",
      isCollection: true,
      hasChildren: false,
      hasError: false,
    },
  ],
  edges: [
    // Outer edges: module and resources at the same (top-level) scope
    { sourceId: "myModule", targetId: "networkInterface" },
    { sourceId: "networkInterface", targetId: "publicIp" },
    // Inner edge: resources within the same module scope
    { sourceId: "myModule::vmResource", targetId: "myModule::storageAccount" },
  ],
  errorCount: 0,
};

/** Flat graph with no modules — just four standalone resources in a chain. */
const FLAT_GRAPH: SampleGraph = {
  nodes: [
    {
      id: "vnet",
      type: "Microsoft.Network/virtualNetworks",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "subnet",
      type: "Microsoft.Network/virtualNetworks/subnets",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "nsg",
      type: "Microsoft.Network/networkSecurityGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "pip",
      type: "Microsoft.Network/publicIPAddresses",
      isCollection: false,
      hasChildren: false,
      hasError: false,
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
const ERROR_GRAPH: SampleGraph = {
  nodes: [
    {
      id: "brokenStorage",
      type: "Microsoft.Storage/storageAccounts",
      isCollection: false,
      hasChildren: false,
      hasError: true,
    },
    {
      id: "webApps",
      type: "Microsoft.Web/sites",
      isCollection: true,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "badModule",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: true,
    },
    {
      id: "badModule::db",
      type: "Microsoft.Sql/servers",
      isCollection: false,
      hasChildren: false,
      hasError: true,
    },
  ],
  edges: [{ sourceId: "webApps", targetId: "brokenStorage" }],
  errorCount: 3,
};

/**
 * Complex graph modeled after modules-vwan-to-vnet-s2s-with-fw Bicep sample.
 * 2 resource groups, 13 modules with child resources, and rich inter-module dependencies.
 */
const COMPLEX_GRAPH: SampleGraph = {
  nodes: [
    // ── Top-level resources ──────────────────────────────────────────────
    {
      id: "hubrg",
      type: "Microsoft.Resources/resourceGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "vwanrg",
      type: "Microsoft.Resources/resourceGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vnet module (scope: hubrg) ───────────────────────────────────────
    {
      id: "vnet",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vnet::servernsg",
      type: "Microsoft.Network/networkSecurityGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "vnet::bastionnsg",
      type: "Microsoft.Network/networkSecurityGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "vnet::vnet",
      type: "Microsoft.Network/virtualNetworks",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vpngw module (scope: hubrg, depends on: vnet) ────────────────────
    {
      id: "vpngw",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vpngw::vpngwpip",
      type: "Microsoft.Network/publicIPAddresses",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "vpngw::vpngw",
      type: "Microsoft.Network/virtualNetworkGateways",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── fwpolicy module (scope: hubrg) ───────────────────────────────────
    {
      id: "fwpolicy",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "fwpolicy::policy",
      type: "Microsoft.Network/firewallPolicies",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "fwpolicy::platformrcgroup",
      type: "Microsoft.Network/firewallPolicies/ruleCollectionGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── fwpip module (scope: hubrg) ──────────────────────────────────────
    {
      id: "fwpip",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "fwpip::fwipprefix",
      type: "Microsoft.Network/publicIPPrefixes",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "fwpip::fwip",
      type: "Microsoft.Network/publicIPAddresses",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── fw module (scope: hubrg, depends on: fwpolicy, fwpip, vnet) ──────
    {
      id: "fw",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "fw::firewall",
      type: "Microsoft.Network/azureFirewalls",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vwan module (scope: vwanrg) ──────────────────────────────────────
    {
      id: "vwan",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vwan::wan",
      type: "Microsoft.Network/virtualWans",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vhub module (scope: vwanrg, depends on: vwan) ────────────────────
    {
      id: "vhub",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vhub::hub",
      type: "Microsoft.Network/virtualHubs",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vhubfwpolicy module (scope: vwanrg) ──────────────────────────────
    {
      id: "vhubfwpolicy",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vhubfwpolicy::policy",
      type: "Microsoft.Network/firewallPolicies",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "vhubfwpolicy::platformrcgroup",
      type: "Microsoft.Network/firewallPolicies/ruleCollectionGroups",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vhubfw module (scope: vwanrg, depends on: vhub, vhubfwpolicy) ────
    {
      id: "vhubfw",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vhubfw::firewall",
      type: "Microsoft.Network/azureFirewalls",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vhubvpngw module (scope: vwanrg, depends on: vhub) ──────────────
    {
      id: "vhubvpngw",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vhubvpngw::hubvpngw",
      type: "Microsoft.Network/vpnGateways",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vwanvpnsite module (scope: vwanrg, depends on: vnet, vpngw, vwan)
    {
      id: "vwanvpnsite",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vwanvpnsite::vpnsite",
      type: "Microsoft.Network/vpnSites",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vhubs2s module (scope: vwanrg, depends on: vhubvpngw, vwanvpnsite)
    {
      id: "vhubs2s",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vhubs2s::hubvpnconnection",
      type: "Microsoft.Network/vpnGateways/vpnConnections",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },

    // ── vnets2s module (scope: hubrg, depends on: vhub, vhubvpngw, vpngw)
    {
      id: "vnets2s",
      type: "<module>",
      isCollection: false,
      hasChildren: true,
      hasError: false,
    },
    {
      id: "vnets2s::localnetworkgw",
      type: "Microsoft.Network/localNetworkGateways",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "vnets2s::s2sconnection",
      type: "Microsoft.Network/connections",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
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

    // Inter-module dependencies (same top-level scope)
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

    // Inner edges (resources within the same module scope)
    { sourceId: "vnet::vnet", targetId: "vnet::servernsg" },
    { sourceId: "vnet::vnet", targetId: "vnet::bastionnsg" },
    { sourceId: "vpngw::vpngw", targetId: "vpngw::vpngwpip" },
    { sourceId: "fwpolicy::platformrcgroup", targetId: "fwpolicy::policy" },
    { sourceId: "fwpip::fwip", targetId: "fwpip::fwipprefix" },
    { sourceId: "vhubfwpolicy::platformrcgroup", targetId: "vhubfwpolicy::policy" },
    { sourceId: "vnets2s::s2sconnection", targetId: "vnets2s::localnetworkgw" },
  ],
  errorCount: 0,
};

/**
 * Named sample graphs available in the dev toolbar.
 */
export const SAMPLE_GRAPHS: Record<string, SampleGraph | null> = {
  "Module graph": MODULE_GRAPH,
  "Flat graph": FLAT_GRAPH,
  "Error graph": ERROR_GRAPH,
  "Complex graph": COMPLEX_GRAPH,
  "Empty (null)": null,
};

// ─── Graph mutations ─────────────────────────────────────────────────────────

/** Returns the scope (parent prefix) of a node id, or "" for top-level nodes. */
function getScope(id: string): string {
  const idx = id.lastIndexOf("::");
  return idx === -1 ? "" : id.slice(0, idx);
}

/**
 * Resource-catalog responses are deliberately delayed so the dev shell exercises loading states.
 * The `catalogDelay` query parameter overrides that delay (in milliseconds) so end-to-end tests can
 * hold the loading state open long enough to assert on it instead of racing the default timing.
 */
function getCatalogDelayMs(defaultDelayMs: number): number {
  const raw = new URLSearchParams(window.location.search).get("catalogDelay");

  if (raw === null) {
    return defaultDelayMs;
  }

  const override = Number(raw);

  return Number.isFinite(override) && override >= 0 ? override : defaultDelayMs;
}

export interface GraphMutation {
  label: string;
  description: string;
  apply: (graph: SampleGraph) => SampleGraph;
}

/** All available mutations for testing incremental updates. */
export const GRAPH_MUTATIONS: GraphMutation[] = [
  {
    label: "+\u00a0Add node",
    description: "Append a new top-level resource node and an edge to the first top-level node",
    apply: (graph) => {
      const index = graph.nodes.filter((n) => !n.hasChildren).length + 1;
      const newId = `addedResource${index}`;
      // Only connect to a top-level node (same scope)
      const firstTopLevel = graph.nodes.find((n) => getScope(n.id) === "");
      return {
        ...graph,
        nodes: [
          ...graph.nodes,
          {
            id: newId,
            type: "Microsoft.Web/sites",
            isCollection: false,
            hasChildren: false,
            hasError: false,
          },
        ],
        edges: firstTopLevel ? [...graph.edges, { sourceId: newId, targetId: firstTopLevel.id }] : graph.edges,
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
            hasChildren: true,
            hasError: false,
          },
          {
            id: childId,
            type: "Microsoft.Storage/storageAccounts",
            isCollection: false,
            hasChildren: false,
            hasError: false,
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
        edges: graph.edges.filter((e) => e.sourceId !== target.id && e.targetId !== target.id),
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
        edges: graph.edges.filter((e) => e.sourceId !== target.id && e.targetId !== target.id),
      };
    },
  },
  {
    label: "− Remove module",
    description: "Remove the last module and all its children, plus any edges referencing them",
    apply: (graph) => {
      const modules = graph.nodes.filter((n) => n.type === "<module>");
      const target = modules[modules.length - 1];
      if (!target) return graph;
      const removedIds = new Set(
        graph.nodes.filter((n) => n.id === target.id || n.id.startsWith(`${target.id}::`)).map((n) => n.id),
      );
      return {
        ...graph,
        nodes: graph.nodes.filter((n) => !removedIds.has(n.id)),
        edges: graph.edges.filter((e) => !removedIds.has(e.sourceId) && !removedIds.has(e.targetId)),
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
        nodes: graph.nodes.map((n) => (n.id === target.id ? { ...n, id: newId } : n)),
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
        nodes: graph.nodes.map((n) => (n.id === target.id ? { ...n, hasError: !n.hasError } : n)),
        errorCount: target.hasError ? Math.max(0, graph.errorCount - 1) : graph.errorCount + 1,
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
        nodes: graph.nodes.map((n) => (n.id === target.id ? { ...n, isCollection: !n.isCollection } : n)),
      };
    },
  },
  {
    label: "+\u00a0Add edge",
    description: "Add an edge between two unconnected nodes in the same scope",
    apply: (graph) => {
      const nodeIds = graph.nodes.filter((n) => !n.hasChildren).map((n) => n.id);
      const moduleIds = graph.nodes.filter((n) => n.hasChildren).map((n) => n.id);
      const allIds = [...nodeIds, ...moduleIds];
      const existingEdgeKeys = new Set(
        graph.edges.flatMap((e) => [`${e.sourceId}->${e.targetId}`, `${e.targetId}->${e.sourceId}`]),
      );
      for (const src of allIds) {
        for (const tgt of allIds) {
          if (src !== tgt && getScope(src) === getScope(tgt) && !existingEdgeKeys.has(`${src}->${tgt}`)) {
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
 * A fake message channel that simulates the VS Code extension host for dev-mode usage.
 * Graph changes are announced with `documentDidChange`; the webview then pulls patch
 * and layout responses through the same request flow used in production.
 */
export class FakeMessageChannel implements WebviewMessageChannelApi {
  private readonly notificationSubscriptions: Record<string, Set<WebviewNotificationCallback>> = {};
  private readonly onWindowMessage = (event: MessageEvent) => {
    if (
      typeof event.data === "object" &&
      event.data !== null &&
      "method" in event.data &&
      typeof event.data.method === "string"
    ) {
      this.dispatchNotification(event.data.method, "params" in event.data ? event.data.params : undefined);
    }
  };

  constructor() {
    window.addEventListener("message", this.onWindowMessage);
  }

  revive() {
    window.addEventListener("message", this.onWindowMessage);
  }

  dispose() {
    window.removeEventListener("message", this.onWindowMessage);
  }

  sendRequest<T>(requestMessage: { method: string; params?: unknown }): Promise<T> {
    if (requestMessage.method === getMotionPolicy.method) {
      return Promise.resolve("animate" as T);
    }

    if (requestMessage.method === getResourceCreationEnablement.method) {
      return Promise.resolve((new URLSearchParams(window.location.search).get("resourceCreation") !== "false") as T);
    }

    const resourceTypeCatalog = [
      {
        group: "Microsoft.Storage",
        resourceTypes: [{ resourceType: "storageAccounts", apiVersion: "2025-01-01" }],
      },
      {
        group: "Microsoft.Network",
        resourceTypes: [{ resourceType: "virtualNetworks", apiVersion: "2024-07-01" }],
      },
    ];

    if (requestMessage.method === getResourceTypeNamespaces.method) {
      return new Promise<T>((resolve) => {
        setTimeout(
          () =>
            resolve({
              catalogId: "dev-catalog",
              namespaces: resourceTypeCatalog.map((group) => ({
                name: group.group,
                resourceTypeCount: group.resourceTypes.length,
              })),
            } as T),
          150,
        );
      });
    }

    if (requestMessage.method === loadResourceTypeCatalog.method) {
      const { providerNamespace, query, loadAll } = (requestMessage.params ?? {}) as {
        providerNamespace?: string;
        query?: string;
        loadAll?: boolean;
      };
      const normalizedQuery = query?.toLocaleLowerCase();
      const groups = resourceTypeCatalog
        .filter((group) => !providerNamespace || group.group === providerNamespace)
        .map((group) => ({
          ...group,
          resourceTypes: group.resourceTypes.filter(
            (resourceType) =>
              !normalizedQuery ||
              `${group.group}/${resourceType.resourceType}`.toLocaleLowerCase().includes(normalizedQuery),
          ),
        }))
        .filter((group) => group.resourceTypes.length > 0);

      return new Promise<T>((resolve) => {
        setTimeout(() => resolve({ catalogId: "dev-catalog", groups } as T), getCatalogDelayMs(loadAll ? 600 : 200));
      });
    }

    if (requestMessage.method === getGraphUpdate.method) {
      const { current } = requestMessage.params as GetGraphUpdateParams;
      const patches = diffGraph(current, this.currentGraph);
      return Promise.resolve({ patches } as GetGraphUpdateResult as T);
    }

    if (requestMessage.method === getGraphLayout.method) {
      const { current } = requestMessage.params as GetGraphLayoutParams;
      const patches = layoutGraph(current, this.currentGraph);
      const result: GetGraphLayoutResult = patches
        ? { status: "ok", patches }
        : { status: "graphChanged", patches: [] };

      return Promise.resolve(result as T);
    }

    if (requestMessage.method === createResource.method) {
      const request = requestMessage.params as CreateResourceParams;
      const current = this.currentGraph ?? { nodes: [], edges: [], errorCount: 0 };
      const baseName = request.resourceType.fullyQualifiedType.split("/").slice(-1)[0]?.replace(/s$/, "") ?? "resource";
      let symbolicName = baseName.charAt(0).toLocaleLowerCase() + baseName.slice(1);
      let suffix = 1;
      const existingIds = new Set(current.nodes.map((node) => node.id));
      while (existingIds.has(symbolicName)) {
        symbolicName = `${baseName}${suffix}`;
        suffix++;
      }

      return new Promise<T>((resolve) => {
        setTimeout(() => {
          this.pushGraph({
            ...current,
            nodes: [
              ...current.nodes,
              {
                id: symbolicName,
                type: request.resourceType.fullyQualifiedType,
                isCollection: false,
                hasChildren: false,
                hasError: true,
              },
            ],
          });

          resolve({
            version: 1,
            operationId: request.operationId,
            expectedNodeId: symbolicName,
            symbolicName,
            unresolvedRequiredProperties: ["name"],
          } satisfies CreateResourceResult as T);
        }, 300);
      });
    }

    return Promise.reject(new Error(`FakeMessageChannel does not support request: ${requestMessage.method}`));
  }

  /** The last graph pushed, so mutations can build on top of it. */
  private currentGraph: SampleGraph | null = null;

  sendNotification(notificationMessage: WebviewNotificationMessage) {
    if (notificationMessage.method === ready.method) {
      // Simulate async response from the extension host:
      // after a short delay, present the sample deployment graph.
      setTimeout(() => {
        this.pushGraph(MODULE_GRAPH);
      }, 50);
    } else if (notificationMessage.method === revealNodeSource.method) {
      // The real host would resolve the node's source location via the language server and reveal it.
      console.log("[FakeMessageChannel] revealNodeSource:", notificationMessage.params);
    } else if (notificationMessage.method === showProblemsPanel.method) {
      console.log("[FakeMessageChannel] showProblemsPanel: would open VS Code Problems panel");
    }
  }

  request<TParams, TResult>(
    descriptor: RequestDescriptor<TParams, TResult>,
    ...args: MessageArgs<TParams>
  ): Promise<TResult> {
    return this.sendRequest<TResult>({ method: descriptor.method, params: args[0] });
  }

  notify<TParams>(descriptor: NotificationDescriptor<TParams>, ...args: MessageArgs<TParams>): void {
    this.sendNotification({ method: descriptor.method, params: args[0] });
  }

  setState<T>(state: T): T {
    return state;
  }

  /** Returns the most recently pushed graph (for mutations). */
  getCurrentGraph(): SampleGraph | null {
    return this.currentGraph;
  }

  /** Simulate the extension host announcing that the graph may have changed. */
  pushGraph(graph: SampleGraph | null) {
    this.currentGraph = graph;
    this.dispatchNotification(documentDidChange.method, {
      documentUri: FAKE_FILE_PATH,
    });
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
