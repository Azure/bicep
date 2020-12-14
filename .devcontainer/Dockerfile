# Start from the dotnet base image to allow easy install of Powershell and ensure common tools are available for things like the bicep language server.
FROM mcr.microsoft.com/dotnet/sdk:5.0

# Configure the release version of Bicep you'd like to use
ARG BICEP_VERSION=latest
# Pinned to ensure compatability with dotnet version installed in container
ARG POWSERSHELL_VERSION=7.1.0

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

# Install common tooling
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends curl build-essential

# Install Azure Powershell
ENV PATH="$PATH:/root/.dotnet/tools"
RUN \
    dotnet tool install -g powershell --version $POWSERSHELL_VERSION \
    && pwsh -c "Install-Module -Name Az -AllowClobber -Scope AllUsers -Force"

# Install Bicep
RUN \
    # Fetch the latest Bicep CLI binary
    curl -Lo bicep https://github.com/Azure/bicep/releases/${BICEP_VERSION}/download/bicep-linux-x64 \
    # Mark it as executable
    && chmod +x ./bicep \
    # Add bicep to your PATH (requires admin)
    && mv ./bicep /usr/local/bin/bicep \
    # Verify you can now access the 'bicep' command
    && bicep --help \
    # Download the bicep vscode extension for installation
    && curl -Lo /tmp/vscode-bicep.vsix https://github.com/Azure/bicep/releases/${BICEP_VERSION}/download/vscode-bicep.vsix