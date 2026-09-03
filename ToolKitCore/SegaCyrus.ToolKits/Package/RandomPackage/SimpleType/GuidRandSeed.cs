using System;
using SegaCyrus.ToolKits.Model.Random.Attributes;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    [RandomSeedRegister(typeof(Guid), int.MaxValue - 1)]
    internal class GuidRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            return Guid.NewGuid();
        }
    }
}
