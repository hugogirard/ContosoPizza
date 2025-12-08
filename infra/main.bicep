targetScope = 'subscription'

param location string
param resourceGroupName string

resource rg 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: resourceGroupName
  location: location
}

var resourceToken = uniqueString(rg.id)

module databaseAccount 'br/public:avm/res/document-db/database-account:0.18.0' = {
  scope: rg
  params: {
    name: 'dddamin001'

    zoneRedundant: false
  }
}
