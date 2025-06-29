// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { deflate, inflate } from "pako";

export function handleShareLink(
  onContents: (contents: string | null) => void,
): void {
  try {
    const rawHash = window.location.hash.substring(1);
    if (!rawHash) {
      onContents(null);
    }

    history.replaceState(null, "", " ");
    const hashContents = decodeHash(rawHash);

    onContents(hashContents);
  } catch {
    onContents(null);
  }
}

export function getShareLink(content: string) {
  const contentHash = encodeHash(content);
  let href = document.location.href;
  if (href === "https://azure.github.io/bicep/") {
    // use aka.ms for the official site
    href = "https://aka.ms/bicepdemo";
  }

  return `${href}#${contentHash}`;
}

function encodeHash(content: string): string {
  const deflatedData = deflate(
    new Uint8Array(content.split("").map((c) => c.charCodeAt(0))),
  );
  const deflatedString = String.fromCharCode(...deflatedData);
  const base64Encoded = btoa(deflatedString);

  return base64Encoded;
}

function decodeHash(base64Encoded: string): string {
  const deflatedString = atob(base64Encoded);
  const deflatedData = new Uint8Array(
    deflatedString.split("").map((c) => c.charCodeAt(0)),
  );
  const content = inflate(deflatedData);

  return String.fromCharCode(...content);
}
