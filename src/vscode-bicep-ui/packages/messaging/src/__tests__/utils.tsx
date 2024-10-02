// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { RenderHookResult } from "@testing-library/react";
import type { ReactNode } from "react";

import { renderHook } from "@testing-library/react";
import { WebviewMessageChannel } from "../webviewMessageChannel";
import { WebviewMessageChannelProvider } from "../WebviewRequestChannelProvider";

export function renderHookWithWebviewMessageChannel<Result, Props>(
  render: (initialProps: Props) => Result,
): RenderHookResult<Result, Props> {
  const wrapper = ({ children }: { children: ReactNode }) => (
    <WebviewMessageChannelProvider messageChannel={new WebviewMessageChannel()}>
      {children}
    </WebviewMessageChannelProvider>
  );

  return renderHook(render, { wrapper });
}
