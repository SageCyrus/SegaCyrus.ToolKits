using System;
using System.Reflection;
using SegaCyrus.ToolKits.Model.Random.Model;
using SegaCyrus.ToolKits.Package.RandomPackage;

namespace SegaCyrus.ToolKits.Model.Random.Attributes
{
    /// <summary>
    /// 随机种子工具,可自定义随机方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class RandomPropertySeedAttribute : Attribute
    {
        /// <summary>
        /// 随机逻辑
        /// </summary>
        /// <param name="context">随机上下文</param>
        /// <param name="type">当前需要构造的数据类型</param>
        /// <param name="configAttrs">当前此属性被标记的随机配置</param>
        /// <param name="propertyInfo">当前属性信息</param>
        /// <returns>随机结果</returns>
        public virtual object GetRandomValue(RandomContext context, Type type, RandomConfigAttribute[] configAttrs, PropertyInfo propertyInfo)
        {
            RandomGenerateContext genContext = RandSeedProxy.Instance.CreateContext(context, configAttrs);
            return RandSeedProxy.Instance.GetPolicy(type).Generate(type, genContext);
        }
    }
}