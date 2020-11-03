param location string {
    default: resourceGroup().location
    allowed: [
        'australiacentral' 
        'australiaeast' 
        'australiasoutheast' 
        'brazilsouth'
        'canadacentral' 
        'centralindia' 
        'centralus' 
        'eastasia' 
        'eastus' 
        'eastus2' 
        'francecentral' 
        'japaneast' 
        'koreacentral' 
        'northcentralus' 
        'northeurope' 
        'southafricanorth' 
        'southcentralus' 
        'southeastasia'
        'switzerlandnorth'
        'switzerlandwest'
        'uksouth' 
        'ukwest' 
        'westcentralus' 
        'westeurope' 
        'westus' 
        'westus2' 
    ]
}

param logAnalyticsWorkspaceName string = 'la-${uniqueString(resourceGroup().id)}'
param appServicePlanName string = 'asp-${uniqueString(resourceGroup().id)}'
param webAppStorageAccountName string = 'sawa${uniqueString(resourceGroup().id)}'
param webAppStorageAccountImagesContainerName string = 'images'
param webAppStorageAccountThumbnailsContainerName string = 'thumbnails'
param functionAppStorageAccountName string = 'safa${uniqueString(resourceGroup().id)}'
param applicationInsightsName string = 'appin-${uniqueString(resourceGroup().id)}'
param webAppName string = 'wa${uniqueString(resourceGroup().id)}'
param webAppRepoURL string = 'https://github.com/Azure-Samples/storage-blob-upload-from-webapp'
param functionAppName string = 'fa${uniqueString(resourceGroup().id)}'
param functionAppRepoURL string = 'https://github.com/Azure-Samples/function-image-upload-resize'

var deploymentSlotName = 'preview'
var functionName = 'Thumbnail'
var functionResourceId = '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${functionAppName}/functions/${functionName}'
var environmentName = 'Production'
var workloadName = 'Image Resizer'
var costCenterName = 'IT'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
    name: logAnalyticsWorkspaceName
    location: location
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    properties: {
        retentionInDays: 30
        features: {
            searchVersion: 1
        }
        sku: {
            name: 'PerGB2018'
        }
    }
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id

resource logAnalyticsWorkspaceDiagnostics 'Microsoft.OperationalInsights/workspaces/providers/diagnosticSettings@2017-05-01-preview' = {
    name: '${logAnalyticsWorkspace.name}/Microsoft.Insights/diagnosticSettings'
    properties: {
        workspaceId: logAnalyticsWorkspace.id
        logs: [
            {
                category: 'Audit'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
        ]
        metrics: [
            {
                category: 'AllMetrics'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
        ]
    }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2016-09-01' = {
    name: appServicePlanName
    location: location
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    sku: {
        name: 'S1'
        tier: 'Standard'
        size: 'S1'
        family: 'S'
        capacity: 1
    }
    kind: 'app'
    properties: {
        name: appServicePlanName
        perSiteScaling: false
        reserved: false
        targetWorkerCount: 0
        targetWorkerSizeId: 0
    }
}

output appServicePlanId string = appServicePlan.id

resource webAppStorageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: webAppStorageAccountName
    location: location
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    kind: 'BlobStorage'
    sku: {
        name: 'Standard_LRS'
        tier: 'Standard'
    }
    properties: {
        accessTier: 'Hot'
        supportsHttpsTrafficOnly: true
    }
}

output webAppStorageAccountId string = webAppStorageAccount.id

resource webAppStorageAccountImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
    name: '${webAppStorageAccount.name}/default/${webAppStorageAccountImagesContainerName}'
}

resource webAppStorageAccountThumbnailsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
    name: '${webAppStorageAccount.name}/default/${webAppStorageAccountThumbnailsContainerName}'
    properties: {
        publicAccess: 'Container'
    }
}

resource functionAppStorageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: functionAppStorageAccountName
    location: location
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    kind: 'StorageV2'
    sku: {
        name: 'Standard_LRS'
        tier: 'Standard'
    }
    properties: {
        accessTier: 'Hot'
        supportsHttpsTrafficOnly: true
    }
}

output functionAppStorageAccountId string = functionAppStorageAccount.id

resource applicationInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
    name: applicationInsightsName
    location: location
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    kind: 'web'
    properties: {
        ApplicationId: applicationInsightsName
        Application_Type: 'web'
        Flow_Type: 'Redfield'
        Request_Source: 'IbizaAIExtension'
        WorkspaceResourceId: logAnalyticsWorkspace.id
    }
}

output applicationInsightsId string = applicationInsights.id

resource webApp 'Microsoft.Web/sites@2018-11-01' = {
    name: webAppName
    location: location
    dependsOn: [
        logAnalyticsWorkspace
        appServicePlan
        webAppStorageAccount
        applicationInsights
    ]
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    properties: {
        serverFarmId: appServicePlan.id
        httpsOnly: false
        siteConfig: {
            appSettings: [
                {
                    name: 'AzureStorageConfig__AccountName'
                    value: webAppStorageAccountName
                }
                {
                    name: 'AzureStorageConfig__AccountKey'
                    value: listKeys(webAppStorageAccount.id, webAppStorageAccount.apiVersion).keys[1].value
                }
                {
                    name: 'AzureStorageConfig__ImageContainer'
                    value: 'images'
                }
                {
                    name: 'AzureStorageConfig__ThumbnailContainer'
                    value: 'thumbnails'
                }
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: '${reference(applicationInsights.id, '2018-05-01-preview').InstrumentationKey}'
                }
                {
                    name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
                    value: '~2'
                }
                {
                    name: 'XDT_MicrosoftApplicationInsights_Mode'
                    value: 'recommended'
                }
                {
                    name: 'InstrumentationEngine_EXTENSION_VERSION'
                    value: '~1'
                }
                {
                    name: 'XDT_MicrosoftApplicationInsights_BaseExtensions'
                    value: '~1'
                }
            ]
        }
    }
}

output webAppId string = webApp.id
output webAppPublicUrl string = webApp.properties.defaultHostName

resource webAppDeployment 'Microsoft.Web/sites/sourcecontrols@2018-11-01' = {
    name: '${webAppName}/web'
    dependsOn: [
        webApp
    ]
    properties: {
        repoUrl: webAppRepoURL
        branch: 'master'
        isManualIntegration: true
    }
}

resource webAppDiagnostics 'Microsoft.Web/sites/providers/diagnosticSettings@2017-05-01-preview' = {
    name: '${webApp.name}/Microsoft.Insights/diagnosticSettings'
    properties: {
        workspaceId: logAnalyticsWorkspace.id
        logs: [
            {
                category: 'AppServiceHTTPLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServiceConsoleLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServiceAppLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServiceFileAuditLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServiceAuditLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServiceAntivirusScanAuditLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServiceIPSecAuditLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
            {
                category: 'AppServicePlatformLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
        ]
        metrics: [
            {
                category: 'AllMetrics'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
        ]
    }
}

resource deploymentSlot 'Microsoft.Web/sites/slots@2018-11-01' = {
    name: '${webApp.name}/${deploymentSlotName}'
    location: location
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    properties: {
        serverFarmId: appServicePlan.id
    }
}

resource deploymentSlotDeployment 'Microsoft.Web/sites/slots/sourcecontrols@2018-11-01' = {
    name: '${deploymentSlot.name}/web'
    dependsOn: [
        deploymentSlot
    ]
    properties: {
        repoUrl: webAppRepoURL
        branch: 'master'
        isManualIntegration: true
    }
}

resource functionApp 'Microsoft.Web/sites@2018-11-01' = {
    name: functionAppName
    location: location
    dependsOn: [
        logAnalyticsWorkspace
        appServicePlan
        functionAppStorageAccount
        applicationInsights
    ]
    tags: {
        Environment: environmentName
        Workload: workloadName
        CostCenter: costCenterName
    }
    kind: 'functionApp'
    properties: {
        serverFarmId: appServicePlan.id
        siteConfig: {
            appSettings: [
                {
                    name: 'AzureWebJobsDashboard'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${functionAppStorageAccount.name};EndpointSuffix=core.windows.net;AccountKey=${listKeys(functionAppStorageAccount.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'AzureWebJobsStorage'
                    value: 'DefaultEndpointsProtocol=https;AccountName=${webAppStorageAccount.name};EndpointSuffix=core.windows.net;AccountKey=${listKeys(webAppStorageAccount.id, '2019-06-01').keys[0].value}'
                }
                {
                    name: 'FUNCTIONS_EXTENSION_VERSION'
                    value: '~2'
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: 'dotnet'
                }
                {
                    name: 'WEBSITE_NODE_DEFAULT_VERSION'
                    value: '10.14.1'
                }
                {
                    name: 'THUMBNAIL_CONTAINER_NAME'
                    value: 'thumbnails'
                }
                {
                    name: 'THUMBNAIL_WIDTH'
                    value: '100'
                }
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: '${reference(applicationInsights.id, '2018-05-01-preview').InstrumentationKey}'
                }
                {
                    name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
                    value: '~2'
                }
                {
                    name: 'XDT_MicrosoftApplicationInsights_Mode'
                    value: 'recommended'
                }
                {
                    name: 'InstrumentationEngine_EXTENSION_VERSION'
                    value: '~1'
                }
                {
                    name: 'XDT_MicrosoftApplicationInsights_BaseExtensions'
                    value: '~1'
                }
            ]
        }
    }
}

output functionAppId string = functionApp.id

resource functionAppDeployment 'Microsoft.Web/sites/sourcecontrols@2018-11-01' = {
    name: '${functionAppName}/web'
    dependsOn: [
        functionApp
    ]
    properties: {
        repoUrl: functionAppRepoURL
        branch: 'master'
        isManualIntegration: true
    }
}

resource functionAppDiagnostics 'Microsoft.Web/sites/providers/diagnosticSettings@2017-05-01-preview' = {
    name: '${functionApp.name}/Microsoft.Insights/diagnosticSettings'
    properties: {
        workspaceId: logAnalyticsWorkspace.id
        logs: [
            {
                category: 'FunctionAppLogs'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
        ]
        metrics: [
            {
                category: 'AllMetrics'
                enabled: true
                retentionPolicy: {
                    days: 7
                    enabled: true
                }
            }
        ]
    }
}

resource eventGridSubscription 'Microsoft.Storage/storageAccounts/providers/eventSubscriptions@2020-04-01-preview' = {
    name: '${webAppStorageAccount.name}/Microsoft.EventGrid/imageResizer'
    location: location
    dependsOn: [
        functionAppStorageAccount
        functionAppDeployment
    ]
    properties: {
        topic: webAppStorageAccount.id
        destination: {
            endpointType: 'AzureFunction'
            properties: {
                resourceId: functionResourceId
                maxEventsPerBatch: 1
                preferredBatchSizeInKilobytes: 64
            }
        }
        filter: {
            includedEventTypes: [
                'Microsoft.Storage.BlobCreated'
            ]
            subjectBeginsWith: '/blobServices/default/containers/images/blobs/'
            advancedFilters: []
        }
        labels: [
           'functions-thumbnail'
        ]
        eventDeliverySchema: 'EventGridSchema'
    }
}
