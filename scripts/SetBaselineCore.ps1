# Use this script if you are using PowerShell 7+
dotnet test --filter "TestCategory=Baseline" -- 'TestRunParameters.Parameter(name="SetBaseLine", value="true")'
