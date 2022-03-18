// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  ManagementGroupInfo,
  ManagementGroupsAPI,
} from "@azure/arm-managementgroups";
import { DefaultAzureCredential } from "@azure/identity";
import { uiUtils } from "@microsoft/vscode-azext-azureutils";
import {
  AzExtParentTreeItem,
  AzExtTreeItem,
} from "@microsoft/vscode-azext-utils";

import { localize } from "../utils/localize";
import { GenericAzExtTreeItem } from "./GenericAzExtTreeItem";

export class AzManagementGroupTreeItem extends AzExtParentTreeItem {
  public readonly childTypeLabel = localize(
    "managementGroup",
    "Management Group"
  );
  public contextValue = "managementGroup";
  public label: string = this.childTypeLabel;

  constructor() {
    super(undefined);
  }

  public async loadMoreChildrenImpl(): Promise<AzExtTreeItem[]> {
    const managementGroupsAPI = new ManagementGroupsAPI(
      new DefaultAzureCredential()
    );

    const managementGroupInfos = managementGroupsAPI.managementGroups.list();

    if (managementGroupInfos.byPage.length == 0) {
      throw new Error(
        "You do not have access to any management group. Please create one in azure portal and try deploy again"
      );
    }

    const managementGroupInfoList: ManagementGroupInfo[] =
      await uiUtils.listAllIterator(managementGroupInfos);

    return await this.createTreeItemsWithErrorHandling(
      managementGroupInfoList,
      "invalidManagementGroup",
      (mg) => new GenericAzExtTreeItem(this, mg.id, mg.name),
      (mg) => mg.name
    );
  }

  public hasMoreChildrenImpl(): boolean {
    return false;
  }
}
