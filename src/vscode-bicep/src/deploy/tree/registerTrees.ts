/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See LICENSE.md in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { ext } from "../../extensionVariables";
import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";
import { AzLocationTreeItem } from "./AzLocationTreeItem";
import { AzResourceGroupTreeItem } from "./AzResourceGroupTreeItem";
import { AzLoginTreeItem } from "./AzLoginTreeItem";
import { AzManagementGroupTreeItem } from "./AzManagementGroupTreeItem";

export function registerTrees(): void {
  const azLocationTreeItem: AzLocationTreeItem = new AzLocationTreeItem();
  ext.azLocationTree = new AzExtTreeDataProvider(azLocationTreeItem, "");

  const azLoginTreeItem: AzLoginTreeItem = new AzLoginTreeItem();
  ext.azLoginTreeItem = new AzExtTreeDataProvider(azLoginTreeItem, "");

  const azManagementGroupTreeItem: AzManagementGroupTreeItem = new AzManagementGroupTreeItem();
  ext.azManagementGroupTreeItem = new AzExtTreeDataProvider(azManagementGroupTreeItem, "");

  const azResourceGroupTreeItem: AzResourceGroupTreeItem = new AzResourceGroupTreeItem();
  ext.azResourceGroupTreeItem = new AzExtTreeDataProvider(azResourceGroupTreeItem,"");
}
