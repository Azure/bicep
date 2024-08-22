// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Expression.Intermediate;

namespace Bicep.Core.Intermediate
{
    public class CrossModuleSentinelVisitor : TemplateLanguageExpressionVisitor
    {
        public HashSet<string> parametersEncountered = new();

        override public void VisitFunctionExpression(FunctionExpression func)
        {
            if (func.Name == "sentinel-placeholder")
            {
                if (func.Arguments.Length != 1 ||
                    func.Arguments.Single() is not StringExpression { Value: string parameterName })
                {
                    throw new InvalidOperationException("Something's not right here...");
                }

                parametersEncountered.Add(parameterName);
            }

            base.VisitFunctionExpression(func);
        }
    }
}
