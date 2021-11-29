// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export async function importResourceIconInline(
  resourceType: string
): Promise<string> {
  switch (resourceType.toLowerCase()) {
    // Microsoft.Compute
    case "microsoft.compute/availabilitysets":
      return (
        await import("./compute/10025-icon-service-Availability-Sets.svg")
      ).default;
    case "microsoft.compute/disks":
      return (await import("./compute/10032-icon-service-Disks.svg")).default;
    case "microsoft.compute/virtualmachines":
      return (await import("./compute/10021-icon-service-Virtual-Machine.svg"))
        .default;
    case "microsoft.compute/virtualmachinescalesets":
      return (await import("./compute/10034-icon-service-VM-Scale-Sets.svg"))
        .default;
    case "microsoft.compute/proximityplacementgroups":
      return (
        await import(
          "./compute/10365-icon-service-Proximity-Placement-Groups.svg"
        )
      ).default;
    case "microsoft.compute/diskencryptionsets":
      return (
        await import("./compute/00398-icon-service-Disk-Encryption-Sets.svg")
      ).default;
    case "microsoft.containerservice/managedclusters":
      return (
        await import("./compute/10023-icon-service-Kubernetes-Services.svg")
      ).default;
    case "microsoft.compute/snapshots":
      return (await import("./compute/10026-icon-service-Disks-Snapshots.svg"))
        .default;
    case "microsoft.batch/batchaccounts":
      return (await import("./compute/10031-icon-service-Batch-Accounts.svg"))
        .default;
    case "microsoft.compute/images":
      return (await import("./compute/10033-icon-service-Images.svg")).default;
    case "microsoft.compute/galleries":
      return (
        await import("./compute/10039-icon-service-Shared-Image-Galleries.svg")
      ).default;

    // Microsoft.SQL
    case "microsoft.sql/servers":
      return (await import("./databases/10132-icon-service-SQL-Server.svg"))
        .default;
    case "microsoft.sql/servers/databases":
      return (await import("./databases/10130-icon-service-SQL-Database.svg"))
        .default;
    case "microsoft.synapse/workspaces":
      return (
        await import(
          "./databases/00606-icon-service-Azure-Synapse-Analytics.svg"
        )
      ).default;
    case "microsoft.documentdb/databaseaccounts":
      return (
        await import("./databases/10121-icon-service-Azure-Cosmos-DB.svg")
      ).default;
    case "microsoft.dbformysql/servers":
      return (
        await import(
          "./databases/10122-icon-service-Azure-Database-MySQL-Server.svg"
        )
      ).default;
    case "microsoft.dbformariadb/servers":
      return (
        await import(
          "./databases/10123-icon-service-Azure-Database-MariaDB-Server.svg"
        )
      ).default;
    case "microsoft.sqlvirtualmachine/sqlvirtualmachines":
      return (await import("./databases/10124-icon-service-Azure-SQL-VM.svg"))
        .default;
    case "microsoft.datafactory/factories":
      return (await import("./databases/10126-icon-service-Data-Factory.svg"))
        .default;
    case "microsoft.dbforpostgressql/servers":
      return (
        await import(
          "./databases/10131-icon-service-Azure-Database-PostgreSQL-Server.svg"
        )
      ).default;
    case "microsoft.datamigration/services":
      return (
        await import(
          "./databases/10133-icon-service-Azure-Database-Migration-Services.svg"
        )
      ).default;
    case "microsoft.sql/servers/elasticpools":
      return (
        await import("./databases/10134-icon-service-SQL-Elastic-Pools.svg")
      ).default;
    case "microsoft.sql/managedinstances":
      return (
        await import("./databases/10136-icon-service-SQL-Managed-Instance.svg")
      ).default;
    case "microsoft.sql/managedinstances/databases":
      return (
        await import("./databases/10135-icon-service-Managed-Database.svg")
      ).default;
    case "microsoft.cache/redis":
      return (await import("./databases/10137-icon-service-Cache-Redis.svg"))
        .default;
    case "microsoft.sql/instancepools":
      return (await import("./databases/10139-icon-service-Instance-Pools.svg"))
        .default;

    // microsoft.network
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
    case "microsoft.network/ipgroups":
      return (await import("./networking/00701-icon-service-IP-Groups.svg"))
        .default;
    case "microsoft.network/privatelinkservices":
      return (
        await import("./networking/01105-icon-service-Private-Link-Service.svg")
      ).default;
    case "microsoft.network/trafficmanagerprofiles":
      return (
        await import(
          "./networking/10065-icon-service-Traffic-Manager-Profiles.svg"
        )
      ).default;
    case "microsoft.network/networkwatchers":
      return (
        await import("./networking/10066-icon-service-Network-Watcher.svg")
      ).default;
    case "microsoft.network/routefilters":
      return (await import("./networking/10071-icon-service-Route-Filters.svg"))
        .default;
    case "microsoft.network/ddosprotectionplans":
      return (
        await import(
          "./networking/10072-icon-service-DDoS-Protection-Plans.svg"
        )
      ).default;
    case "microsoft.network/frontdoors":
      return (await import("./networking/10073-icon-service-Front-Doors.svg"))
        .default;
    case "microsoft.network/applicationgateways":
      return (
        await import("./networking/10076-icon-service-Application-Gateways.svg")
      ).default;
    case "microsoft.network/localnetworkgateways":
      return (
        await import(
          "./networking/10077-icon-service-Local-Network-Gateways.svg"
        )
      ).default;
    case "microsoft.network/expressroutecircuits":
      return (
        await import(
          "./networking/10079-icon-service-ExpressRoute-Circuits.svg"
        )
      ).default;
    case "microsoft.network/connections":
      return (await import("./networking/10081-icon-service-Connections.svg"))
        .default;
    case "microsoft.network/routetables":
      return (await import("./networking/10082-icon-service-Route-Tables.svg"))
        .default;
    case "microsoft.network/azurefirewalls":
      return (await import("./networking/10084-icon-service-Firewalls.svg"))
        .default;
    case "microsoft.network/serviceendpointpolicies":
      return (
        await import(
          "./networking/10085-icon-service-Service-Endpoint-Policies.svg"
        )
      ).default;
    case "microsoft.network/natgateways":
      return (await import("./networking/10310-icon-service-NAT.svg")).default;
    case "microsoft.network/virtualwans":
      return (await import("./networking/10353-icon-service-Virtual-WANs.svg"))
        .default;
    case "microsoft.network/firewallpolicies":
      return (
        await import(
          "./networking/10362-icon-service-Web-Application-Firewall-Policies(WAF).svg"
        )
      ).default;
    case "microsoft.network/publicipprefixes":
      return (
        await import("./networking/10372-icon-service-Public-IP-Prefixes.svg")
      ).default;
    case "microsoft.network/applicationsecuritygroups":
      return (
        await import(
          "./security/10244-icon-service-Application-Security-Groups.svg"
        )
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
    case "microsoft.recoveryservices/vaults":
      return (
        await import(
          "./storage/00017-icon-service-Recovery-Services-Vaults.svg"
        )
      ).default;
    case "microsoft.storagesync/storagesyncservices":
      return (
        await import("./storage/10093-icon-service-Storage-Sync-Services.svg")
      ).default;
    case "microsoft.databox/jobs":
      return (await import("./storage/10094-icon-service-Data-Box.svg"))
        .default;
    case "microsoft.databoxedge/databoxedgedevices":
      return (await import("./storage/10095-icon-service-Data-Box-Edge.svg"))
        .default;
    case "microsoft.netapp/netappaccounts":
      return (
        await import("./storage/10096-icon-service-Azure-NetApp-Files.svg")
      ).default;
    case "microsoft.datashare/accounts":
      return (await import("./storage/10098-icon-service-Data-Shares.svg"))
        .default;

    // Microsoft.Web
    case "microsoft.web/serverfarms":
      return (
        await import("./appServices/00046-icon-service-App-Service-Plans.svg")
      ).default;
    case "microsoft.web/sites":
      return (await import("./appServices/10035-icon-service-App-Services.svg"))
        .default;
    case "microsoft.web/certificates":
      return (
        await import(
          "./appServices/00049-icon-service-App-Service-Certificates.svg"
        )
      ).default;
    case "microsoft.web/hostingenvironments":
      return (
        await import(
          "./appServices/10047-icon-service-App-Service-Environments.svg"
        )
      ).default;

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
