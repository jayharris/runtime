// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing.Printing
{
    internal static partial class LibcupsNative
    {
        internal const string LibraryName = "libcups";

        static LibcupsNative()
        {
            LibraryResolver.EnsureRegistered();
        }

        internal static IntPtr LoadLibcups()
        {
            // We allow both "libcups.so" and "libcups.so.2" to be loaded.
            if (!NativeLibrary.TryLoad("libcups.so", out IntPtr lib))
            {
                NativeLibrary.TryLoad("libcups.so.2", out lib);
            }

            return lib;
        }

        [GeneratedDllImport(LibraryName)]
        internal static partial int cupsGetDests(ref IntPtr dests);

        [GeneratedDllImport(LibraryName)]
        internal static partial void cupsFreeDests(int num_dests, IntPtr dests);

        [GeneratedDllImport(LibraryName)]
        internal static partial IntPtr cupsTempFd(sbyte[] sb, int len);

        [GeneratedDllImport(LibraryName)]
        internal static partial IntPtr cupsGetDefault();

        [GeneratedDllImport(LibraryName)]
        internal static partial int cupsPrintFile(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string printer,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string title,
            int num_options,
            IntPtr options);

        [GeneratedDllImport(LibraryName)]
        internal static partial IntPtr cupsGetPPD([MarshalAs(UnmanagedType.LPUTF8Str)] string printer);

        [GeneratedDllImport(LibraryName)]
        internal static partial IntPtr ppdOpenFile([MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

        [GeneratedDllImport(LibraryName)]
        internal static partial IntPtr ppdFindOption(IntPtr ppd_file, [MarshalAs(UnmanagedType.LPUTF8Str)] string keyword);

        [GeneratedDllImport(LibraryName)]
        internal static partial void ppdClose(IntPtr ppd);

        [GeneratedDllImport(LibraryName)]
        internal static partial int cupsParseOptions([MarshalAs(UnmanagedType.LPUTF8Str)] string arg, int number_of_options, ref IntPtr options);

        [GeneratedDllImport(LibraryName)]
        internal static partial void cupsFreeOptions(int number_options, IntPtr options);
    }
}
