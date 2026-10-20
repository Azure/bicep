// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import { workspace } from "vscode";
import { executeCloseAllEditors, executeDecompileCommand } from "./utils/commands";
import { withTempDirectory } from "./utils/temp-directory";

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
    await withTempDirectory("decompile", async (folder) => {
      const inputPath = path.join(folder, "my template.json");
      fs.writeFileSync(inputPath, json);

      const textDocument = await workspace.openTextDocument(inputPath);

      await executeDecompileCommand(textDocument.uri);

      const outputPath1 = path.join(folder, "my template.bicep");
      const outputPath2 = path.join(folder, "nested_nestedDeployment1.bicep");

      expect(fs.existsSync(outputPath1)).toBe(true);
      expect(fs.existsSync(outputPath2)).toBe(true);
    });
  });
});
