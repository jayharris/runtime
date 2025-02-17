// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Sockets;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Security.Tests
{
    public class ServerAllowNoEncryptionTest
    {
        private readonly ITestOutputHelper _log;

        public ServerAllowNoEncryptionTest(ITestOutputHelper output)
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
        public async Task ServerAllowNoEncryption_ClientRequireEncryption_ConnectWithEncryption()
        {
            using (var serverAllowNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 0), EncryptionPolicy.AllowNoEncryption))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverAllowNoEncryption.RemoteEndPoint.Address, serverAllowNoEncryption.RemoteEndPoint.Port);

                using (var sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.RequireEncryption))
                {
                    await sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocolSupport.DefaultSslProtocols, false);
                    _log.WriteLine("Client authenticated to server({0}) with encryption cipher: {1} {2}-bit strength",
                        serverAllowNoEncryption.RemoteEndPoint, sslStream.CipherAlgorithm, sslStream.CipherStrength);
                    Assert.NotEqual(CipherAlgorithmType.Null, sslStream.CipherAlgorithm);
                    Assert.True(sslStream.CipherStrength > 0);
                }
            }
        }

        [Fact]
        public async Task ServerAllowNoEncryption_ClientAllowNoEncryption_ConnectWithEncryption()
        {
            using (var serverAllowNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 0), EncryptionPolicy.AllowNoEncryption))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverAllowNoEncryption.RemoteEndPoint.Address, serverAllowNoEncryption.RemoteEndPoint.Port);

                using (var sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.AllowNoEncryption))
                {
                    await sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocolSupport.DefaultSslProtocols, false);
                    _log.WriteLine("Client authenticated to server({0}) with encryption cipher: {1} {2}-bit strength",
                        serverAllowNoEncryption.RemoteEndPoint, sslStream.CipherAlgorithm, sslStream.CipherStrength);
                    Assert.NotEqual(CipherAlgorithmType.Null, sslStream.CipherAlgorithm);
                    Assert.True(sslStream.CipherStrength > 0, "Cipher strength should be greater than 0");
                }
            }
        }

        [ConditionalFact(nameof(SupportsNullEncryption))]
        public async Task ServerAllowNoEncryption_ClientNoEncryption_ConnectWithNoEncryption()
        {
            using (var serverAllowNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 0), EncryptionPolicy.AllowNoEncryption))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(serverAllowNoEncryption.RemoteEndPoint.Address, serverAllowNoEncryption.RemoteEndPoint.Port);

                using (var sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.NoEncryption))
                {
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
                    // null encryption is not permitted with Tls13
                    await sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);
#pragma warning restore SYSLIB0039
                    _log.WriteLine("Client authenticated to server({0}) with encryption cipher: {1} {2}-bit strength",
                        serverAllowNoEncryption.RemoteEndPoint, sslStream.CipherAlgorithm, sslStream.CipherStrength);

                    CipherAlgorithmType expected = CipherAlgorithmType.Null;
                    Assert.Equal(expected, sslStream.CipherAlgorithm);
                    Assert.Equal(0, sslStream.CipherStrength);
                }
            }
        }

        private static bool SupportsNullEncryption => TestConfiguration.SupportsNullEncryption;
    }
}
