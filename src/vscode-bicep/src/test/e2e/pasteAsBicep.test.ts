// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fse from "fs-extra";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";
import fs from "fs";
import path from "path";
import vscode, { ConfigurationTarget } from "vscode";
import { executeCloseAllEditors } from "./commands";
import { getBicepConfiguration } from "../../language/getBicepConfiguration";

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

describe("pasteAsBicep", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("todo", async () => {
    const folder = createUniqueTempFolder("pasteAsBicep");
    expect(true).toBeTruthy();

    const inputPath = path.join(folder, "main.bicep");
    fs.writeFileSync(inputPath, json);

    const textDocument = await vscode.workspace.openTextDocument(inputPath);
    await vscode.window.showTextDocument(textDocument);

    // Make sure Decompile on Paste is on
    await getBicepConfiguration().update(
      "decompileOnPaste",
      true,
      ConfigurationTarget.Global
    );

    vscode.env.clipboard.writeText(json);
    await vscode.commands.executeCommand("editor.action.clipboardPasteAction");

    const buffer = textDocument.getText();

    expect(buffer).toBe("asdfg");

    // Delete the folder
    fse.rmdirSync(folder, {
      recursive: true,
      maxRetries: 5,
      retryDelay: 1000,
    });
  });
});
