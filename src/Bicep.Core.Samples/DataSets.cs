using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming
namespace Bicep.Core.Samples
{
    public static class DataSets
    {
        public static DataSet Empty => CreateDataSet();

        public static DataSet InvalidParameters_CRLF => CreateDataSet();

        public static DataSet InvalidParameters_LF => CreateDataSet();

        public static DataSet Outputs_CRLF => CreateDataSet();

        public static DataSet Parameters_CRLF => CreateDataSet();

        public static DataSet Parameters_LF => CreateDataSet();

        public static DataSet Resources_CRLF => CreateDataSet();

        public static DataSet Variables_LF => CreateDataSet();

        public static IEnumerable<DataSet> AllDataSets =>
            typeof(DataSets)
                .GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static)
                .Where(property => property.PropertyType == typeof(DataSet))
                .Select(property => property.GetValue(null))
                .Cast<DataSet>();


        private static DataSet CreateDataSet([CallerMemberName] string? dataSetName = null) => new DataSet(dataSetName!);
    }
}
