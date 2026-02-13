// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ExportFormat } from "./types";

/* eslint-disable @typescript-eslint/no-explicit-any */
declare global {
  interface Window {
    showSaveFilePicker?: (options?: any) => Promise<any>;
  }
}
/* eslint-enable @typescript-eslint/no-explicit-any */

import { getDefaultStore } from "jotai";
import { toSvg, toPng, toJpeg } from "html-to-image";
import { nodesAtom } from "../graph-engine/atoms";

type Store = ReturnType<typeof getDefaultStore>;

/**
 * Compute the bounding box of all graph nodes from the Jotai store.
 */
function computeGraphBounds(store: Store) {
  const nodes = store.get(nodesAtom);
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
 * Strip bloat from the SVG data URL produced by html-to-image.
 *
 * html-to-image inlines every computed style on every element —
 * hundreds of properties per node, most at their default values,
 * plus thousands of inherited CSS custom variables.  We use a
 * whitelist to keep only the properties that affect visual rendering.
 */
/**
 * Snapshot the computed style of a blank detached <div> to learn
 * every default property value.  Call this while we still have
 * access to the live DOM (before serialisation).
 */
function computeCssDefaults(): Map<string, string> {
  const el = document.createElement("div");
  document.body.appendChild(el);
  const cs = getComputedStyle(el);
  const defaults = new Map<string, string>();
  for (let i = 0; i < cs.length; i++) {
    const prop = cs[i]!;
    defaults.set(prop, cs.getPropertyValue(prop));
  }
  document.body.removeChild(el);
  return defaults;
}

function cleanSvgDataUrl(
  dataUrl: string,
  cssDefaults: Map<string, string>,
): string {
  const prefix = "data:image/svg+xml;charset=utf-8,";
  if (!dataUrl.startsWith(prefix)) return dataUrl;

  try {
    const svgText = decodeURIComponent(dataUrl.slice(prefix.length));
    const doc = new DOMParser().parseFromString(svgText, "image/svg+xml");

    // Bail on parse errors.
    if (doc.querySelector("parsererror")) {
      console.warn("SVG parse error, returning original");
      return dataUrl;
    }

    // Snapshot all elements before mutating the tree.
    const root = doc.documentElement;
    const elements: Element[] = [root];
    const tw = doc.createTreeWalker(root, NodeFilter.SHOW_ELEMENT);
    let n: Node | null;
    while ((n = tw.nextNode())) elements.push(n as Element);

    for (const el of elements) {
      // Remove <style> — styles are already inlined by html-to-image.
      if (el.localName === "style") {
        el.parentNode?.removeChild(el);
        continue;
      }

      // Remove non-visual attributes.
      el.removeAttribute("class");
      el.removeAttribute("data-testid");

      // Clean inline styles.
      const raw = el.getAttribute("style");
      if (raw) {
        const cleaned = stripBloatedDeclarations(raw, cssDefaults);
        if (cleaned) {
          el.setAttribute("style", cleaned);
        } else {
          el.removeAttribute("style");
        }
      }
    }

    // --- Pass 2: deduplicate inline styles into shared CSS classes ---
    // Re-snapshot the elements (some may have been removed in pass 1).
    const remaining: Element[] = [];
    const tw2 = doc.createTreeWalker(root, NodeFilter.SHOW_ELEMENT);
    let n2: Node | null = root;
    while (n2) {
      remaining.push(n2 as Element);
      n2 = tw2.nextNode();
    }

    // Count how many elements share each exact style string.
    const styleCounts = new Map<string, number>();
    for (const el of remaining) {
      const s = el.getAttribute("style");
      if (s) styleCounts.set(s, (styleCounts.get(s) ?? 0) + 1);
    }

    // Build class mappings for styles that appear 2+ times.
    const styleToClass = new Map<string, string>();
    let classIdx = 0;
    for (const [style, count] of styleCounts) {
      if (count >= 2) {
        styleToClass.set(style, `s${classIdx++}`);
      }
    }

    if (styleToClass.size > 0) {
      // Replace inline styles with class references.
      for (const el of remaining) {
        const s = el.getAttribute("style");
        if (s && styleToClass.has(s)) {
          el.removeAttribute("style");
          el.setAttribute("class", styleToClass.get(s)!);
        }
      }

      // Build and inject the <style> element.
      // The SVG uses foreignObject with XHTML, so we need the style
      // inside a <defs> in the SVG namespace.
      let css = "";
      for (const [style, cls] of styleToClass) {
        css += `.${cls} { ${style} }\n`;
      }

      const defs =
        root.querySelector("defs") ??
        root.insertBefore(
          doc.createElementNS("http://www.w3.org/2000/svg", "defs"),
          root.firstChild,
        );
      const styleEl = doc.createElementNS(
        "http://www.w3.org/2000/svg",
        "style",
      );
      styleEl.textContent = css;
      defs.appendChild(styleEl);
    }

    const out = new XMLSerializer().serializeToString(root);
    return prefix + encodeURIComponent(out);
  } catch (error) {
    console.warn("SVG cleanup failed, returning original:", error);
    return dataUrl;
  }
}

/**
 * Properties that never affect the visual output of a static SVG export.
 * We use a BLACKLIST so that any unrecognised property is kept by default,
 * which is much safer than a whitelist that can accidentally drop things
 * the browser serialises under a different name (logical vs physical, etc.).
 */
const NON_VISUAL_PROPS = new Set([
  // Interaction & behaviour
  "cursor", "caret-color", "pointer-events", "user-select",
  "-webkit-user-select", "touch-action", "resize",
  // Outline (not rendered in foreign-object context)
  "outline", "outline-color", "outline-offset", "outline-style", "outline-width",
  // Print / paged media
  "orphans", "widows", "page",
  "page-break-after", "page-break-before", "page-break-inside",
  "break-after", "break-before", "break-inside",
  // Browser-internal / non-visual
  "accent-color", "appearance", "backface-visibility", "buffered-rendering",
  "contain", "container", "container-name", "container-type",
  "content-visibility",
  "forced-color-adjust", "image-orientation", "image-rendering",
  "interpolate-size", "isolation",
  "math-depth", "math-shift", "math-style",
  "mix-blend-mode",
  "object-fit", "object-position", "object-view-box",
  "perspective", "perspective-origin",
  "print-color-adjust",
  "ruby-align", "ruby-position",
  "shape-image-threshold", "shape-margin", "shape-outside",
  "speak", "table-layout",
  "text-combine-upright", "text-orientation",
  "text-size-adjust",
  "timeline-scope", "unicode-bidi",
  "will-change", "writing-mode",
  "counter-increment", "counter-reset", "counter-set", "content",
]);

/** Prefix families that are entirely non-visual for a static export. */
const NON_VISUAL_PREFIXES = [
  "animation",       // animation, animation-delay, animation-*, …
  "transition",      // transition, transition-duration, …
  "scroll-",         // scroll-behavior, scroll-margin-*, scroll-snap-*, …
  "scrollbar-",      // scrollbar-color, scrollbar-gutter, scrollbar-width
  "overscroll-",     // overscroll-behavior-*
  "contain-intrinsic-", // contain-intrinsic-size, -block-size, …
  "view-transition-",   // view-transition-name, -class
  "view-timeline-",     // view-timeline-*
  "scroll-timeline-",   // scroll-timeline-*
  "anchor-",         // anchor-name, anchor-scope
  "app-region",      // app-region (non-standard)
];

function isNonVisualProperty(prop: string): boolean {
  if (prop.startsWith("--")) return true;
  if (NON_VISUAL_PROPS.has(prop)) return true;
  return NON_VISUAL_PREFIXES.some((pfx) => prop.startsWith(pfx));
}

/**
 * Inheritable properties must be preserved even when they match the
 * browser default, because a standalone SVG has no parent to inherit
 * from — stripping them would lose fonts, colours, text settings, etc.
 */
const INHERITABLE_PROPS = new Set([
  "color", "direction", "font", "font-family", "font-size", "font-style",
  "font-variant", "font-weight", "font-stretch", "font-size-adjust",
  "letter-spacing", "line-height", "text-align", "text-indent",
  "text-transform", "white-space-collapse", "word-spacing", "word-break",
  "visibility", "cursor",
  "-webkit-text-fill-color", "-webkit-text-stroke",
  // SVG inheritable presentation attributes
  "fill", "fill-opacity", "fill-rule",
  "stroke", "stroke-dasharray", "stroke-dashoffset",
  "stroke-linecap", "stroke-linejoin", "stroke-miterlimit",
  "stroke-opacity", "stroke-width",
]);

/**
 * Remove custom properties, known non-visual declarations,
 * and declarations whose values match the browser default.
 */
function stripBloatedDeclarations(
  style: string,
  cssDefaults: Map<string, string>,
): string {
  return style
    .split(";")
    .filter((decl) => {
      const trimmed = decl.trim();
      if (!trimmed) return false;
      const colonIdx = trimmed.indexOf(":");
      if (colonIdx === -1) return false;
      const prop = trimmed.substring(0, colonIdx).trim().toLowerCase();
      if (isNonVisualProperty(prop)) return false;
      // Never strip inheritable properties — the SVG won't have
      // a parent to inherit these from when viewed standalone.
      if (INHERITABLE_PROPS.has(prop)) return true;
      // Strip declarations that match the browser default.
      const val = trimmed.substring(colonIdx + 1).trim();
      const defaultVal = cssDefaults.get(prop);
      if (defaultVal !== undefined && val === defaultVal) return false;
      return true;
    })
    .map((d) => d.trim())
    .join("; ");
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
  format: ExportFormat,
  padding: number,
  backgroundColor: string,
): Promise<string> {
  const bounds = computeGraphBounds(store);
  if (!bounds) throw new Error("No graph nodes to export");

  const captureWidth = Math.round(bounds.width + padding * 2);
  const captureHeight = Math.round(bounds.height + padding * 2);

  // Deep-clone the entire canvas subtree.
  const clone = canvasElement.cloneNode(true) as HTMLElement;

  // The clone must have clean positioning so html-to-image doesn't
  // inline "left: -99999px" into the SVG foreignObject (which would
  // push all content off-screen in the output file).
  // We wrap the clone in a hidden container instead.
  clone.style.position = "relative";
  clone.style.inset = "unset";
  clone.style.width = `${captureWidth}px`;
  clone.style.height = `${captureHeight}px`;
  clone.style.overflow = "visible";
  clone.style.pointerEvents = "none";

  // The wrapper keeps the clone off-screen so it's not visible.
  // html-to-image only captures `clone`, not the wrapper.
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
    const panZoomContainer = clone.querySelector(
      '[data-testid="pan-zoom"]',
    ) as HTMLElement | null;
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
      backgroundColor: format === "jpeg" ? backgroundColor : undefined,
      filter: (node: HTMLElement) => {
        // Exclude the dot-pattern background SVGs from the export.
        if (node.tagName === "svg" && node.querySelector?.("pattern")) {
          return false;
        }
        return true;
      },
    };

    // Snapshot CSS defaults while we have live DOM access.
    const cssDefaults = computeCssDefaults();

    switch (format) {
      case "svg":
        return cleanSvgDataUrl(await toSvg(clone, options), cssDefaults);
      case "png":
        return await toPng(clone, { ...options, pixelRatio: 2 });
      case "jpeg":
        return await toJpeg(clone, {
          ...options,
          quality: 0.95,
          backgroundColor,
        });
    }
  } finally {
    document.body.removeChild(wrapper);
  }
}

/** MIME types for each export format. */
const FORMAT_MIME: Record<ExportFormat, string> = {
  svg: "image/svg+xml",
  png: "image/png",
  jpeg: "image/jpeg",
};

/** File extension descriptions for the Save dialog. */
const FORMAT_DESC: Record<ExportFormat, string> = {
  svg: "SVG Image",
  png: "PNG Image",
  jpeg: "JPEG Image",
};

/**
 * Convert a data-URL to a Blob.
 */
function dataUrlToBlob(dataUrl: string): Blob {
  // Handle both base64 and URI-encoded data URLs.
  if (dataUrl.includes(";base64,")) {
    const parts = dataUrl.split(";base64,");
    const mime = parts[0]?.split(":")[1] ?? "application/octet-stream";
    const b64 = parts[1] ?? "";
    const bytes = atob(b64);
    const buf = new Uint8Array(bytes.length);
    for (let i = 0; i < bytes.length; i++) buf[i] = bytes.charCodeAt(i);
    return new Blob([buf], { type: mime });
  }
  // charset=utf-8, URI-encoded (SVG path)
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
export async function saveDataUrl(
  dataUrl: string,
  defaultName: string,
  format: ExportFormat,
): Promise<void> {
  const blob = dataUrlToBlob(dataUrl);

  // Try the File System Access API (Chromium-based browsers).
  if (typeof window.showSaveFilePicker === "function") {
    try {
      const handle = await window.showSaveFilePicker({
        suggestedName: defaultName,
        types: [
          {
            description: FORMAT_DESC[format],
            accept: { [FORMAT_MIME[format]]: [`.${format}`] },
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
      // Any other error: fall through to legacy download.
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
