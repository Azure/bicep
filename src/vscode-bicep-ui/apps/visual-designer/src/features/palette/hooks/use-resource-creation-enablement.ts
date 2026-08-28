// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useWebviewNotification, useWebviewRequest } from "@vscode-bicep-ui/messaging";
import { useCallback, useState } from "react";
import {
  GET_RESOURCE_CREATION_ENABLEMENT_REQUEST,
  RESOURCE_CREATION_ENABLEMENT_DID_CHANGE_NOTIFICATION,
} from "@/lib/messaging";

export function useResourceCreationEnablement(): boolean {
  const [initialEnablement] = useWebviewRequest<boolean>(GET_RESOURCE_CREATION_ENABLEMENT_REQUEST);
  const [updatedEnablement, setUpdatedEnablement] = useState<boolean>();

  useWebviewNotification(
    RESOURCE_CREATION_ENABLEMENT_DID_CHANGE_NOTIFICATION,
    useCallback((value: unknown) => {
      if (typeof value === "boolean") {
        setUpdatedEnablement(value);
      }
    }, []),
  );

  return updatedEnablement ?? initialEnablement ?? false;
}
