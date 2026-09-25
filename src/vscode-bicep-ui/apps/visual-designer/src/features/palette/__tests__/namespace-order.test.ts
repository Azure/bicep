// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { orderNamespaces } from "../namespace-order";

describe("orderNamespaces", () => {
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
    ].map((name) => ({ name, resourceTypeCount: 1 }));

    expect(orderNamespaces(namespaces).map(({ name }) => name)).toEqual([
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
    expect(namespaces[0].name).toBe("Other.Rp");
  });

  it("sorts providers alphabetically within the Microsoft and third-party groups when none are featured", () => {
    expect(
      orderNamespaces([
        { name: "Zeta.Rp", resourceTypeCount: 500 },
        { name: "alpha.Rp", resourceTypeCount: 1 },
        { name: "Microsoft.Zeta", resourceTypeCount: 1 },
        { name: "microsoft.Alpha", resourceTypeCount: 1 },
      ]).map(({ name }) => name),
    ).toEqual(["microsoft.Alpha", "Microsoft.Zeta", "alpha.Rp", "Zeta.Rp"]);
  });
});
