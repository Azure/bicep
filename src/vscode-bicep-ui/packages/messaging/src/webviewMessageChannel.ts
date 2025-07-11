// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WebviewApi } from "vscode-webview";

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

type WebviewResponseCallback = (result?: unknown, error?: unknown) => void;

function isResponseMessage(message: unknown): message is WebviewResponseMessage {
  return typeof message === "object" && message !== null && "id" in message;
}

function isNotificationMessage(message: unknown): message is WebviewNotificationMessage {
  return typeof message === "object" && message !== null && "method" in message;
}

export class WebviewMessageChannel {
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

  subscribeToNotification(method: string, callback: WebviewNotificationCallback) {
    this.notificationSubscriptions[method] ??= new Set();
    this.notificationSubscriptions[method].add(callback);
  }

  unsubscribeFromNotification(method: string, callback: WebviewNotificationCallback) {
    this.notificationSubscriptions[method]?.delete(callback);
  }
}
