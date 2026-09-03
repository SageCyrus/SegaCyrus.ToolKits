using System;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Attributes.Configs;

namespace SegaCyrus.ToolKits.Package.RandomPackage.SimpleType
{
    /// <summary>
    /// 整数随机种子 V2 —— 统一处理所有整数类型。
    /// 无符号类型使用 ulong 中间表示，有符号类型使用 long。
    /// </summary>
    [RandomSeedRegister(typeof(byte), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(sbyte), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(char), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(Int16), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(Int32), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(Int64), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(UInt16), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(UInt32), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(UInt64), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(long), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(ulong), int.MaxValue - 1)]
#if NET8_0_OR_GREATER
    [RandomSeedRegister(typeof(Int128), int.MaxValue - 1)]
    [RandomSeedRegister(typeof(UInt128), int.MaxValue - 1)]
#endif
    internal class IntegerRandSeed : BaseRandSeed
    {
        private static readonly HashSet<Type> UnsignedTypes = new HashSet<Type>
        {
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong), typeof(UInt16), typeof(UInt32), typeof(UInt64)
        };

        public override object Generate(Type typeDefine, RandomGenerateContext context)
        {
            RandomRangeConfigAttribute config = context.GetConfig<RandomRangeConfigAttribute>();

            if (config != null)
                return GenerateWithRange(typeDefine, config);

            return GenerateDefault(typeDefine);
        }

        private static object GenerateWithRange(Type type, RandomRangeConfigAttribute config)
        {
            long min = Convert.ToInt64(config.MinValue);
            long max = Convert.ToInt64(config.MaxValue);

            if (UnsignedTypes.Contains(type))
            {
                if (min < 0) min = 0;
                if (max < 0) max = long.MaxValue;
            }

            long value = RandomKit.NextLong(min, max);
            return Convert.ChangeType(value, type);
        }

        private static object GenerateDefault(Type type)
        {
            if (type == typeof(byte)) 
                return (byte)RandomKit.NextInt(byte.MinValue, byte.MaxValue + 1);
            if (type == typeof(sbyte))
                return (sbyte)RandomKit.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);
            if (type == typeof(char)) 
                return (char)RandomKit.NextInt(char.MinValue, char.MaxValue + 1);
            if (type == typeof(short) || type == typeof(Int16))
                return (short)RandomKit.NextInt(short.MinValue, short.MaxValue + 1);
            if (type == typeof(int) || type == typeof(Int32))
                return RandomKit.NextInt(int.MinValue, int.MaxValue);
            if (type == typeof(long) || type == typeof(Int64))
                return RandomKit.NextLong(long.MinValue, long.MaxValue);
            if (type == typeof(ushort) || type == typeof(UInt16))
                return (ushort)RandomKit.NextInt(ushort.MinValue, ushort.MaxValue + 1);
            if (type == typeof(uint) || type == typeof(UInt32))
                return (uint)RandomKit.NextLong(uint.MinValue, uint.MaxValue);
            if (type == typeof(ulong) || type == typeof(UInt64))
                return (ulong)RandomKit.NextLong(0, long.MaxValue);

            return 0;
        }
    }
}