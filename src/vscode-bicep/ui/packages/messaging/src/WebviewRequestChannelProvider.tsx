import { ReactNode, createContext, useEffect } from "react";
import { WebviewMessageChannel } from "./webview-message-channel";

export interface WebviewChannelProviderProps {
  messageChannel: WebviewMessageChannel;
  children: ReactNode;
}

export const WebviewMessageChannelContext = createContext<WebviewMessageChannel | undefined>(undefined);

export function WebviewMessageChannelProvider({ messageChannel, children }: WebviewChannelProviderProps) {
  useEffect(() => {
    return () => {
      messageChannel.dispose();
    };
  }, [messageChannel]);

  return (
    <WebviewMessageChannelContext.Provider value={messageChannel}>{children}</WebviewMessageChannelContext.Provider>
  );
}
