// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Assertions
{
    public class JTokenAssertions : ReferenceTypeAssertions<JToken, JTokenAssertions>
    {
        public JTokenAssertions(JToken instance)
        {
            Subject = instance;
        }

        protected override string Identifier => "jtoken";
    }
}