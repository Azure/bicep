// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNodeSourceStatus } from "@/core";

/**
 * The dev playground's stand-in for the document a language server would compile. The dev panel
 * switches between and mutates these, and `toGraph` turns one into the graph the protocol carries.
 *
 * Only the fields the graph needs are modelled; the rest are derived from the id and type.
 */
export interface SampleGraph {
  nodes: SampleGraphNode[];
  edges: SampleGraphEdge[];
  errorCount: number;
}

export interface SampleGraphNode {
  id: string;
  type: string;
  isCollection: boolean;
  hasChildren: boolean;
  hasError: boolean;
  /** The container, when it is not the module the id names, as for a resource inside its parent resource. */
  parentId?: string | null;
  /** Where the fake layout puts the node's top-left corner, instead of stacking it. */
  position?: { x: number; y: number };
  /** A module from a registry or template spec. */
  isRemote?: boolean;
  /** Why a remote module shows no children, and what went wrong when that is an error. */
  sourceStatus?: GraphNodeSourceStatus;
  sourceError?: string;
  /** The child nodes, and the edges between them, that restoring a registry module supplies. */
  restoredSubgraph?: { nodes: SampleGraphNode[]; edges: SampleGraphEdge[] };
}

export interface SampleGraphEdge {
  sourceId: string;
  targetId: string;
}

// ─── Sample graphs ───────────────────────────────────────────────────────────

/**
 * A module with two child resources, plus two standalone resources.
 * Edges only connect nodes within the same scope (no cross-boundary edges).
 */
export const MODULE_GRAPH: SampleGraph = {
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
 * Registry modules as the language server shows them: one expanded from its published source, with a nested
 * local module; one that is not restored yet, which expanding restores; and one whose source cannot be shown.
 */
const REMOTE_MODULE_GRAPH: SampleGraph = {
  nodes: [
    { id: "storage", type: "<module>", isCollection: false, hasChildren: true, hasError: false, isRemote: true },
    {
      id: "storage::account",
      type: "Microsoft.Storage/storageAccounts",
      isCollection: false,
      hasChildren: false,
      hasError: false,
    },
    { id: "storage::blobs", type: "<module>", isCollection: false, hasChildren: true, hasError: false },
    {
      id: "storage::blobs::container",
      type: "Microsoft.Storage/storageAccounts/blobServices/containers",
      isCollection: true,
      hasChildren: false,
      hasError: false,
    },
    {
      id: "network",
      type: "<module>",
      isCollection: false,
      hasChildren: false,
      hasError: false,
      sourceStatus: "unavailable",
      isRemote: true,
    },
    {
      id: "monitoring",
      type: "<module>",
      isCollection: false,
      hasChildren: false,
      hasError: false,
      sourceStatus: "unrestored",
      isRemote: true,
      restoredSubgraph: {
        nodes: [
          {
            id: "monitoring::workspace",
            type: "Microsoft.OperationalInsights/workspaces",
            isCollection: false,
            hasChildren: false,
            hasError: false,
          },
          {
            id: "monitoring::alerts",
            type: "Microsoft.Insights/metricAlerts",
            isCollection: true,
            hasChildren: false,
            hasError: false,
          },
          // A local module in the published source, so a container arrives with the restore too.
          { id: "monitoring::diagnostics", type: "<module>", isCollection: false, hasChildren: true, hasError: false },
          {
            id: "monitoring::diagnostics::settings",
            type: "Microsoft.Insights/diagnosticSettings",
            isCollection: false,
            hasChildren: false,
            hasError: false,
          },
        ],
        edges: [
          { sourceId: "monitoring::alerts", targetId: "monitoring::workspace" },
          { sourceId: "monitoring::diagnostics", targetId: "monitoring::workspace" },
        ],
      },
    },
  ],
  edges: [
    { sourceId: "storage::blobs", targetId: "storage::account" },
    { sourceId: "monitoring", targetId: "storage" },
  ],
  errorCount: 0,
};

/** A sample node with defaults for the fields most samples leave unset. */
function sampleNode(id: string, type: string, extra: Partial<SampleGraphNode> = {}): SampleGraphNode {
  return { id, type, isCollection: false, hasChildren: false, hasError: false, ...extra };
}

/** Child resources inside their parent resource, with dependencies that cross the parent's frame. */
const RESOURCE_CONTAINER_GRAPH: SampleGraph = {
  nodes: [
    sampleNode("vnet", "Microsoft.Network/virtualNetworks", { hasChildren: true }),
    sampleNode("subnetApp", "Microsoft.Network/virtualNetworks/subnets", {
      parentId: "vnet",
      position: { x: 0, y: 0 },
    }),
    sampleNode("subnetData", "Microsoft.Network/virtualNetworks/subnets", {
      parentId: "vnet",
      position: { x: 300, y: 0 },
    }),
    sampleNode("nsg", "Microsoft.Network/networkSecurityGroups", { position: { x: 680, y: 0 } }),
    sampleNode("nic", "Microsoft.Network/networkInterfaces", { position: { x: 0, y: 230 } }),
    sampleNode("vm", "Microsoft.Compute/virtualMachines", { position: { x: 300, y: 230 } }),
    sampleNode("monitoring", "<module>", { hasChildren: true }),
    sampleNode("monitoring::workspace", "Microsoft.OperationalInsights/workspaces", { position: { x: 680, y: 230 } }),
  ],
  edges: [
    { sourceId: "nic", targetId: "subnetApp" },
    { sourceId: "subnetData", targetId: "nsg" },
    { sourceId: "vm", targetId: "nic" },
  ],
  errorCount: 0,
};

/** Modules and an existing resource scoped to a resource group, with a VNet and its subnets inside a module. */
const RESOURCE_GROUP_CONTAINER_GRAPH: SampleGraph = {
  nodes: [
    sampleNode("rg", "Microsoft.Resources/resourceGroups", { hasChildren: true }),
    sampleNode("network", "<module>", { parentId: "rg", hasChildren: true }),
    sampleNode("network::vnet", "Microsoft.Network/virtualNetworks", { parentId: "network", hasChildren: true }),
    sampleNode("network::subnetApp", "Microsoft.Network/virtualNetworks/subnets", {
      parentId: "network::vnet",
      position: { x: 0, y: 0 },
    }),
    sampleNode("network::subnetData", "Microsoft.Network/virtualNetworks/subnets", {
      parentId: "network::vnet",
      position: { x: 300, y: 0 },
    }),
    sampleNode("app", "<module>", { parentId: "rg", isRemote: true, position: { x: 720, y: -100 } }),
    sampleNode("keyVault", "Microsoft.KeyVault/vaults", { parentId: "rg", position: { x: 720, y: 80 } }),
  ],
  edges: [
    { sourceId: "app", targetId: "network" },
    { sourceId: "app", targetId: "keyVault" },
  ],
  errorCount: 0,
};

/** Two levels of child resources, an error on the middle one, and a dependency crossing both frames. */
const NESTED_RESOURCE_CONTAINER_GRAPH: SampleGraph = {
  nodes: [
    sampleNode("storage", "Microsoft.Storage/storageAccounts", { hasChildren: true }),
    sampleNode("blobService", "Microsoft.Storage/storageAccounts/blobServices", {
      parentId: "storage",
      hasChildren: true,
      hasError: true,
    }),
    sampleNode("logs", "Microsoft.Storage/storageAccounts/blobServices/containers", {
      parentId: "blobService",
      position: { x: 0, y: 0 },
    }),
    sampleNode("uploads", "Microsoft.Storage/storageAccounts/blobServices/containers", {
      parentId: "blobService",
      position: { x: 300, y: 0 },
    }),
    sampleNode("worker", "Microsoft.Compute/virtualMachines", { position: { x: 720, y: 0 } }),
  ],
  edges: [{ sourceId: "worker", targetId: "uploads" }],
  errorCount: 1,
};

/**
 * Named sample graphs available in the dev panel.
 */
export const SAMPLE_GRAPHS: Record<string, SampleGraph | null> = {
  "Module graph": MODULE_GRAPH,
  "Flat graph": FLAT_GRAPH,
  "Error graph": ERROR_GRAPH,
  "Complex graph": COMPLEX_GRAPH,
  "Remote module graph": REMOTE_MODULE_GRAPH,
  "Resource containers": RESOURCE_CONTAINER_GRAPH,
  "Resource group containers": RESOURCE_GROUP_CONTAINER_GRAPH,
  "Nested resource containers": NESTED_RESOURCE_CONTAINER_GRAPH,
  "Empty (null)": null,
};

// ─── Graph mutations ─────────────────────────────────────────────────────────

/** Returns the scope (parent prefix) of a node id, or "" for top-level nodes. */
function getScope(id: string): string {
  const idx = id.lastIndexOf("::");
  return idx === -1 ? "" : id.slice(0, idx);
}

export interface GraphMutation {
  /** Where the dev panel lists the mutation. */
  group: "Nodes" | "Modules" | "Edges";
  /** The whole name, such as "Add node", for screen readers and test IDs. */
  label: string;
  /** What the dev panel shows within its group, such as "Add". */
  action: string;
  /** The Codicon shown with the action. */
  icon: string;
  description: string;
  apply: (graph: SampleGraph) => SampleGraph;
}

/** All available mutations for testing incremental updates. */
export const GRAPH_MUTATIONS: GraphMutation[] = [
  {
    group: "Nodes",
    label: "Add node",
    action: "Add",
    icon: "add",
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
    group: "Modules",
    label: "Add module",
    action: "Add",
    icon: "add",
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
    group: "Nodes",
    label: "Remove last node",
    action: "Remove last",
    icon: "trash",
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
    group: "Nodes",
    label: "Remove first node",
    action: "Remove first",
    icon: "trash",
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
    group: "Modules",
    label: "Remove module",
    action: "Remove",
    icon: "trash",
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
    group: "Nodes",
    label: "Rename node",
    action: "Rename",
    icon: "edit",
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
    group: "Nodes",
    label: "Toggle error",
    action: "Error",
    icon: "error",
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
    group: "Nodes",
    label: "Toggle collection",
    action: "Collection",
    icon: "layers",
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
    group: "Edges",
    label: "Add edge",
    action: "Add",
    icon: "add",
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
    group: "Edges",
    label: "Remove edge",
    action: "Remove",
    icon: "trash",
    description: "Remove the last edge",
    apply: (graph) => ({
      ...graph,
      edges: graph.edges.slice(0, -1),
    }),
  },
];
