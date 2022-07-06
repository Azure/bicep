@echo off

echo Running IntegrationTests

SET IntegrationTestsDllPath=..\Bicep.VSLanguageServerClient.IntegrationTests\bin\Release\net472\Bicep.VSLanguageServerClient.IntegrationTests.dll

echo IntegrationTestsDllPath %IntegrationTestsDllPath%

call vstest.console.exe %IntegrationTestsDllPath% /Platform:x64
