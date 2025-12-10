param location string
param resourceToken string

var accountName = 'foundry-${resourceToken}'

resource account 'Microsoft.CognitiveServices/accounts@2025-04-01-preview' = {
  name: accountName
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'AIServices'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    allowProjectManagement: true
    customSubDomainName: accountName
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: []
      ipRules: []
      bypass: 'AzureServices'
    }
    publicNetworkAccess: 'Enabled'
    networkInjections: null
    disableLocalAuth: false
  }
}

resource project 'Microsoft.CognitiveServices/accounts/projects@2025-04-01-preview' = {
  parent: account
  name: 'contoso-pizza'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    description: 'Contoso Pizza restaurant ordering agents'
    displayName: 'Contoso Pizza'
  }
}

output foundryResourceName string = account.name
