// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { RenderHookResult } from "@testing-library/react";
import type { PropsWithChildren } from "react";

import { fireEvent, renderHook } from "@testing-library/react";
import { PanZoom } from "../PanZoom";
import { PanZoomTransformed } from "../PanZoomTransformed";
import { PanZoomProvider } from "../PanZoomProvider";

export function firePanZoomEvent(element: HTMLElement) {
  // Workaround for d3. See https://github.com/d3/d3-drag/issues/79#issuecomment-1631409544.
  const options = { view: window };

  // Skip simulating wheel events because jsdom / happy-dom cannot calculate the size of DOM elements,
  // causing d3 to return NaN for the translation values (i.e., x and y).
  fireEvent.mouseDown(element, { ...options, clientX: 0, clientY: 0 });
  fireEvent.mouseMove(element, { ...options, clientX: 10, clientY: 10 });
  fireEvent.mouseUp(element, options);
}

export function renderPanZoomHook<Result, Props>(
  render: (initialProps: Props) => Result,
): RenderHookResult<Result, Props> {
  const wrapper = ({ children }: PropsWithChildren) => (
    <PanZoomProvider>
      <PanZoom transition={{ duration: 0 }}>
        <>
          <div>pan-zoom handle</div>
          <PanZoomTransformed>pan-zoom transform spy</PanZoomTransformed>
          {children}
        </>
      </PanZoom>
    </PanZoomProvider>
  );

  return renderHook(render, { wrapper });
}
