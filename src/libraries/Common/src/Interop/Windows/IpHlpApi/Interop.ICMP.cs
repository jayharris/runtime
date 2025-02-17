// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class IpHlpApi
    {
        internal const int IP_STATUS_BASE = 11000;

        [StructLayout(LayoutKind.Sequential)]
        internal struct IPOptions
        {
            internal byte ttl;
            internal byte tos;
            internal byte flags;
            internal byte optionsSize;
            internal IntPtr optionsData;

            internal IPOptions(PingOptions? options)
            {
                ttl = 128;
                tos = 0;
                flags = 0;
                optionsSize = 0;
                optionsData = IntPtr.Zero;

                if (options != null)
                {
                    this.ttl = (byte)options.Ttl;

                    if (options.DontFragment)
                    {
                        flags = 2;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IcmpEchoReply
        {
            internal uint address;
            internal uint status;
            internal uint roundTripTime;
            internal ushort dataSize;
            internal ushort reserved;
            internal IntPtr data;
            internal IPOptions options;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Ipv6Address
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            internal byte[] Goo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            // Replying address.
            internal byte[] Address;
            internal uint ScopeID;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Icmp6EchoReply
        {
            internal Ipv6Address Address;
            // Reply IP_STATUS.
            internal uint Status;
            // RTT in milliseconds.
            internal uint RoundTripTime;
            internal IntPtr data;
            // internal IPOptions options;
            // internal IntPtr data; data os after tjos
        }

        internal sealed class SafeCloseIcmpHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeCloseIcmpHandle() : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return Interop.IpHlpApi.IcmpCloseHandle(handle);
            }
        }

        [GeneratedDllImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        internal static partial SafeCloseIcmpHandle IcmpCreateFile();

        [GeneratedDllImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        internal static partial SafeCloseIcmpHandle Icmp6CreateFile();

        [GeneratedDllImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool IcmpCloseHandle(IntPtr handle);

        [GeneratedDllImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        internal static partial uint IcmpSendEcho2(SafeCloseIcmpHandle icmpHandle, SafeWaitHandle Event, IntPtr apcRoutine, IntPtr apcContext,
            uint ipAddress, SafeLocalAllocHandle data, ushort dataSize, ref IPOptions options, SafeLocalAllocHandle replyBuffer, uint replySize, uint timeout);

        [GeneratedDllImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        internal static partial uint Icmp6SendEcho2(SafeCloseIcmpHandle icmpHandle, SafeWaitHandle Event, IntPtr apcRoutine, IntPtr apcContext,
            byte[] sourceSocketAddress, byte[] destSocketAddress, SafeLocalAllocHandle data, ushort dataSize, ref IPOptions options, SafeLocalAllocHandle replyBuffer, uint replySize, uint timeout);
    }
}
