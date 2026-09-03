using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    [RandomSeedRegister(typeof(double), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(decimal), int.MaxValue - 1)]
    internal class DecimalRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomDecimalConfigAttribute config = context.GetConfig<RandomDecimalConfigAttribute>();

            if (config != null)
            {
                if (typeDefine == typeof(double))
                    return RandomKit.NextDouble((double)config.MinValue, (double)config.MaxValue, config.DecimalPlace);

                return RandomKit.NextDecimal(
                    Convert.ToDecimal(config.MinValue),
                    Convert.ToDecimal(config.MaxValue),
                    config.DecimalPlace);
            }

            if (typeDefine == typeof(double))
                return RandomKit.NextDouble(decimalPlaces: 5);

            return RandomKit.NextDecimal(decimal.MinValue / 2, decimal.MaxValue / 2, 5);
        }
    }
}
