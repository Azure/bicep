// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { WebviewNotificationCallback } from "./webviewMessageChannel";

import { useEffect } from "react";
import { useWebviewMessageChannel } from "./useWebviewMessageChannel";

export function useWebviewNotification(method: string, callback: WebviewNotificationCallback) {
  const messageChannel = useWebviewMessageChannel();

  useEffect(() => {
    messageChannel.subscribeToNotification(method, callback);

    return () => {
      messageChannel.unsubscribeFromNotification(method, callback);
    };
  }, [method, callback, messageChannel]);
}
