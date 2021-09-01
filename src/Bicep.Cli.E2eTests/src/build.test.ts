// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import * as fs from "fs";
import { runBicepCommand } from "./command";

const exampleDirectory = path.resolve(__dirname, "../examples/101/aks/");

function getExampleFilePath(
  exampleFileName: string,
  exampleFileExtension: string
) {
  return path.join(
    exampleDirectory,
    `${exampleFileName}.${exampleFileExtension}`
  );
}

describe("bicep build", () => {
  it("should build a bicep file", () => {
    const exampleBicepFile = getExampleFilePath("main", "bicep");
    const result = runBicepCommand(["build", exampleBicepFile]);
    expect(result.status).toBe(0);

    const exampleJsonFile = getExampleFilePath("main", "json");
    expect(fs.existsSync(exampleJsonFile)).toBeTruthy();

    const jsonContents = fs.readFileSync(exampleJsonFile, {
      encoding: "utf-8",
    });
    expect(jsonContents.length).toBeGreaterThan(0);

    // Building with --stdout should emit consistent result.
    const stdoutResult = runBicepCommand([
      "build",
      "--stdout",
      exampleBicepFile,
    ]);
    expect(stdoutResult.status).toBe(0);
    expect(stdoutResult.stdout).toBe(jsonContents);
  });

  it("should log to stderr if a bicep file has errors", () => {
    const exampleBicepFile = getExampleFilePath("flawed", "bicep");
    const result = runBicepCommand(["build", exampleBicepFile], true);
    expect(result.status).toBe(1);
    expect(result.stderr.length).toBeGreaterThan(0);

    const exampleJsonFile = getExampleFilePath("flawed", "json");
    expect(fs.existsSync(exampleJsonFile)).toBeFalsy();
  });
});
