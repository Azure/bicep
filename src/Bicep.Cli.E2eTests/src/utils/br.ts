// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { v4 as uuidv4 } from "uuid";
import { expectFileExists, pathToCachedBrModuleFile } from "./fs";
import path from "path";
import { invokingBicepCommand } from "./command";
import { pathToExampleFile } from "./fs";

// The modules published from live tests to our test ACR instances need to be periodically
// purged. ACR purge tasks support wildcards but only on tags. This means that we have to have
// predictable repository names in our tests. This class simplifies this logic.
export class BicepRegistryReferenceBuilder {
  readonly tagSuffix: string;

  constructor(readonly registry: string, readonly testArea: string) {
    const runId = uuidv4();

    // round down to full hour
    const creationDate = new Date();
    creationDate.setMinutes(0, 0, 0);

    // can't have colons in tag names and replace() only replaces the first occurrence
    const datePart = creationDate.toISOString().split(":").join("-");

    this.tagSuffix = `${datePart}_${runId}`;
  }

  public getRepository(name: string): string {
    return `${this.testArea}/${name}`;
  }

  public getBicepReference(name: string, tagPrefix: string): string {
    return `br:${this.registry}/${this.getRepository(name)}:${tagPrefix}_${
      this.tagSuffix
    }`;
  }
}

export function expectBrModuleStructure(...pathNames: string[]): void {
  const moduleFiles = ["lock", "main.json", "manifest", "metadata"];
  const directoryPath = pathToCachedBrModuleFile(...pathNames);

  moduleFiles.forEach((fileName) => {
    const filePath = path.join(directoryPath, fileName);
    expectFileExists(filePath);
  });
}

export function publishModule(
  moduleReference: string,
  ...examplePathNames: string[]
): void {
  const storagePath = pathToExampleFile(...examplePathNames);
  invokingBicepCommand(
    "publish",
    storagePath,
    "--target",
    moduleReference
  ).shouldSucceed();
}
