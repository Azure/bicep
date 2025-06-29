// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import styled from "styled-components";
import { useAzureSvg } from "./useAzureSvg";

interface AzureIconProps {
  /**
   * Fully qualified Azure resource type, e.g. Microsoft.Compute/virtualMachines
   */
  resourceType: string;
  /**
   * Size of the icon in pixels
   */
  size: number;
}

const $AzureIcon = styled.div<{ $size: number }>`
  width: ${(props) => props.$size}px;
  height: ${(props) => props.$size}px;
  text-align: center;
  display: inline-block;
`;

/**
 * UI component for rendering an Azure resource type icon
 */
export function AzureIcon({ resourceType, size }: AzureIconProps) {
  const { loading, AzureSvg } = useAzureSvg(resourceType);

  return (
    <$AzureIcon $size={size} aria-hidden={true} data-testid={`${resourceType}-icon`}>
      {!loading && AzureSvg && <AzureSvg width="100%" height="100%" data-testid={`${resourceType}-svg`} />}
    </$AzureIcon>
  );
}
