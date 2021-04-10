// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { deflate, inflate } from "pako";

export function handleShareLink(onContents: (contents : string | null) => void): void {
  try {
    const rawHash = window.location.hash.substr(1);
    if (!rawHash) {
      onContents(null);
    }

    history.replaceState(null, null, ' ');
    const hashContents = decodeHash(rawHash);

    onContents(hashContents);
  } catch {
    onContents(null);
  }
}

export function copyShareLinkToClipboard(content: string): void {
  document.addEventListener('copy', function onCopy(e: ClipboardEvent) {
    const contentHash = encodeHash(content);
    e.clipboardData.setData('text/plain', `https://aka.ms/bicepdemo#${contentHash}`);
    e.preventDefault();
    document.removeEventListener('copy', onCopy, true);
  });

  document.execCommand('copy');
}

function encodeHash(content: string): string {
  const deflatedData = deflate(new Uint8Array(content.split('').map(c => c.charCodeAt(0))));
  const deflatedString = String.fromCharCode(...deflatedData);
  const base64Encoded = btoa(deflatedString);

  return base64Encoded
}

function decodeHash(base64Encoded: string): string {
  const deflatedString = atob(base64Encoded);
  const deflatedData =  new Uint8Array(deflatedString.split('').map(c => c.charCodeAt(0)));
  const content = inflate(deflatedData);

  return String.fromCharCode(...content);
}