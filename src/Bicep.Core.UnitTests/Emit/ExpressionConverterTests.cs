// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Serializers;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class ExpressionConverterTests
    {
        [DataTestMethod]
        [DataRow("null", "[null()]")]
        [DataRow("true","[true()]")]
        [DataRow("false", "[false()]")]
        [DataRow("32", "[32]")]
        [DataRow("'hello world'", "hello world")]
        [DataRow(@"'\rI\nlike\ttabs\tand\'single\'quotes'", "\rI\nlike\ttabs\tand'single'quotes")]
        [DataRow(@"'\rI\nlike\ttabs\tand\'single\'quotes' + 2", "[add('\rI\nlike\ttabs\tand''single''quotes', 2)]")]
        [DataRow(@"!'escaping single quotes \'is\' simple'", "[not('escaping single quotes ''is'' simple')]")]
        [DataRow("!true", "[not(true())]")]
        [DataRow("-10", "[-10]")]
        [DataRow("-'foo'", "[sub(0, 'foo')]")]
        [DataRow("-(2+4)", "[sub(0, add(2, 4))]")]
        [DataRow("'fake' ? 12 : null", "[if('fake', 12, null())]")]
        [DataRow("[\n]", "[createArray()]")]
        [DataRow("{\n}", "[createObject()]")]
        [DataRow("2+3*4-10", "[sub(add(2, mul(3, 4)), 10)]")]
        [DataRow("true == false != null == 4 != 'a'", "[not(equals(equals(not(equals(equals(true(), false()), null())), 4), 'a'))]")]
        [DataRow("-2 && 3 && !4 && 5", "[and(and(and(-2, 3), not(4)), 5)]")]
        [DataRow("-2 +-3 + -4 -10", "[sub(add(add(-2, -3), -4), 10)]")]
        [DataRow("true || false && null", "[or(true(), and(false(), null()))]")]
        [DataRow("3 / 2 + 4 % 0", "[add(div(3, 2), mod(4, 0))]")]
        [DataRow("3 / (2 + 4) % 0", "[mod(div(3, add(2, 4)), 0)]")]
        [DataRow("true < false", "[less(true(), false())]")]
        [DataRow("null > 1", "[greater(null(), 1)]")]
        [DataRow("'aa' >= 14", "[greaterOrEquals('aa', 14)]")]
        [DataRow("10 <= -11", "[lessOrEquals(10, -11)]")]
        [DataRow("{\nfoo: 12\nbar: true\n}", "[createObject('foo', 12, 'bar', true())]")]
        [DataRow("[\ntrue\nfalse\n12\nnull\n]", "[createArray(true(), false(), 12, null())]")]
        [DataRow("[]", "[createArray()]")]
        [DataRow("'aaa' =~ 'bbb'", "[equals(toLower('aaa'), toLower('bbb'))]")]
        [DataRow("'aaa' !~ 'bbb'", "[not(equals(toLower('aaa'), toLower('bbb')))]")]
        [DataRow("resourceGroup().location", "[resourceGroup().location]")]
        [DataRow("resourceGroup()['location']", "[resourceGroup().location]")]
        [DataRow("[\n4\n][0]", "[createArray(4)[0]]")]
        [DataRow("[\n[]\n[\n12\n's'\n][1]\n\n]","[createArray(createArray(), createArray(12, 's')[1])]")]
        [DataRow("42[33].foo","[int(42)[33].foo]")]
        [DataRow("'foo'[x()]","[string('foo')[x()]]")]
        [DataRow("1 ?? 2 ?? 3","[coalesce(coalesce(1, 2), 3)]")]
        [DataRow("5 ?? 3 + 2 ?? 7", "[coalesce(coalesce(5, add(3, 2)), 7)]")]
        [DataRow("true ?? true && false ?? false || true", "[coalesce(coalesce(true(), and(true(), false())), or(false(), true()))]")]
        [DataRow("null ?? true", "[coalesce(null(), true())]")]
        public void ShouldConvertExpressionsCorrectly(string text, string expected)
        {
            var programText = $"var test = {text}";
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(programText, BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);

            var programSyntax = compilation.SourceFileGrouping.EntryPoint.ProgramSyntax;
            var variableDeclarationSyntax = programSyntax.Children.OfType<VariableDeclarationSyntax>().First();

            var converter = new ExpressionConverter(new EmitterContext(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettings));
            var converted = converter.ConvertExpression(variableDeclarationSyntax.Value);

            var serializer = new ExpressionSerializer(new ExpressionSerializerSettings
            {
                IncludeOuterSquareBrackets = true, SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
            });

            var actual = serializer.SerializeExpression(converted);

            actual.Should().Be(expected);
        }
    }
}
