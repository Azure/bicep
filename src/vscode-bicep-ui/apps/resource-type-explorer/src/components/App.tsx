// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Accordion } from "@vscode-bicep-ui/components";
import { useWebviewRequest } from "@vscode-bicep-ui/messaging";
import { ResourceTypeGroupHeader } from "./ResourceTypeGroupHeader";
import { ResourceTypeGroupList } from "./ResourceTypeGroupList";

export type ResourceTypes = {
  resourceType: string;
  apiVersion: string;
};

type ResourceTypeCatalog = Array<{
  group: string;
  resourceTypes: ResourceTypes[];
}>;

export function App() {
  const [resourceTypeCatalog, error] = useWebviewRequest<ResourceTypeCatalog>("resourceTypeCatalog/load");

  if (error instanceof Error) {
    console.error(error);
  }

  return (
    <>
      {error && <div>Failed to load resource types</div>}
      {resourceTypeCatalog && (
        <section>
          <Accordion>
            {resourceTypeCatalog.map(({ group, resourceTypes }) => (
              <Accordion.Item key={group}>
                <Accordion.ItemCollapse>
                  <ResourceTypeGroupHeader group={group} />
                </Accordion.ItemCollapse>
                <Accordion.ItemContent>
                  <ResourceTypeGroupList group={group} resourceTypes={resourceTypes} />
                </Accordion.ItemContent>
              </Accordion.Item>
            ))}
          </Accordion>
        </section>
      )}
    </>
  );
}
