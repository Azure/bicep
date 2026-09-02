// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { WebviewMessageChannelApi } from "./webviewMessageChannel";

import { createContext, useCallback, useEffect, useRef } from "react";
import { WebviewMessageChannel } from "./webviewMessageChannel";

export interface WebviewMessageChannelProviderProps {
  /**
   * A channel to use instead of the real one. Typed as the interface rather than the class so dev and
   * test doubles are checked against the surface the app actually calls.
   */
  messageChannel?: WebviewMessageChannelApi;
  children: ReactNode;
}

export const WebviewMessageChannelContext = createContext<(() => WebviewMessageChannelApi) | undefined>(undefined);

export function WebviewMessageChannelProvider({ messageChannel, children }: WebviewMessageChannelProviderProps) {
  const messageChannelRef = useRef<WebviewMessageChannelApi | undefined>(messageChannel);

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
