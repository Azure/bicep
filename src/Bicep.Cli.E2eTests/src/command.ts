// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import * as child_process from "child_process";
import * as spawn from "cross-spawn";

const bicepCliExecutable = path.resolve(
  __dirname,
  process.env.BICEP_CLI_EXECUTABLE || "../../Bicep.Cli/bin/Debug/net5.0/bicep"
);

export function runBicepCommand(
  args: readonly string[],
  expectError = false
): child_process.SpawnSyncReturns<string> {
  console.log(`bicepCliExecutable = ${bicepCliExecutable}`);
  const result = spawn.sync(bicepCliExecutable, args, {
    stdio: "pipe",
    encoding: "utf-8",
  });

  if (!expectError && result.stderr.length > 0) {
    console.error(result.stderr);
  }

  return result;
}
