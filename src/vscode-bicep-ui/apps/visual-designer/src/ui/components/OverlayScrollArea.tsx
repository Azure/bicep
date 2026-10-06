// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode, PointerEvent as ReactPointerEvent } from "react";

import { useCallback, useEffect, useRef, useState } from "react";
import styled from "styled-components";

/**
 * Gap between the thumb and the top and bottom of the scroll area. It matches the palette popover's 12px corner
 * radius, so the pill ends where the corner curve begins instead of running into it.
 */
const THUMB_INSET = 12;
const MIN_THUMB_HEIGHT = 24;
/** How long the thumb stays visible after scrolling stops. */
const HIDE_DELAY_MS = 800;

const $Root = styled.div`
  position: relative;
  display: flex;
  flex: 1;
  flex-direction: column;
  min-height: 0;
`;

const $Viewport = styled.div`
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  /* The native bar is hidden; the overlay thumb below takes its place without reserving width. */
  scrollbar-width: none;

  &::-webkit-scrollbar {
    display: none;
  }
`;

/**
 * The element is a 12px-wide, transparent grab area; the visible pill is drawn inside it, 2px from the edge,
 * leaving a 2px gap to row highlights that end at the usual 8px content inset. It widens only while pointed at
 * or dragged, when no row under it can be hovered.
 */
const $Thumb = styled.div.attrs<{ $top: number; $height: number }>(({ $top, $height }) => ({
  style: { transform: `translateY(${$top}px)`, height: `${$height}px` },
}))`
  position: absolute;
  top: 0;
  right: 0;
  width: 12px;
  opacity: 0;
  pointer-events: none;
  transition: opacity 150ms ease;

  &::after {
    content: "";
    position: absolute;
    top: 0;
    right: 2px;
    bottom: 0;
    width: 4px;
    border-radius: 999px;
    background: ${({ theme }) => theme.scrollbar.thumb};
    transition:
      width 120ms ease,
      background-color 120ms ease;
  }

  &[data-visible="true"] {
    opacity: 1;
    pointer-events: auto;
  }

  &:hover::after,
  &[data-dragging="true"]::after {
    width: 6px;
    background: ${({ theme }) => theme.scrollbar.thumbActive};
  }

  @media (prefers-reduced-motion: reduce) {
    transition: none;

    &::after {
      transition: none;
    }
  }
`;

interface ThumbMetrics {
  top: number;
  height: number;
  scrollable: boolean;
}

/**
 * A vertical scroll area with an overlay scrollbar: hidden at rest, a thin thumb fades in while the area is
 * hovered, scrolled, or dragged. It reserves no layout width, so content keeps even insets on both sides.
 * Scrolling itself stays native, so wheel, touch, and keyboard scrolling are unaffected.
 */
export function OverlayScrollArea({
  children,
  viewportRef,
  viewportTestId,
}: {
  children: ReactNode;
  /** Receives the scrolling element, for callers that observe scrolling (for example intersection roots). */
  viewportRef?: (element: HTMLDivElement | null) => void;
  viewportTestId?: string;
}) {
  const [viewport, setViewport] = useState<HTMLDivElement | null>(null);
  const contentRef = useRef<HTMLDivElement>(null);
  const hideTimerRef = useRef<number | undefined>(undefined);
  const [metrics, setMetrics] = useState<ThumbMetrics>({ top: 0, height: 0, scrollable: false });
  const [recentlyScrolled, setRecentlyScrolled] = useState(false);
  const [hovered, setHovered] = useState(false);
  const [dragging, setDragging] = useState(false);

  const setViewportElement = useCallback(
    (element: HTMLDivElement | null) => {
      setViewport(element);
      viewportRef?.(element);
    },
    [viewportRef],
  );

  const measure = useCallback(() => {
    if (!viewport) {
      return;
    }

    const { scrollTop, scrollHeight, clientHeight } = viewport;
    const scrollable = scrollHeight > clientHeight + 1;
    const track = clientHeight - THUMB_INSET * 2;
    const height = scrollable ? Math.max(MIN_THUMB_HEIGHT, (clientHeight / scrollHeight) * track) : 0;
    const top = THUMB_INSET + (scrollable ? (scrollTop / (scrollHeight - clientHeight)) * (track - height) : 0);

    setMetrics((current) =>
      current.top === top && current.height === height && current.scrollable === scrollable
        ? current
        : { top, height, scrollable },
    );
  }, [viewport]);

  // Content grows as more rows render, so both the viewport and its content are observed. The observer also
  // reports each element once when observation starts, which provides the initial measurement.
  useEffect(() => {
    if (!viewport) {
      return;
    }

    const observer = new ResizeObserver(measure);
    observer.observe(viewport);
    if (contentRef.current) {
      observer.observe(contentRef.current);
    }
    return () => observer.disconnect();
  }, [measure, viewport]);

  useEffect(() => () => window.clearTimeout(hideTimerRef.current), []);

  const handleScroll = useCallback(() => {
    measure();
    setRecentlyScrolled(true);
    window.clearTimeout(hideTimerRef.current);
    hideTimerRef.current = window.setTimeout(() => setRecentlyScrolled(false), HIDE_DELAY_MS);
  }, [measure]);

  const handleThumbPointerDown = useCallback(
    (event: ReactPointerEvent<HTMLDivElement>) => {
      if (!viewport || event.button !== 0) {
        return;
      }

      event.preventDefault();
      event.stopPropagation();

      const thumb = event.currentTarget;
      const startY = event.clientY;
      const startScrollTop = viewport.scrollTop;
      const travel = viewport.clientHeight - THUMB_INSET * 2 - metrics.height;
      const scrollPerPixel = travel > 0 ? (viewport.scrollHeight - viewport.clientHeight) / travel : 0;

      const handleMove = (moveEvent: PointerEvent) => {
        viewport.scrollTop = startScrollTop + (moveEvent.clientY - startY) * scrollPerPixel;
      };
      const handleEnd = () => {
        setDragging(false);
        thumb.removeEventListener("pointermove", handleMove);
        thumb.removeEventListener("pointerup", handleEnd);
        thumb.removeEventListener("pointercancel", handleEnd);
      };

      thumb.setPointerCapture(event.pointerId);
      thumb.addEventListener("pointermove", handleMove);
      thumb.addEventListener("pointerup", handleEnd);
      thumb.addEventListener("pointercancel", handleEnd);
      setDragging(true);
    },
    [metrics.height, viewport],
  );

  const visible = metrics.scrollable && (hovered || recentlyScrolled || dragging);

  return (
    <$Root onPointerEnter={() => setHovered(true)} onPointerLeave={() => setHovered(false)}>
      <$Viewport ref={setViewportElement} data-testid={viewportTestId} onScroll={handleScroll}>
        <div ref={contentRef}>{children}</div>
      </$Viewport>
      <$Thumb
        aria-hidden="true"
        data-testid="overlay-scrollbar-thumb"
        data-visible={visible}
        data-dragging={dragging}
        $top={metrics.top}
        $height={metrics.height}
        onPointerDown={handleThumbPointerDown}
      />
    </$Root>
  );
}
