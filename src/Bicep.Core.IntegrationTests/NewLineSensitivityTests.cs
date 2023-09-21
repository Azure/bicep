// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class NewLineSensitivityTests
    {
        [TestMethod]
        public void Function_argument_newlines_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat(
  'foo',
  'bar'
)
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", "[concat('foo', 'bar')]");
        }

        [TestMethod]
        public void Function_argument_multiple_newlines_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat(
  // this is the foo parameter!
  'foo',

  // this is the bar parameter!
  'bar'

)
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", "[concat('foo', 'bar')]");
        }

        [TestMethod]
        public void Function_argument_leading_and_trailing_newlines_can_be_dropped()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat('foo',
  'bar')
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", "[concat('foo', 'bar')]");
        }

        [TestMethod]
        public void Function_argument_commas_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat('foo', 'bar')
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", "[concat('foo', 'bar')]");
        }

        [TestMethod]
        public void Function_arguments_must_be_separated_by_a_comma()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat('foo' 'bar')
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP236", DiagnosticLevel.Error, "Expected a new line or comma character at this location."),
            });
        }

        [TestMethod]
        public void Function_argument_should_raise_error_for_trailing_comma()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat('foo',)
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location."),
            });
        }

        [TestMethod]
        public void Function_argument_error_recovery_works()
        {
            var result = CompilationHelper.Compile(@"
output test1 string = concat('foo', 'bar'
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \")\" character at this location."),
            });
        }

        [TestMethod]
        public void Array_newline_is_optional_at_start_and_end()
        {
            // missing new line at the start and end of the array
            var result = CompilationHelper.Compile(@"
var foo = ['hi']
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables['foo']", new JArray {
                "hi"
            });

            // missing new line at the start of the array
            result = CompilationHelper.Compile(@"
var foo = ['hi'
]
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables['foo']", new JArray {
                "hi"
            });

            // missing new line at the end of the array
            result = CompilationHelper.Compile(@"
var foo = [
  'hi']
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables['foo']", new JArray {
                "hi"
            });
        }

        [TestMethod]
        public void Object_newline_is_optional_at_start_and_end()
        {
            // missing new line at the start and end of the object
            var result = CompilationHelper.Compile(@"
var foo = {foo: 'bar'}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables['foo']", new JObject
            {
                ["foo"] = "bar"
            });

            // missing new line at the start of the object
            result = CompilationHelper.Compile(@"
var foo = {foo: 'bar'
}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables['foo']", new JObject
            {
                ["foo"] = "bar"
            });

            // missing new line at the end of the object
            result = CompilationHelper.Compile(@"
var foo = {
  foo: 'bar'}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables['foo']", new JObject
            {
                ["foo"] = "bar"
            });
        }


        [TestMethod]
        public void Array_item_newlines_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 array = [
  'foo'
  'bar'
]
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", new JArray {
                "foo",
                "bar"
            });
        }

        [TestMethod]
        public void Array_item_multiple_newlines_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 array = [
  // this is the foo parameter!
  'foo'

  // this is the bar parameter!
  'bar'

]
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", new JArray {
                "foo",
                "bar"
            });
        }

        [TestMethod]
        public void Array_item_leading_and_trailing_newlines_can_be_dropped()
        {
            var result = CompilationHelper.Compile(@"
output test1 array = ['foo'
  'bar']
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", new JArray {
                "foo",
                "bar"
            });
        }

        [TestMethod]
        public void Array_item_commas_are_permitted_on_single_line_definition()
        {
            var result = CompilationHelper.Compile(@"
output test1 array = ['foo', 'bar']
output test2 array = ['foo', 'bar',]
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Array_item_commas_are_not_permitted_on_multi_line_definition()
        {
            var result = CompilationHelper.Compile(@"
output test1 array = [
  'abc',
  'def'
]
output test2 array = [
  'abc',
  'def',
]
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP238", DiagnosticLevel.Error, "Unexpected new line character after a comma."),
                ("BCP238", DiagnosticLevel.Error, "Unexpected new line character after a comma."),
                ("BCP238", DiagnosticLevel.Error, "Unexpected new line character after a comma."),
            });
        }

        [TestMethod]
        public void Array_item_error_recovery_works()
        {
            var result = CompilationHelper.Compile(@"
output test1 array = ['foo', 'bar'
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"]\" character at this location."),
            });
        }


        [TestMethod]
        public void Object_item_newlines_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 object = {
  abc: 'foo'
  def: 'bar'
}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", new JObject
            {
                ["abc"] = "foo",
                ["def"] = "bar"
            });
        }

        [TestMethod]
        public void Object_item_multiple_newlines_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
output test1 object = {
  // this is the foo parameter!
  abc: 'foo'

  // this is the bar parameter!
  def: 'bar'

}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", new JObject
            {
                ["abc"] = "foo",
                ["def"] = "bar"
            });
        }

        [TestMethod]
        public void Object_item_leading_and_trailing_newlines_can_be_dropped()
        {
            var result = CompilationHelper.Compile(@"
output test1 object = { abc: 'foo'
  def: 'bar' }
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['test1'].value", new JObject
            {
                ["abc"] = "foo",
                ["def"] = "bar"
            });
        }

        [TestMethod]
        public void Object_item_commas_are_permitted_on_single_line_definition()
        {
            var result = CompilationHelper.Compile(@"
output test1 object = {abc: 'foo', def: 'bar'}
output test2 object = {abc: 'foo', def: 'bar',}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Object_item_commas_are_not_permitted_on_multi_line_definition()
        {
            var result = CompilationHelper.Compile(@"
output test1 object = {
  abc: 'abc',
  def: 'def'
}
output test2 object = {
  abc: 'abc',
  def: 'def',
}
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP238", DiagnosticLevel.Error, "Unexpected new line character after a comma."),
                ("BCP238", DiagnosticLevel.Error, "Unexpected new line character after a comma."),
                ("BCP238", DiagnosticLevel.Error, "Unexpected new line character after a comma."),
            });
        }

        [TestMethod]
        public void Object_item_error_recovery_works()
        {
            var result = CompilationHelper.Compile(@"
output test1 object = { abc: 'foo', def: 'bar'
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"}\" character at this location."),
            });
        }
    }
}
