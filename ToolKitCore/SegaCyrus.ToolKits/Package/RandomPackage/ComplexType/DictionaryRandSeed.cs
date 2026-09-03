using System;
using System.Collections;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Extension.InvokeExtension;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.ComplexType
{
    [RandomSeedRegister(typeof(Dictionary<,>), int.MaxValue - 1)]
#if NET8_0_OR_GREATER
    [RandomSeedRegister(typeof(System.Collections.Frozen.FrozenDictionary<,>), int.MaxValue - 1)]
#endif
    internal class DictionaryRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomSetConfigAttribute attr = context.GetConfig<RandomSetConfigAttribute>();
            int count = attr != null ? attr.Count : 1;

            Type keyType = typeDefine.GenericTypeArguments[0];
            Type valueType = typeDefine.GenericTypeArguments[1];

            BaseRandSeed keySeed = context.GetPolicy(keyType);
            BaseRandSeed valueSeed = context.GetPolicy(valueType);

            var instance = (IDictionary)ReflectKit.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
            for (int i = 0; i < count; i++)
            {
                object key = keySeed.Generate(keyType, context);
                if (!instance.Contains(key))
                    instance.Add(key, valueSeed.Generate(valueType, context));
            }
            var genericType = typeDefine.GetGenericTypeDefinition();

            if (genericType == typeof(Dictionary<,>))
            {
                return instance;

            }
#if NET8_0_OR_GREATER
            else if (genericType == typeof(System.Collections.Frozen.FrozenDictionary<,>))
                return InvokeKit.CallGenericStaticMethod(typeof(System.Collections.Frozen.FrozenDictionary),
                    "ToFrozenDictionary",
                    new List<Type>() { keyType, valueType }, instance);
#endif
            return instance;
        }
    }
}
