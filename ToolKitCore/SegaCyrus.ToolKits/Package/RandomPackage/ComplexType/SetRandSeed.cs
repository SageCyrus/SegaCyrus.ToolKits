using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;
using System;
using System.Collections.Generic;

namespace SegaCyrus.ToolKits.Package.RandomPackage.ComplexType
{
    [RandomSeedRegister(typeof(HashSet<>), int.MaxValue - 1)]
#if NET8_0_OR_GREATER
    [RandomSeedRegister(typeof(System.Collections.Frozen.FrozenSet<>), int.MaxValue - 1)]
#endif
    internal class SetRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomSetConfigAttribute attr = context.GetConfig<RandomSetConfigAttribute>();
            int count = attr != null ? attr.Count : 1;

            Type elementType = typeDefine.GenericTypeArguments[0];
            BaseRandSeed seed = context.GetPolicy(elementType);
            var hashType = typeof(HashSet<>);
            object instance = ReflectKit.CreateInstance(hashType.MakeGenericType(elementType));
            var addMethod = instance.GetType().GetMethod("Add");


            if (addMethod != null)
            {
                for (int i = 0; i < count; i++)
                    addMethod.Invoke(instance, new object[] { seed.Generate(elementType, context) });
            }

            var genericType = typeDefine.GetGenericTypeDefinition();

            if (genericType == hashType)
            {
                return instance;

            }
#if NET8_0_OR_GREATER
            else if (genericType == typeof(System.Collections.Frozen.FrozenSet<>))
                return InvokeKit.CallGenericStaticMethod(typeof(System.Collections.Frozen.FrozenSet),
                    "ToFrozenSet", new List<Type>() { elementType }, instance);
#endif

            return instance;
        }
    }
}
