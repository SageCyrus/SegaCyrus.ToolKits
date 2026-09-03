using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;

namespace SegaCyrus.ToolKits.Package.RandomPackage
{
    /// <summary>
    /// 随机种子策略基类 
    /// </summary>
    public abstract class BaseRandSeed
    {
        /// <summary>
        /// 生成随机值。
        /// </summary>
        /// <param name="typeDefine">目标类型</param>
        /// <param name="context">生成上下文（含配置、死循环保护、策略工厂）</param>
        public abstract object Generate(Type typeDefine, RandomGenerateContext context);

        /// <summary>
        /// 填充复杂对象的所有可写属性。递归生成每个属性的随机值。
        /// </summary>
        protected virtual void FillProperties(object instance, Type typeDefine, RandomGenerateContext context,
            PropertyInfo[] targetProperties = null)
        {
            if (instance == null) return;

            PropertyInfo[] properties = targetProperties
                                        ?? typeDefine.GetProperties().Where(p => p.CanWrite).ToArray();

            foreach (PropertyInfo prop in properties)
            {
                string visitKey = string.Concat(typeDefine.FullName, "|", prop.PropertyType.FullName, "|", prop.Name);

                if (!context.TryEnter(visitKey))
                {
                    prop.SetValue(instance, ReflectKit.GetDefaultValue(prop.PropertyType), null);
                    continue;
                }

                object childValue = ResolvePropertyValue(prop, context);
                prop.SetValue(instance, childValue, null);
            }
        }

        /// <summary>
        /// 解析属性的随机值：优先使用 <see cref="RandomPropertySeedAttribute"/>，
        /// 否则委托给上下文中的策略工厂。
        /// </summary>
        protected virtual object ResolvePropertyValue(PropertyInfo propertyInfo, RandomGenerateContext context)
        {
            RandomPropertySeedAttribute specialSeed =
                (RandomPropertySeedAttribute)propertyInfo.GetCustomAttribute(typeof(RandomPropertySeedAttribute), true);

            RandomConfigAttribute[] configAttrs =
                (RandomConfigAttribute[])propertyInfo.GetCustomAttributes(typeof(RandomConfigAttribute), true);

            if (specialSeed != null)
            {
                return specialSeed.GetRandomValue(
                    context.RandomContext, propertyInfo.PropertyType, configAttrs, propertyInfo);
            }

            // 子属性使用新的 context（携带子属性的 configAttrs）
            RandomGenerateContext childContext = context.CreateChild(configAttrs);
            return context.GetPolicy(propertyInfo.PropertyType)
                .Generate(propertyInfo.PropertyType, childContext);
        }
    }
}