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
  GetGraphParams,
  GetGraphResult,
  LayoutGraphParams,
  LayoutGraphResult,
  ReplaySourceStepParams,
  ReplaySourceStepResult,
  RestoreModuleSourceParams,
  RestoreModuleSourceResult,
  Settings,
  SourceStepReference,
  TargetScope,
} from "@/core";
import type { ListResourceTypesParams, ListResourceTypesResult } from "@/features/palette";
import type { SampleGraph, SampleGraphNode } from "./sample-graphs";

// The fake host implements the whole protocol, so it is the one legitimate consumer of the core's
// and every feature's `api` surface.
import {
  createResource,
  documentDidChange,
  getGraph,
  layoutGraph as layoutGraphRequest,
  ready,
  replaySourceStep,
  restoreModuleSource,
  revealNode,
  settingsDidChange,
} from "@/core";
import { getResourceTypeVersions, listResourceTypes } from "@/features/palette";
import { showProblems } from "@/features/status";
import { layoutGraph, toGraph } from "./fake-graph";
import { MODULE_GRAPH } from "./sample-graphs";
import { getCatalogDelayMs, getSampleApiVersions, getSampleResourceTypes } from "./sample-resource-types";

const FAKE_FILE_PATH = "file:///main.bicep";

/** A delay in milliseconds from the page's query string, or `fallback` when it is absent or not a valid delay. */
function getDelayParam(name: string, fallback: number): number {
  const delay = Number(new URLSearchParams(window.location.search).get(name) ?? fallback);
  return Number.isFinite(delay) && delay >= 0 ? delay : fallback;
}

/**
 * A fake message channel that simulates the VS Code extension host for dev-mode usage.
 * Graph changes are announced with `document/didChange`; the webview then pulls the graph
 * and its layout through the same request flow used in production.
 */
export class FakeMessageChannel implements WebviewMessageChannelApi {
  /** Resources created through the designer, keyed by operation ID, and whether each is currently in source. */
  private readonly createdResources = new Map<string, { node: SampleGraphNode; isInSource: boolean }>();
  private catalogRevision = 0;
  private versionRequestCount = 0;
  private restoreRequestCount = 0;
  private restoreRefreshFailuresRemaining = 0;
  private moduleLayoutRequestCount = 0;
  private targetScope: TargetScope = "resourceGroup";
  private colorThemeMatched = new URLSearchParams(window.location.search).get("matchColorTheme") === "true";
  /** Set by `skipGraphUpdateAfterUndo`: graph updates omit an undone creation until the graph changes again. */
  private isWithholdingUndoneRemoval = false;
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
    const params = new URLSearchParams(window.location.search);
    const scope = params.get("targetScope");
    if (scope === "subscription" || scope === "managementGroup" || scope === "tenant") {
      this.targetScope = scope;
    }
    window.addEventListener("message", this.onWindowMessage);
  }

  revive() {
    window.addEventListener("message", this.onWindowMessage);
  }

  dispose() {
    window.removeEventListener("message", this.onWindowMessage);
  }

  sendRequest<T>(requestMessage: { method: string; params?: unknown }): Promise<T> {
    const catalogId = `dev-catalog-${this.catalogRevision}-${this.targetScope}`;
    if (requestMessage.method === listResourceTypes.method) {
      const { knownCatalogId } = (requestMessage.params ?? {}) as ListResourceTypesParams;
      const result: ListResourceTypesResult = {
        catalogId,
        resourceTypes:
          knownCatalogId === catalogId ? null : getSampleResourceTypes(this.catalogRevision, this.targetScope),
      };

      return new Promise<T>((resolve) => {
        setTimeout(() => resolve(result as T), getCatalogDelayMs(200));
      });
    }
    if (requestMessage.method === getResourceTypeVersions.method) {
      const { fullyQualifiedType } = requestMessage.params as { fullyQualifiedType: string };
      const params = new URLSearchParams(window.location.search);
      const shouldFail = this.versionRequestCount++ < Number(params.get("versionFailures") ?? 0);
      const delay = Number(params.get("versionsDelay") ?? 200);
      return new Promise<T>((resolve, reject) => {
        setTimeout(
          () => {
            if (shouldFail) {
              reject(new Error("Failed to load API versions."));
            } else {
              resolve({ catalogId, apiVersions: getSampleApiVersions(fullyQualifiedType, this.catalogRevision) } as T);
            }
          },
          Number.isFinite(delay) && delay >= 0 ? delay : 200,
        );
      });
    }

    if (requestMessage.method === getGraph.method) {
      if (this.restoreRefreshFailuresRemaining > 0) {
        this.restoreRefreshFailuresRemaining--;
        return Promise.reject(new Error("The restored module's graph could not be loaded."));
      }
      const { sourceSteps } = requestMessage.params as GetGraphParams;
      const isWithholding =
        this.isWithholdingUndoneRemoval ||
        (new URLSearchParams(window.location.search).get("withholdGraphUpdatesAfterCreation") === "true" &&
          [...this.createdResources.values()].some((creation) => creation.isInSource));
      if (!isWithholding) {
        this.servedGraph = this.currentGraph;
      }

      return Promise.resolve({
        graph: toGraph(this.servedGraph),
        targetScope: this.servedGraph ? this.targetScope : null,
        errorCount: this.servedGraph?.errorCount ?? 0,
        replayableSourceSteps: sourceSteps.filter((step) => this.canReplay(step)),
      } satisfies GetGraphResult as T);
    }

    if (requestMessage.method === layoutGraphRequest.method) {
      const { graph, collapsedNodeIds, rootNodeId } = requestMessage.params as LayoutGraphParams;
      // `moduleLayout=graphChanged` or `moduleLayout=layoutFailed` answers the first layout of a module on its own
      // with that status, to exercise the retry and the fallback to laying out the whole graph.
      const forcedStatus = new URLSearchParams(window.location.search).get("moduleLayout");
      if (
        rootNodeId !== undefined &&
        this.moduleLayoutRequestCount++ === 0 &&
        (forcedStatus === "graphChanged" || forcedStatus === "layoutFailed")
      ) {
        return Promise.resolve({ status: forcedStatus, positions: [], bounds: null } satisfies LayoutGraphResult as T);
      }
      // Laid out against the graph served when the request arrives, like the language server; `layoutDelay` holds the
      // response back, so a local change can land while it is in flight.
      const result = layoutGraph(graph, this.servedGraph, new Set(collapsedNodeIds), rootNodeId);
      return new Promise<T>((resolve) => setTimeout(() => resolve(result as T), getDelayParam("layoutDelay", 0)));
    }
    if (requestMessage.method === createResource.method) {
      if (new URLSearchParams(window.location.search).get("resourceEditing") === "false") {
        return Promise.reject({ code: "editingDisabled", message: "Resource editing is disabled." });
      }
      const request = requestMessage.params as CreateResourceParams;
      if (this.createdResources.has(request.operationId)) {
        return Promise.reject({ code: "duplicateOperation", message: "This creation was already applied." });
      }
      window.dispatchEvent(new CustomEvent("dev-resource-create", { detail: request.resourceType }));
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
        setTimeout(
          () => {
            const node: SampleGraphNode = {
              id: symbolicName,
              type: request.resourceType.fullyQualifiedType,
              isCollection: false,
              hasChildren: false,
              hasError: true,
            };
            this.pushGraph({
              ...current,
              nodes: [...current.nodes, node],
            });
            this.createdResources.set(request.operationId, { node, isInSource: true });

            resolve({
              version: 1,
              operationId: request.operationId,
              expectedNodeId: symbolicName,
              unresolvedRequiredProperties: ["name"],
            } satisfies CreateResourceResult as T);
          },
          getDelayParam("createDelay", 300),
        );
      });
    }

    if (requestMessage.method === replaySourceStep.method) {
      const request = requestMessage.params as ReplaySourceStepParams;
      const creation = this.createdResources.get(request.operationId);
      const isUndo = request.direction === "undo";
      const current = this.currentGraph;
      if (!creation || !current || !this.canReplay(request)) {
        return Promise.reject({ code: "replayUnavailable", message: "The graph changed outside the designer." });
      }

      this.pushGraph({
        ...current,
        nodes: isUndo
          ? current.nodes.filter((node) => node.id !== creation.node.id)
          : [...current.nodes, creation.node],
      });
      creation.isInSource = !isUndo;
      this.isWithholdingUndoneRemoval =
        isUndo && new URLSearchParams(window.location.search).get("skipGraphUpdateAfterUndo") === "true";
      return Promise.resolve({
        version: 1,
        operationId: request.operationId,
        direction: request.direction,
      } satisfies ReplaySourceStepResult as T);
    }

    if (requestMessage.method === restoreModuleSource.method) {
      return this.restoreModuleSource(requestMessage.params as RestoreModuleSourceParams) as Promise<T>;
    }

    return Promise.reject(new Error(`FakeMessageChannel does not support request: ${requestMessage.method}`));
  }

  /**
   * Like the language server, restore a remote module whose source is not cached. `restoreDelay` holds the
   * restore open, and `restoreFailures=N` makes the first N attempts fail. The webview fetches the graph afterwards.
   */
  private restoreModuleSource({ nodeId }: RestoreModuleSourceParams): Promise<RestoreModuleSourceResult> {
    const fails =
      this.restoreRequestCount++ < Number(new URLSearchParams(window.location.search).get("restoreFailures") ?? 0);

    return new Promise((resolve) => {
      setTimeout(
        () => {
          const current = this.currentGraph;
          const node = current?.nodes.find((candidate) => candidate.id === nodeId);
          if (!current || !node?.requiresRestore) {
            resolve({ status: "notFound" });
            return;
          }

          if (fails) {
            resolve({ status: "failed", message: "The registry could not be reached." });
            return;
          }

          const child = (name: string, type: string): SampleGraphNode => ({
            id: `${nodeId}::${name}`,
            type,
            isCollection: false,
            hasChildren: false,
            hasError: false,
          });

          // A restore changes no document, so the host announces nothing: the webview's own fetch picks it up.
          this.currentGraph = {
            ...current,
            nodes: [
              ...current.nodes.map((candidate) =>
                candidate.id === nodeId ? { ...candidate, hasChildren: true, requiresRestore: false } : candidate,
              ),
              child("thing", "Microsoft.Network/networkSecurityGroups"),
              child("lock", "Microsoft.Authorization/locks"),
            ],
            edges: [...current.edges, { sourceId: `${nodeId}::lock`, targetId: `${nodeId}::thing` }],
          };
          this.restoreRefreshFailuresRemaining = getDelayParam("restoreRefreshFailures", 0);
          resolve({ status: "restored" });
        },
        getDelayParam("restoreDelay", 300),
      );
    });
  }

  /** The last graph pushed, so mutations can build on top of it. */
  private currentGraph: SampleGraph | null = null;

  /** The graph last returned by `graph/get`, which lags `currentGraph` while an update is withheld. */
  private servedGraph: SampleGraph | null = null;

  /** Like the language server: undo needs the created node present, redo needs its id to be free. */
  private canReplay({ operationId, direction }: SourceStepReference): boolean {
    // Simulates the created declarations having been edited in the Bicep file.
    if (new URLSearchParams(window.location.search).get("sourceReplay") === "unavailable") {
      return false;
    }

    const creation = this.createdResources.get(operationId);
    const isUndo = direction === "undo";
    const isInGraph = this.currentGraph?.nodes.some((node) => node.id === creation?.node.id) ?? false;
    return creation !== undefined && creation.isInSource === isUndo && isInGraph === isUndo;
  }

  sendNotification(notificationMessage: WebviewNotificationMessage) {
    if (notificationMessage.method === ready.method) {
      this.sendSettings();
      // Simulate async response from the extension host: after a short delay (or `graphDelay`, standing in for
      // a language server that is still starting), present the sample deployment graph.
      setTimeout(() => this.pushGraph(MODULE_GRAPH), getDelayParam("graphDelay", 50));
    } else if (notificationMessage.method === revealNode.method) {
      // The real host would resolve the node's source location via the language server and reveal it.
      console.log("[FakeMessageChannel] revealNode:", notificationMessage.params);
    } else if (notificationMessage.method === showProblems.method) {
      console.log("[FakeMessageChannel] showProblems: would open VS Code Problems panel");
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

  setTargetScope(scope: TargetScope) {
    this.targetScope = scope;
    this.pushGraph(this.currentGraph);
  }

  changeCatalog() {
    this.catalogRevision++;
    this.pushGraph(this.currentGraph);
  }

  isColorThemeMatched(): boolean {
    return this.colorThemeMatched;
  }

  /** Simulate changing `bicep.visualizer.matchColorTheme`. */
  setColorThemeMatched(matched: boolean) {
    this.colorThemeMatched = matched;
    this.sendSettings();
  }

  private sendSettings() {
    const params = new URLSearchParams(window.location.search);
    const motionPolicy = params.get("motionPolicy");
    this.dispatchNotification(settingsDidChange.method, {
      motionPolicy: motionPolicy === "reduce" || motionPolicy === "system" ? motionPolicy : "animate",
      isResourceEditingEnabled: params.get("resourceEditing") !== "false",
      isColorThemeMatched: this.colorThemeMatched,
    } satisfies Settings);
  }

  /** Simulate the extension host announcing that the graph may have changed. */
  pushGraph(graph: SampleGraph | null) {
    this.currentGraph = graph;
    this.isWithholdingUndoneRemoval = false;
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
