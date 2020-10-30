﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem
{
    public class ErrorType : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly ImmutableArray<ErrorDiagnostic> errors;

        public ErrorType(string name, TypeKind kind, ImmutableArray<ErrorDiagnostic> errors)
            : base(name)
        {
            this.TypeKind = kind;
            this.errors = errors;
        }

        public ErrorType(string name, TypeKind kind)
            : this(name, kind, ImmutableArray<ErrorDiagnostic>.Empty)
        {
        }

        public static ErrorType Create(ErrorDiagnostic error)
        {
            return new ErrorType(ErrorTypeName, TypeKind.Error, ImmutableArray.Create<ErrorDiagnostic>(error));
        }

        public static ErrorType Create(IEnumerable<ErrorDiagnostic> errors)
        {
            return new ErrorType(ErrorTypeName, TypeKind.Error, errors.ToImmutableArray());
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            return this.errors;
        }

        public override TypeKind TypeKind { get; }
    }
}

