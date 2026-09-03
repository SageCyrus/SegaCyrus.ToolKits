using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    [RandomSeedRegister(typeof(Enum), int.MaxValue - 1)]
    internal class EnumRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            Array values = Enum.GetValues(typeDefine);
            if (values.Length == 0)
                return Activator.CreateInstance(typeDefine);

            return values.GetValue(RandomKit.NextInt(0, values.Length));
        }
    }
}
