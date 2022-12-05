// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import fse from "fs-extra";
import path from "path";
import vscode from "vscode";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";

import { executeCloseAllEditors, executeDecompileCommand } from "./commands";

const json = `
{
	"$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
	"contentVersion": "1.0.0.0",
	"metadata": {
		"prefix": "arm-nested-template-inner",
		"description": "Nested (inline) Inner-Scoped Deployment. Defines its own local parameters."
	},
	"resources": [
		{
            "name": "nestedDeployment1",
            "type": "Microsoft.Resources/deployments",
            "apiVersion": "2021-04-01",
            "properties": {
                "mode": "Incremental",
                "template": {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "variables": {},
                    "resources": [],
                    "outputs": {}
                }
            }
        }
	]
}`;

describe("decompile", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate decompiled files", async () => {
    const folder = createUniqueTempFolder("decompile");

    const inputPath = path.join(folder, "my template.json");
    fs.writeFileSync(inputPath, json);

    const textDocument = await vscode.workspace.openTextDocument(inputPath);

    await executeDecompileCommand(textDocument.uri);

    const outputPath1 = path.join(folder, "my template.bicep");
    const outputPath2 = path.join(folder, "nested_nestedDeployment1.bicep");

    expect(fs.existsSync(outputPath1)).toBe(true);
    expect(fs.existsSync(outputPath2)).toBe(true);

    // Delete the folder
    fse.rmdirSync(folder, {
      recursive: true,
      maxRetries: 5,
      retryDelay: 1000,
    });
  });
});
