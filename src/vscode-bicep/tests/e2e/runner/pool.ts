// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PoolRunnerInitializer, PoolWorker, WorkerRequest } from "vitest/node" with {
  "resolution-mode": "import",
};

import { EventEmitter } from "events";

const poolName = "vscode-extension-host";

export function createExtensionHostPool(): PoolRunnerInitializer {
  return {
    name: poolName,
    createPoolWorker: () => new ExtensionHostPoolWorker(),
  };
}

// Vitest normally puts the controller and worker in different threads/processes and sends messages
// between them. This worker implements the same two-way protocol with in-memory callbacks so test
// modules execute in the VS Code extension-host process.
class ExtensionHostPoolWorker implements PoolWorker {
  public readonly name = poolName;

  private readonly controllerEvents = new EventEmitter();
  private readonly workerListeners = new Set<(message: unknown) => void>();

  // Worker -> controller responses are exposed as EventEmitter `message` events.
  public on(event: string, callback: (arg: unknown) => void): void {
    this.controllerEvents.on(event, callback);
  }

  public off(event: string, callback: (arg: unknown) => void): void {
    this.controllerEvents.off(event, callback);
  }

  public send(message: WorkerRequest): void {
    // Controller -> worker requests are delivered to callbacks registered by vitest/worker.init().
    // Real workers clone messages at the process boundary. Snapshot before queueing so later mutations
    // cannot change a request that the worker has not processed yet.
    const messageSnapshot = structuredClone(message);
    queueMicrotask(() => {
      for (const listener of this.workerListeners) {
        listener(messageSnapshot);
      }
    });
  }

  public deserialize(data: unknown): unknown {
    return data;
  }

  public async start(): Promise<void> {
    const { init, runBaseTests, setupEnvironment } = await import("vitest/worker");

    // init() turns these callbacks into Vitest's worker runtime. runBaseTests performs normal Vitest
    // collection/execution; only the transport differs from the built-in threads/forks pools.
    init({
      // Worker -> controller.
      post: (response) => {
        const responseSnapshot = structuredClone(response);
        queueMicrotask(() => this.controllerEvents.emit("message", responseSnapshot));
      },
      // Controller -> worker subscription.
      on: (listener) => this.workerListeners.add(listener),
      off: (listener) => this.workerListeners.delete(listener),
      runTests: (state, traces) => runBaseTests("run", state, traces),
      collectTests: (state, traces) => runBaseTests("collect", state, traces),
      setup: setupEnvironment,
    });
  }

  public async stop(): Promise<void> {}
}
