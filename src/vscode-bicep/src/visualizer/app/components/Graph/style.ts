// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Stylesheet } from "cytoscape";

import moduleSvg from "../../assets/icons/graph/module.svg";

const backgroundColor = "#333333";
const backgroundColorSecondary = "#3f3f3f";
const foregroundColor = "#ffffff";
const foregroundColorSecondary = "#9c9c9c";
const errorColor = "red";

function escapeXml(text: string) {
  return text.replace(/[<>&'"]/g, (c) => {
    switch (c) {
      case "<":
        return "&lt;";
      case ">":
        return "&gt;";
      case "&":
        return "&amp;";
      case "'":
        return "&apos;";
      case '"':
        return "&quot;";
      default:
        return c;
    }
  });
}

function truncate(text: string, lengthLimit: number) {
  if (text.length <= lengthLimit) {
    return text;
  }

  const charsLength = lengthLimit - 3;
  const headLength = Math.ceil(charsLength / 2);
  const tailLength = Math.floor(charsLength / 2);

  return (
    text.substr(0, headLength) + "..." + text.substr(text.length - tailLength)
  );
}

function createDataUri(svg: string) {
  const domParser = new DOMParser();
  const svgElement = domParser.parseFromString(svg, "text/xml").documentElement;

  return "data:image/svg+xml;utf8," + encodeURIComponent(svgElement.outerHTML);
}

export async function createChildlessNodeBackgroundUri(
  symbol: string,
  type: string,
  isCollection: boolean
): Promise<string> {
  const iconSvg =
    type !== "<module>"
      ? (await import("../../assets/icons/graph/resource.svg")).default
      : moduleSvg;

  type = type.split("/").pop() ?? type;
  type += isCollection ? "[]" : "";

  const backgroundSvg = `<?xml version="1.0" encoding="UTF-8"?><!DOCTYPE svg>
    <svg xmlns="http://www.w3.org/2000/svg" width="220" height="80" viewBox="0 0 220 80">
      <g transform="translate(12, 16)">
        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48">
          ${iconSvg}
        </svg>
      </g>
      <text x="72" y="36" font-family="Helvetica Neue, Helvetica, sans-serif" font-size="16" fill="${foregroundColor}">
       ${escapeXml(truncate(symbol, 17))}
      </text>
      <text x="72" y="56" font-family="Helvetica Neue, Helvetica, sans-serif" font-size="12" fill="${foregroundColorSecondary}">
       ${escapeXml(truncate(type, 23))}
      </text>
    </svg>
    `;

  return createDataUri(backgroundSvg);
}

export async function createContainerNodeBackgroundUri(
  symbol: string,
  isCollection: boolean
): Promise<string> {
  symbol += isCollection ? " <collection>" : "";

  const backgroundSvg = `<?xml version="1.0" encoding="UTF-8"?><!DOCTYPE svg>
    <svg xmlns="http://www.w3.org/2000/svg">
      <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18">
        ${moduleSvg}
      </svg>
      <text x="28" y="14" font-family="Helvetica Neue, Helvetica, sans-serif" font-size="12" fill="#9c9c9c">
       ${escapeXml(truncate(symbol, 37))}
      </text>
    </svg>
    `;

  return createDataUri(backgroundSvg);
}

export const stylesheet: Stylesheet[] = [
  {
    selector: "node:childless",
    style: {
      shape: "round-rectangle",
      width: 220,
      height: 80,
      "background-color": backgroundColor,
      "background-image": "data(backgroundDataUri)",
      "border-width": 1,
      "border-color": (node) =>
        node.data("hasError") === true ? errorColor : backgroundColorSecondary,
      "border-opacity": 0.8,
    },
  },
  {
    selector: "node:parent",
    style: {
      shape: "round-rectangle",
      "background-color": backgroundColor,
      "background-image": "data(backgroundDataUri)",
      "background-position-x": 12,
      "background-position-y": 8,
      "border-width": 1,
      "border-color": (node) =>
        node.data("hasError") === true ? errorColor : foregroundColorSecondary,
      "background-blacken": 0.4,
      "background-opacity": 0.1,
      "padding-top": "40px",
    },
  },
  {
    selector: "edge",
    style: {
      width: 2,
      color: foregroundColor,
      opacity: 0.5,
      "curve-style": "bezier",
      "target-arrow-shape": "triangle",
    },
  },
];
