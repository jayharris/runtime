// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Security.Tests
{
    public class ServerNoEncryptionTest
    {
        private readonly ITestOutputHelper _log;

        public ServerNoEncryptionTest(ITestOutputHelper output)
        {
            _log = output;
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public bool AllowAnyServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            return true;  // allow everything
        }

        [Fact]
        public async Task ServerNoEncryption_ClientRequireEncryption_NoConnect()
        {
            using (var serverNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 0), EncryptionPolicy.NoEncryption))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverNoEncryption.RemoteEndPoint.Address, serverNoEncryption.RemoteEndPoint.Port);

                using (var sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.RequireEncryption))
                {
                    await Assert.ThrowsAsync<AuthenticationException>(() =>
                        sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocolSupport.DefaultSslProtocols, false));
                }
            }
        }

        [ConditionalFact(nameof(SupportsNullEncryption))]
        public async Task ServerNoEncryption_ClientAllowNoEncryption_ConnectWithNoEncryption()
        {
            using (var serverNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 0), EncryptionPolicy.NoEncryption))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverNoEncryption.RemoteEndPoint.Address, serverNoEncryption.RemoteEndPoint.Port);

                using (var sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.AllowNoEncryption))
                {
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
                    await sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);
#pragma warning restore SYSLIB0039

                    _log.WriteLine("Client authenticated to server({0}) with encryption cipher: {1} {2}-bit strength",
                        serverNoEncryption.RemoteEndPoint, sslStream.CipherAlgorithm, sslStream.CipherStrength);

                    CipherAlgorithmType expected = CipherAlgorithmType.Null;
                    Assert.Equal(expected, sslStream.CipherAlgorithm);
                    Assert.Equal(0, sslStream.CipherStrength);
                }
            }
        }

        [Fact]
        public async Task ServerNoEncryption_ClientNoEncryption_ConnectWithNoEncryption()
        {
            using (var serverNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 0), EncryptionPolicy.NoEncryption))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverNoEncryption.RemoteEndPoint.Address, serverNoEncryption.RemoteEndPoint.Port);

                using (var sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.NoEncryption))
                {
                    if (SupportsNullEncryption)
                    {
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
                        // null encryption is not permitted with Tls13
                        await sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);
#pragma warning restore SYSLIB0039
                        _log.WriteLine("Client authenticated to server({0}) with encryption cipher: {1} {2}-bit strength",
                            serverNoEncryption.RemoteEndPoint, sslStream.CipherAlgorithm, sslStream.CipherStrength);

                        CipherAlgorithmType expected = CipherAlgorithmType.Null;
                        Assert.Equal(expected, sslStream.CipherAlgorithm);
                        Assert.Equal(0, sslStream.CipherStrength);
                    }
                    else
                    {
                        var ae = await Assert.ThrowsAsync<AuthenticationException>(() => sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocolSupport.DefaultSslProtocols, false));
                        if (!OperatingSystem.IsWindows())
                        {
                            Assert.IsType<PlatformNotSupportedException>(ae.InnerException);
                        }
                    }
                }
            }
        }

        private static bool SupportsNullEncryption => TestConfiguration.SupportsNullEncryption;
    }
}
