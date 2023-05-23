// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.FileSystem;
using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using Bicep.Decompiler.Exceptions;
using Bicep.Decompiler;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Baselines;
using System.Threading;
using System.Globalization;
using Bicep.Core.UnitTests.FileSystem;
using System.Threading.Tasks;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParamsDecompilationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static BicepparamDecompiler CreateBicepparamDecompiler(IFileResolver fileResolver)
          => ServiceBuilder.Create(s => s.WithFileResolver(fileResolver)).GetBicepparamDecompiler();

        [TestMethod]
        public void Decompiler_Decompiles_ValidParametersFile()
        {
            var jsonParametersFile = 
@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""first"": {
      ""value"": ""test""
    },
    ""second"": {
      ""value"": 1
    },
    ""third"" : {
      ""value"" : [
        1,
        ""foo""
      ]
    },
    ""fourth"" : {
      ""value"" : {
        ""firstKey"" : ""bar"",
        ""secondKey"" : 1
      }
    }
  }
}";
            var expectedBicepparamFile = 
@"using '' /*TODO: Provide a path to a bicep template*/

param first = 'test'

param second = 1

param third = [
  1
  'foo'
]

param fourth = {
  firstKey: 'bar'
  secondKey: 1
}";

            var paramFileUri = new Uri("file:///path/to/main.json");

            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
              [paramFileUri] = jsonParametersFile
            });

            var bicepparamDecompiler = CreateBicepparamDecompiler(fileResolver);

            var (entryPointUri, filesToSave) = bicepparamDecompiler.Decompile(paramFileUri, PathHelper.ChangeExtension(paramFileUri, LanguageConstants.ParamsFileExtension), null);

            filesToSave[entryPointUri].Should().Be(expectedBicepparamFile);         
        }

              [TestMethod]
        public void Decompiler_Decompiles_ValidParametersFileWithBicepFileReference()
        {
            var jsonParametersFile = 
@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""first"": {
      ""value"": ""test""
    },
    ""second"": {
      ""value"": 1
    },
    ""third"" : {
      ""value"" : true
    },
  }
}";
            var expectedBicepparamFile = 
@"using './dir/main.bicep'

param first = 'test'

param second = 1

param third = true";

            var paramFileUri = new Uri("file:///path/to/main.json");

            var bicepFilePath = "./dir/main.bicep";

            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
              [paramFileUri] = jsonParametersFile
            });

            var bicepparamDecompiler = CreateBicepparamDecompiler(fileResolver);

            var (entryPointUri, filesToSave) = bicepparamDecompiler.Decompile(
              paramFileUri, 
              PathHelper.ChangeExtension(paramFileUri, LanguageConstants.ParamsFileExtension), 
              bicepFilePath);

            filesToSave[entryPointUri].Should().Be(expectedBicepparamFile);         
        }


    }
}
