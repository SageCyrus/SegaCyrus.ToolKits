using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
            var result =  await response.Content.ReadAsStringAsync();
            return SerializeKit.DeserializeJSON<T>(result);
        }


        private static readonly HttpClient SharedHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpGetString(string url, Dictionary<string, string> head = null)
            => HttpGetStringAsync(url, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Post请求（请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字符串）</returns>
        public static string HttpPostString(string url, string json, Dictionary<string, string> head = null)
            => HttpPostStringAsync(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Get 请求（异步）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpGetStringAsync(string url, Dictionary<string, string> head = null)
        {
            return await HttpGetAsync<string>(url, head, HTTPDelegateReadStringEnsure200);
        }

        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字符串）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<string> HttpPostStringAsync(string url, string json, Dictionary<string, string> head = null)
        {
            return await HttpPostAsync<string>(url, json, head, HTTPDelegateReadStringEnsure200);
        }
        /// <summary>
        /// Http Get请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpGetJson<T>(string url, Dictionary<string, string> head = null)
            => HttpGetJsonAsync<T>(url, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Post请求（响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static T HttpPostJson<T>(string url, string json, Dictionary<string, string> head = null)
            => HttpPostJsonAsync<T>(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Get 请求（异步，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpGetJsonAsync<T>(string url, Dictionary<string, string> head = null)
        {
            return await HttpGetAsync<T>(url, head, HTTPDelegateReadJsonEnsure200<T>);
        }


        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON，响应体反序列化为 JSON 对象）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <returns>反序列化后的对象；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<T> HttpPostJsonAsync<T>(string url, string json, Dictionary<string, string> head = null)
        {
            return await HttpPostAsync<T>(url, json, head, HTTPDelegateReadJsonEnsure200<T>);
        }

        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpGetBytes(string url, Dictionary<string, string> head = null)
            => HttpGetBytesAsync(url, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字节）</returns>
        public static byte[] HttpPostBytes(string url, string json, Dictionary<string, string> head = null)
            => HttpPostBytesAsync(url, json, head).GetAwaiter().GetResult();

        /// <summary>
        /// Http Get 请求（异步）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpGetBytesAsync(string url, Dictionary<string, string> head = null)
        {
            return await HttpGetAsync<byte[]>(url, head, HTTPDelegateReadBytesEnsure200);
        }


        /// <summary>
        /// Http Post 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="json">请求体(JSON)</param>
        /// <param name="head">请求头</param>
        /// <returns>请求结果（响应体字节）；非 2xx 状态码会抛 HttpRequestException</returns>
        public static async Task<byte[]> HttpPostBytesAsync(string url, string json, Dictionary<string, string> head = null)
        {
            return await HttpPostAsync<byte[]>(url, json, head, HTTPDelegateReadBytesEnsure200);
        }

        /// <summary>
        /// Http Get 请求（异步，请求体为 JSON）
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="head">请求头</param>
        /// <param name="httpMessagehandle">处理方法</param>
        /// <returns>返回结果</returns>
        public static async Task<T> HttpGetAsync<T>(string url, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                ApplyRequestHeaders(request, head, url);
                using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                {
                    return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
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
        /// <returns>返回结果</returns>
        public static async Task<T> HttpPostAsync<T>(string url, string json, Dictionary<string, string> head, HTTPMessageHandle<T> httpMessagehandle)
        {
            AssertKit.AssertNotEmpty(url, nameof(url));
            AssertKit.AssertNotNull(json, nameof(json));

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                ApplyRequestHeaders(request, head, url);
                using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                {
                    return await httpMessagehandle.Invoke(response).ConfigureAwait(false);
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
