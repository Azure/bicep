// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { beforeAll, describe, it } from "vitest";
import { invokingBicepCommand } from "../utils/command";
import { expectFileNotExists, pathToTempFile, writeTempFile } from "../utils/fs";

// In-process scenarios are covered by Bicep.Cli.IntegrationTests (ConfigOptionTests);
// these tests cover behavior that depends on a real process and file system.
describe("bicep --config", () => {
  const testArea = "config-option";
  const projectArea = `${testArea}/project`;

  let mainBicepPath: string;
  let ciConfigPath: string;

  beforeAll(() => {
    mainBicepPath = writeTempFile(projectArea, "main.bicep", "param foo string\nmodule mod 'modules/mod.bicep' = {}\n");
    writeTempFile(`${projectArea}/modules`, "mod.bicep", "param bar string = 'bar'\n");
    // A nested config that the explicit configuration must override.
    writeTempFile(
      `${projectArea}/modules`,
      "bicepconfig.json",
      `{ "analyzers": { "core": { "rules": { "no-unused-params": { "level": "off" } } } } }`,
    );
    ciConfigPath = writeTempFile(
      `${testArea}/eng`,
      "bicepconfig.ci.json",
      `{ "analyzers": { "core": { "rules": { "no-unused-params": { "level": "error" } } } } }`,
    );
  });

  it("should apply the explicit configuration to the entrypoint and its modules", () => {
    invokingBicepCommand("build", mainBicepPath, "--stdout", "--config", ciConfigPath)
      .shouldFail()
      .withStderrContaining("main.bicep(1,7) : Error no-unused-params")
      .withStderrContaining("mod.bicep(1,7) : Error no-unused-params");
  });

  it("should resolve a relative --config path from the working directory", () => {
    invokingBicepCommand("lint", mainBicepPath, "--config", path.join("..", "eng", "bicepconfig.ci.json"))
      .withCwd(pathToTempFile(projectArea))
      .shouldFail()
      .withStderrContaining("main.bicep(1,7) : Error no-unused-params");
  });

  it("should fail without producing output if the configuration file does not exist", () => {
    invokingBicepCommand("build", mainBicepPath, "--config", "./missing.json")
      .shouldFail()
      .withStderr(/^Could not find configuration file "\.\/missing\.json"\.\r?\n$/);

    expectFileNotExists(path.join(path.dirname(mainBicepPath), "main.json"));
  });
});
