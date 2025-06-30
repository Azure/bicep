// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Meta, StoryObj } from "@storybook/react-vite";

import { useRef } from "react";
import styled from "styled-components";
import { PanZoom } from "./PanZoom";
import { PanZoomProvider } from "./PanZoomProvider";
import { PanZoomTransformed } from "./PanZoomTransformed";
import { usePanZoomTransform } from "./usePanZoomTransform";
import { usePanZoomTransformListener } from "./usePanZoomTransformListener";
import { usePanZoomControl } from "./usePanZoomControl";

const meta: Meta<typeof PanZoom> = {
  title: "Examples/PanZoom",
  component: PanZoom,
  tags: ["autodocs"],
  parameters: {
    layout: "padded",
    controls: { hideNoControlsWarning: true },
  },
};

export default meta;

type Story = StoryObj<typeof PanZoom>;

const $PanZoom = styled(PanZoom)`
  width: 100%;
  height: 400px;
  border: 1px solid black;
  overflow: hidden;
  cursor: grab;
`;

const $PanZoomTransformed = styled(PanZoomTransformed)`
  margin: 0 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 40px;
  cursor: inherit;
`;

const $CenteredText = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  font-size: 40px;
`;

function PanZoomTransformValue() {
  const { x, y, scale } = usePanZoomTransform();

  return (
    <$CenteredText>
      x: {x.toFixed(2)}, y: {y.toFixed(2)}, scale: {scale.toFixed(2)}
    </$CenteredText>
  );
}

function PanZoomTransformListenerValue() {
  const ref = useRef<HTMLDivElement>(null);

  usePanZoomTransformListener((x, y, k) => {
    if (ref.current) {
      ref.current.innerText = `x: ${x.toFixed(2)}, y: ${y.toFixed(2)}, scale: ${k.toFixed(2)}`;
    }
  });

  return <$CenteredText ref={ref}>x: 0, y: 0, scale: 1</$CenteredText>;
}

function PanZoomControl() {
  const { zoomIn, zoomOut, reset } = usePanZoomControl();

  return (
    <div>
      <button onClick={zoomIn}>Zoom In</button>
      <button onClick={zoomOut}>Zoom Out</button>
      <button onClick={reset}>Reset</button>
    </div>
  );
}

export const BasicUsage: Story = {
  render: () => (
    <PanZoomProvider>
      <$PanZoom maximumScale={10}>
        <$PanZoomTransformed>💪</$PanZoomTransformed>
      </$PanZoom>
    </PanZoomProvider>
  ),
};

export const MultipleProviders: Story = {
  render: () => (
    <>
      <PanZoomProvider>
        <$PanZoom maximumScale={10}>
          <$PanZoomTransformed>💪</$PanZoomTransformed>
        </$PanZoom>
      </PanZoomProvider>
      <br />
      <PanZoomProvider>
        <$PanZoom maximumScale={10}>
          <$PanZoomTransformed>🦾</$PanZoomTransformed>
        </$PanZoom>
      </PanZoomProvider>
    </>
  ),
};

export const Hooks: Story = {
  render: () => (
    <>
      <div>
        <h2>usePanZoomTransfrom</h2>
        <p>Triggers re-render, slower than usePanZoomTransformListener.</p>
      </div>
      <PanZoomProvider>
        <$PanZoom>
          <PanZoomTransformValue />
        </$PanZoom>
      </PanZoomProvider>
      <br />
      <div>
        <h2>usePanZoomTransfromListener </h2>
        <p>Does not trigger re-render, preferred for better performance.</p>
      </div>
      <PanZoomProvider>
        <$PanZoom>
          <PanZoomTransformListenerValue />
        </$PanZoom>
      </PanZoomProvider>
      <br />
      <div>
        <h2>usePanZoomControl </h2>
      </div>
      <PanZoomProvider>
        <PanZoomControl />
        <$PanZoom maximumScale={10}>
          <$PanZoomTransformed>💪</$PanZoomTransformed>
        </$PanZoom>
      </PanZoomProvider>
    </>
  ),
};
