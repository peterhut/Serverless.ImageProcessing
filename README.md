# Serverless Demo - ImageProcessing

Azure Function that triggers on an image place in an Azure Storage account container. For the image the function will:
- Generate a thumbnail into the container 'thumbnails'
- Call the Computer Vision endpoint of Azure Cognitive Services to analyze the image and place the output in Cosmos DB

# More information
- Works together with a [web application that uploads images to an Azure Storage container](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-upload-process-images?tabs=dotnet)
- [Azure Blob trigger for Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-output?tabs=csharp)
- [Analyze an image using the Computer Vision REST API and C#](https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp-analyze)
- [Where to find the Cognitive Services settings](https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/tutorials/storage-lab-tutorial)

# Required settings
| Setting | Remarks
|---|---
| StorageAccountConnection | Storage account connection string containing the 'images' and 'thumbnails' containers
| VisionEndpoint  | URL for the Cognitive Services endpoint. Example: https://serverless-demo.cognitiveservices.azure.com/
| VisionKey | Key for the Computer Vision service
| CosmosDBConnection | Cosmos DB connection string where Computer Vision analysis is stored.
  