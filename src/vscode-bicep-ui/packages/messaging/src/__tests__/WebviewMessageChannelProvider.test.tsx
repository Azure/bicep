// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { render } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { WebviewMessageChannel } from "../webviewMessageChannel";
import { WebviewMessageChannelProvider } from "../WebviewRequestChannelProvider";

describe("WebviewMessageChannelProvider", () => {
  it("should dispose message channel upon unmount", () => {
    const messageChannel = new WebviewMessageChannel();
    const disposeSpy = vi.spyOn(messageChannel, "dispose");

    const { unmount } = render(
      <WebviewMessageChannelProvider messageChannel={messageChannel}>
        <div />
      </WebviewMessageChannelProvider>,
    );

    unmount();

    expect(disposeSpy).toHaveBeenCalled();
  });
});
