// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from "path";
import os from "os";
import { spawn } from "child_process";
import { randomBytes } from "crypto";
import {
  RequestType,
  createMessageConnection,
  createClientPipeTransport,
} from "vscode-jsonrpc/node";
import { bicepCli } from "./fs";

interface CompileRequest {
  path: string;
}

interface CompileResponse {
  success: boolean;
  diagnostics: CompileResponseDiagnostic[];
  contents?: string;
}

interface CompileResponseDiagnostic {
  line: number;
  char: number;
  level: string;
  code: string;
  message: string;
}

export const compileRequestType = new RequestType<
  CompileRequest,
  CompileResponse,
  never
>("bicep/compile");

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
  child.stderr.on("data", (x) => console.error(x.toString()));

  const [reader, writer] = await transport.onConnected();
  const connection = createMessageConnection(reader, writer, console);
  process.on("SIGINT", connection.end);
  process.on("SIGTERM", connection.end);

  connection.listen();
  return connection;
}
