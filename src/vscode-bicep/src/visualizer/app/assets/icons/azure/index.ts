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
    case "Microsoft.Cdn/profiles":
      return (
        await import("./networking/00056-icon-service-CDN-Profiles.svg")
        ).default;
    case "Microsoft.Network/ipGroups":
      return (
        await import("./networking/00701-icon-service-IP-Groups.svg")
        ).default;
    case "Microsoft.Network/privateLinkServices":
      return (
        await import("./networking/01105-icon-service-Private-Link-Service.svg")
        ).default;
    case "Microsoft.Network/trafficmanagerprofiles":
      return (
        await import("./networking/10065-icon-service-Traffic-Manager-Profiles.svg")
        ).default;
    case "Microsoft.Network/NetworkWatchers":
      return (
        await import("./networking/10066-icon-service-Network-Watcher.svg")
        ).default;
    case "Microsoft.Network/routeFilters":
      return (
        await import("./networking/10071-icon-service-Route-Filters.svg")
        ).default;
    case "Microsoft.Network/ddosProtectionPlans":
      return (
        await import("./networking/10072-icon-service-DDoS-Protection-Plans.svg")
        ).default;
    case "Microsoft.Network/frontdoors":
      return (
        await import("./networking/10073-icon-service-Front-Doors.svg")
        ).default;
    case "Microsoft.Network/applicationGateways":
      return (
        await import("./networking/10076-icon-service-Application-Gateways.svg")
        ).default;
    case "Microsoft.Network/localNetworkGateways":
      return (
        await import("./networking/10077-icon-service-Local-Network-Gateways.svg")
        ).default;
    case "Microsoft.Network/expressRouteCircuits":
      return (
        await import("./networking/10079-icon-service-ExpressRoute-Circuits.svg")
        ).default;
    case "Microsoft.Network/connections":
      return (
        await import("./networking/10081-icon-service-Connections.svg")
        ).default;
    case "Microsoft.Network/route-Tables":
      return (
        await import("./networking/10082-icon-service-Route-Tables.svg")
        ).default;
    case "Microsoft.Network/azureFirewalls":
      return (
        await import("./networking/10084-icon-service-Firewalls.svg")
        ).default;
    case "Microsoft.Network/serviceEndpointPolicies":
      return (
        await import("./networking/10085-icon-service-Service-Endpoint-Policies.svg")
        ).default;
    case "Microsoft.Network/natGateways":
      return (
        await import("./networking/10310-icon-service-NAT.svg")
        ).default;
    case "Microsoft.Network/virtualWans":
      return (
        await import("./networking/10353-icon-service-Virtual-WANs.svg")
        ).default;
    case "Microsoft.Network/firewallPolicies":
      return (
        await import("./networking/10362-icon-service-Web-Application-Firewall-Policies(WAF).svg")
        ).default;
    case "Microsoft.Network/publicIPPrefixes":
      return (
        await import("./networking/10372-icon-service-Public-IP-Prefixes.svg")
        ).default;
               
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
