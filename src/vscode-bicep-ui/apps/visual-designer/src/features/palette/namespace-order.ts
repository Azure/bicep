// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeNamespace } from "./types";

const featuredNamespaces = [
  "Microsoft.Compute",
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
];

const featuredRanks = new Map(featuredNamespaces.map((name, index) => [name.toLowerCase(), index]));

export function orderNamespaces(namespaces: readonly ResourceTypeNamespace[]): ResourceTypeNamespace[] {
  return [...namespaces].sort((left, right) => {
    const leftName = left.name.toLowerCase();
    const rightName = right.name.toLowerCase();
    const leftRank = featuredRanks.get(leftName) ?? featuredNamespaces.length;
    const rightRank = featuredRanks.get(rightName) ?? featuredNamespaces.length;

    return (
      leftRank - rightRank ||
      Number(rightName.startsWith("microsoft.")) - Number(leftName.startsWith("microsoft.")) ||
      leftName.localeCompare(rightName)
    );
  });
}
