// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { toPng } from "html-to-image";
import { getDefaultStore } from "jotai";
import { nodesByIdAtom } from "@/lib/graph";

interface SaveFilePickerOptions {
  suggestedName?: string;
  types?: Array<{
    description?: string;
    accept: Record<string, string[]>;
  }>;
}

interface FileSystemWritableFileStream extends WritableStream {
  write(data: Blob | BufferSource | string): Promise<void>;
  close(): Promise<void>;
}

interface FileSystemFileHandle {
  createWritable(): Promise<FileSystemWritableFileStream>;
}

declare global {
  interface Window {
    showSaveFilePicker?: (options?: SaveFilePickerOptions) => Promise<FileSystemFileHandle>;
  }
}

type Store = ReturnType<typeof getDefaultStore>;

/**
 * Compute the bounding box of all graph nodes from the Jotai store.
 */
function computeGraphBounds(store: Store) {
  const nodes = store.get(nodesByIdAtom);
  const nodeList = Object.values(nodes);
  if (nodeList.length === 0) return null;

  const boxes = nodeList.map((node) => store.get(node.boxAtom));
  const minX = Math.min(...boxes.map((b) => b.min.x));
  const minY = Math.min(...boxes.map((b) => b.min.y));
  const maxX = Math.max(...boxes.map((b) => b.max.x));
  const maxY = Math.max(...boxes.map((b) => b.max.y));

  return { minX, minY, maxX, maxY, width: maxX - minX, height: maxY - minY };
}

/**
 * Find the PanZoomTransformed element inside the canvas.
 * It's the div whose inline style.transform contains "translate".
 */
function findTransformedElement(root: HTMLElement): HTMLElement | null {
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_ELEMENT);
  let node = walker.nextNode() as HTMLElement | null;
  while (node) {
    if (node.style?.transform?.includes("translate")) {
      return node;
    }
    node = walker.nextNode() as HTMLElement | null;
  }
  return null;
}

/**
 * Capture the graph by cloning the canvas into an off-screen element.
 *
 * This avoids touching the live canvas at all — no flicker, no
 * temporary transform changes.  The clone lives in the document
 * (so styled-components class-based styles still resolve) but is
 * positioned off-screen so it's invisible.
 */
export async function captureGraphElement(
  canvasElement: HTMLElement,
  store: Store,
  padding: number,
  backgroundColor: string | undefined,
): Promise<string> {
  const bounds = computeGraphBounds(store);
  if (!bounds) throw new Error("No graph nodes to export");

  const captureWidth = Math.round(bounds.width + padding * 2);
  const captureHeight = Math.round(bounds.height + padding * 2);

  // Deep-clone the entire canvas subtree.
  const clone = canvasElement.cloneNode(true) as HTMLElement;

  clone.style.position = "relative";
  clone.style.inset = "unset";
  clone.style.width = `${captureWidth}px`;
  clone.style.height = `${captureHeight}px`;
  clone.style.overflow = "visible";
  clone.style.pointerEvents = "none";

  // The wrapper keeps the clone off-screen so it's not visible.
  const wrapper = document.createElement("div");
  wrapper.style.position = "fixed";
  wrapper.style.left = "-99999px";
  wrapper.style.top = "0";
  wrapper.style.width = "0";
  wrapper.style.height = "0";
  wrapper.style.overflow = "hidden";
  wrapper.style.pointerEvents = "none";
  wrapper.appendChild(clone);
  document.body.appendChild(wrapper);

  try {
    // Fix overflow on the PanZoom container inside the clone.
    const panZoomContainer = clone.querySelector('[data-testid="pan-zoom"]') as HTMLElement | null;
    if (panZoomContainer) {
      panZoomContainer.style.overflow = "visible";
    }

    // Set the export transform on the cloned PanZoomTransformed.
    const transformedEl = findTransformedElement(clone);
    if (!transformedEl) throw new Error("PanZoomTransformed not found in clone");

    const offsetX = padding - bounds.minX;
    const offsetY = padding - bounds.minY;
    transformedEl.style.transition = "none";
    transformedEl.style.transform = `translate(${offsetX}px, ${offsetY}px) scale(1)`;

    // Yield to the event loop so the "Exporting…" UI can render
    // before the heavy capture work begins.
    await new Promise((r) => setTimeout(r, 0));

    const options = {
      width: captureWidth,
      height: captureHeight,
      backgroundColor,
      pixelRatio: 2,
      filter: (node: HTMLElement) => {
        // Exclude the dot-pattern background SVGs from the export.
        if (node.tagName === "svg" && node.querySelector?.("pattern")) {
          return false;
        }
        return true;
      },
    };

    return await toPng(clone, options);
  } finally {
    document.body.removeChild(wrapper);
  }
}

/**
 * Convert a data-URL to a Blob.
 */
function dataUrlToBlob(dataUrl: string): Blob {
  if (dataUrl.includes(";base64,")) {
    const parts = dataUrl.split(";base64,");
    const mime = parts[0]?.split(":")[1] ?? "application/octet-stream";
    const b64 = parts[1] ?? "";
    const bytes = atob(b64);
    const buf = new Uint8Array(bytes.length);
    for (let i = 0; i < bytes.length; i++) buf[i] = bytes.charCodeAt(i);
    return new Blob([buf], { type: mime });
  }
  // Fallback: URI-encoded data URL.
  const commaIdx = dataUrl.indexOf(",");
  const meta = dataUrl.substring(0, commaIdx);
  const mime = meta.split(":")[1]?.split(";")[0] ?? "application/octet-stream";
  const decoded = decodeURIComponent(dataUrl.substring(commaIdx + 1));
  return new Blob([decoded], { type: mime });
}

/**
 * Show a "Save As" dialog so the user can pick location and filename.
 * Falls back to a direct download if the File System Access API
 * is not available (e.g. non-Chromium browsers).
 */
export async function saveDataUrl(dataUrl: string, defaultName: string): Promise<void> {
  const blob = dataUrlToBlob(dataUrl);

  // Try the File System Access API (Chromium-based browsers).
  if (typeof window.showSaveFilePicker === "function") {
    try {
      const handle = await window.showSaveFilePicker({
        suggestedName: defaultName,
        types: [
          {
            description: "PNG Image",
            accept: { "image/png": [".png"] },
          },
        ],
      });
      const writable = await handle.createWritable();
      await writable.write(blob);
      await writable.close();
      return;
    } catch (err) {
      // User cancelled the dialog — not an error.
      if (err instanceof DOMException && err.name === "AbortError") return;
      console.warn("showSaveFilePicker failed, falling back:", err);
    }
  }

  // Fallback: trigger a browser download.
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.download = defaultName;
  link.href = url;
  link.click();
  URL.revokeObjectURL(url);
}
