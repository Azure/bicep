FROM mcr.microsoft.com/vscode/devcontainers/base:buster

# This Dockerfile adds a non-root user with sudo access. Use the "remoteUser"
# property in devcontainer.json to use it. On Linux, the container user's GID/UIDs
# will be updated to match your local UID/GID (when using the dockerFile property).
# See https://aka.ms/vscode-remote/containers/non-root-user for details.
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

ARG PS_VERSION=7.0.3
ARG PS_PACKAGE=powershell_${PS_VERSION}-1.debian.10_amd64.deb
ARG PS_PACKAGE_URL=https://github.com/PowerShell/PowerShell/releases/download/v${PS_VERSION}/${PS_PACKAGE}

# Download the Linux package and save it
ADD ${PS_PACKAGE_URL} /tmp/powershell.deb

# Define ENVs for Localization/Globalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    # set a fixed location for the Module analysis cache
    PSModuleAnalysisCachePath=/var/cache/microsoft/powershell/PSModuleAnalysisCache/ModuleAnalysisCache

# Configure apt and install packages
RUN apt-get update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends apt-utils dialog 2>&1 \
    #
    # Verify git and needed tools are installed
    && apt-get -y install \
    git \
    openssh-client \
    iproute2 \
    procps \
    curl \
    apt-transport-https \
    gnupg2 \
    lsb-release \
    # less is required for help in PowerShell
    less \
    # requied to setup the locale
    locales \
    # required for SSL
    ca-certificates \
    gss-ntlmssp \
    && apt-get dist-upgrade -y \
    # enable en_US.UTF-8 locale
    && sed -i 's/# en_US.UTF-8 UTF-8/en_US.UTF-8 UTF-8/g' /etc/locale.gen \
    # generate locale
    && locale-gen && update-locale \
    && apt install -y /tmp/powershell.deb \
    # remove PowerShell package
    && rm /tmp/powershell.deb \
    #
    # Install Azure CLI
    && echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $(lsb_release -cs) main" > /etc/apt/sources.list.d/azure-cli.list \
    && curl -sL https://packages.microsoft.com/keys/microsoft.asc | (OUT=$(apt-key add - 2>&1) || echo $OUT) \
    && apt-get update \
    && apt-get install -y azure-cli \
    #
    # Fetch the latest Bicep CLI binary
    && curl -Lo bicep https://github.com/Azure/bicep/releases/latest/download/bicep-linux-x64 \
    # Mark it as executable
    && chmod +x ./bicep \
    # Add bicep to your PATH (requires admin)
    && sudo mv ./bicep /usr/local/bin/bicep \
    #
    # Fetch the latest Bicep VSCode extension
    && curl -Lo vscode-bicep.vsix https://github.com/Azure/bicep/releases/latest/download/vscode-bicep.vsix \
    #
    # [Optional] Add sudo support for the non-root user
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME\
    && chmod 0440 /etc/sudoers.d/$USERNAME \
    #
    # install Az modules - Account & Resource as that is all that's required for Deploying resources.
    && pwsh \
    -NoLogo \
    -NoProfile \
    -Command " \
    Set-PSRepository PSGallery -InstallationPolicy Trusted; \
    Install-Module -Name Az.Accounts, Az.Resources -AllowClobber -Scope AllUsers -Confirm:\$False -Force" \
    # initialize PowerShell module cache
    # invoke a nonexistent command to force PowerShell to perform complete module analysis
    && pwsh -NoLogo -NoProfile -Command "try { IAmSureThisCommandDoesNotExist } catch { exit 0 }" \
    && pwsh \
    -NoLogo \
    -NoProfile \
    -Command " \
    \$ErrorActionPreference = 'Stop' ; \
    \$ProgressPreference = 'SilentlyContinue' ; \
    while(!(Test-Path -Path \$env:PSModuleAnalysisCachePath)) {  \
    Write-Host "'Waiting for $env:PSModuleAnalysisCachePath'" ; \
    Start-Sleep -Seconds 6 ; \
    }" \
    #
    # Clean up
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*