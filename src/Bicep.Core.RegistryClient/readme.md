# Bicep.Core.RegistryClient
The majority of the source code is generated via autorest except for a small set of specialized/customized code.

This project will be removed entirely once there is a .net ACR SDK that supports push/pull operations on nuget.org.

To regenerate using autorest, run the following command:
`autorest --input-file=containerregistry.json --csharp --output-folder=./generated --add-credentials=true --clear-output-folder=true --use:@autorest/csharp@3.0.0-beta.20210604.3 --namespace=Bicep.Core.RegistryClient --eol=lf --skip-csproj`