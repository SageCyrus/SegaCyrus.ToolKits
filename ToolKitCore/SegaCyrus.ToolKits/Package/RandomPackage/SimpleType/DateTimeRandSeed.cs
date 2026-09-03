using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    [RandomSeedRegister(typeof(DateTime), int.MaxValue - 1)]
    internal class DateTimeRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomDateTimeConfigAttribute config = context.GetConfig<RandomDateTimeConfigAttribute>();
            if (config != null)
                return RandomKit.NextDateTime((DateTime)config.MinValue, (DateTime)config.MaxValue);

            return RandomKit.NextDateTime();
        }
    }
}