FROM mcr.microsoft.com/powershell:latest

COPY entry.ps1 /entry.ps1

ENTRYPOINT ["pwsh", "-File", "/entry.ps1"]