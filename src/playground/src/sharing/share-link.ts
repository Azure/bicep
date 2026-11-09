// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { deflate, inflate } from "pako";

export type Workspace = "editor" | "console";

export type Fragment =
  | { kind: "workspace"; workspace: Workspace }
  | { kind: "editor"; workspace: "editor"; content: string }
  | { kind: "console"; workspace: "console"; commands: readonly string[] }
  | { kind: "invalid"; workspace: Workspace; message: string };

const consoleSharePrefix = "console/";
const editorSharePrefix = "editor/";

export function parseFragment(hash: string): Fragment {
  const rawHash = hash.startsWith("#") ? hash.substring(1) : hash;
  if (!rawHash || rawHash === "editor" || rawHash === "console") {
    return {
      kind: "workspace",
      workspace: rawHash === "console" ? "console" : "editor",
    };
  }

  const workspace = rawHash.startsWith(consoleSharePrefix)
    ? "console"
    : "editor";
  try {
    if (workspace === "console") {
      return {
        kind: "console",
        workspace,
        commands: decodeConsoleHistory(
          rawHash.substring(consoleSharePrefix.length),
        ),
      };
    }
    return {
      kind: "editor",
      workspace,
      content: decodeHash(
        rawHash.startsWith(editorSharePrefix)
          ? rawHash.substring(editorSharePrefix.length)
          : rawHash,
      ),
    };
  } catch {
    return {
      kind: "invalid",
      workspace,
      message:
        workspace === "console"
          ? "The shared console history is invalid."
          : "The shared editor source is invalid.",
    };
  }
}

export function getShareLink(content: string) {
  const contentHash = encodeHash(content);
  return `${getShareBaseUrl()}#${editorSharePrefix}${contentHash}`;
}

export function getConsoleShareLink(commands: readonly string[]): string {
  const data = deflate(new TextEncoder().encode(JSON.stringify(commands)));
  const payload = btoa(
    Array.from(data, (byte) => String.fromCharCode(byte)).join(""),
  );
  return `${getShareBaseUrl()}#${consoleSharePrefix}${payload}`;
}

function decodeConsoleHistory(payload: string): string[] {
  const data = Uint8Array.from(atob(payload), (char) => char.charCodeAt(0));
  const json = new TextDecoder("utf-8", { fatal: true }).decode(inflate(data));
  const commands: unknown = JSON.parse(json);
  if (
    !Array.isArray(commands) ||
    !commands.every(
      (command): command is string =>
        typeof command === "string" && command.trim().length > 0,
    )
  ) {
    throw new Error("Expected an array of console commands.");
  }

  return commands;
}

function getShareBaseUrl(): string {
  let href = document.location.href.split("#")[0];
  if (href === "https://azure.github.io/bicep/") {
    // use aka.ms for the official site
    href = "https://aka.ms/bicepdemo";
  }

  return href;
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
