// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";

import { ext } from "../../extensionVariables";
import { AzLocationTreeItem } from "./AzLocationTreeItem";
import { AzManagementGroupTreeItem } from "./AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "./AzResourceGroupTreeItem";

export function registerTrees(): void {
  const azLocationTreeItem: AzLocationTreeItem = new AzLocationTreeItem();
  ext.azLocationTree = new AzExtTreeDataProvider(azLocationTreeItem, "");

  const azManagementGroupTreeItem: AzManagementGroupTreeItem =
    new AzManagementGroupTreeItem();
  ext.azManagementGroupTreeItem = new AzExtTreeDataProvider(
    azManagementGroupTreeItem,
    ""
  );

  const azResourceGroupTreeItem: AzResourceGroupTreeItem =
    new AzResourceGroupTreeItem();
  ext.azResourceGroupTreeItem = new AzExtTreeDataProvider(
    azResourceGroupTreeItem,
    ""
  );
}
