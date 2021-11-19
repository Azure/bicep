// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from "path";
import fs from "fs";
import rimraf from "rimraf";
import { promisify } from "util";
import { homedir } from "os";

const rimrafAsync = promisify(rimraf);

export const bicepCli = path.resolve(
  __dirname,
  process.env.BICEP_CLI_EXECUTABLE ||
    "../../../Bicep.Cli/bin/Debug/net6.0/bicep"
);

export const moduleCacheRoot = path.resolve(homedir(), ".bicep");

export function pathToExampleFile(...pathNames: string[]): string {
  return path.join(__dirname, "../examples", ...pathNames);
}

export function pathToTempFile(...pathNames: string[]): string {
  const tempPath = path.join(__dirname, "../temp", ...pathNames);
  return tempPath;
}

export function pathToCachedTsModuleFile(...pathNames: string[]): string {
  return path.join(moduleCacheRoot, "ts", ...pathNames);
}

export function pathToCachedBrModuleFile(...pathNames: string[]): string {
  return path.join(moduleCacheRoot, "br", ...pathNames);
}

export function readFileSync(filePath: string): string {
  return fs.readFileSync(filePath, { encoding: "utf-8" });
}

export function logFiles(dirPath: string) {
  const files: string[] = [];
  logFilesInternal(files, dirPath);
  console.log(`File count ${files.length}\n${files.join("\n")}`);
}

function logFilesInternal(files: string[], dirPath: string) {
  const items = fs.readdirSync(dirPath);
  items.forEach((name) => {
    const itemPath = path.join(dirPath, name);
    const stat = fs.statSync(itemPath);
    if (stat.isFile()) {
      files.push(itemPath);
    }

    if (stat.isDirectory()) {
      logFiles(itemPath);
    }
  });
}

export async function emptyDir(dirPath: string): Promise<void> {
  await rimrafAsync(dirPath);
}

export function expectFileExists(filePath: string): void {
  expect(fs.existsSync(filePath)).toBeTruthy();
}

export function expectFileNotExists(filePath: string): void {
  expect(fs.existsSync(filePath)).toBeFalsy();
}

export function writeTempFile(
  testArea: string,
  fileName: string,
  contents: string
): string {
  const tempDir = pathToTempFile(testArea);
  fs.mkdirSync(tempDir, { recursive: true });

  const filePath = path.join(tempDir, fileName);
  fs.writeFileSync(filePath, contents);

  return filePath;
}
