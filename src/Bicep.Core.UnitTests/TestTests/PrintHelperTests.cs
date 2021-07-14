// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TestTests
{
    [TestClass]
    public class PrintHelperTests
    {
        [TestMethod]
        public void PrintHelper_should_add_annotations()
        {
            var bicepFile = SourceFileGroupingFactory.CreateFromText(@"
resource domainServices 'Microsoft.MadeUpRp/madeUpType@2017-06-01' = {
  name: 'hello'
  location: location
  properties: {
    someMadeUpProp: 'boo'
  }
}
", BicepTestConstants.FileResolver).EntryPoint;

            var output = PrintHelper.PrintWithAnnotations(bicepFile, new[] {
                new PrintHelper.Annotation(new TextSpan(26, 18), "what is this!?"),
                // 0 length span should produce a '^' rather than a '~'
                new PrintHelper.Annotation(new TextSpan(80, 0), "oh, hi!"),
                new PrintHelper.Annotation(new TextSpan(129, 14), "i can't believe you've done this"),
            }, 1, true);

            output.Should().Be(
@"1| 
2| resource domainServices 'Microsoft.MadeUpRp/madeUpType@2017-06-01' = {
                            ~~~~~~~~~~~~~~~~~~ what is this!?
3|   name: 'hello'
           ^ oh, hi!
4|   location: location
5|   properties: {
6|     someMadeUpProp: 'boo'
       ~~~~~~~~~~~~~~ i can't believe you've done this
7|   }
");
        }

        [TestMethod]
        public void PrintHelper_only_includes_nearby_context()
        {
            var bicepFile = SourceFileGroupingFactory.CreateFromText(@"
var test = '''
here's
a
really
long
multiline
string
that
we
don't
care
about
'''

// 
// give me a cursor here please!
//


var test = '''
here's
another
really
long
multiline
string
that
we
don't
care
about
'''
", BicepTestConstants.FileResolver).EntryPoint;

            var output = PrintHelper.PrintWithAnnotations(bicepFile, new[] {
                new PrintHelper.Annotation(new TextSpan(108, 4), "here's your cursor!"),
            }, 1, true);

            output.Should().Be(
@"16| // 
17| // give me a cursor here please!
                        ~~~~ here's your cursor!
18| //
");
        }
    }
}
