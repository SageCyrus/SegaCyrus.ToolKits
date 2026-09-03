using System;
using SegaCyrus.ToolKits.Model.Random.Attributes;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    [RandomSeedRegister(typeof(Nullable<>), int.MaxValue - 1)]
    internal class NullableRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            Type innerType = typeDefine.GenericTypeArguments[0];
            return context.GetPolicy(innerType).Generate(innerType, context);
        }
    }
}
