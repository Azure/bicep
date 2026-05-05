# Step 1: Expand Azure Icon Coverage in Shared Components

## Goal

Migrate all ~80+ Azure resource type → SVG icon mappings from the old visualizer into the shared `@vscode-bicep-ui/components` package so that the new visual designer can render icons for every resource type the old visualizer supports.

## Background

The old visualizer has a comprehensive icon mapping in `src/vscode-bicep/src/visualizer/app/assets/icons/azure/index.ts` covering ~80+ Azure resource types across 16 categories. The shared `@vscode-bicep-ui/components` package currently only maps **6 resource types** in `packages/components/src/azure-icon/useAzureSvg.ts`. Without expanding this, the new visual designer would show the generic fallback icon for most resources.

## Current State

### Old visualizer icon system (`src/vscode-bicep/src/visualizer/app/assets/icons/azure/`)

- Uses `svg-inline-loader` (webpack) to import SVGs as raw strings
- Async `switch` statement on `resourceType.toLowerCase()` with dynamic `import()`
- SVG strings are embedded into data URIs for Cytoscape node backgrounds
- SVG files organized in subdirectories: `ai/`, `analytics/`, `appServices/`, `compute/`, `containers/`, `custom/`, `databases/`, `devops/`, `general/`, `identity/`, `iot/`, `management/`, `networking/`, `security/`, `storage/`

### New shared icon system (`packages/components/src/azure-icon/`)

- Uses Vite's `import.meta.glob` with `?react` query to import SVGs as React components
- `svgPathsByResourceType` record maps resource type string → relative SVG path (without extension)
- `<AzureIcon resourceType="..." size={36} />` component renders the SVG as a React component
- SVG files in `packages/components/assets/azure-architecture-icons/` with only 4 subdirectories: `app-services/`, `compute/`, `custom/`, `networking/`

## Tasks

### 1.1 Copy SVG files

Copy all SVG files from the old visualizer's icon directory into the shared components' assets directory. The target path is:

```
src/vscode-bicep-ui/packages/components/assets/azure-architecture-icons/
```

Create these new subdirectories (that don't already exist in shared components):
- `ai/`
- `analytics/`
- `containers/`
- `databases/`
- `devops/`
- `general/`
- `identity/`
- `iot/`
- `management/`
- `security/`
- `storage/`

Also copy any files from the old categories that are missing in existing directories (`app-services/`, `compute/`, `networking/`).

**Note on naming**: The old visualizer uses `appServices/` while the shared components use `app-services/`. Stick with the `app-services/` convention (kebab-case) already used in the shared components. This means updating path references in the mapping accordingly.

**Complete file list to copy** (grouped by category, some already exist):

<details>
<summary>ai/ (3 files — all new)</summary>

- `10162-icon-service-Cognitive-Services.svg`
- `10165-icon-service-Bot-Services.svg`
- `10167-icon-service-Machine-Learning-Studio-Workspaces.svg`

</details>

<details>
<summary>analytics/ (8 files — all new)</summary>

- `00009-icon-service-Log-Analytics-Workspaces.svg`
- `00039-icon-service-Event-Hubs.svg`
- `00042-icon-service-Stream-Analytics-Jobs.svg`
- `00606-icon-service-Azure-Synapse-Analytics.svg`
- `10142-icon-service-HD-Insight-Clusters.svg`
- `10148-icon-service-Analysis-Services.svg`
- `10149-icon-service-Event-Hub-Clusters.svg`
- `10787-icon-service-Azure-Databricks.svg`

</details>

<details>
<summary>app-services/ (4 files — some already exist)</summary>

Already exist:
- `00046-icon-service-App-Service-Plans.svg`
- `10035-icon-service-App-Services.svg`

Need to copy:
- `00049-icon-service-App-Service-Certificates.svg` (if not already present)
- `10042-icon-service-API-Management-Services.svg`
- `10047-icon-service-App-Service-Environments.svg` (if not already present)
- `00056-icon-service-CDN-Profiles.svg` (if not already present)

</details>

<details>
<summary>compute/ (9 files — most new)</summary>

Already exist:
- `10021-icon-service-Virtual-Machine.svg`
- `10034-icon-service-VM-Scale-Sets.svg`

Need to copy:
- `00398-icon-service-Disk-Encryption-Sets.svg`
- `10023-icon-service-Kubernetes-Services.svg`
- `10025-icon-service-Availability-Sets.svg`
- `10026-icon-service-Disks-Snapshots.svg`
- `10031-icon-service-Batch-Accounts.svg`
- `10032-icon-service-Disks.svg`
- `10033-icon-service-Images.svg`
- `10039-icon-service-Shared-Image-Galleries.svg`
- `10365-icon-service-Proximity-Placement-Groups.svg`

</details>

<details>
<summary>containers/ (4 files — all new)</summary>

- `02884-icon-service-Worker-Container-App.svg`
- `02989-icon-service-Container-App-Environments.svg`
- `10104-icon-service-Container-Instances.svg`
- `10105-icon-service-Container-Registries.svg`

</details>

<details>
<summary>custom/ (1 file — new)</summary>

Already exists:
- `resource.svg`

Need to copy:
- `subnets.svg`

</details>

<details>
<summary>databases/ (14 files — all new)</summary>

- `10121-icon-service-Azure-Cosmos-DB.svg`
- `10122-icon-service-Azure-Database-MySQL-Server.svg`
- `10123-icon-service-Azure-Database-MariaDB-Server.svg`
- `10124-icon-service-Azure-SQL-VM.svg`
- `10126-icon-service-Data-Factory.svg`
- `10130-icon-service-SQL-Database.svg`
- `10131-icon-service-Azure-Database-PostgreSQL-Server.svg`
- `10132-icon-service-SQL-Server.svg`
- `10133-icon-service-Azure-Database-Migration-Services.svg`
- `10134-icon-service-SQL-Elastic-Pools.svg`
- `10135-icon-service-Managed-Database.svg`
- `10136-icon-service-SQL-Managed-Instance.svg`
- `10137-icon-service-Cache-Redis.svg`
- `10139-icon-service-Instance-Pools.svg`

</details>

<details>
<summary>devops/ (3 files — all new)</summary>

- `00012-icon-service-Application-Insights.svg`
- `10264-icon-service-DevTest-Labs.svg`
- `10265-icon-service-Lab-Services.svg`

</details>

<details>
<summary>general/ (7 files — all new)</summary>

- `10002-icon-service-Subscriptions.svg`
- `10007-icon-service-Resource-Groups.svg`
- `10802-icon-service-Folder-Blank.svg` ← **Important: used as the module icon**
- `10836-icon-service-Service-Bus.svg`
- `10838-icon-service-Storage-Azure-Files.svg`
- `10840-icon-service-Storage-Queue.svg`
- `10841-icon-service-Table.svg`

</details>

<details>
<summary>identity/ (3 files — all new)</summary>

- `10222-icon-service-Azure-AD-Domain-Services.svg`
- `10227-icon-service-Managed-Identities.svg`
- `10228-icon-service-Azure-AD-B2C.svg`

</details>

<details>
<summary>iot/ (5 files — all new)</summary>

- `10045-icon-service-Notification-Hubs.svg`
- `10181-icon-service-Time-Series-Insights-Environments.svg`
- `10182-icon-service-IoT-Hub.svg`
- `10184-icon-service-IoT-Central-Applications.svg`
- `10201-icon-service-Logic-Apps.svg`

</details>

<details>
<summary>management/ (2 files — all new)</summary>

- `00022-icon-service-Automation-Accounts.svg`
- `10316-icon-service-Policy.svg`

</details>

<details>
<summary>networking/ (22 files — most new)</summary>

Already exist:
- `10062-icon-service-Load-Balancers.svg`
- `10080-icon-service-Network-Interfaces.svg`

Need to copy:
- `00701-icon-service-IP-Groups.svg`
- `01105-icon-service-Private-Link-Service.svg`
- `10061-icon-service-Virtual-Networks.svg`
- `10063-icon-service-Virtual-Network-Gateways.svg`
- `10064-icon-service-DNS-Zones.svg`
- `10065-icon-service-Traffic-Manager-Profiles.svg`
- `10066-icon-service-Network-Watcher.svg`
- `10067-icon-service-Network-Security-Groups.svg`
- `10069-icon-service-Public-IP-Addresses.svg`
- `10071-icon-service-Route-Filters.svg`
- `10072-icon-service-DDoS-Protection-Plans.svg`
- `10073-icon-service-Front-Doors.svg`
- `10076-icon-service-Application-Gateways.svg`
- `10077-icon-service-Local-Network-Gateways.svg`
- `10079-icon-service-ExpressRoute-Circuits.svg`
- `10081-icon-service-Connections.svg`
- `10082-icon-service-Route-Tables.svg`
- `10084-icon-service-Firewalls.svg`
- `10085-icon-service-Service-Endpoint-Policies.svg`
- `10310-icon-service-NAT.svg`
- `10353-icon-service-Virtual-WANs.svg`
- `10362-icon-service-Web-Application-Firewall-Policies(WAF).svg`
- `10372-icon-service-Public-IP-Prefixes.svg`

</details>

<details>
<summary>security/ (2 files — all new)</summary>

- `10244-icon-service-Application-Security-Groups.svg`
- `10245-icon-service-Key-Vaults.svg`

</details>

<details>
<summary>storage/ (7 files — all new)</summary>

- `00017-icon-service-Recovery-Services-Vaults.svg`
- `10086-icon-service-Storage-Accounts.svg`
- `10093-icon-service-Storage-Sync-Services.svg`
- `10094-icon-service-Data-Box.svg`
- `10095-icon-service-Data-Box-Edge.svg`
- `10096-icon-service-Azure-NetApp-Files.svg`
- `10098-icon-service-Data-Shares.svg`

</details>

### 1.2 Expand the `svgPathsByResourceType` mapping

Update `src/vscode-bicep-ui/packages/components/src/azure-icon/useAzureSvg.ts` to include all resource types from the old visualizer. The mapping should use the **exact resource type string** as the key (preserving original casing to match what the language server returns).

The complete mapping to add (replacing the current 6-entry record):

```typescript
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
};
```

### 1.3 Handle case-insensitive lookup

The old visualizer does `resourceType.toLowerCase()` before matching. The language server returns the canonical casing from the ARM schema, but there may be inconsistencies. Update `importAzureSvg` in `useAzureSvg.ts` to do a case-insensitive lookup:

```typescript
// Build a case-insensitive lookup map (once at module load)
const lowercaseKeyMap = new Map(
  Object.entries(svgPathsByResourceType).map(([key, value]) => [key.toLowerCase(), value])
);

async function importAzureSvg(resourceType: string): Promise<SvgComponent | undefined> {
  const svgPath = lowercaseKeyMap.get(resourceType.toLowerCase()) ?? "custom/resource";
  const svgImport = svgImportsByPath[`../../assets/azure-architecture-icons/${svgPath}.svg`];
  return svgImport();
}
```

### 1.4 Add the module/folder icon

The old visualizer uses `general/10802-icon-service-Folder-Blank.svg` as the icon for module nodes (type `"<module>"`). The `ModuleDeclaration` component currently passes `resourceType={"folder"}` to `<AzureIcon>`. Either:

- Add a special mapping: `"folder": "general/10802-icon-service-Folder-Blank"` in `svgPathsByResourceType`, or
- Map `"<module>"` to the folder icon and have the data source pass `"<module>"` as the type.

The first approach is simpler since the `ModuleDeclaration` component already uses `"folder"`.

## Verification

1. Run `npm run build` in `src/vscode-bicep-ui` — should build without errors.
2. Run `npm run dev` in `src/vscode-bicep-ui/apps/visual-designer/` and update the hardcoded test data in `App.tsx` to use various resource types (e.g., `Microsoft.KeyVault/vaults`, `Microsoft.Sql/servers`, `Microsoft.Network/virtualNetworks`). Verify the correct icons render.
3. Verify the fallback `custom/resource` icon still renders for unknown resource types.
4. Verify the folder icon renders for module nodes.
5. Run existing tests: `npm test` in `src/vscode-bicep-ui`.

## Notes

- The old visualizer has a typo: `microsoft.dbforpostgressql` (three s's). Keep the same typo in the mapping to maintain backward compatibility with the language server data.
- Some SVG files exist in multiple old directories (e.g., `00056-icon-service-CDN-Profiles.svg` is in both `appServices/` and `networking/`). Only one copy is needed in the shared assets.
