// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import fs from "fs";
import { homedir } from "os";
import path from "path";
import { rimraf } from "rimraf";
import { expect } from "vitest";

export const bicepCli = path.resolve(
  __dirname,
  process.env.BICEP_CLI_EXECUTABLE || "../../../Bicep.Cli/bin/Debug/net10.0/bicep",
);

export const defaultModuleCacheRoot = path.resolve(homedir(), ".bicep");

export function pathToExampleFile(...pathNames: string[]): string {
  return path.join(__dirname, "../examples", ...pathNames);
}

export function pathToTempFile(...pathNames: string[]): string {
  const tempPath = path.join(__dirname, "../temp", ...pathNames);
  return tempPath;
}

export function pathToCachedTsModuleFile(moduleCacheRoot: string, ...pathNames: string[]): string {
  return path.join(moduleCacheRoot, "ts", ...pathNames);
}

export function pathToCachedBrModuleFile(moduleCacheRoot: string, ...pathNames: string[]): string {
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
  await rimraf(dirPath);
}

export function expectFileExists(filePath: string): void {
  expect(fs.existsSync(filePath)).toBeTruthy();
}

export function expectFileNotExists(filePath: string): void {
  expect(fs.existsSync(filePath)).toBeFalsy();
}

export function writeTempFile(testArea: string, fileName: string, contents: string): string {
  const tempDir = pathToTempFile(testArea);
  fs.mkdirSync(tempDir, { recursive: true });

  const filePath = path.join(tempDir, fileName);
  fs.writeFileSync(filePath, contents);

  return filePath;
}

export function ensureParentDirExists(filePath: string): void {
  fs.mkdirSync(path.dirname(filePath), { recursive: true });
}

export function copyToTempFile(
  baseFolder: string,
  relativePath: string,
  testArea: string,
  replace?: {
    values: Record<string, string>;
    relativePath: string;
  },
) {
  const fileContents = readFileSync(path.join(baseFolder, relativePath));

  const replacedContents = Object.entries(replace?.values ?? {}).reduce(
    (contents, [from, to]) => contents.replace(from, to),
    fileContents,
  );

  return writeTempFile(testArea, replace?.relativePath ?? relativePath, replacedContents);
}
