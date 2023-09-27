import { useEffect } from "react";
import { useWebviewMessageChannel } from "./use-webview-message-channel";
import type { WebviewNotificationCallback } from "./webview-message-channel";

export function useWebviewNotification(method: string, callback: WebviewNotificationCallback) {
  const messageChannel = useWebviewMessageChannel();

  useEffect(() => {
    messageChannel.subscribeToNotification(method, callback);

    return () => {
      messageChannel.unsubscribeFromNotification(method, callback);
    };
  }, [method, callback, messageChannel]);
}
