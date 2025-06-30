// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { createContext, useCallback, useEffect, useRef } from "react";
import { WebviewMessageChannel } from "./webviewMessageChannel";

export interface WebviewMessageChannelProviderProps {
  messageChannel?: WebviewMessageChannel;
  children: ReactNode;
}

export const WebviewMessageChannelContext = createContext<(() => WebviewMessageChannel) | undefined>(undefined);

export function WebviewMessageChannelProvider({ messageChannel, children }: WebviewMessageChannelProviderProps) {
  const messageChannelRef = useRef<WebviewMessageChannel | undefined>(messageChannel);

  const getMessageChannel = useCallback(() => {
    if (!messageChannelRef.current) {
      messageChannelRef.current = new WebviewMessageChannel();
    }

    return messageChannelRef.current;
  }, []);

  useEffect(() => {
    // Make it work with React <StrictMode>.
    messageChannelRef.current?.revive();

    return () => {
      messageChannelRef.current?.dispose();
    };
  }, []);

  return (
    <WebviewMessageChannelContext.Provider value={getMessageChannel}>{children}</WebviewMessageChannelContext.Provider>
  );
}
