// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as fs from "fs";
import { SpawnSyncReturns } from "node:child_process";
import * as path from "path";
import * as spawn from "cross-spawn";
import { expect } from "vitest";

export class Example {
  readonly projectDir: string;
  readonly projectFile: string;

  constructor(projectName: string, projectFile?: string) {
    const projectDir = path.normalize(path.join(__dirname, `../examples/${projectName}/`));
    this.projectDir = projectDir;
    this.projectFile = path.join(projectDir, projectFile ?? `${projectName}.proj`);
  }

  public cleanProjectDir(): void {
    const result = spawn.sync("git", ["clean", "-dfx", "--", this.projectDir], {
      cwd: this.projectDir,
      stdio: "pipe",
      encoding: "utf-8",
    });

    if (result.status !== 0) {
      this.handleFailure("git", result);
    }
  }

  public clean(expectSuccess = true): SpawnSyncReturns<string> {
    return this.runMsBuild("clean", null, expectSuccess);
  }

  public build(expectSuccess = true): SpawnSyncReturns<string> {
    return this.runMsBuild("build", null, expectSuccess);
  }

  public publish(targetFramework: string | null, expectSuccess = true): SpawnSyncReturns<string> {
    return this.runMsBuild("publish", targetFramework, expectSuccess);
  }

  public expectTemplate(relativeFilePath: string): void {
    const filePath = this.resolveRelativePath(relativeFilePath);
    this.expectFile(relativeFilePath);

    // we don't need to do full template validation
    // (the CLI tests already cover it)
    // this will throw if JSON is not valid
    JSON.parse(fs.readFileSync(filePath, { encoding: "utf-8" }));
  }

  public expectFile(relativeFilePath: string): void {
    const filePath = this.resolveRelativePath(relativeFilePath);
    expect(fs.existsSync(filePath)).toBeTruthy();
  }

  public expectNoFile(relativeFilePath: string): void {
    const filePath = this.resolveRelativePath(relativeFilePath);
    expect(fs.existsSync(filePath)).toBeFalsy();
  }

  private resolveRelativePath(relativeFilePath: string): string {
    return path.join(this.projectDir, relativeFilePath);
  }

  private runMsBuild(verb: string, targetFramework: string | null, expectSuccess: boolean): SpawnSyncReturns<string> {
    const runtimeSuffix = process.env.RuntimeSuffix;
    if (!runtimeSuffix) {
      throw new Error(
        "Please set the RuntimeSuffix environment variable to a .net runtime ID to run these tests. Possible values: win-x64, linux-x64, osx-x64",
      );
    }
    const result = spawn.sync(
      "dotnet",
      [
        verb,
        "/nr:false",
        `/p:RuntimeSuffix=${runtimeSuffix}`,
        targetFramework ? `/p:TargetFramework=${targetFramework}` : "",
        "/bl",
        this.projectFile,
      ],
      {
        cwd: this.projectDir,
        stdio: "pipe",
        encoding: "utf-8",
      },
    );

    if (expectSuccess && result.status !== 0) {
      this.handleFailure("MSBuild", result);
    }

    return result;
  }

  private handleFailure(tool: string, result: SpawnSyncReturns<string>) {
    if (result.stderr.length > 0) {
      throw new Error(`Unexpected StdErr content:\n${result.stderr}`);
    }

    if (result.stdout.length > 0) {
      throw new Error(`Unexpected StdOut content:\n${result.stdout}`);
    }

    throw new Error(`Unexpected ${tool} exit code: ${result.status}`);
  }
}
