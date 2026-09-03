using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.ComplexType
{
    /// <summary>
    /// 默认复杂对象随机种子 —— 兜底策略，通过反射填充所有可写属性。
    /// 注册 typeof(object) 作为兜底，匹配所有未精确命中的类型。
    /// </summary>
    [RandomSeedRegister(typeof(object))]
    internal class DefaultRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            if (typeDefine == typeof(object))
                return GenerateObjectValue(context);

            object instance = ReflectKit.CreateInstanceWithDefaultValue(typeDefine);
            FillProperties(instance, typeDefine, context);
            return instance;
        }

        private object GenerateObjectValue(RandomGenerateContext context)
        {
            RandomObjectTypeConfigAttribute attr = context.GetConfig<RandomObjectTypeConfigAttribute>();

            Type[] candidateTypes = (attr != null && attr.MinValue is Type[])
                ? (Type[])attr.MinValue
                : null;

            if (candidateTypes == null || candidateTypes.Length == 0)
                candidateTypes = new Type[] { typeof(string) };

            Type chosenType = RandomKit.NextItem(candidateTypes);
            return context.GetPolicy(chosenType).Generate(chosenType, context);
        }
    }
}
