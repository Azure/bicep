// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { TargetScope } from "../api";

import { AzureIcon, Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import styled from "styled-components";
import { targetScopeAtom } from "../atoms";

const SCOPE_PRESENTATION: Record<TargetScope, { label: string; resourceType?: string }> = {
  resourceGroup: { label: "Resource group", resourceType: "Microsoft.Resources/resourceGroups" },
  subscription: { label: "Subscription", resourceType: "Microsoft.Subscription/aliases" },
  managementGroup: { label: "Management group", resourceType: "Microsoft.Management/managementGroups" },
  tenant: { label: "Tenant" },
};

const $Indicator = styled.div`
  position: absolute;
  top: 16px;
  left: 16px;
  z-index: 100;
  display: flex;
  align-items: center;
  gap: 7px;
  color: ${({ theme }) => theme.text.secondary};
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  pointer-events: none;
  user-select: none;
`;

/** Passive document metadata, deliberately outside the exported canvas subtree. */
export function ScopeIndicator() {
  const scope = useAtomValue(targetScopeAtom);
  if (!scope) {
    return null;
  }
  const { label, resourceType } = SCOPE_PRESENTATION[scope];

  return (
    <$Indicator data-testid="target-scope" role="group" aria-label={`Target scope: ${label}`}>
      {resourceType ? <AzureIcon resourceType={resourceType} size={18} /> : <Codicon name="organization" size={18} />}
      <span>{label}</span>
    </$Indicator>
  );
}
