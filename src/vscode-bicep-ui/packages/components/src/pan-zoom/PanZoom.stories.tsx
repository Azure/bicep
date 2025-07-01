// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Meta, StoryObj } from "@storybook/react-vite";

import { useRef, useState } from "react";
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
  border: 1px solid #e1e1e1;
  border-radius: 8px;
  overflow: hidden;
  cursor: grab;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  background: #fff;
`;

const $PanZoomTransformed = styled(PanZoomTransformed)`
  margin: 0 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 48px;
  cursor: inherit;
  user-select: none;
`;

const $CenteredText = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  font-size: 24px;
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 8px;
  padding: 20px;
  margin: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  font-weight: 500;
  letter-spacing: 0.5px;
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
  const { zoomIn, zoomOut, reset, transform } = usePanZoomControl();
  const { x: currentX, y: currentY, scale: currentScale } = usePanZoomTransform();
  const [x, setX] = useState(0);
  const [y, setY] = useState(0);
  const [scale, setScale] = useState(1);

  const handleTransform = () => {
    transform(x, y, scale);
  };

  const buttonStyle = {
    padding: '8px 16px',
    border: '1px solid #ccc',
    borderRadius: '6px',
    backgroundColor: '#fff',
    cursor: 'pointer',
    fontSize: '14px',
    fontWeight: '500',
    transition: 'all 0.2s ease',
  };

  const primaryButtonStyle = {
    ...buttonStyle,
    backgroundColor: '#0078d4',
    color: 'white',
    border: '1px solid #0078d4',
  };

  const inputStyle = {
    width: '70px',
    padding: '6px 8px',
    border: '1px solid #ccc',
    borderRadius: '4px',
    fontSize: '14px',
    textAlign: 'center' as const,
  };

  const labelStyle = {
    fontSize: '14px',
    fontWeight: '500',
    minWidth: '40px',
    textAlign: 'left' as const,
  };

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      gap: '16px',
      maxWidth: '450px',
      padding: '16px',
      border: '1px solid #e1e1e1',
      borderRadius: '8px',
      backgroundColor: '#fafafa',
      fontFamily: 'system-ui, -apple-system, sans-serif'
    }}>
      <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
        <button
          onClick={() => zoomIn(2)}
          style={buttonStyle}
          onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f5f5f5'}
          onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#fff'}
        >
          Zoom In
        </button>
        <button
          onClick={() => zoomOut(2)}
          style={buttonStyle}
          onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f5f5f5'}
          onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#fff'}
        >
          Zoom Out
        </button>
        <button
          onClick={reset}
          style={buttonStyle}
          onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f5f5f5'}
          onMouseOut={(e) => e.currentTarget.style.backgroundColor = '#fff'}
        >
          Reset
        </button>
      </div>

      <div style={{
        padding: '12px',
        backgroundColor: '#e3f2fd',
        borderRadius: '6px',
        border: '1px solid #90caf9'
      }}>
        <div style={{ fontSize: '14px', fontWeight: '600', marginBottom: '4px', color: '#1565c0' }}>
          Current Transform
        </div>
        <div style={{ fontSize: '13px', fontFamily: 'monospace', color: '#1976d2' }}>
          x: {currentX.toFixed(2)}, y: {currentY.toFixed(2)}, scale: {currentScale.toFixed(2)}
        </div>
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
        <h4 style={{ margin: '0', fontSize: '16px', fontWeight: '600', color: '#333' }}>
          Custom Transform
        </h4>
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'auto 1fr auto 1fr auto 1fr',
          gap: '8px',
          alignItems: 'center'
        }}>
          <label style={labelStyle}>X:</label>
          <input
            type="number"
            value={x}
            onChange={(e) => setX(Number(e.target.value))}
            style={inputStyle}
          />
          <label style={labelStyle}>Y:</label>
          <input
            type="number"
            value={y}
            onChange={(e) => setY(Number(e.target.value))}
            style={inputStyle}
          />
          <label style={labelStyle}>Scale:</label>
          <input
            type="number"
            min="0.1"
            max="10"
            step="0.1"
            value={scale}
            onChange={(e) => setScale(Number(e.target.value))}
            style={inputStyle}
          />
        </div>
        <button
          onClick={handleTransform}
          style={primaryButtonStyle}
          onMouseOver={(e) => {
            e.currentTarget.style.backgroundColor = '#106ebe';
            e.currentTarget.style.borderColor = '#106ebe';
          }}
          onMouseOut={(e) => {
            e.currentTarget.style.backgroundColor = '#0078d4';
            e.currentTarget.style.borderColor = '#0078d4';
          }}
        >
          Apply Transform
        </button>
      </div>
    </div>
  );
}

export const BasicUsage: Story = {
  render: () => (
    <div style={{
      padding: '20px',
      backgroundColor: '#f8f9fa',
      borderRadius: '12px',
      border: '1px solid #e9ecef'
    }}>
      <h3 style={{
        margin: '0 0 16px 0',
        fontSize: '18px',
        fontWeight: '600',
        color: '#495057'
      }}>
        Basic Pan & Zoom
      </h3>
      <p style={{
        margin: '0 0 16px 0',
        fontSize: '14px',
        color: '#6c757d',
        lineHeight: '1.5'
      }}>
        Click and drag to pan, use mouse wheel to zoom. Maximum scale is set to 10x.
      </p>
      <PanZoomProvider>
        <$PanZoom maximumScale={10}>
          <$PanZoomTransformed>💪</$PanZoomTransformed>
        </$PanZoom>
      </PanZoomProvider>
    </div>
  ),
};

export const MultipleProviders: Story = {
  render: () => (
    <div style={{
      padding: '20px',
      backgroundColor: '#f8f9fa',
      borderRadius: '12px',
      border: '1px solid #e9ecef'
    }}>
      <h3 style={{
        margin: '0 0 16px 0',
        fontSize: '18px',
        fontWeight: '600',
        color: '#495057'
      }}>
        Multiple Independent Pan & Zoom Areas
      </h3>
      <p style={{
        margin: '0 0 20px 0',
        fontSize: '14px',
        color: '#6c757d',
        lineHeight: '1.5'
      }}>
        Each area maintains its own independent pan and zoom state.
      </p>

      <div style={{
        display: 'grid',
        gap: '20px',
        gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))'
      }}>
        <div>
          <h4 style={{
            margin: '0 0 8px 0',
            fontSize: '14px',
            fontWeight: '600',
            color: '#6c757d'
          }}>
            Area 1
          </h4>
          <PanZoomProvider>
            <$PanZoom maximumScale={10}>
              <$PanZoomTransformed>💪</$PanZoomTransformed>
            </$PanZoom>
          </PanZoomProvider>
        </div>

        <div>
          <h4 style={{
            margin: '0 0 8px 0',
            fontSize: '14px',
            fontWeight: '600',
            color: '#6c757d'
          }}>
            Area 2
          </h4>
          <PanZoomProvider>
            <$PanZoom maximumScale={10}>
              <$PanZoomTransformed>🦾</$PanZoomTransformed>
            </$PanZoom>
          </PanZoomProvider>
        </div>
      </div>
    </div>
  ),
};

export const Hooks: Story = {
  render: () => (
    <div style={{
      padding: '20px',
      backgroundColor: '#f8f9fa',
      borderRadius: '12px',
      border: '1px solid #e9ecef'
    }}>
      <h3 style={{
        margin: '0 0 16px 0',
        fontSize: '18px',
        fontWeight: '600',
        color: '#495057'
      }}>
        Pan & Zoom Hooks Demo
      </h3>
      <p style={{
        margin: '0 0 24px 0',
        fontSize: '14px',
        color: '#6c757d',
        lineHeight: '1.5'
      }}>
        Demonstration of different hooks for accessing and controlling pan & zoom state.
      </p>

      <div style={{ display: 'flex', flexDirection: 'column', gap: '32px' }}>
        <div>
          <div style={{
            padding: '12px 16px',
            backgroundColor: '#e3f2fd',
            borderRadius: '8px',
            border: '1px solid #90caf9',
            marginBottom: '16px'
          }}>
            <h4 style={{ margin: '0 0 4px 0', fontSize: '16px', fontWeight: '600', color: '#1565c0' }}>
              usePanZoomTransform
            </h4>
            <p style={{ margin: '0', fontSize: '13px', color: '#1976d2' }}>
              Triggers re-render, slower than usePanZoomTransformListener.
            </p>
          </div>
          <PanZoomProvider>
            <$PanZoom>
              <PanZoomTransformValue />
            </$PanZoom>
          </PanZoomProvider>
        </div>

        <div>
          <div style={{
            padding: '12px 16px',
            backgroundColor: '#f3e5f5',
            borderRadius: '8px',
            border: '1px solid #ce93d8',
            marginBottom: '16px'
          }}>
            <h4 style={{ margin: '0 0 4px 0', fontSize: '16px', fontWeight: '600', color: '#7b1fa2' }}>
              usePanZoomTransformListener
            </h4>
            <p style={{ margin: '0', fontSize: '13px', color: '#8e24aa' }}>
              Does not trigger re-render, preferred for better performance.
            </p>
          </div>
          <PanZoomProvider>
            <$PanZoom>
              <PanZoomTransformListenerValue />
            </$PanZoom>
          </PanZoomProvider>
        </div>

        <div>
          <div style={{
            padding: '12px 16px',
            backgroundColor: '#e8f5e8',
            borderRadius: '8px',
            border: '1px solid #81c784',
            marginBottom: '16px'
          }}>
            <h4 style={{ margin: '0 0 4px 0', fontSize: '16px', fontWeight: '600', color: '#2e7d32' }}>
              usePanZoomControl
            </h4>
            <p style={{ margin: '0', fontSize: '13px', color: '#388e3c' }}>
              Provides programmatic control over pan and zoom operations.
            </p>
          </div>
          <PanZoomProvider>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <PanZoomControl />
              <$PanZoom maximumScale={10}>
                <$PanZoomTransformed>💪</$PanZoomTransformed>
              </$PanZoom>
            </div>
          </PanZoomProvider>
        </div>
      </div>
    </div>
  ),
};
