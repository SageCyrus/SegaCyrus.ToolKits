using System;
using System.IO;
using System.Runtime.CompilerServices;
//using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// 反序列化工具包
    /// </summary>
    public class SerializeKit
    {
        ///// <summary>
        ///// 获取格式化后序列化串
        ///// </summary>
        ///// <typeparam name="T">待序列化的类型</typeparam>
        ///// <param name="obj">待序列化数据</param>
        ///// <param name="spaceCount">格式化空格数量</param>
        ///// <returns>序列化内容</returns>
        //public static string GetFormatDataContract<T>(T obj, int spaceCount = 4)
        //{
        //    DataContractSerializer datacontractSerializer = new DataContractSerializer(typeof(T));
        //    StringBuilder stringBuilder = new StringBuilder();
        //    var settings = new XmlWriterSettings();
        //    settings.Indent = true;
        //    settings.OmitXmlDeclaration = true;
        //    settings.IndentChars = new string(' ', spaceCount);
        //    settings.NewLineOnAttributes = true;
        //    using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
        //    {
        //        datacontractSerializer.WriteObject(xmlWriter, obj);
        //    }
        //    return stringBuilder.ToString();
        //}
        ///// <summary>
        ///// DataContract序列化
        ///// </summary>
        ///// <typeparam name="T">类型</typeparam>
        ///// <param name="data">待处理数据</param>
        ///// <returns>序列化结果</returns>
        //public static string GetDataContract<T>(T data)
        //{
        //    DataContractSerializer datacontractSerializer = new DataContractSerializer(typeof(T));
        //    XmlDocument doc = new XmlDocument();
        //    StringBuilder stringBuilder = new StringBuilder();
        //    using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
        //    {
        //        datacontractSerializer.WriteObject(xmlWriter, data);
        //    }
        //    return stringBuilder.ToString();
        //}

        /// <summary>
        /// 获取未格式化的JSON串
        /// </summary>
        /// <typeparam name="T">待序列化的类型</typeparam>
        /// <param name="obj">待序列化数据</param>
        /// <returns>序列化后的JSON串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetJSON<T>(T obj)
        {
#if NET6_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Serialize(obj);
#else
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
#endif
        }

        /// <summary>
        /// 获取未格式化的XML串
        /// </summary>
        /// <typeparam name="T">待序列化的类型</typeparam>
        /// <param name="obj">待序列化数据</param>
        /// <returns>序列化后的XML串</returns>
        public static string GetXML<T>(T obj)
        {
            using (var sw = new StringWriter())
            {
                var xz = new XmlSerializer(obj.GetType());
                xz.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        /// <summary>
        /// 获取格式化后的的XML串
        /// </summary>
        /// <typeparam name="T">待序列化的类型</typeparam>
        /// <param name="obj">待序列化数据</param>
        /// <param name="spaceCount">格式化空格数量</param>
        /// <returns>序列化后的XML串</returns>
        public static string GetFormatXML<T>(T obj, int spaceCount = 4)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var buff = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.IndentChars = new string(' ', spaceCount);
            settings.NewLineOnAttributes = true;

            //serialize
            using (var xmlTextWriter = XmlWriter.Create(buff, settings))
            {
                serializer.Serialize(xmlTextWriter, obj);
                return buff.ToString();
            }
        }

        /// <summary>
        /// 获取格式化后的的JSON串
        /// </summary>
        /// <typeparam name="T">待序列化的类型</typeparam>
        /// <param name="obj">待序列化数据</param>
        /// <param name="spaceCount">格式化空格数量</param>
        /// <returns>序列化后的JSON串</returns>
        public static string GetFormatJSON<T>(T obj, int spaceCount = 4)
        {
#if NET6_0_OR_GREATER
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };

            return System.Text.Json.JsonSerializer.Serialize(obj, options);
#else
            var serializer = new Newtonsoft.Json.JsonSerializer();
            if (obj != null)
            {
                var textWriter = new StringWriter();
                var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter)
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Indentation = spaceCount, //缩进字符数
                    IndentChar = ' ' //缩进字符
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }

            return string.Empty;
#endif
        }

        /// <summary>
        /// 反序列化XML串
        /// </summary>
        /// <typeparam name="T">待反序列化的类型</typeparam>
        /// <param name="xml">XML串</param>
        /// <returns>反序列化后的强模型</returns>
        public static T DeserializeXML<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var desXml = new XmlSerializer(typeof(T));
                return (T)desXml.Deserialize(sr);
            }
        }

        /// <summary>
        /// 反序列化JSON串
        /// </summary>
        /// <typeparam name="T">待反序列化的类型</typeparam>
        /// <param name="json">JSON串</param>
        /// <returns>反序列化后的强模型</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DeserializeJSON<T>(string json)
        {
#if NET6_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
#else
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
#endif
        }

        ///// <summary>
        ///// 反序列化DataContract串
        ///// </summary>
        ///// <typeparam name="T">待反序列化的类型</typeparam>
        ///// <param name="dataContract">dataContract串</param>
        ///// <returns>反序列化后的强模型</returns>
        //public static T DeserializeDataContract<T>(string dataContract)
        //{
        //    var datacontractSerializer = new DataContractSerializer(typeof(T));
        //    using (var textReader = new StringReader(dataContract))
        //    {
        //        using (var xmlReader = XmlReader.Create(textReader))
        //        {
        //            return (T)datacontractSerializer.ReadObject(xmlReader);
        //        }
        //    }
        //}

        /// <summary>
        /// 反序列化XML串
        /// </summary>
        /// <param name="xml">XML串</param>
        /// <param name="type">待反序列化的类型</param>
        /// <returns>反序列化后的强模型</returns>
        public static object DeserializeXML(string xml, Type type)
        {
            using (var sr = new StringReader(xml))
            {
                var desXml = new XmlSerializer(type);
                return desXml.Deserialize(sr);
            }
        }

        /// <summary>
        /// 反序列化JSON串
        /// </summary>
        /// <param name="json">JSON串</param>
        /// <param name="type">待反序列化的类型</param>
        /// <returns>反序列化后的强模型</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object DeserializeJSON(string json, Type type)
        {
#if NET6_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Deserialize(json, type);
#else
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
#endif
        }

        ///// <summary>
        ///// 反序列化DataContract串
        ///// </summary>
        ///// <param name="dataContract">dataContract串</param>
        ///// <param name="type">待反序列化的类型</param>
        ///// <returns>反序列化后的强模型</returns>
        //public static object DeserializeDataContract(string dataContract,Type type)
        //{
        //    var datacontractSerializer = new DataContractSerializer(type);
        //    using (var textReader = new StringReader(dataContract))
        //    {
        //        using (var xmlReader = XmlReader.Create(textReader))
        //        {
        //            return datacontractSerializer.ReadObject(xmlReader);
        //        }
        //    }
        //}
    }
}