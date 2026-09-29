// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { compareNamespaces, filterResourceTypeGroups, groupResourceTypes } from "../resource-type-groups";

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
