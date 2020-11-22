// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Cli.UnitTests;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class CleanTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static Program CreateProgram(TextWriter outputWriter, TextWriter errorWriter)
        {
            return new Program(TestResourceTypeProvider.Create(), outputWriter, errorWriter);
        }

        [TestMethod]
        public void ZeroDirectoriesToCleanShouldProduceError()
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "clean" });
            });

            result.Should().Be(1);
            output.Should().BeEmpty();

            error.Should().NotBeEmpty();
            error.Should().Be($"At least one directory must be specified to the clean command.{Environment.NewLine}");
        }

        [TestMethod]
        public void CleanSingleDirectoryShouldRemoveOneTemplate()
        {
            var directoryName = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName);
            Directory.CreateDirectory(directoryName);

            var bicepName = Path.Combine(directoryName, "main.bicep");
            File.WriteAllText(bicepName, "");

            var jsonName = Path.Combine(directoryName, "main.json");
            File.WriteAllText(jsonName, "{}");

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "clean", directoryName });
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            File.Exists(bicepName).Should().BeTrue();
            File.Exists(jsonName).Should().BeFalse();
        }

        [TestMethod]
        public void CleanSingleDirectoryShouldRemoveManyTemplates()
        {
            var directoryName = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName);
            Directory.CreateDirectory(directoryName);

            var bicepName = Path.Combine(directoryName, "main.bicep");
            File.WriteAllText(bicepName, "");

            var jsonName = Path.Combine(directoryName, "main.json");
            File.WriteAllText(jsonName, "{}");

            var secondBicepName = Path.Combine(directoryName, "secondMain.bicep");
            File.WriteAllText(secondBicepName, "");

            var secondJsonName = Path.Combine(directoryName, "secondMain.json");
            File.WriteAllText(secondJsonName, "{}");

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);
                return p.Run(new[] { "clean", directoryName });
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            File.Exists(bicepName).Should().BeTrue();
            File.Exists(jsonName).Should().BeFalse();
            File.Exists(secondBicepName).Should().BeTrue();
            File.Exists(secondJsonName).Should().BeFalse();
        }

        [TestMethod]
        public void CleanMultipleDirectoriesShouldRemoveOneTemplate()
        {
            string firstDirectory = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName);
            string secondDirectory = Path.Combine(firstDirectory, TestContext.TestName);

            string[] directories = new string[] { firstDirectory, secondDirectory };
            List<string> files = new List<string>();
            string fileName;

            foreach (string dir in directories)
            {
                Directory.CreateDirectory(dir);

                fileName = Path.Combine(dir, "main.bicep");
                File.WriteAllText(fileName, "{}");
                files.Add(fileName);

                fileName = Path.Combine(dir, "main.json");
                File.WriteAllText(fileName, "{}");
                files.Add(fileName);
            }
   

            var (output, error, result) = TextWriterHelper.InvokeWriterAction((@out, err) =>
            {
                var p = CreateProgram(@out, err);

                string[] args = "clean".AsEnumerable().Concat(directories).ToArray();
                return p.Run(args);
            });

            using (new AssertionScope())
            {
                result.Should().Be(0);
                output.Should().BeEmpty();
                error.Should().BeEmpty();
            }

            foreach (string file in files)
            {
                if(Path.GetExtension(file).Equals(".json"))
                {
                    File.Exists(file).Should().BeFalse();
                }
                else
                {
                   File.Exists(file).Should().BeTrue();
                }
            }
        }

    }
}
