// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useContext } from "react";
import { WebviewMessageChannelContext } from "./WebviewRequestChannelProvider";

export function useWebviewMessageChannel() {
  const messageChannel = useContext(WebviewMessageChannelContext);

  if (!messageChannel) {
    throw new Error(
      "No WebviewMessageChannel found in context. Forgot to wrap your component in a WebviewMessageChannelProvider?",
    );
  }

  return messageChannel;
}
