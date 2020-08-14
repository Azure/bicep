using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Serialization;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class SemanticModelTests
    {
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ProgramsShouldProduceExpectedDiagnostics(DataSet dataSet)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(dataSet.Bicep));
            var model = compilation.GetSemanticModel();
            
            var errors = model.GetAllDiagnostics().Select(error => new DiagnosticItem(error, dataSet.Bicep));

            var actual = JToken.FromObject(errors, DataSetSerialization.CreateSerializer());
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Diagnostics_Actual.json", actual.ToString(Formatting.Indented));

            var expected = JToken.Parse(dataSet.Errors);
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Diagnostics_Expected.json", expected.ToString(Formatting.Indented));
            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Diagnostics_Delta.json");
        }

        [TestMethod]
        public void EndOfFileFollowingSpaceAfterParameterKeyWordShouldNotThrow()
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText("parameter "));
            compilation.GetSemanticModel().GetParseDiagnostics();
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();
    }
}
