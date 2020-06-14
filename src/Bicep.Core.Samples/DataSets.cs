using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Samples
{
    public static class DataSets
    {
        public static DataSet InvalidParameters => CreateDataSet();

        public static DataSet Parameters => CreateDataSet();

        public static IEnumerable<DataSet> AllDataSets =>
            typeof(DataSets)
                .GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static)
                .Where(property => property.PropertyType == typeof(DataSet))
                .Select(property => property.GetValue(null))
                .Cast<DataSet>();


        private static DataSet CreateDataSet([CallerMemberName] string? dataSetName = null) => new DataSet(dataSetName!);
    }
}
