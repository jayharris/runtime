// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        internal static unsafe uint[]? GetGroupList(string userName, uint primaryGroupId)
        {
            const int InitialGroupsLength =
#if DEBUG
                1;
#else
                64;
#endif
            Span<uint> groups = stackalloc uint[InitialGroupsLength];
            do
            {
                int rv;
                int ngroups = groups.Length;
                fixed (uint* pGroups = groups)
                {
                    rv = Interop.Sys.GetGroupList(userName, primaryGroupId, pGroups, &ngroups);
                }
                if (rv >= 0)
                {
                    // success
                    return groups.Slice(0, ngroups).ToArray();
                }
                else if (rv == -1 && ngroups > groups.Length)
                {
                    // increase buffer size
                    groups = new uint[ngroups];
                }
                else
                {
                    // failure
                    return null;
                }
            } while (true);
        }

        [GeneratedDllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetGroupList", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        private static unsafe partial int GetGroupList(string name, uint group, uint* groups, int* ngroups);
    }
}
