// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { spawn } from "child_process";
import { randomBytes } from "crypto";
import os from "os";
import path from "path";
import { createClientPipeTransport, createMessageConnection, RequestType } from "vscode-jsonrpc/node";
import { bicepCli } from "./fs";
import { logStdErr } from "./log";

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
interface VersionRequest {}

interface VersionResponse {
  version: string;
}

interface GetDeploymentGraphRequest {
  path: string;
}

interface GetDeploymentGraphResponse {
  nodes: GetDeploymentGraphResponseNode[];
  edges: GetDeploymentGraphResponseEdge[];
}

interface GetDeploymentGraphResponseNode {
  range: Range;
  name: string;
  type: string;
  isExisting: boolean;
  relativePath?: string;
}

interface GetDeploymentGraphResponseEdge {
  source: string;
  target: string;
}

interface GetFileReferencesRequest {
  path: string;
}

interface GetFileReferencesResponse {
  filePaths: string[];
}

interface Position {
  line: number;
  char: number;
}

interface Range {
  start: Position;
  end: Position;
}

interface CompileRequest {
  path: string;
}

interface CompileResponse {
  success: boolean;
  diagnostics: CompileResponseDiagnostic[];
  contents?: string;
}

interface CompileParamsRequest {
  path: string;
  parameterOverrides: Record<string, unknown>;
}

interface CompileParamsResponse {
  success: boolean;
  diagnostics: CompileResponseDiagnostic[];
  parameters?: string;
  template?: string;
  templateSpecId?: string;
}

interface CompileResponseDiagnostic {
  source: string;
  range: Range;
  level: string;
  code: string;
  message: string;
}

interface GetMetadataRequest {
  path: string;
}

interface GetMetadataResponse {
  metadata: MetadataDefinition[];
  parameters: SymbolDefinition[];
  outputs: SymbolDefinition[];
  exports: ExportDefinition[];
}

interface MetadataDefinition {
  name: string;
  value: string;
}

interface SymbolDefinition {
  range: Range;
  name: string;
  type?: TypeDefinition;
  description?: string;
}

interface ExportDefinition {
  range: Range;
  name: string;
  kind: string;
  description?: string;
}

interface TypeDefinition {
  range?: Range;
  name: string;
}

export const versionRequestType = new RequestType<VersionRequest, VersionResponse, never>("bicep/version");

export const compileRequestType = new RequestType<CompileRequest, CompileResponse, never>("bicep/compile");

export const compileParamsRequestType = new RequestType<CompileParamsRequest, CompileParamsResponse, never>(
  "bicep/compileParams",
);

export const getMetadataRequestType = new RequestType<GetMetadataRequest, GetMetadataResponse, never>(
  "bicep/getMetadata",
);

export const getDeploymentGraphRequestType = new RequestType<
  GetDeploymentGraphRequest,
  GetDeploymentGraphResponse,
  never
>("bicep/getDeploymentGraph");

export const getFileReferencesRequestType = new RequestType<GetFileReferencesRequest, GetFileReferencesResponse, never>(
  "bicep/getFileReferences",
);

function generateRandomPipeName(): string {
  const randomSuffix = randomBytes(21).toString("hex");
  if (process.platform === "win32") {
    return `\\\\.\\pipe\\bicep-${randomSuffix}-sock`;
  }

  return path.join(os.tmpdir(), `bicep-${randomSuffix}.sock`);
}

export async function openConnection() {
  const pipePath = generateRandomPipeName();
  const transport = await createClientPipeTransport(pipePath, "utf-8");

  const child = spawn(bicepCli, ["jsonrpc", "--pipe", pipePath]);
  child.stdout.on("data", (x) => console.log(x.toString()));
  child.stderr.on("data", (x) => logStdErr(x.toString()));

  const [reader, writer] = await transport.onConnected();
  const connection = createMessageConnection(reader, writer, console);
  process.on("SIGINT", () => connection.end());
  process.on("SIGTERM", () => connection.end());

  connection.listen();
  return connection;
}
