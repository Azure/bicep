// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilerFlags
    {
        public static bool IsWindowsBuild
        {
#if WINDOWS_BUILD
            get => true;
#else
            get => false;
#endif
        }

        public static bool IsLinuxBuild
        {
#if LINUX_BUILD
            get => true;
#else
            get => false;
#endif
        }

        public static bool IsOsxBuild
        {
#if OSX_BUILD
            get => true;
#else
            get => false;
#endif
        }
    }
}