// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NotificationDescriptor } from "./messageDescriptor";

import { useEffect } from "react";
import { useWebviewMessageChannel } from "./useWebviewMessageChannel";

/**
 * Subscribes to a declared notification. The callback receives the descriptor's parameter type, so
 * handlers no longer begin by casting or re-validating an `unknown`.
 */
export function useNotification<TParams>(
  descriptor: NotificationDescriptor<TParams>,
  callback: (params: TParams) => void,
) {
  const messageChannel = useWebviewMessageChannel();

  useEffect(() => {
    const subscription = (params?: unknown) => callback(params as TParams);

    messageChannel.subscribeToNotification(descriptor.method, subscription);

    return () => {
      messageChannel.unsubscribeFromNotification(descriptor.method, subscription);
    };
  }, [descriptor.method, callback, messageChannel]);
}
