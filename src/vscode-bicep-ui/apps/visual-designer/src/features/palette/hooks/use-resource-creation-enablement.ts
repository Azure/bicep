// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useNotification, useRequest } from "@vscode-bicep-ui/messaging";
import { useCallback, useState } from "react";
import { getResourceCreationEnablement, resourceCreationEnablementDidChange } from "../api";

export function useResourceCreationEnablement(): boolean {
  const [initialEnablement] = useRequest(getResourceCreationEnablement);
  const [updatedEnablement, setUpdatedEnablement] = useState<boolean>();

  useNotification(
    resourceCreationEnablementDidChange,
    useCallback((enabled: boolean) => setUpdatedEnablement(enabled), []),
  );

  return updatedEnablement ?? initialEnablement ?? false;
}
