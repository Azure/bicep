// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces
{
    public class SystemNamespaceSymbol : NamespaceSymbol
    {
        private static readonly ImmutableArray<FunctionOverload> SystemOverloads = new[]
        {
            new FunctionOverloadBuilder("any")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("concat")
                .WithReturnType(LanguageConstants.Array)
                .WithVariableParameters(1, LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("concat")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(1, UnionType.Create(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool))
                .Build(),

            new FunctionOverloadBuilder("format")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParametersAndOptionalVariableParameters(LanguageConstants.Any, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("base64")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("padLeft")
                .WithReturnType(LanguageConstants.String)
                .WithOptionalFixedParameters(2, UnionType.Create(LanguageConstants.String, LanguageConstants.Int), LanguageConstants.Int, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("replace")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("toLower")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("toUpper")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("length")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(UnionType.Create(LanguageConstants.String, LanguageConstants.Object, LanguageConstants.Array))
                .Build(),

            new FunctionOverloadBuilder("split")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.String, UnionType.Create(LanguageConstants.String, LanguageConstants.Array))
                .Build(),

            new FunctionOverloadBuilder("string")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("int")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(UnionType.Create(LanguageConstants.String, LanguageConstants.Int))
                .Build(),

            new FunctionOverloadBuilder("uniqueString")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(1, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("guid")
                .WithReturnType(LanguageConstants.String)
                .WithVariableParameters(1, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("trim")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("uri")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("substring")
                .WithReturnType(LanguageConstants.String)
                .WithOptionalFixedParameters(2, LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Array, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Array, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("empty")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(UnionType.Create(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String))
                .Build(),

            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.Object, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.Array, LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("intersection")
                .WithReturnType(LanguageConstants.Object)
                .WithVariableParameters(2, LanguageConstants.Object)
                .Build(),

            new FunctionOverloadBuilder("intersection")
                .WithReturnType(LanguageConstants.Array)
                .WithVariableParameters(2, LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("union")
                .WithReturnType(LanguageConstants.Object)
                .WithVariableParameters(2, LanguageConstants.Object)
                .Build(),

            new FunctionOverloadBuilder("union")
                .WithReturnType(LanguageConstants.Array)
                .WithVariableParameters(2, LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("indexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("lastIndexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("startsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("endsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            // TODO: Needs to support number type as well
            new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithVariableParameters(1, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),

            // TODO: Needs to support number type as well
            new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithVariableParameters(1, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("range")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Int, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("base64ToString")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("base64ToJson")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("uriComponentToString")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("uriComponent")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("dataUriToString")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("dataUri")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("array")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("coalesce")
                .WithReturnType(LanguageConstants.Any)
                .WithVariableParameters(1, LanguageConstants.Any)
                .Build(),

            // TODO: Requires number type
            //new FunctionOverloadBuilder("float")
            //    .WithReturnType(LanguageConstants.Number)
            //    .WithFixedParameters(LanguageConstants.Any)
            //    .Build(),

            new FunctionOverloadBuilder("bool")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("json")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("dateTimeAdd")
                .WithReturnType(LanguageConstants.String)
                .WithOptionalFixedParameters(2, LanguageConstants.String, LanguageConstants.String, LanguageConstants.String)
                .Build(),

            // newGuid and utcNow are only allowed in parameter default values
            new FunctionOverloadBuilder("utcNow")
                .WithReturnType(LanguageConstants.String)
                .WithOptionalFixedParameters(0, LanguageConstants.String)
                .WithFlags(FunctionFlags.ParamDefaultsOnly)
                .Build(),

            new FunctionOverloadBuilder("newGuid")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters()
                .WithFlags(FunctionFlags.ParamDefaultsOnly)
                .Build(),
        }.ToImmutableArray();

        // TODO: Add copyIndex here when we support loops.
        private static readonly ImmutableArray<BannedFunction> BannedFunctions = new[]
        {
            /*
             * The true(), false(), and null() functions are not included in this list because
             * we parse true, false and null as keywords in the lexer, so they can't be used as functions anyway.
             */

            new BannedFunction("variables", builder => builder.VariablesFunctionNotSupported()),
            new BannedFunction("parameters", builder => builder.ParametersFunctionNotSupported()),
            new BannedFunction("if", builder => builder.IfFunctionNotSupported()),
            new BannedFunction("createArray", builder => builder.CreateArrayFunctionNotSupported()),
            new BannedFunction("createObject", builder => builder.CreateObjectFunctionNotSupported()),

            BannedFunction.CreateForOperator("add", "+"),
            BannedFunction.CreateForOperator("sub", "-"),
            BannedFunction.CreateForOperator("mul", "*"),
            BannedFunction.CreateForOperator("div", "/"),
            BannedFunction.CreateForOperator("mod", "%"),
            BannedFunction.CreateForOperator("less", "<"),
            BannedFunction.CreateForOperator("lessOrEquals", "<="),
            BannedFunction.CreateForOperator("greater", ">"),
            BannedFunction.CreateForOperator("greaterOrEquals", ">="),
            BannedFunction.CreateForOperator("equals", "=="),
            BannedFunction.CreateForOperator("not", "!"),
            BannedFunction.CreateForOperator("and", "&&"),
            BannedFunction.CreateForOperator("or", "||")
        }.ToImmutableArray();

        public SystemNamespaceSymbol() : base("sys", SystemOverloads, BannedFunctions)
        {
        }
    }
}

