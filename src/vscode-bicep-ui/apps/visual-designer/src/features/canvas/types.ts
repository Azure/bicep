// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * A resource type the user can create. Mirrors the host's resource-creation contract.
 *
 * This lives here rather than with the palette because it is the parameter of `resources/create`,
 * the request this feature declares. The palette produces these values; the canvas owns the contract
 * they satisfy.
 */
export interface ResourceTypeReference {
  fullyQualifiedType: string;
  apiVersion: string;
}
