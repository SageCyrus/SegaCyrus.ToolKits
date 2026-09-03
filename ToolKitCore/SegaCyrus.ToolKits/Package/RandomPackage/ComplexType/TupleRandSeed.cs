using System;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;

namespace SegaCyrus.ToolKits.Package.RandomPackage.ComplexType
{
    [RandomSeedRegister("System.Runtime.CompilerServices.ITuple", int.MaxValue - 1)]
    internal class TupleRandSeed : BaseRandSeed
    {
        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            Type[] typeArgs = typeDefine.GenericTypeArguments;
            object[] args = new object[typeArgs.Length];

            for (int i = 0; i < typeArgs.Length; i++)
                args[i] = context.GetPolicy(typeArgs[i]).Generate(typeArgs[i], context);

            return ReflectKit.CreateInstance(typeDefine, args);
        }
    }
}
