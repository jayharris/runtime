// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        /// <summary>
        /// Takes a path containing relative subpaths or links and returns the absolute path.
        /// This function works on both files and folders and returns a null-terminated string.
        /// </summary>
        /// <param name="path">The path to the file system object</param>
        /// <returns>Returns the result string on success and null on failure</returns>
        [GeneratedDllImport(Libraries.SystemNative, EntryPoint = "SystemNative_RealPath", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string RealPath(string path);
    }
}
