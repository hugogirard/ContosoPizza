targetScope = 'subscription'

param location string
param resourceGroupName string

var tags = {
  SecurityControl: 'Ignore'
}

resource rg 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: resourceGroupName
  location: location
}

var resourceToken = uniqueString(rg.id)

module workload 'modules/workload.bicep' = {
  scope: rg
  params: {
    location: location
    resourceToken: resourceToken
    tags: tags
  }
}
