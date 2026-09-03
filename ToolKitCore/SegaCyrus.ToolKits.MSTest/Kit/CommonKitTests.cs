using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SegaCyrus.ToolKits.Kit;

namespace SegaCyrus.ToolKits.MSTest.Kit
{
    /// <summary>
    /// <see cref="CommonKit"/> 单元测试
    /// </summary>
    [TestClass]
    public class CommonKitTests
    {
        #region ExecuteWithoutException

        [TestMethod]
        public void ExecuteWithoutException_NoException_ReturnsValue()
        {
            var result = CommonKit.ExecuteWithoutException(() => 100);
            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void ExecuteWithoutException_WithException_ReturnsDefaultValue()
        {
            var result = CommonKit.ExecuteWithoutException<int>(() => throw new InvalidOperationException("boom"), -1);
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ExecuteWithoutException_WithException_ReturnsTypeDefault()
        {
            var result = CommonKit.ExecuteWithoutException<int>(() => throw new InvalidOperationException());
            Assert.AreEqual(0, result);
            var refResult = CommonKit.ExecuteWithoutException<string>(() => throw new InvalidOperationException());
            Assert.IsNull(refResult);
        }

        [TestMethod]
        public void ExecuteWithoutException_Action_NoException_DoesNotThrow()
        {
            var flag = false;
            CommonKit.ExecuteWithoutException<int>(() => flag = true);
            Assert.IsTrue(flag);
        }

        [TestMethod]
        public void ExecuteWithoutException_Action_WithException_IsIgnored()
        {
            CommonKit.ExecuteWithoutException<int>(() => throw new NotSupportedException("ignore me"));
        }

        #endregion

        #region CallTrace

        [TestMethod]
        public void CallTrace_ReturnsFormattedStack()
        {
            var trace = CommonKit.CallTrace();
            Assert.IsNotNull(trace);
            StringAssert.Contains(trace, "->");
        }

        #endregion

        #region JSON / XML 文件

        private static string NewTempPath(string ext)
        {
            var path = Path.Combine(Path.GetTempPath(), $"CommonKit_{Guid.NewGuid():N}{ext}");
            return path;
        }

        [TestMethod]
        public void WriteJSONFile_ReadJSONFile_RoundTrip()
        {
            var path = NewTempPath(".json");
            try
            {
                var sample = new XmlSample { Id = 10, Name = "round-json" };
                CommonKit.WriteJSONFile(path, sample);
                Assert.IsTrue(File.Exists(path));
                var restored = CommonKit.ReadJSONFile<XmlSample>(path);
                Assert.AreEqual(10, restored.Id);
                Assert.AreEqual("round-json", restored.Name);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [TestMethod]
        public void WriteXMLFile_ReadXMLFile_RoundTrip()
        {
            var path = NewTempPath(".xml");
            try
            {
                var sample = new XmlSample { Id = 20, Name = "round-xml" };
                CommonKit.WriteXMLFile(path, sample);
                Assert.IsTrue(File.Exists(path));
                var restored = CommonKit.ReadXMLFile<XmlSample>(path);
                Assert.AreEqual(20, restored.Id);
                Assert.AreEqual("round-xml", restored.Name);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [TestMethod]
        public void WriteJSONFile_EmptyPath_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => CommonKit.WriteJSONFile(string.Empty, new XmlSample()));
        }

        [TestMethod]
        public void ReadJSONFile_MissingFile_Throws()
        {
            Assert.ThrowsExactly<FileNotFoundException>(
                () => CommonKit.ReadJSONFile<XmlSample>(Path.Combine(Path.GetTempPath(), "no_such_file_12345.json")));
        }

        #endregion

        #region Http

        private sealed class LoopbackHttpServer : IDisposable
        {
            private readonly TcpListener _listener;
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();
            private readonly byte[] _responseBody;
            private readonly string _contentType;
            private readonly int _statusCode;
            public string Url { get; }

            public LoopbackHttpServer(byte[] responseBody, string contentType = "text/plain",
                int statusCode = 200, bool captureRequest = false)
            {
                _responseBody = responseBody;
                _contentType = contentType;
                _statusCode = statusCode;
                _listener = new TcpListener(IPAddress.Loopback, 0);
                _listener.Start();
                var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                Url = $"http://127.0.0.1:{port}/";
                _ = AcceptLoopAsync(_cts.Token);
            }

            private async Task AcceptLoopAsync(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client;
                    try
                    {
                        client = await _listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    _ = Task.Run(() => HandleAsync(client));
                }
            }

            private async Task HandleAsync(TcpClient client)
            {
                using (client)
                {
                    try
                    {
                        using var stream = client.GetStream();
                        // 读请求头（直到 \r\n\r\n）
                        var buffer = new byte[4096];
                        var builder = new StringBuilder();
                        while (true)
                        {
                            int n = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (n <= 0) return;
                            builder.Append(Encoding.ASCII.GetString(buffer, 0, n));
                            if (builder.ToString().Contains("\r\n\r\n")) break;
                        }
                        var reason = _statusCode == 200 ? "OK" : "Not Found";
                        var header = $"HTTP/1.1 {_statusCode} {reason}\r\n" +
                                     $"Content-Type: {_contentType}\r\n" +
                                     $"Content-Length: {_responseBody.Length}\r\n" +
                                     "Connection: close\r\n\r\n";
                        var headerBytes = Encoding.ASCII.GetBytes(header);
                        await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
                        await stream.WriteAsync(_responseBody, 0, _responseBody.Length);
                        await stream.FlushAsync();
                    }
                    catch
                    {
                        // 忽略客户端断连
                    }
                }
            }

            public void Dispose()
            {
                _cts.Cancel();
                _listener.Stop();
                _cts.Dispose();
            }
        }

        [TestMethod]
        public void HttpGet_WithServer_ReturnsBody()
        {
            var body = Encoding.UTF8.GetBytes("{\"key\":\"value\"}");
            using var server = new LoopbackHttpServer(body, "application/json");
            var result = HttpKit.HttpGetBytes(server.Url);
            Assert.IsNotNull(result);
            Assert.AreEqual("{\"key\":\"value\"}", Encoding.UTF8.GetString(result));
        }

        [TestMethod]
        public void HttpGet_WithHeaders_Works()
        {
            var body = Encoding.UTF8.GetBytes("ok");
            using var server = new LoopbackHttpServer(body);
            var result = HttpKit.HttpGetBytes(server.Url, new System.Collections.Generic.Dictionary<string, string>
            {
                ["Accept"] = "text/plain",
                ["X-Custom"] = "abc"
            });
            Assert.AreEqual("ok", Encoding.UTF8.GetString(result));
        }

        [TestMethod]
        public void HttpPost_WithServer_ReturnsBody()
        {
            var body = Encoding.UTF8.GetBytes("posted-ok");
            using var server = new LoopbackHttpServer(body);
            var result = HttpKit.HttpPostBytes(server.Url, "{\"a\":1}");
            Assert.AreEqual("posted-ok", Encoding.UTF8.GetString(result));
        }

        [TestMethod]
        public void HttpGet_ConnectionRefused_Throws()
        {
            // 关闭所有监听后访问未占用端口，连接被拒应抛出异常
            var port = 1;
            Assert.ThrowsExactly<System.Net.Http.HttpRequestException>(
                () => HttpKit.HttpGetBytes($"http://127.0.0.1:{port}/"));
        }

        #endregion
    }
}
