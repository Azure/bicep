// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Samples
{
    public static class DataSetsExtensions
    {
        public static IEnumerable<object[]> ToDynamicTestData(this IEnumerable<DataSet> source) => source.Select(ToDynamicTestData);

        public static object[] ToDynamicTestData(this DataSet ds) => new object[] {ds};

        public static bool HasCrLfNewlines(this DataSet dataSet)
            => dataSet.Name.EndsWith("_CRLF");
    }
}

