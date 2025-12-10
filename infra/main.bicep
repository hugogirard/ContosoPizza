targetScope = 'subscription'

param location string
param resourceGroupName string

@description('The chat completion model deployment name')
param chatCompleteionDeploymentName string

@description('The chat completion model deployment SKU')
@allowed(['GlobalStandard'])
param chatDeploymentSku string

@description('The chat completion model properties')
param chatModelProperties object

@description('The chat completion model SKU capacity')
param chatModelSkuCapacity int

@description('The embedding model deployment name')
param embeddingDeploymentName string

@description('The embedding model deployment SKU')
@allowed(['GlobalStandard'])
param embeddingDeploymentSku string

@description('The embedding model properties')
param embeddingModelProperties object

@description('The embedding model SKU capacity')
param embeddingModelSkuCapacity int

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

module foundry 'modules/foundry.bicep' = {
  scope: rg
  params: {
    location: location
    resourceToken: resourceToken
  }
}

module chatCompletionModel 'modules/model.deployment.bicep' = {
  scope: rg
  params: {
    aiFoundryAccountName: foundry.outputs.foundryResourceName
    deploymentName: chatCompleteionDeploymentName
    deploymentSku: chatDeploymentSku
    modelProperties: chatModelProperties
    skuCapacity: chatModelSkuCapacity
    versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
  }
}

module embeddingnModel 'modules/model.deployment.bicep' = {
  scope: rg
  params: {
    aiFoundryAccountName: foundry.outputs.foundryResourceName
    deploymentName: embeddingDeploymentName
    deploymentSku: embeddingDeploymentSku
    modelProperties: embeddingModelProperties
    skuCapacity: embeddingModelSkuCapacity
    versionUpgradeOption: 'NoAutoUpgrade'
  }
  dependsOn: [
    chatCompletionModel
  ]
}
