// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.FileIO;

namespace Bicep.Core.Constants
{
    public readonly record struct FileKind
    {
        public readonly static FileKind Unknown = new(FileExtensions.None);

        public readonly static FileKind Bicep = new(FileExtensions.Bicep);

        public readonly static FileKind BicepParam = new(FileExtensions.BicepParam);

        public readonly static FileKind Json = new(FileExtensions.Json);

        public readonly static FileKind Jsonc = new(FileExtensions.Jsonc);

        public readonly static FileKind ArmTemplate = new(FileExtensions.ArmTemplate);

        private FileKind(string extension)
        {
            Extension = extension;
        }

        public string Extension { get; }

        public static FileKind FromExtension(string extension) => extension.ToLowerInvariant() switch
        {
            FileExtensions.Bicep => Bicep,
            FileExtensions.BicepParam => Bicep,
            FileExtensions.Json => Json,
            FileExtensions.Jsonc => Jsonc,
            FileExtensions.ArmTemplate => ArmTemplate,
            _ => Unknown,
        };

        public bool Is(FileKind kind) => this == kind;

        public bool IsArmTemplateLike() => this.Is(ArmTemplate) || this.Is(Json) || this.Is(Jsonc);
    }
}
