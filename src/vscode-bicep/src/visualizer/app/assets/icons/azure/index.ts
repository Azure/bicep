// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export async function importResourceIconInline(
  resourceType: string
): Promise<string> {
  switch (resourceType.toLowerCase()) {
    // Microsoft.Compute
    case "microsoft.compute/availabilitysets":
      return (await import("./compute/10034-icon-service-VM-Scale-Sets.svg"))
        .default;
    case "microsoft.compute/disks":
      return (await import("./compute/10032-icon-service-Disks.svg")).default;
    case "microsoft.compute/virtualmachines":
      return (await import("./compute/10021-icon-service-Virtual-Machine.svg"))
        .default;
    case "microsoft.compute/virtualmachinescalesets":
      return (await import("./compute/10034-icon-service-VM-Scale-Sets.svg"))
        .default;

    // Microsoft.SQL
    case "microsoft.sql/servers":
      return (await import("./databases/10132-icon-service-SQL-Server.svg"))
        .default;
    case "microsoft.sql/servers/databases":
      return (await import("./databases/10130-icon-service-SQL-Database.svg"))
        .default;

    // Microsoft.Network
    case "microsoft.network/dnszones":
      return (await import("./networking/10064-icon-service-DNS-Zones.svg"))
        .default;
    case "microsoft.network/loadbalancers":
      return (
        await import("./networking/10062-icon-service-Load-Balancers.svg")
      ).default;
    case "microsoft.network/networkinterfaces":
      return (
        await import("./networking/10080-icon-service-Network-Interfaces.svg")
      ).default;
    case "microsoft.network/networksecuritygroups":
      return (
        await import(
          "./networking/10067-icon-service-Network-Security-Groups.svg"
        )
      ).default;
    case "microsoft.network/publicipaddresses":
      return (
        await import("./networking/10069-icon-service-Public-IP-Addresses.svg")
      ).default;
    case "microsoft.network/virtualnetworkgateways":
      return (
        await import(
          "./networking/10063-icon-service-Virtual-Network-Gateways.svg"
        )
      ).default;
    case "microsoft.network/virtualnetworks":
      return (
        await import("./networking/10061-icon-service-Virtual-Networks.svg")
      ).default;
    case "microsoft.network/virtualnetworks/subnets":
      return (await import("./custom/subnets.svg")).default;

    // Microsoft.Resources
    case "microsoft.resources/resourcegroups":
      return (await import("./general/10007-icon-service-Resource-Groups.svg"))
        .default;

    // Microsoft.Security
    case "microsoft.keyvault/vaults":
      return (await import("./security/10245-icon-service-Key-Vaults.svg"))
        .default;

    // Microsoft.Subscriptions
    case "microsoft.subscription/aliases":
      return (await import("./general/10002-icon-service-Subscriptions.svg"))
        .default;

    // Microsoft.Storage
    case "microsoft.storage/storageaccounts":
      return (await import("./storage/10086-icon-service-Storage-Accounts.svg"))
        .default;

    // Microsoft.Web
    case "microsoft.web/serverfarms":
      return (
        await import("./appServices/00046-icon-service-App-Service-Plans.svg")
      ).default;
    case "microsoft.web/sites":
      return (await import("./appServices/10035-icon-service-App-Services.svg"))
        .default;

    // Microsoft.ApiManagement
    case "microsoft.apimanagement/service":
      return (
        await import(
          "./appServices/10042-icon-service-API-Management-Services.svg"
        )
      ).default;

    // Microsoft.Cdn
    case "microsoft.cdn/service":
      return (await import("./appServices/00056-icon-service-CDN-Profiles.svg"))
        .default;

    default:
      return (await import("./custom/resource.svg")).default;
  }
}
