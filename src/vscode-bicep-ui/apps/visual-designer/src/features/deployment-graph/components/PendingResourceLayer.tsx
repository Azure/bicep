// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { PanZoomTransformed } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import styled from "styled-components";
import { pendingResourcesAtom } from "../atoms";
import { ResourceNodePreview } from "./nodes/ResourceNodePreview";

const $Layer = styled(PanZoomTransformed)`
  position: absolute;
  inset: 0;
  transform-origin: 0 0;
  pointer-events: none;
  z-index: 90;
`;

const $PendingPosition = styled.div`
  position: absolute;
  transform: translate(-50%, -50%);
`;

export function PendingResourceLayer() {
  const pendingResources = useAtomValue(pendingResourcesAtom);

  return (
    <$Layer aria-live="polite">
      {pendingResources.map((pending) => (
        <$PendingPosition key={pending.operationId} style={{ left: pending.origin.x, top: pending.origin.y }}>
          <div data-testid="pending-resource-node">
            <ResourceNodePreview
              fullyQualifiedType={pending.resourceType.fullyQualifiedType}
              testId="pending-resource-card"
            />
          </div>
        </$PendingPosition>
      ))}
    </$Layer>
  );
}
