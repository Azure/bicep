// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";

import { createContext, useEffect } from "react";
import { WebviewMessageChannel } from "./webviewMessageChannel";

export interface WebviewMessageChannelProviderProps {
  messageChannel: WebviewMessageChannel;
  children: ReactNode;
}

export const WebviewMessageChannelContext = createContext<WebviewMessageChannel | undefined>(undefined);

export function WebviewMessageChannelProvider({ messageChannel, children }: WebviewMessageChannelProviderProps) {
  useEffect(() => {
    return () => {
      messageChannel.dispose();
    };
  }, [messageChannel]);

  return (
    <WebviewMessageChannelContext.Provider value={messageChannel}>{children}</WebviewMessageChannelContext.Provider>
  );
}
