param location string
param resourceToken string
param tags object

module databaseAccount 'br/public:avm/res/document-db/database-account:0.18.0' = {
  params: {
    name: 'cos-${resourceToken}'
    tags: tags
    disableLocalAuthentication: false
    location: location
    networkRestrictions: {
      publicNetworkAccess: 'Enabled'
    }
    sqlDatabases: [
      {
        name: 'contosopizza'
        autoscaleSettingsMaxThroughput: 4000
        containers: [
          {
            name: 'menu'
            paths: ['/type']
            kind: 'Hash'
          }
        ]
      }
    ]
    zoneRedundant: false
    enableFreeTier: false
  }
}

module storageAccount 'br/public:avm/res/storage/storage-account:0.30.0' = {
  params: {
    name: 'str${replace(resourceToken,'-','')}'
    location: location
    tags: tags
    publicNetworkAccess: 'Enabled'
    allowSharedKeyAccess: true
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
    }
    tableServices: {
      tables: [
        {
          name: 'basket'
        }
      ]
    }
  }
}

resource storageMCP 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'strm${replace(resourceToken,'-','')}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  tags: tags
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: []
    }
    allowSharedKeyAccess: true
  }
}

resource blobservices 'Microsoft.Storage/storageAccounts/blobServices@2025-06-01' = {
  parent: storageMCP
  name: 'default'
  properties: {}
}

resource containerDeploymentOrder 'Microsoft.Storage/storageAccounts/blobServices/containers@2025-06-01' = {
  parent: blobservices
  name: 'app-package-order'
  properties: {}
}

resource containerDeploymentMenu 'Microsoft.Storage/storageAccounts/blobServices/containers@2025-06-01' = {
  parent: blobservices
  name: 'app-package-menu'
  properties: {}
}

var funcProperties = [
  {
    name: 'func-menu-${resourceToken}'
    appSettings: [
      {
        name: 'DEPLOYMENT_STORAGE_CONNECTION_STRING'
        value: 'DefaultEndpointsProtocol=https;AccountName=${storageMCP.name};AccountKey=${storageMCP.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
      }
      {
        name: 'AzureWebJobsStorage'
        value: 'DefaultEndpointsProtocol=https;AccountName=${storageMCP.name};AccountKey=${storageMCP.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
      }
      {
        name: 'CosmosDBConnectionString'
        value: databaseAccount.outputs.primaryReadWriteConnectionString
      }
    ]
    deploymentContainerName: 'https://${storageMCP.name}.blob.${environment().suffixes.storage}/app-package-menu'
  }
  {
    name: 'func-order-${resourceToken}'
    appSettings: [
      {
        name: 'DEPLOYMENT_STORAGE_CONNECTION_STRING'
        value: 'DefaultEndpointsProtocol=https;AccountName=${storageMCP.name};AccountKey=${storageMCP.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
      }
      {
        name: 'AzureWebJobsStorage'
        value: 'DefaultEndpointsProtocol=https;AccountName=${storageMCP.name};AccountKey=${storageMCP.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
      }
      {
        name: 'StorageAccountUri'
        value: 'https://${storageAccount.outputs.name}.table.${environment().suffixes.storage}'
      }
      {
        name: 'StorageAccountName'
        value: storageAccount.outputs.name
      }
      {
        name: 'StorageAccountKey'
        value: storageAccount.outputs.primaryAccessKey
      }
    ]
    deploymentContainerName: 'https://${storageMCP.name}.blob.${environment().suffixes.storage}/app-package-order'
  }
]

module function 'function.bicep' = {
  params: {
    location: location
    funcProperties: funcProperties
    resourceToken: resourceToken
  }
}
