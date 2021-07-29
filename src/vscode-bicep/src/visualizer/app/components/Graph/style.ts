// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Stylesheet } from "cytoscape";
import { DefaultTheme } from "styled-components";

import { importResourceIconInline } from "../../assets/icons/azure";
import moduleIcon from "../../assets/icons/azure/general/10802-icon-service-Folder-Blank.svg";

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
  isCollection: boolean,
  theme: DefaultTheme
): Promise<string> {
  const icon =
    type !== "<module>" ? await importResourceIconInline(type) : moduleIcon;

  console.log(icon);

  type = type.split("/").pop() ?? type;
  type += isCollection ? "[]" : "";

  const { foregroundColor, foregroundSecondaryColor } = theme.common;
  const backgroundSvg = `<?xml version="1.0" encoding="UTF-8"?><!DOCTYPE svg>
    <svg xmlns="http://www.w3.org/2000/svg" width="220" height="80" viewBox="0 0 220 80">
      <g transform="translate(12, 16)">
        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48">
          ${icon}
        </svg>
      </g>
      <text x="72" y="36" font-family="${
        theme.fontFamily
      }" font-size="16" fill="${foregroundColor}">
       ${escapeXml(truncate(symbol, 17))}
      </text>
      <text x="72" y="56" font-family="${
        theme.fontFamily
      }" font-size="12" fill="${foregroundSecondaryColor}">
       ${escapeXml(truncate(type, 23))}
      </text>
    </svg>
    `;

  return createDataUri(backgroundSvg);
}

export function createContainerNodeBackgroundUri(
  symbol: string,
  isCollection: boolean,
  theme: DefaultTheme
): string {
  symbol += isCollection ? " <collection>" : "";

  const { foregroundSecondaryColor } = theme.common;
  const backgroundSvg = `<?xml version="1.0" encoding="UTF-8"?><!DOCTYPE svg>
    <svg xmlns="http://www.w3.org/2000/svg">
      <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18">
        ${moduleIcon}
      </svg>
      <text x="28" y="14" font-family="${
        theme.fontFamily
      }" font-size="12" fill="${foregroundSecondaryColor}">
       ${escapeXml(truncate(symbol, 37))}
      </text>
    </svg>
    `;

  return createDataUri(backgroundSvg);
}

export function createStylesheet(theme: DefaultTheme): Stylesheet[] {
  const {
    common: { errorIndicatorColor },
    graph: { childlessNode, containerNode, edge },
  } = theme;

  return [
    {
      selector: "node:childless",
      style: {
        shape: "round-rectangle",
        width: 220,
        height: 80,
        "background-color": childlessNode.backgroundColor,
        "background-image": "data(backgroundDataUri)",
        "border-width": childlessNode.borderWidth,
        "border-color": (node) =>
          node.data("hasError") === true
            ? errorIndicatorColor
            : childlessNode.borderColor,
        "border-opacity": childlessNode.borderOpacity,
      },
    },
    {
      selector: "node:parent",
      style: {
        shape: "round-rectangle",
        "background-color": containerNode.backgroundColor,
        "background-image": "data(backgroundDataUri)",
        "background-position-x": 12,
        "background-position-y": 8,
        "border-width": containerNode.borderWidth,
        "border-color": (node) =>
          node.data("hasError") === true
            ? errorIndicatorColor
            : containerNode.borderColor,
        "border-opacity": containerNode.borderOpacity,
        "background-opacity": containerNode.backgroundOpacity,
        "padding-top": "40px",
      },
    },
    {
      selector: "edge",
      style: {
        width: 2,
        "line-color": edge.color,
        "target-arrow-color": edge.color,
        opacity: edge.opacity,
        "curve-style": "bezier",
        "target-arrow-shape": "triangle",
      },
    },
  ];
}
