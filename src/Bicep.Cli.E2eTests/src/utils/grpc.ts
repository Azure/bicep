// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from "path";
import os from "os";
import { spawn, ChildProcess } from "child_process";
import { randomBytes } from "crypto";
import { bicepCli } from "./fs";

import * as grpc from '@grpc/grpc-js';
import * as RpcTypes from '../proto/src/Bicep.Cli/bicep_pb';
import { RpcClient } from '../proto/src/Bicep.Cli/bicep_grpc_pb';

function generateRandomPipeName(): string {
  const randomSuffix = randomBytes(21).toString("hex");
  if (process.platform === "win32") {
    return `\\\\.\\pipe\\bicep-${randomSuffix}-sock`;
  }

  return path.join(os.tmpdir(), `bicep-${randomSuffix}.sock`);
}

function connectClient(pipePath: string, timeoutMs: number, child: ChildProcess) {
  return new Promise<RpcClient>((resolve, reject) => {
    const handleConnectionError = (err: unknown) => {
      child.kill('SIGINT');
      client.close();
      reject(err);
    };

    const client = new RpcClient(`unix://${pipePath}`, grpc.credentials.createInsecure());
    const deadline = new Date();
    deadline.setMilliseconds(deadline.getMilliseconds() + timeoutMs);

    child.on('exit', handleConnectionError);
    child.on('error', handleConnectionError);

    client.waitForReady(deadline, (err) => {
      if (err) {
        handleConnectionError(err);
        return;
      }

      child.removeListener('exit', handleConnectionError)
      resolve(client);
    });
  });
}

export async function getClient() {
  const pipePath = generateRandomPipeName();

  const child = spawn(bicepCli, ["grpc", "--socket", pipePath]);
  child.stderr.on("data", (x) => console.error(x.toString()));
  child.stdout.on("data", (x) => console.log(x.toString()));

  const processExitedEarly = new Promise<void>((_, reject) => {
    child.on("error", (err) => {
      reject(`Failed to invoke '${bicepCli} grpc'. Error: ${err}`);
    });
    child.on("exit", () => {
      reject(`Failed to invoke '${bicepCli} grpc'`);
    });
  });

  const connectionReady = connectClient(pipePath, 30000, child);

  const client = await Promise.race([
    connectionReady,
    processExitedEarly,
  ]);

  return new BicepClient(child, client!);
}

export class BicepClient {
  private child: ChildProcess;
  private client: RpcClient;

  constructor(child: ChildProcess, client: RpcClient) {
    this.child = child;
    this.client = client;
  }

  version(request: RpcTypes.VersionRequest) {
    return new Promise<RpcTypes.VersionResponse>((resolve, reject) => {
      this.client.version(request, (err, resp) => err ? reject(err) : resolve(resp));
    });
  }

  compile(request: RpcTypes.CompileRequest) {
    return new Promise<RpcTypes.CompileResponse>((resolve, reject) => {
      this.client.compile(request, (err, resp) => err ? reject(err) : resolve(resp));
    });
  }

  compileParams(request: RpcTypes.CompileParamsRequest) {
    return new Promise<RpcTypes.CompileParamsResponse>((resolve, reject) => {
      this.client.compileParams(request, (err, resp) => err ? reject(err) : resolve(resp));
    });
  }

  getMetadata(request: RpcTypes.GetMetadataRequest) {
    return new Promise<RpcTypes.GetMetadataResponse>((resolve, reject) => {
      this.client.getMetadata(request, (err, resp) => err ? reject(err) : resolve(resp));
    });
  }

  getDeploymentGraph(request: RpcTypes.GetDeploymentGraphRequest) {
    return new Promise<RpcTypes.GetDeploymentGraphResponse>((resolve, reject) => {
      this.client.getDeploymentGraph(request, (err, resp) => err ? reject(err) : resolve(resp));
    });
  }

  getFileReferences(request: RpcTypes.GetFileReferencesRequest) {
    return new Promise<RpcTypes.GetFileReferencesResponse>((resolve, reject) => {
      this.client.getFileReferences(request, (err, resp) => err ? reject(err) : resolve(resp));
    });
  }

  close() {
    return new Promise((resolve, reject) => {
      this.child.on('exit', resolve);
      this.child.kill('SIGINT');
      this.client.close();
    });
  }
}