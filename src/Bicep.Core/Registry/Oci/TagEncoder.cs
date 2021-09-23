// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using System;
using System.Diagnostics;
using System.Text;

namespace Bicep.Core.Registry.Oci
{
    /// <summary>
    /// Encodes the casing of an OCI tag into a file name that can be used with case-insensitive file systems.
    /// </summary>
    public static class TagEncoder
    {
        // not sure there's a way to get the constant programmatically
        private const int BitsInUnsignedLong = 64;

        private const int BitsInHexDigit = 4;

        // this should be equivalent to ceil(OciArtifactModuleReference.MaxTagLength / 64.0) but without floating point conversion
        private const int MaskComponents = (OciArtifactModuleReference.MaxTagLength + BitsInUnsignedLong - 1) / BitsInUnsignedLong;

        private const string HexFormat = "x";

        // TODO: change to const in .net 6
        private static readonly string UnsignedLongHexFormat = $"{HexFormat}{BitsInUnsignedLong / BitsInHexDigit}";

        /// <summary>
        /// Encodes the specified OCI tag into a file name that can be used with case-insensitive file systems.
        /// </summary>
        /// <param name="tag">The tag value. The tag should be validated to match the OCI spec before calling this function.</param>
        public static string Encode(string tag)
        {
            if(tag.Length > OciArtifactModuleReference.MaxTagLength)
            {
                throw new ArgumentException($"The specified tag '{tag}' exceeds max length of {OciArtifactModuleReference.MaxTagLength}.");
            }

            var mask = new ulong[MaskComponents];
            for (int i = 0; i < tag.Length; i++)
            {
                if (char.IsUpper(tag, i))
                {
                    int maskIndex = i / BitsInUnsignedLong;
                    ulong maskValue = 1UL << (i % BitsInUnsignedLong);

                    mask[maskIndex] |= maskValue;
                }
            }

            var buffer = new StringBuilder();
            buffer.Append(tag);

            // any char that is not allowed in a tag is ok here
            buffer.Append('$');

            var includeLeadingZeros = false;
            for(int i = mask.Length - 1; i >= 0; i--)
            {
                var format = includeLeadingZeros ? UnsignedLongHexFormat : HexFormat;
                ulong maskValue = mask[i];
                if (maskValue != 0UL || includeLeadingZeros)
                {
                    buffer.Append(maskValue.ToString(format));

                    // we've inserted some leading digits, the next part must be included even if it's just zeros
                    includeLeadingZeros = true;
                }
            }

            return buffer.ToString();
        }
    }
}
