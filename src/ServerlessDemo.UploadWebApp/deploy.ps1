$resourceGroup = "serverless-webapp"
$webAppName = "serverless-webapp-web"
$registry = "serverlessdemo"
$image = "uploadwebapp:dev"

$storageResourceGroup = "serverless"
$storageKeyVaultName = "serverlessws-kv"

az group create --name $resourceGroup --location westeurope
az appservice plan create --name "$resourceGroup-plan" -g $resourceGroup --is-linux --sku B1

# From docker - https://docs.microsoft.com/en-us/azure/app-service/scripts/cli-linux-acr-aspnetcore

# Create a web app.
az webapp create --name $webAppName --plan "$resourceGroup-plan" -g $resourceGroup --deployment-container-image-name "$registry.azurecr.io/$image" --assign-identity '[system]'

# Give access to central Key Vault
$principalId = az functionapp identity show -n $webAppName -g $resourceGroup --query principalId -o tsv
az keyvault set-policy -n $storageKeyVaultName -g $storageResourceGroup  --object-id $principalId --secret-permissions list get

# Configure for access to storage account
$storageAccountKey = """@Microsoft.KeyVault(SecretUri=https://serverlessws-kv.vault.azure.net/secrets/storageaccountkey/92113b4fa5c74fb7807c3db44327fd62)"""
az webapp config appsettings set -g $resourceGroup -n $webAppName --settings AzureStorageConfig__AccountName=serverlessst AzureStorageConfig__ImageContainer=images AzureStorageConfig__ThumbnailContainer=thumbnails AzureStorageConfig__AccountKey=$storageAccountKey


# Setting credentials. Not needed for create, az CLI will retrieve them automatically.
#$credentials = (az acr credential show --name $registry -g serverless | ConvertFrom-Json)
#az webapp config container set -g $resourceGroup --name $webAppName --docker-registry-server-url "https://$registry.azurecr.io" --docker-registry-server-user $registry --docker-registry-server-password $credentials.passwords[0].value

# From GitHub
#az webapp create --name serverless-webapp-web -g serverless-webapp --plan serverless-webapp-plan
#az webapp deployment source config --name serverless-webapp-web -g myResourceGroup --branch master --manual-integration --repo-url https://github.com/Azure-Samples/storage-blob-upload-from-webapp