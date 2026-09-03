using System;
using System.Collections;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.ComplexType
{
    [RandomSeedRegister(typeof(IList), int.MaxValue - 1)]
    internal class ListRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomSetConfigAttribute attr = context.GetConfig<RandomSetConfigAttribute>();
            int count = attr != null ? attr.Count : 1;

            if (typeDefine.IsArray)
                return GenerateArray(typeDefine, count, context);

            return GenerateList(typeDefine, count, context);
        }

        private static object GenerateArray(Type typeDefine, int count, RandomGenerateContext context)
        {
            Type elementType = typeDefine.GetElementType();
            BaseRandSeed seed = context.GetPolicy(elementType);

            Array instance = (Array)ReflectKit.CreateInstance(typeDefine, new object[] { count });
            for (int i = 0; i < count; i++)
                instance.SetValue(seed.Generate(elementType, context), i);

            return instance;
        }

        private static object GenerateList(Type typeDefine, int count, RandomGenerateContext context)
        {
            Type elementType = typeDefine.GenericTypeArguments[0];
            BaseRandSeed seed = context.GetPolicy(elementType);

            IList instance = (IList)ReflectKit.CreateInstance(typeDefine);
            for (int i = 0; i < count; i++)
                instance.Add(seed.Generate(elementType, context));

            return instance;
        }
    }
}
