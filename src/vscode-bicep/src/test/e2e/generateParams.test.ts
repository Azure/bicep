// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { afterEach, describe, expect, it } from "@jest/globals";
import fs from "fs";
import path from "path";
import vscode from "vscode";
import { sleep } from "../../utils/time";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";
import { executeCloseAllEditors, executeGenerateParamsCommand } from "./commands";

describe("generateParams", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate parameters file if the compiled template already exists", async () => {
    const tempFolder = createUniqueTempFolder("bicep-generate-params-");

    try {
      const bicepFilePath = path.join(tempFolder, "main.bicep");
      const templateJsonPath = path.join(tempFolder, "main.json");
      const parametersJsonPath = path.join(tempFolder, "main.parameters.json");

      fs.writeFileSync(bicepFilePath, "param name string\noutput used string = name\n");
      fs.writeFileSync(
        templateJsonPath,
        '{ "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#" }',
      );

      const textDocument = await vscode.workspace.openTextDocument(bicepFilePath);

      // Give the language server some time to finish compilation.
      await sleep(2000);

      await executeGenerateParamsCommand(textDocument.uri, "json", "requiredonly");

      expect(fs.existsSync(parametersJsonPath)).toBe(true);
      expect(fs.existsSync(templateJsonPath)).toBe(true);
    } finally {
      fs.rmSync(tempFolder, { recursive: true, force: true });
    }
  });
});
