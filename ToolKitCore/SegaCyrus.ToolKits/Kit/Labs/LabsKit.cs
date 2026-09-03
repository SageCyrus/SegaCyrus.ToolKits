using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SegaCyrus.ToolKits.Extension.CommonExtension;
using SegaCyrus.ToolKits.Model.Labs;
using SegaCyrus.ToolKits.Model.Labs.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SegaCyrus.ToolKits.Kit.Labs
{
    /// <summary>
    /// 实验性工具包
    /// </summary>
    [Obsolete("实验性工具包,慎重使用")]
    public class LabsKit
    {
        /// <summary>
        /// 按照JSON的方式对比
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">对比方</param>
        /// <param name="target">对比方</param>
        /// <returns></returns>
        [Obsolete("试验阶段")]
        [Description("其实吧...我觉得他没有用 但食之无味去之可惜")]
        public static JsonDifferenceModelResult CompareModelByJson<T>(T source, T target)
        {
            return CompareJson((JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source)),
                (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(target)));
        }

        /// <summary>
        /// 对比JSON的方式对比
        /// </summary>
        /// <param name="source">对比方</param>
        /// <param name="target">对比方</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Obsolete("试验阶段")]
        [Description("其实吧...我觉得他没有用 但食之无味去之可惜")]
        public static JsonDifferenceModelResult CompareJson(JToken source, JToken target)
        {
            var result = new JsonDifferenceModelResult();
            var stack = new Stack<Tuple<JToken, JToken>>();

            stack.Push(Tuple.Create(source, target));

            while (stack.NotEmpty())
            {
                var item = stack.Pop();
                if (item.Item1 == null && item.Item2 == null)
                    continue;
                var objSource = item.Item1;
                var objTarget = item.Item2;

                if (objSource == null)
                {
                    result.Append(new JsonDifferenceItemModelResult("", DifferenceTypeEnum.SourceNotExist,
                        targetValue: SerializeKit.GetJSON(objTarget)));
                    continue;
                }

                if (objTarget == null)
                {
                    result.Append(new JsonDifferenceItemModelResult("", DifferenceTypeEnum.SourceNotExist,
                        SerializeKit.GetJSON(objSource)));
                    continue;
                }

                var onlySourceProperty = new HashSet<string>();
                var onlyTargetProperty = new HashSet<string>();

                foreach (var itemSource in objSource)
                {
                    onlySourceProperty.Add(itemSource.Path);
                }

                foreach (var itemTarget in objTarget)
                {
                    onlyTargetProperty.Add(itemTarget.Path);
                }

                var commonProperty = onlySourceProperty.Intersect(onlyTargetProperty).ToHashSet();
                onlySourceProperty = onlySourceProperty.Except(commonProperty).ToHashSet();
                onlyTargetProperty = onlyTargetProperty.Except(commonProperty).ToHashSet();
                if (onlySourceProperty.NotEmpty())
                {
                    foreach (var onlySourceItem in onlySourceProperty)
                    {
                        result.Append(new JsonDifferenceItemModelResult(onlySourceItem,
                            DifferenceTypeEnum.TargetNotExist,
                            SerializeKit.GetJSON(source.SelectToken(onlySourceItem))));
                    }
                }

                if (onlyTargetProperty.NotEmpty())
                {
                    foreach (var onlyTargetItem in onlyTargetProperty)
                    {
                        result.Append(new JsonDifferenceItemModelResult(onlyTargetItem,
                            DifferenceTypeEnum.SourceNotExist,
                            targetValue: SerializeKit.GetJSON(target.SelectToken(onlyTargetItem))));
                    }
                }

                foreach (var commonPropertyItem in commonProperty)
                {
                    var childSourceItemJToken = source.SelectToken(commonPropertyItem);
                    var childTargetItemJToken = target.SelectToken(commonPropertyItem);

                    var typeChildSource = childSourceItemJToken.GetType();
                    var typeChildTarget = childTargetItemJToken.GetType();

                    if (typeChildSource.GUID != typeChildTarget.GUID)
                    {
                        result.Append(new JsonDifferenceItemModelResult(commonPropertyItem,
                            DifferenceTypeEnum.TypeDifference,
                            SerializeKit.GetJSON(childSourceItemJToken), SerializeKit.GetJSON(childTargetItemJToken),
                            "对比双方值类型不一致"));
                    }

                    if (typeChildSource.GUID == typeof(JValue).GUID)
                    {
                        //JTokenType
                        if (childSourceItemJToken.Type == childTargetItemJToken.Type)
                        {
                            bool compareResult;
                            switch (childSourceItemJToken.Type)
                            {
                                case JTokenType.Integer:
                                    compareResult = childSourceItemJToken.Value<long>() ==
                                                    childTargetItemJToken.Value<long>();
                                    break;
                                case JTokenType.Float:
                                    compareResult = Math.Abs(childSourceItemJToken.Value<float>() -
                                                             childTargetItemJToken.Value<float>()) < 1e-6;
                                    break;
                                case JTokenType.String:
                                    compareResult = childSourceItemJToken.Value<string>() ==
                                                    childTargetItemJToken.Value<string>();
                                    break;
                                case JTokenType.Boolean:
                                    compareResult = childSourceItemJToken.Value<bool>() ==
                                                    childTargetItemJToken.Value<bool>();
                                    break;
                                case JTokenType.Null:
                                    compareResult = true;
                                    break;
                                case JTokenType.Date:
                                    compareResult = childSourceItemJToken.Value<DateTime>() ==
                                                    childTargetItemJToken.Value<DateTime>();
                                    break;
                                case JTokenType.Guid:
                                    compareResult = childSourceItemJToken.Value<Guid>() ==
                                                    childTargetItemJToken.Value<Guid>();
                                    break;
                                default:
                                    // case JTokenType.Bytes:
                                    // case JTokenType.Uri:
                                    // case JTokenType.TimeSpan:
                                    throw new NotImplementedException("没见过这种数据，见到在实现，保留报错数据便于后续测试");
                            }

                            if (false == compareResult)
                            {
                                result.Append(new JsonDifferenceItemModelResult(commonPropertyItem,
                                    DifferenceTypeEnum.ValueDifference,
                                    SerializeKit.GetJSON(childSourceItemJToken),
                                    SerializeKit.GetJSON(childTargetItemJToken), "对比双方数据不一致"));
                            }
                        }
                        else
                        {
                            result.Append(new JsonDifferenceItemModelResult(commonPropertyItem,
                                DifferenceTypeEnum.TypeDifference,
                                SerializeKit.GetJSON(childSourceItemJToken),
                                SerializeKit.GetJSON(childTargetItemJToken), "对比双方值类型不一致"));
                        }

                        continue;
                    }

                    if (typeChildSource.GUID == typeof(JArray).GUID)
                    {
                        var childSourceItemJArray = childSourceItemJToken.Children().ToList();
                        var childTargetItemJArray = childTargetItemJToken.Children().ToList();
                        var count = Math.Max(childSourceItemJArray.Count, childTargetItemJArray.Count);
                        for (var i = 0; i < count; ++i)
                        {
                            var src = i < childSourceItemJArray.Count ? childSourceItemJArray[i] : null;
                            var tag = i < childTargetItemJArray.Count ? childTargetItemJArray[i] : null;
                            stack.Push(Tuple.Create(src, tag));
                        }

                        continue;
                    }

                    stack.Push(Tuple.Create(childSourceItemJToken,
                        childTargetItemJToken));
                }
            }

            return result;
        }
    }
}