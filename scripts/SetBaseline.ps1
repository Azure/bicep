# Use this script if you are using Windows PowerShell
dotnet test --filter "TestCategory=Baseline" -- 'TestRunParameters.Parameter(name=\"SetBaseLine\", value=\"true\")'
