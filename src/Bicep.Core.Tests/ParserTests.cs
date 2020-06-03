using System.Text;
using FluentAssertions;
using Bicep.Core.Parser;
using Bicep.Core.Tests.UnitSamples;
using Bicep.Core.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Tests
{
    [TestClass]
    public class ParserTests
    {
        class PrintVisitor : TokenVisitor
        {
            private readonly StringBuilder stringBuilder = new StringBuilder();

            protected override void VisitToken(Token token)
            {
                stringBuilder.Append(token.LeadingTrivia);
                stringBuilder.Append(token.Text);
                stringBuilder.Append(token.TrailingTrivia);
            }

            public string GetOutput()
                => stringBuilder.ToString();
        }

        private string BasicTemplate = @"
parameter string rgLocation;
parameter string namePrefix;

/*
module publicIpAddress {
  input string name
  input string location

  resource azrm 'Network/publicIpAddresses@2019-11-01' publicIp {
    name: name
    location: location
    properties: {
      publicIPAllocationMethod: 'Dynamic'
    }
  }
}
*/

resource azrm 'Network/virtualNetworks/subnets@2019-11-01' mySubnet: {
  name: concat('myVnet/', namePrefix, '-subnet'),
  location: rgLocation,
  properties: {
    addressPrefix: '10.0.0.0/24'
  }
};

resource azrm 'Network/networkInterfaces@2019-11-01' myNic: {
  name: concat(namePrefix, '-nic'),
  location: (rgLocation),
  properties: {
    ipConfigurations: [{
      name: 'myConfig',
      properties: {
        subnet: subnetReference,
        privateIPAllocationMethod: 'Dynamic',
      }
    }]
  }
};

variable subnetReference: {
  id: resourceId(mySubnet)
};

// this comment should be ignored

/* this block comment should be ignored
resource azrm 'Network/publicIpAddresses@2019-11-01' myPip {
  name: concat(namePrefix, '-pip')
  properties: {
  }
}
*/

resource mod 'publicIpAddress' pip1: {
  name: concat(namePrefix, '-pip1'),
  location: rgLocation
};

resource mod 'publicIpAddress' pip2: {
  name: concat(namePrefix, '-pip2'),
  location: rgLocation
};

output nicResourceId: resourceId(myNic);
";

        [TestMethod]
        public void RoundTripTest()
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(BasicTemplate));
            lexer.Lex();

            var tokens = lexer.GetTokens();
            var parser = new Parser.Parser(tokens);

            var program = parser.Parse();

            var output = new StringBuilder();
            var printer = new PrintVisitor();
            printer.Visit(program);

            printer.GetOutput().Should().Be(BasicTemplate);
        }

        [DataTestMethod]
        [UnitSamplesDataSource]
        public void ParameterTest(string displayName, string contents)
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            var tokens = lexer.GetTokens();
            var parser = new Parser.Parser(tokens);
            
            var program = parser.Parse();
        }
    }
}
