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
