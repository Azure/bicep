// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WebviewApi } from "vscode-webview";
import type { MessageArgs, NotificationDescriptor, RequestDescriptor } from "./messageDescriptor";

export interface WebviewRequestMessage {
  id: string;
  method: string;
  params?: unknown;
}

export interface WebviewResponseMessage {
  id: string;
  result?: unknown;
  error?: unknown;
}

export interface WebviewNotificationMessage {
  method: string;
  params?: unknown;
}

export type WebviewNotificationCallback = (params?: unknown) => void;

/**
 * The channel surface consumers depend on.
 *
 * This exists so test and dev doubles can be checked against the real channel. `WebviewMessageChannel`
 * has private fields, so a double could never be structurally assignable to the class itself, and the
 * dev shell previously bridged that with `as unknown as WebviewMessageChannel` — which silently
 * accepted a fake that was missing methods the app called at runtime.
 */
export interface WebviewMessageChannelApi {
  revive(): void;
  dispose(): void;
  sendRequest<T>(requestMessage: Omit<WebviewRequestMessage, "id">): Promise<T>;
  sendNotification(notificationMessage: WebviewNotificationMessage): void;
  request<TParams, TResult>(
    descriptor: RequestDescriptor<TParams, TResult>,
    ...args: MessageArgs<TParams>
  ): Promise<TResult>;
  notify<TParams>(descriptor: NotificationDescriptor<TParams>, ...args: MessageArgs<TParams>): void;
  setState<T>(state: T): T;
  subscribeToNotification(method: string, callback: WebviewNotificationCallback): void;
  unsubscribeFromNotification(method: string, callback: WebviewNotificationCallback): void;
}

type WebviewResponseCallback = (result?: unknown, error?: unknown) => void;

function isResponseMessage(message: unknown): message is WebviewResponseMessage {
  return typeof message === "object" && message !== null && "id" in message;
}

function isNotificationMessage(message: unknown): message is WebviewNotificationMessage {
  return typeof message === "object" && message !== null && "method" in message;
}

export class WebviewMessageChannel implements WebviewMessageChannelApi {
  private readonly webviewApi: WebviewApi<unknown>;
  private readonly responseCallbacks: Record<string, WebviewResponseCallback>;
  private readonly notificationSubscriptions: Record<string, Set<WebviewNotificationCallback>>;
  private readonly onMessage: (messageEvent: MessageEvent) => void;

  constructor() {
    this.webviewApi = acquireVsCodeApi();
    this.responseCallbacks = {};
    this.notificationSubscriptions = {};
    this.onMessage = (messageEvent: MessageEvent) => {
      if (isResponseMessage(messageEvent.data)) {
        const { id, result, error } = messageEvent.data;

        if (!this.responseCallbacks[id]) {
          throw new Error(`No response callback found for request ID: ${id}.`);
        }

        this.responseCallbacks[id](result, error);

        return;
      }

      if (isNotificationMessage(messageEvent.data)) {
        const { method, params } = messageEvent.data;

        if (!this.notificationSubscriptions[method]) {
          throw new Error(`No subscriptions found for notification method: ${method}.`);
        }

        for (const notificationCallback of this.notificationSubscriptions[method]) {
          notificationCallback(params);
        }

        return;
      }
    };

    window.addEventListener("message", this.onMessage);
  }

  revive() {
    window.addEventListener("message", this.onMessage);
  }

  dispose() {
    window.removeEventListener("message", this.onMessage);
  }

  sendRequest<T>(requestMessage: Omit<WebviewRequestMessage, "id">): Promise<T> {
    return new Promise((resolve, reject) => {
      const id = window.crypto.randomUUID();

      this.responseCallbacks[id] = (result: unknown, error: unknown) => {
        if (error) {
          reject(error);
        } else {
          resolve(result as T);
        }

        if (this.responseCallbacks[id]) {
          delete this.responseCallbacks[id];
        }
      };

      this.webviewApi.postMessage({ id, ...requestMessage } satisfies WebviewRequestMessage);
    });
  }

  sendNotification(notificationMessage: WebviewNotificationMessage) {
    this.webviewApi.postMessage(notificationMessage);
  }

  /**
   * Sends a declared request. Params and result are both taken from the descriptor, so the method,
   * what it is sent with, and what it resolves to cannot drift apart at a call site.
   */
  request<TParams, TResult>(
    descriptor: RequestDescriptor<TParams, TResult>,
    ...args: MessageArgs<TParams>
  ): Promise<TResult> {
    return this.sendRequest<TResult>({ method: descriptor.method, params: args[0] });
  }

  /** Sends a declared notification, with its parameters checked against the descriptor. */
  notify<TParams>(descriptor: NotificationDescriptor<TParams>, ...args: MessageArgs<TParams>): void {
    this.sendNotification({ method: descriptor.method, params: args[0] });
  }

  setState<T>(state: T): T {
    return this.webviewApi.setState(state);
  }

  subscribeToNotification(method: string, callback: WebviewNotificationCallback) {
    this.notificationSubscriptions[method] ??= new Set();
    this.notificationSubscriptions[method].add(callback);
  }

  unsubscribeFromNotification(method: string, callback: WebviewNotificationCallback) {
    this.notificationSubscriptions[method]?.delete(callback);
  }
}
