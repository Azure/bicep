// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import {
  allResourceTypeGroups,
  compareNamespaces,
  featuredResourceTypeGroups,
  filterResourceTypeGroups,
  findResourceTypes,
  groupResourceTypes,
  isFeaturedNamespace,
} from "../resource-type-groups";

describe("compareNamespaces", () => {
  it("puts featured providers first, then other Microsoft providers, then third-party providers", () => {
    const namespaces = [
      "Other.Rp",
      "Microsoft.Storage",
      "Microsoft.Web",
      "Microsoft.Preview",
      "microsoft.Other",
      "Microsoft.Network",
      "microsoft.compute",
      "Microsoft.KeyVault",
      "Microsoft.Sql",
      "Microsoft.App",
      "Microsoft.ContainerService",
      "Microsoft.ContainerRegistry",
      "Microsoft.ManagedIdentity",
      "Microsoft.Authorization",
      "Microsoft.Resources",
      "Microsoft.DBforPostgreSQL",
      "Microsoft.DBforMySQL",
      "Microsoft.DocumentDB",
      "Microsoft.Cache",
      "Microsoft.CognitiveServices",
      "Microsoft.Insights",
      "Microsoft.OperationalInsights",
      "Microsoft.ServiceBus",
      "Microsoft.EventHub",
      "Another.Rp",
      "alpha.Rp",
    ];

    expect([...namespaces].sort(compareNamespaces)).toEqual([
      "microsoft.compute",
      "Microsoft.Network",
      "Microsoft.Storage",
      "Microsoft.Web",
      "Microsoft.App",
      "Microsoft.ContainerService",
      "Microsoft.ContainerRegistry",
      "Microsoft.KeyVault",
      "Microsoft.ManagedIdentity",
      "Microsoft.Authorization",
      "Microsoft.Resources",
      "Microsoft.Sql",
      "Microsoft.DBforPostgreSQL",
      "Microsoft.DBforMySQL",
      "Microsoft.DocumentDB",
      "Microsoft.Cache",
      "Microsoft.CognitiveServices",
      "Microsoft.Insights",
      "Microsoft.OperationalInsights",
      "Microsoft.ServiceBus",
      "Microsoft.EventHub",
      "microsoft.Other",
      "Microsoft.Preview",
      "alpha.Rp",
      "Another.Rp",
      "Other.Rp",
    ]);
  });

  it("sorts providers alphabetically within the Microsoft and third-party groups when none are featured", () => {
    expect(["Zeta.Rp", "alpha.Rp", "Microsoft.Zeta", "microsoft.Alpha"].sort(compareNamespaces)).toEqual([
      "microsoft.Alpha",
      "Microsoft.Zeta",
      "alpha.Rp",
      "Zeta.Rp",
    ]);
  });
});

describe("groupResourceTypes", () => {
  it("groups types by namespace in browsing order, sorting each group's types", () => {
    const groups = groupResourceTypes([
      { fullyQualifiedType: "Other.Rp/widgets", apiVersion: "2024-01-01" },
      { fullyQualifiedType: "Microsoft.Storage/storageAccounts/blobServices", apiVersion: "2024-01-01" },
      { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2025-01-01" },
      { fullyQualifiedType: "invalid", apiVersion: "2024-01-01" },
    ]);

    expect(groups).toEqual([
      {
        group: "Microsoft.Storage",
        resourceTypes: [
          { resourceType: "storageAccounts", apiVersion: "2025-01-01" },
          { resourceType: "storageAccounts/blobServices", apiVersion: "2024-01-01" },
        ],
      },
      { group: "Other.Rp", resourceTypes: [{ resourceType: "widgets", apiVersion: "2024-01-01" }] },
    ]);
  });
});

describe("filterResourceTypeGroups", () => {
  const groups = groupResourceTypes([
    { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2025-01-01" },
    { fullyQualifiedType: "Microsoft.Network/virtualNetworks", apiVersion: "2024-01-01" },
  ]);

  it("matches the namespace or type name, ignoring case, and drops empty groups", () => {
    expect(filterResourceTypeGroups(groups, "STORAGEACC").map(({ group }) => group)).toEqual(["Microsoft.Storage"]);
    expect(filterResourceTypeGroups(groups, "microsoft.network/").map(({ group }) => group)).toEqual([
      "Microsoft.Network",
    ]);
    expect(filterResourceTypeGroups(groups, "missing")).toEqual([]);
  });
});

describe("palette views", () => {
  const groups = groupResourceTypes([
    { fullyQualifiedType: "Other.Rp/widgets", apiVersion: "2024-01-01" },
    { fullyQualifiedType: "Microsoft.Preview/widgets", apiVersion: "2026-01-01-preview" },
    { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2025-01-01" },
    { fullyQualifiedType: "Microsoft.Network/virtualNetworks", apiVersion: "2024-07-01" },
    { fullyQualifiedType: "Microsoft.Network/virtualNetworks/subnets", apiVersion: "2024-07-01" },
  ]);

  it("recognizes featured namespaces regardless of case", () => {
    expect(isFeaturedNamespace("microsoft.storage")).toBe(true);
    expect(isFeaturedNamespace("Microsoft.Preview")).toBe(false);
  });

  it("Featured lists only featured providers, in featured order", () => {
    expect(featuredResourceTypeGroups(groups).map(({ group }) => group)).toEqual([
      "Microsoft.Network",
      "Microsoft.Storage",
    ]);
  });

  it("All lists every provider alphabetically, Microsoft providers first", () => {
    expect(allResourceTypeGroups(groups).map(({ group }) => group)).toEqual([
      "Microsoft.Network",
      "Microsoft.Preview",
      "Microsoft.Storage",
      "Other.Rp",
    ]);
  });

  it("finds types in the order given, at the catalog's default version, skipping types it no longer offers", () => {
    expect(
      findResourceTypes(groups, [
        "microsoft.network/virtualnetworks/subnets",
        "Microsoft.Compute/virtualMachines",
        "Microsoft.Storage/storageAccounts",
      ]),
    ).toEqual([
      { fullyQualifiedType: "Microsoft.Network/virtualNetworks/subnets", apiVersion: "2024-07-01" },
      { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2025-01-01" },
    ]);
  });
});
