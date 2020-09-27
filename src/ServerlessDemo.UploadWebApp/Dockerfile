FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /app

RUN git clone https://github.com/Azure-Samples/storage-blob-upload-from-webapp.git
WORKDIR /app/storage-blob-upload-from-webapp/ImageResizeWebApp

# Restore solution
RUN dotnet restore

# Build & publish project
WORKDIR /app/storage-blob-upload-from-webapp/ImageResizeWebApp/ImageResizeWebApp
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime
WORKDIR /app
COPY --from=build /app/storage-blob-upload-from-webapp/ImageResizeWebApp/ImageResizeWebApp/out ./
ENTRYPOINT ["dotnet", "ImageResizeWebApp.dll"]