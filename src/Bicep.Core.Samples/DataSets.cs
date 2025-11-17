// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming
namespace Bicep.Core.Samples
{
    public static class DataSets
    {
        public static DataSet AKS_LF => CreateDataSet();

        public static DataSet Dependencies_LF => CreateDataSet();

        public static DataSet DisableNextLineDiagnosticsDirective_CRLF => CreateDataSet();

        public static DataSet Empty => CreateDataSet();

        public static DataSet Extensions_CRLF => CreateDataSet();

        public static DataSet Functions_LF => CreateDataSet();

        public static DataSet Imports_LF => CreateDataSet();

        public static DataSet InvalidCycles_CRLF => CreateDataSet();

        public static DataSet InvalidDisableNextLineDiagnosticsDirective_CRLF => CreateDataSet();

        public static DataSet InvalidExpressions_LF => CreateDataSet();

        public static DataSet InvalidExtensions_CRLF => CreateDataSet();

        public static DataSet InvalidFunctions_LF => CreateDataSet();

        public static DataSet InvalidMetadata_CRLF => CreateDataSet();

        public static DataSet InvalidOutputs_CRLF => CreateDataSet();

        public static DataSet InvalidParameters_LF => CreateDataSet();

        public static DataSet InvalidResources_CRLF => CreateDataSet();

        public static DataSet InvalidResourceDerivedTypes_LF => CreateDataSet();

        public static DataSet InvalidRuntimeValueUsages_LF => CreateDataSet();

        public static DataSet ValidDeployTimeUsages_LF => CreateDataSet();

        public static DataSet InvalidTargetScopes_LF => CreateDataSet();

        public static DataSet InvalidTypeDeclarations_LF => CreateDataSet();

        public static DataSet InvalidVariables_LF => CreateDataSet();

        public static DataSet LargeTemplate_Stress_LF => CreateDataSet();

        public static DataSet Loops_LF => CreateDataSet();

        public static DataSet LoopsIndexed_LF => CreateDataSet();

        public static DataSet Outputs_CRLF => CreateDataSet();

        public static DataSet Metadata_CRLF => CreateDataSet();

        public static DataSet NestedResources_LF => CreateDataSet();

        public static DataSet Parameters_CRLF => CreateDataSet();

        public static DataSet Parameters_LF => CreateDataSet();

        public static DataSet Registry_LF => CreateDataSet();

        public static DataSet Resources_CRLF => CreateDataSet();

        public static DataSet ResourcesSubscription_CRLF => CreateDataSet();

        public static DataSet ResourcesManagementGroup_CRLF => CreateDataSet();

        public static DataSet ResourceDerivedTypes_LF => CreateDataSet();

        public static DataSet ResourcesTenant_CRLF => CreateDataSet();

        public static DataSet TypeDeclarations_LF => CreateDataSet();

        public static DataSet TypedVariables_LF => CreateDataSet();

        public static DataSet Unicode_LF => CreateDataSet();

        public static DataSet Variables_LF => CreateDataSet();

        public static DataSet VariablesTenant_LF => CreateDataSet();

        public static DataSet VariablesSubscription_LF => CreateDataSet();

        public static DataSet VariablesManagementGroup_LF => CreateDataSet();

        public static DataSet Modules_CRLF => CreateDataSet();

        public static DataSet ModulesSubscription_LF => CreateDataSet();

        public static DataSet ModulesWithScopes_LF => CreateDataSet();

        public static DataSet TestFramework_CRLF => CreateDataSet();

        public static DataSet InvalidTestFramework_CRLF => CreateDataSet();

        public static DataSet InvalidModules_LF => CreateDataSet();

        public static DataSet InvalidModulesTenant_LF => CreateDataSet();

        public static DataSet InvalidModulesManagementGroup_LF => CreateDataSet();

        public static DataSet InvalidModulesSubscription_LF => CreateDataSet();

        public static DataSet InvalidMultilineString_CRLF => CreateDataSet();

        public static DataSet IntermediaryVariables_LF => CreateDataSet();

        public static DataSet InvalidLoadFunctions_CRLF => CreateDataSet();

        public static DataSet InvalidLambdas_LF => CreateDataSet();

        public static DataSet LoadFunctions_CRLF => CreateDataSet();

        public static DataSet NewlineSensitivity_LF => CreateDataSet();

        public static DataSet InvalidNewlineSensitivity_LF => CreateDataSet();

        public static DataSet Lambdas_LF => CreateDataSet();

        public static DataSet PrettyPrint_LF => CreateDataSet();

        public static DataSet MultilineStrings_LF => CreateDataSet();

        public static IEnumerable<DataSet> AllDataSets =>
            typeof(DataSets)
                .GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static)
                .Where(property => property.PropertyType == typeof(DataSet))
                .Select(property => property.GetValue(null))
                .Cast<DataSet>();

        public static IEnumerable<DataSet> NonStressDataSets => AllDataSets.Where(ds => !ds.IsStress);

        public static ImmutableDictionary<string, string> Completions => DataSet.ReadDataSetDictionary($"{DataSet.Prefix}{DataSet.TestCompletionsPrefix}");

        public static ImmutableDictionary<string, string> Functions => DataSet.ReadDataSetDictionary($"{DataSet.Prefix}{DataSet.TestFunctionsPrefix}");

        private static DataSet CreateDataSet([CallerMemberName] string? dataSetName = null) => new(dataSetName!);
    }
}
