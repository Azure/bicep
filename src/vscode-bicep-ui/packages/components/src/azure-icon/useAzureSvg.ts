// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FunctionComponent, SVGProps } from "react";

import { useEffect, useRef, useState } from "react";

type SvgComponent = FunctionComponent<SVGProps<SVGSVGElement>>;

const svgImportsByPath = import.meta.glob<SvgComponent>(`../../assets/azure-architecture-icons/**/*.svg`, {
  query: "react",
  import: "default",
});

const svgPathsByResourceType: Record<string, string> = {
  // Microsoft.Compute
  "Microsoft.Compute/availabilitySets": "compute/10025-icon-service-Availability-Sets",
  "Microsoft.Compute/disks": "compute/10032-icon-service-Disks",
  "Microsoft.Compute/virtualMachines": "compute/10021-icon-service-Virtual-Machine",
  "Microsoft.Compute/virtualMachineScaleSets": "compute/10034-icon-service-VM-Scale-Sets",
  "Microsoft.Compute/proximityPlacementGroups": "compute/10365-icon-service-Proximity-Placement-Groups",
  "Microsoft.Compute/diskEncryptionSets": "compute/00398-icon-service-Disk-Encryption-Sets",
  "Microsoft.ContainerService/managedClusters": "compute/10023-icon-service-Kubernetes-Services",
  "Microsoft.Compute/snapshots": "compute/10026-icon-service-Disks-Snapshots",
  "Microsoft.Batch/batchAccounts": "compute/10031-icon-service-Batch-Accounts",
  "Microsoft.Compute/images": "compute/10033-icon-service-Images",
  "Microsoft.Compute/galleries": "compute/10039-icon-service-Shared-Image-Galleries",

  // Microsoft.SQL / Databases
  "Microsoft.Sql/servers": "databases/10132-icon-service-SQL-Server",
  "Microsoft.Sql/servers/databases": "databases/10130-icon-service-SQL-Database",
  "Microsoft.DocumentDB/databaseAccounts": "databases/10121-icon-service-Azure-Cosmos-DB",
  "Microsoft.DBforMySQL/servers": "databases/10122-icon-service-Azure-Database-MySQL-Server",
  "Microsoft.DBforMySQL/flexibleServers": "databases/10122-icon-service-Azure-Database-MySQL-Server",
  "Microsoft.DBforMariaDB/servers": "databases/10123-icon-service-Azure-Database-MariaDB-Server",
  "Microsoft.SqlVirtualMachine/sqlVirtualMachines": "databases/10124-icon-service-Azure-SQL-VM",
  "Microsoft.DataFactory/factories": "databases/10126-icon-service-Data-Factory",
  "Microsoft.DBforPostgreSQL/servers": "databases/10131-icon-service-Azure-Database-PostgreSQL-Server",
  "Microsoft.DBforPostgreSQL/flexibleServers": "databases/10131-icon-service-Azure-Database-PostgreSQL-Server",
  "Microsoft.DataMigration/services": "databases/10133-icon-service-Azure-Database-Migration-Services",
  "Microsoft.Sql/servers/elasticPools": "databases/10134-icon-service-SQL-Elastic-Pools",
  "Microsoft.Sql/managedInstances": "databases/10136-icon-service-SQL-Managed-Instance",
  "Microsoft.Sql/managedInstances/databases": "databases/10135-icon-service-Managed-Database",
  "Microsoft.Cache/redis": "databases/10137-icon-service-Cache-Redis",
  "Microsoft.Sql/instancePools": "databases/10139-icon-service-Instance-Pools",

  // Microsoft.Network
  "Microsoft.Network/privateDnsZones": "networking/10064-icon-service-DNS-Zones",
  "Microsoft.Network/dnsZones": "networking/10064-icon-service-DNS-Zones",
  "Microsoft.Network/loadBalancers": "networking/10062-icon-service-Load-Balancers",
  "Microsoft.Network/networkInterfaces": "networking/10080-icon-service-Network-Interfaces",
  "Microsoft.Network/networkSecurityGroups": "networking/10067-icon-service-Network-Security-Groups",
  "Microsoft.Network/publicIPAddresses": "networking/10069-icon-service-Public-IP-Addresses",
  "Microsoft.Network/virtualNetworkGateways": "networking/10063-icon-service-Virtual-Network-Gateways",
  "Microsoft.Network/virtualNetworks": "networking/10061-icon-service-Virtual-Networks",
  "Microsoft.Network/virtualNetworks/subnets": "custom/subnets",
  "Microsoft.Network/ipGroups": "networking/00701-icon-service-IP-Groups",
  "Microsoft.Network/privateLinkServices": "networking/01105-icon-service-Private-Link-Service",
  "Microsoft.Network/trafficManagerProfiles": "networking/10065-icon-service-Traffic-Manager-Profiles",
  "Microsoft.Network/networkWatchers": "networking/10066-icon-service-Network-Watcher",
  "Microsoft.Network/routeFilters": "networking/10071-icon-service-Route-Filters",
  "Microsoft.Network/ddosProtectionPlans": "networking/10072-icon-service-DDoS-Protection-Plans",
  "Microsoft.Network/frontDoors": "networking/10073-icon-service-Front-Doors",
  "Microsoft.Network/applicationGateways": "networking/10076-icon-service-Application-Gateways",
  "Microsoft.Network/localNetworkGateways": "networking/10077-icon-service-Local-Network-Gateways",
  "Microsoft.Network/expressRouteCircuits": "networking/10079-icon-service-ExpressRoute-Circuits",
  "Microsoft.Network/connections": "networking/10081-icon-service-Connections",
  "Microsoft.Network/routeTables": "networking/10082-icon-service-Route-Tables",
  "Microsoft.Network/azureFirewalls": "networking/10084-icon-service-Firewalls",
  "Microsoft.Network/serviceEndpointPolicies": "networking/10085-icon-service-Service-Endpoint-Policies",
  "Microsoft.Network/natGateways": "networking/10310-icon-service-NAT",
  "Microsoft.Network/virtualWans": "networking/10353-icon-service-Virtual-WANs",
  "Microsoft.Network/firewallPolicies": "networking/10362-icon-service-Web-Application-Firewall-Policies(WAF)",
  "Microsoft.Network/publicIPPrefixes": "networking/10372-icon-service-Public-IP-Prefixes",
  "Microsoft.Network/applicationSecurityGroups": "security/10244-icon-service-Application-Security-Groups",

  // Microsoft.Resources
  "Microsoft.Resources/resourceGroups": "general/10007-icon-service-Resource-Groups",

  // Microsoft.Security
  "Microsoft.KeyVault/vaults": "security/10245-icon-service-Key-Vaults",

  // Microsoft.Automation
  "Microsoft.Automation/automationAccounts": "management/00022-icon-service-Automation-Accounts",

  // Microsoft.Authorization
  "Microsoft.Authorization/policyDefinitions": "management/10316-icon-service-Policy",

  // Microsoft.Subscription
  "Microsoft.Subscription/aliases": "general/10002-icon-service-Subscriptions",

  // Microsoft.Storage
  "Microsoft.Storage/storageAccounts": "storage/10086-icon-service-Storage-Accounts",
  "Microsoft.Storage/storageAccounts/fileServices": "general/10838-icon-service-Storage-Azure-Files",
  "Microsoft.Storage/storageAccounts/queueServices": "general/10840-icon-service-Storage-Queue",
  "Microsoft.Storage/storageAccounts/tableServices": "general/10841-icon-service-Table",
  "Microsoft.RecoveryServices/vaults": "storage/00017-icon-service-Recovery-Services-Vaults",
  "Microsoft.StorageSync/storageSyncServices": "storage/10093-icon-service-Storage-Sync-Services",
  "Microsoft.DataBox/jobs": "storage/10094-icon-service-Data-Box",
  "Microsoft.DataBoxEdge/dataBoxEdgeDevices": "storage/10095-icon-service-Data-Box-Edge",
  "Microsoft.NetApp/netAppAccounts": "storage/10096-icon-service-Azure-NetApp-Files",
  "Microsoft.DataShare/accounts": "storage/10098-icon-service-Data-Shares",

  // Microsoft.Web
  "Microsoft.Web/serverfarms": "app-services/00046-icon-service-App-Service-Plans",
  "Microsoft.Web/sites": "app-services/10035-icon-service-App-Services",
  "Microsoft.Web/certificates": "app-services/00049-icon-service-App-Service-Certificates",
  "Microsoft.Web/hostingEnvironments": "app-services/10047-icon-service-App-Service-Environments",

  // Microsoft.NotificationHubs
  "Microsoft.NotificationHubs/namespaces": "iot/10045-icon-service-Notification-Hubs",

  // Microsoft.Devices
  "Microsoft.Devices/iotHubs": "iot/10182-icon-service-IoT-Hub",

  // Microsoft.IoTCentral
  "Microsoft.IoTCentral/iotApps": "iot/10184-icon-service-IoT-Central-Applications",

  // Microsoft.TimeSeriesInsights
  "Microsoft.TimeSeriesInsights/environments": "iot/10181-icon-service-Time-Series-Insights-Environments",

  // Microsoft.OperationalInsights
  "Microsoft.OperationalInsights/workspaces": "analytics/00009-icon-service-Log-Analytics-Workspaces",

  // Microsoft.EventHub
  "Microsoft.EventHub/namespaces": "analytics/00039-icon-service-Event-Hubs",
  "Microsoft.EventHub/clusters": "analytics/10149-icon-service-Event-Hub-Clusters",

  // Microsoft.StreamAnalytics
  "Microsoft.StreamAnalytics/streamingjobs": "analytics/00042-icon-service-Stream-Analytics-Jobs",

  // Microsoft.Synapse
  "Microsoft.Synapse/workspaces": "analytics/00606-icon-service-Azure-Synapse-Analytics",

  // Microsoft.Databricks
  "Microsoft.Databricks/workspaces": "analytics/10787-icon-service-Azure-Databricks",

  // Microsoft.BotService
  "Microsoft.BotService/botServices": "ai/10165-icon-service-Bot-Services",

  // Microsoft.CognitiveServices
  "Microsoft.CognitiveServices/accounts": "ai/10162-icon-service-Cognitive-Services",

  // Microsoft.MachineLearning
  "Microsoft.MachineLearning/workspaces": "ai/10167-icon-service-Machine-Learning-Studio-Workspaces",

  // Microsoft.HDInsight
  "Microsoft.HDInsight/clusters": "analytics/10142-icon-service-HD-Insight-Clusters",

  // Microsoft.AnalysisServices
  "Microsoft.AnalysisServices/servers": "analytics/10148-icon-service-Analysis-Services",

  // Microsoft.Insights
  "Microsoft.Insights/components": "devops/00012-icon-service-Application-Insights",

  // Microsoft.DevTestLab
  "Microsoft.DevTestLab/labs": "devops/10264-icon-service-DevTest-Labs",

  // Microsoft.AAD
  "Microsoft.AAD/domainServices": "identity/10222-icon-service-Azure-AD-Domain-Services",

  // Microsoft.Logic
  "Microsoft.Logic/workflows": "iot/10201-icon-service-Logic-Apps",

  // Microsoft.AzureActiveDirectory
  "Microsoft.AzureActiveDirectory/b2cDirectories": "identity/10228-icon-service-Azure-AD-B2C",

  // Microsoft.ManagedIdentity
  "Microsoft.ManagedIdentity/identities": "identity/10227-icon-service-Managed-Identities",

  // Microsoft.LabServices
  "Microsoft.LabServices/labAccounts": "devops/10265-icon-service-Lab-Services",

  // Microsoft.ApiManagement
  "Microsoft.ApiManagement/service": "app-services/10042-icon-service-API-Management-Services",

  // Microsoft.ServiceBus
  "Microsoft.ServiceBus/namespaces": "general/10836-icon-service-Service-Bus",

  // Microsoft.ContainerInstance
  "Microsoft.ContainerInstance/containerGroups": "containers/10104-icon-service-Container-Instances",

  // Microsoft.ContainerRegistry
  "Microsoft.ContainerRegistry/registries": "containers/10105-icon-service-Container-Registries",

  // Microsoft.App
  "Microsoft.App/containerApps": "containers/02884-icon-service-Worker-Container-App",
  "Microsoft.App/managedEnvironments": "containers/02989-icon-service-Container-App-Environments",

  // Microsoft.Cdn
  "Microsoft.Cdn/service": "app-services/00056-icon-service-CDN-Profiles",

  // Module icon
  folder: "general/10802-icon-service-Folder-Blank",
};

// Build a case-insensitive lookup map (once at module load)
const lowercaseKeyMap = new Map(
  Object.entries(svgPathsByResourceType).map(([key, value]) => [key.toLowerCase(), value]),
);

async function importAzureSvg(resourceType: string): Promise<SvgComponent | undefined> {
  const svgPath = lowercaseKeyMap.get(resourceType.toLowerCase()) ?? "custom/resource";
  const svgImport = svgImportsByPath[`../../assets/azure-architecture-icons/${svgPath}.svg`];

  return svgImport();
}

export function useAzureSvg(resourceType: string) {
  const svgRef = useRef<SvgComponent>();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);

    const loadIcon = async () => {
      try {
        svgRef.current = await importAzureSvg(resourceType);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadIcon();
  }, [resourceType]);

  return { loading, AzureSvg: svgRef.current };
}
