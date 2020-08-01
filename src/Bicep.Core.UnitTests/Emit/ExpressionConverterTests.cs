using Azure.ResourceManager.Deployments.Expression.Configuration;
using Azure.ResourceManager.Deployments.Expression.Serializers;
using Bicep.Core.Emit;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class ExpressionConverterTests
    {
        [DataTestMethod]
        [DataRow("null", "[json('null')]")]
        [DataRow("true","[json('true')]")]
        [DataRow("false", "[json('false')]")]
        [DataRow("32", "[32]")]
        [DataRow("'hello world'", "hello world")]
        [DataRow(@"'\rI\nlike\ttabs\tand\'single\'quotes'", "\rI\nlike\ttabs\tand'single'quotes")]
        [DataRow(@"'\rI\nlike\ttabs\tand\'single\'quotes' + 2", "[add('\rI\nlike\ttabs\tand''single''quotes', 2)]")]
        [DataRow(@"!'escaping single quotes \'is\' simple'", "[not('escaping single quotes ''is'' simple')]")]
        [DataRow("!true", "[not(json('true'))]")]
        [DataRow("-10", "[-10]")]
        [DataRow("-'foo'", "[sub(0, 'foo')]")]
        [DataRow("-(2+4)", "[sub(0, add(2, 4))]")]
        [DataRow("'fake' ? 12 : null", "[if('fake', 12, json('null'))]")]
        [DataRow("[\n]", "[json('[]')]")]
        [DataRow("{\n}", "[json('{}')]")]
        [DataRow("2+3*4-10", "[sub(add(2, mul(3, 4)), 10)]")]
        [DataRow("true == false != null == 4 != 'a'", "[not(equals(equals(not(equals(equals(json('true'), json('false')), json('null'))), 4), 'a'))]")]
        [DataRow("-2 && 3 && !4 && 5", "[and(and(and(-2, 3), not(4)), 5)]")]
        [DataRow("-2 +-3 + -4 -10", "[sub(add(add(-2, -3), -4), 10)]")]
        [DataRow("true || false && null", "[or(json('true'), and(json('false'), json('null')))]")]
        [DataRow("3 / 2 + 4 % 0", "[add(div(3, 2), mod(4, 0))]")]
        [DataRow("3 / (2 + 4) % 0", "[mod(div(3, add(2, 4)), 0)]")]
        [DataRow("true < false", "[less(json('true'), json('false'))]")]
        [DataRow("null > 1", "[greater(json('null'), 1)]")]
        [DataRow("'aa' >= 14", "[greaterOrEquals('aa', 14)]")]
        [DataRow("10 <= -11", "[lessOrEquals(10, -11)]")]
        [DataRow("{\nfoo: 12\nbar: true\n}", "[json('{\"foo\":12,\"bar\":true}')]")]
        [DataRow("[\ntrue\nfalse\n12\nnull\n]", "[json('[true,false,12,null]')]")]
        public void ShouldConvertExpressionsCorrectly(string text, string expected)
        {
            var parsed = ParserHelper.ParseExpression(text);
            var converted = parsed.ToTemplateExpression();

            var serializer = new ExpressionSerializer(new ExpressionSerializerSettings
            {
                IncludeOuterSquareBrackets = true, SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
            });

            var actual = serializer.SerializeExpression(converted);

            actual.Should().Be(expected);
        }
    }
}