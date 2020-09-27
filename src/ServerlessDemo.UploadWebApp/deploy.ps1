$resourceGroup = "serverless-webapp"
$webAppName = "serverless-webapp-web"
$registry = "serverlessdemo"
$image = "uploadwebapp:dev"

az group create --name $resourceGroup --location westeurope
az appservice plan create --name "$resourceGroup-plan" -g $resourceGroup --is-linux --sku Free


# From docker - https://docs.microsoft.com/en-us/azure/app-service/scripts/cli-linux-acr-aspnetcore

# Create a web app.
az webapp create --name $webAppName --plan "$resourceGroup-plan" -g $resourceGroup --deployment-container-image-name "$registry.azurecr.io/$image"

$credentials = (az acr credential show --name $registry -g serverless | ConvertFrom-Json)
az webapp config container set -g $resourceGroup --name $webAppName --docker-registry-server-url "https://$registry.azurecr.io" --docker-registry-server-user $registry --docker-registry-server-password $credentials.passwords[0].value

# TODO: configure storage account settings, from Key Vault? With Managed Identity?

# Configure web app with a custom Docker Container from Azure Container Registry.
#az webapp config container set --resource-group myResourceGroup --name <app_name> --docker-registry-server-url http://<acr_registry_name>.azurecr.io --docker-registry-server-user <registry_user> --docker-registry-server-password <registry_password>
#az webapp create --name serverless-webapp-web --plan serverless-webapp-plan --resource-group myResourceGroup --deployment-container-image-name <acr_registry_name>.azurecr.io/<container_name:version>

# From GitHub
#az webapp create --name serverless-webapp-web -g serverless-webapp --plan serverless-webapp-plan
#az webapp deployment source config --name serverless-webapp-web -g myResourceGroup --branch master --manual-integration --repo-url https://github.com/Azure-Samples/storage-blob-upload-from-webapp


