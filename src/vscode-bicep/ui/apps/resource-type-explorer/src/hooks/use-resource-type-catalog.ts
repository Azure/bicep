import { useEffect, useState } from "react";
import { webviewMessageChannel } from "@vscode-bicep/ui-messaging";

export type ResourceTypeCatalog = Record<string, string[]>;

export function useResourceTypeCatalog() {
  const [resourceTypeCatalog, setResourceTypeCatalog] =
    useState<ResourceTypeCatalog>({});

  useEffect(() => {
    const loadResourceTypeCatalog = async () => {
      const resourceTypeCatalogData = await webviewMessageChannel.sendRequest({
        kind: "resourceTypeCatalog/load",
      }) as ResourceTypeCatalog;
      
      setResourceTypeCatalog(resourceTypeCatalogData);
    };
    
    loadResourceTypeCatalog();
  }, []);
  
  return resourceTypeCatalog;
}
