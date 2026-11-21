// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { SourceLocation, StackSource } from "./api";

function vscodeLink(location: SourceLocation) {
  if (!location.uri.startsWith("file:///") || !Number.isSafeInteger(location.line) || location.line < 1) return undefined;
  return `vscode://file/${location.uri.slice("file:///".length)}:${location.line}`;
}

export function SourceLinks({ source }: { source?: StackSource | null }) {
  if (!source) return <p className="source-links">Source location unavailable in this viewer.</p>;
  const requires = source.requires && vscodeLink(source.requires);
  return requires ? <p className="source-links"><a href={requires}>Open requires in VS Code</a></p> : null;
}
