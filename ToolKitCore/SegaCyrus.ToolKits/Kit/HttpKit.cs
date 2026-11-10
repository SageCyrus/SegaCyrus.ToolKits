using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// Http请求工具包
    /// </summary>
    public class HttpKit
    {

        /// <summary>
        /// HTTP请求处理委托
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="response">HTTP请求</param>
        /// <returns>处理结果</returns>
        public delegate Task<T> HTTPMessageHandle<T>(HttpResponseMessage response);

        /// <summary>
        /// HTTP请求处理委托（读取字节，确保状态码为200）
        /// </summary>
        /// <param name="response">响应体</param>
        /// <returns>响应体字节</returns>
        public static async Task<byte[]> HTTPDelegateReadBytesEnsure200(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// HTTP请求处理委托（读取字符串，确保状态码为200）
        /// </summary>
        /// <param name="response">响应体</param>
        /// <returns>响应体字符串</returns>
        public static async Task<string> HTTPDelegateReadStringEnsure200(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// HTTP请求处理委托（读取JSON并反序列化，确保状态码为200）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">响应体</param>
        /// <returns>反序列化模型</returns>
        public static async Task<T> HTTPDelegateReadJsonEnsure200<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return SerializeKit.DeserializeJSON<T>(result);
        }

        private static readonly HttpClient SharedHttpClient = new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpGetString(string url, Dictionary<string, string> head = null, int timeout = -1)
            => HttpGetStringAsync(url, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Get请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpGetJson<T>(string url, Dictionary<string, string> head = null, int timeout = -1)
            => HttpGetJsonAsync<T>(url, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpGetBytes(string url, Dictionary<string, string> head = null, int timeout = -1)
            => HttpGetBytesAsync(url, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Get 请求（异步）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpGetBytesAsync(string url, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpGetAsync<byte[]>(url, head, HTTPDelegateReadBytesEnsure200, timeout);
        }

        /// <summary>
        /// Http Get 请求（异步）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpGetStringAsync(string url, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpGetAsync<string>(url, head, HTTPDelegateReadStringEnsure200, timeout);
        }

        /// <summary>
        /// Http Get 请求（异步，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpGetJsonAsync<T>(string url, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpGetAsync<T>(url, head, HTTPDelegateReadJsonEnsure200<T>, timeout);
        }


        /// <summary>
        /// Http Post请求（请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpPostString(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPostStringAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Post请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpPostJson<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPostJsonAsync<T>(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpPostBytes(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPostBytesAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpPostBytesAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPostAsync<byte[]>(url, json, head, HTTPDelegateReadBytesEnsure200, timeout);
        }

        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpPostJsonAsync<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPostAsync<T>(url, json, head, HTTPDelegateReadJsonEnsure200<T>, timeout);
        }

        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpPostStringAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPostAsync<string>(url, json, head, HTTPDelegateReadStringEnsure200, timeout);
        }

        /// <summary>
        /// Http Delete请求（请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpDeleteString(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpDeleteStringAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Delete请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpDeleteJson<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpDeleteJsonAsync<T>(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Delete请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpDeleteBytes(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpDeleteBytesAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Delete 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpDeleteBytesAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpDeleteAsync<byte[]>(url, head, HTTPDelegateReadBytesEnsure200, timeout);
        }

        /// <summary>
        /// Http Delete 请求（异步，请求体为 JSON，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpDeleteJsonAsync<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpDeleteAsync<T>(url, head, HTTPDelegateReadJsonEnsure200<T>, timeout);
        }

        /// <summary>
        /// Http Delete 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpDeleteStringAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpDeleteAsync<string>(url, head, HTTPDelegateReadStringEnsure200, timeout);
        }



        /// <summary>
        /// Http Put请求（请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpPutString(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPutStringAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Put请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpPutJson<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPutJsonAsync<T>(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Put请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpPutBytes(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPutBytesAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Put 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpPutBytesAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPutAsync<byte[]>(url, json, head, HTTPDelegateReadBytesEnsure200, timeout);
        }

        /// <summary>
        /// Http Put 请求（异步，请求体为 JSON，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpPutJsonAsync<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPutAsync<T>(url, json, head, HTTPDelegateReadJsonEnsure200<T>, timeout);
        }

        /// <summary>
        /// Http Put 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpPutStringAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPutAsync<string>(url, json, head, HTTPDelegateReadStringEnsure200, timeout);
        }

#if NET6_0_OR_GREATER

        /// <summary>
        /// Http Petch请求（请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpPetchString(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPetchStringAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Petch请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpPetchJson<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPetchJsonAsync<T>(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Petch请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpPetchBytes(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
            => HttpPetchBytesAsync(url, json, head, timeout).GetAwaiter().GetResult();

        /// <summary>
        /// Http Petch 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpPetchBytesAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPetchAsync<byte[]>(url, json, head, HTTPDelegateReadBytesEnsure200, timeout);
        }

        /// <summary>
        /// Http Petch 请求（异步，请求体为 JSON，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpPetchJsonAsync<T>(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPetchAsync<T>(url, json, head, HTTPDelegateReadJsonEnsure200<T>, timeout);
        }

        /// <summary>
        /// Http Petch 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpPetchStringAsync(string url, string json, Dictionary<string, string> head = null, int timeout = -1)
        {
            return await HttpPetchAsync<string>(url, json, head, HTTPDelegateReadStringEnsure200, timeout);
        }
#endif

        /// <summary>
        /// Http Get 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpGetAsync<T>(string url, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle, int timeout = -1)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                ApplyRequestHeaders(request, head, url);
                if (timeout > 0)
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                    {
                        using (var response = await SharedHttpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
                        {
                            return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Http Head 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpHeadAsync<T>(string url, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle, int timeout = -1)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                ApplyRequestHeaders(request, head, url);
                if (timeout > 0)
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                    {
                        using (var response = await SharedHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false))
                        {
                            return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    using (var response = await SharedHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpPostAsync<T>(string url, string json, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle, int timeout = -1)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));
            AssertKit.AssertNotNull(json, nameof(json));

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                ApplyRequestHeaders(request, head, url);
                if (timeout > 0)
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                    {
                        using (var response = await SharedHttpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
                        {
                            return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                    }
                }
            }
        }


#if NET6_0_OR_GREATER
        /// <summary>
        /// Http Petch 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpPetchAsync<T>(string url, string json, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle, int timeout = -1)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));
            AssertKit.AssertNotNull(json, nameof(json));

            using (var request = new HttpRequestMessage(HttpMethod.Patch, url))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                ApplyRequestHeaders(request, head, url);
                if (timeout > 0)
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                    {
                        using (var response = await SharedHttpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
                        {
                            return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Http Delete 请求（异步）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpDeleteAsync<T>(string url, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle, int timeout = -1)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));

            using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                ApplyRequestHeaders(request, head, url);
                if (timeout > 0)
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                    {
                        using (var response = await SharedHttpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
                        {
                            return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Http Put 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <param name="timeout">超时时间/毫秒</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpPutAsync<T>(string url, string json, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle, int timeout = -1)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));
            AssertKit.AssertNotNull(json, nameof(json));

            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                ApplyRequestHeaders(request, head, url);
                if (timeout > 0)
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
                    {
                        using (var response = await SharedHttpClient.SendAsync(request, cts.Token).ConfigureAwait(false))
                        {
                            return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// 将自定义请求头应用到 HttpRequestMessage。
        /// 头部设置在请求消息上而非共享 HttpClient 实例上，保证多线程安全。
        /// </summary>
        private static void ApplyRequestHeaders(HttpRequestMessage request, Dictionary<string, string> head, string url)
        {
            if (head == null || head.Count == 0)
                return;

            foreach (var header in head)
            {
                if (string.IsNullOrWhiteSpace(header.Value))
                    continue;

                if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    request.Headers.Host = new Uri(url).Host;
                }
                else
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }
    }
}
