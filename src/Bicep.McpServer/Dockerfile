# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Copy build infrastructure
COPY global.json version.json .editorconfig ./
COPY src/Directory.Build.props src/Directory.Build.targets src/Directory.Packages.props src/BannedSymbols.txt src/
COPY src/build/ src/build/
COPY src/icons/ src/icons/

# Copy project files for restore
COPY src/Bicep.McpServer/Bicep.McpServer.csproj src/Bicep.McpServer/
COPY src/Bicep.Core/Bicep.Core.csproj src/Bicep.Core/
COPY src/Bicep.Decompiler/Bicep.Decompiler.csproj src/Bicep.Decompiler/
COPY src/Bicep.IO/Bicep.IO.csproj src/Bicep.IO/

RUN dotnet restore src/Bicep.McpServer/Bicep.McpServer.csproj

# Copy source code
COPY src/Bicep.McpServer/ src/Bicep.McpServer/
COPY src/Bicep.Core/ src/Bicep.Core/
COPY src/Bicep.Decompiler/ src/Bicep.Decompiler/
COPY src/Bicep.IO/ src/Bicep.IO/

RUN dotnet publish src/Bicep.McpServer/Bicep.McpServer.csproj \
    -c Release \
    -o /app \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "Azure.Bicep.McpServer.dll", "--transport", "http"]
