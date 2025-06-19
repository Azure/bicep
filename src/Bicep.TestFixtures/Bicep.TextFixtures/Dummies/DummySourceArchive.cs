// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.SourceLink;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;
using Bicep.TextFixtures.Mocks;

namespace Bicep.TextFixtures.Dummies
{
    public static class DummySourceArchive
    {
        public static readonly SourceArchive Default = Create("main.bicep");

        public static SourceArchive Create(string entryPointPath, string entryPointContent = "// nothing")
        {
            var stream = new MemoryStream();
            using (var writer = new TgzWriter(stream, leaveOpen: true))
            {
                writer.WriteEntry("__metadata.json", $$"""
                    {
                      "entryPoint": "{{entryPointPath}}",
                      "bicepVersion": "0.1.2",
                      "metadataVersion": 1,
                      "sourceFiles": [
                        {
                          "path": "{{entryPointPath}}",
                          "archivePath": "files/{{entryPointPath}}",
                          "kind": "bicep"
                        }
                      ],
                      "documentLinks": {}
                    }
                    """);

                writer.WriteEntry($"files/{entryPointPath}", entryPointContent);
            }

            stream.Seek(0, SeekOrigin.Begin);

            var tgzFileHandleMock = StrictMock.Of<IFileHandle>();
            tgzFileHandleMock.Setup(x => x.Exists()).Returns(true);
            tgzFileHandleMock.Setup(x => x.OpenRead()).Returns(stream);

            return SourceArchive.TryUnpackFromFile(tgzFileHandleMock.Object).Unwrap();
        }
    }
}
