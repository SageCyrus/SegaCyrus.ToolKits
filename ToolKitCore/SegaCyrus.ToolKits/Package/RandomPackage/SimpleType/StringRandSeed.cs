using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    [RandomSeedRegister(typeof(string), int.MaxValue - 1)]
    internal class StringRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomStringConfigAttribute config = context.GetConfig<RandomStringConfigAttribute>();

            if (config != null)
            {
                string pool = config.Complex
                    ? Common.Pool.ConstCharPools.AllPrintable
                    : Common.Pool.ConstCharPools.AlphaNumeric;
                return RandomKit.NextString((int)config.MinValue, (int)config.MaxValue, pool);
            }

            return RandomKit.NextString(10, 30);
        }
    }
}
