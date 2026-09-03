using SegaCyrus.ToolKits.Common.Pool;
using SegaCyrus.ToolKits.Package.RandomPackage;
using SegaCyrus.ToolKits.Package.RandomPackage.Builder;
using System;
using System.Collections.Generic;
#if !NET8_0_OR_GREATER
using System.Drawing;
using System.Drawing.Imaging;
#endif
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SegaCyrus.ToolKits.Kit
{
    /// <summary>
    /// RandomKit —— 随机数据构造工具包。
    /// </summary>
    public class RandomKit
    {

        #region 随机源（线程安全）

#if NET8_0_OR_GREATER
        private static Random Shared
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return System.Random.Shared; }
        }
#else
        [ThreadStatic] private static Random _threadRandom;

        private static Random Shared
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_threadRandom == null)
                    _threadRandom = new Random(Guid.NewGuid().GetHashCode());
                return _threadRandom;
            }
        }
#endif

        #endregion

        #region 数值

        /// <summary>
        /// 生成 [min, max) 范围内的随机整数。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextInt(int min = 0, int max = int.MaxValue)
        {
            AssertKit.AssertFalse(min >= max, "min >= max", "min must be less than max");
            return Shared.Next(min, max);
        }

        /// <summary>
        /// 生成 [min, max) 范围内的随机长整数。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NextLong(long min = 0, long max = long.MaxValue)
        {
            AssertKit.AssertFalse(min >= max, "min >= max", "min must be less than max");

#if NET8_0_OR_GREATER
            return Shared.NextInt64(min, max);
#else
            ulong range = (ulong)(max - min);
            if (range <= (ulong)uint.MaxValue)
                return min + (long)(range * Shared.NextDouble());

            byte[] buf = new byte[8];
            Shared.NextBytes(buf);
            long value = (long)(BitConverter.ToUInt64(buf, 0) % range);
            return min + value;
#endif
        }

        /// <summary>
        /// 生成 [min, max) 范围内的随机双精度浮点数。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NextDouble(double min = 0.0, double max = 1.0, int? decimalPlaces = null)
        {
            AssertKit.AssertFalse(min >= max, "min >= max", "min must be less than max");
            double value = min + Shared.NextDouble() * (max - min);
            return decimalPlaces.HasValue ? Math.Round(value, decimalPlaces.Value) : value;
        }

        /// <summary>
        /// 生成 [min, max) 范围内的随机单精度浮点数。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextFloat(float min = 0f, float max = 1f, int? decimalPlaces = null)
        {
            AssertKit.AssertFalse(min >= max, "min >= max", "min must be less than max");
            float value = min + (float)Shared.NextDouble() * (max - min);
            return decimalPlaces.HasValue ? (float)Math.Round(value, decimalPlaces.Value) : value;
        }

        /// <summary>
        /// 生成 [min, max) 范围内的随机 decimal。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal NextDecimal(decimal min = 0m, decimal max = 1m, int? decimalPlaces = null)
        {
            AssertKit.AssertFalse(min >= max, "min >= max", "min must be less than max");
            double range = (double)(max - min);
            decimal value = min + (decimal)(Shared.NextDouble() * range);
            return decimalPlaces.HasValue ? Math.Round(value, decimalPlaces.Value) : value;
        }

        #endregion

        #region 布尔

        /// <summary>
        /// 生成随机布尔值，可指定 true 的概率。
        /// </summary>
        /// <param name="trueProbability">返回 true 的概率，范围 [0.0, 1.0]，默认 0.5。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NextBool(double trueProbability = 0.5)
        {
            AssertKit.AssertFalse(trueProbability < 0 || trueProbability > 1,
                "trueProbability < 0 || trueProbability > 1",
                "trueProbability must be in [0, 1]");
            return Shared.NextDouble() < trueProbability;
        }

        #endregion

        #region 字符 & 字符串

        /// <summary>
        /// 从字符池中随机选取一个字符。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char NextChar(string pool = ConstCharPools.AlphaNumeric)
        {
            AssertKit.AssertNotEmpty(pool, nameof(pool));
            return pool[Shared.Next(pool.Length)];
        }

        /// <summary>
        /// 生成指定长度的随机字符串。
        /// </summary>
        /// <param name="length">字符串长度（固定）。</param>
        /// <param name="pool">字符池。</param>
        public static string NextString(int length, string pool = ConstCharPools.AlphaNumeric)
        {
            AssertKit.AssertPositive(length, nameof(length));
            AssertKit.AssertNotEmpty(pool, nameof(pool));

            if (length == 0) return string.Empty;

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = pool[Shared.Next(pool.Length)];
            return new string(chars);
        }

        /// <summary>
        /// 生成长度在 [minLength, maxLength) 范围内的随机字符串。
        /// </summary>
        public static string NextString(int minLength, int maxLength, string pool = ConstCharPools.AlphaNumeric)
        {
            AssertKit.AssertPositive(minLength, nameof(minLength));
            AssertKit.AssertFalse(minLength >= maxLength, "minLength >= maxLength",
                "minLength must be less than maxLength");
            int length = Shared.Next(minLength, maxLength);
            return NextString(length, pool);
        }

        /// <summary>
        /// 使用 <see cref="StringBuilder"/> 高效构建超长随机字符串。
        /// </summary>
        public static string NextLargeString(int length, string pool = ConstCharPools.AlphaNumeric)
        {
            AssertKit.AssertPositive(length, nameof(length));
            AssertKit.AssertNotEmpty(pool, nameof(pool));
            if (length == 0) return string.Empty;

            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(pool[Shared.Next(pool.Length)]);
            return sb.ToString();
        }

        /// <summary>
        /// 按自定义规则生成随机字符串。
        /// 规则格式：每个字符表示一种类型——'L'=大写, 'l'=小写, 'd'=数字, 's'=符号, 其他字符原样保留。
        /// <para>例如：<c>"LLLlddd"</c> 生成如 "ABCx789" 的字符串。</para>
        /// </summary>
        public static string NextStringByPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return string.Empty;

            StringBuilder sb = new StringBuilder(pattern.Length);
            foreach (char ch in pattern)
            {
                switch (ch)
                {
                    case 'L': sb.Append(NextChar(ConstCharPools.UpperLetters)); break;
                    case 'l': sb.Append(NextChar(ConstCharPools.LowerLetters)); break;
                    case 'd': sb.Append(NextChar(ConstCharPools.Digits)); break;
                    case 's': sb.Append(NextChar(ConstCharPools.Symbols)); break;
                    default: sb.Append(ch); break;
                }
            }

            return sb.ToString();
        }

        #endregion

        #region 枚举

        /// <summary>
        /// 从枚举类型中随机选取一个值。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NextEnum<T>() where T : struct
        {
            Array values = Enum.GetValues(typeof(T));
            if (values.Length == 0)
                throw new InvalidOperationException(string.Format("Enum type '{0}' has no values.", typeof(T).Name));
            return (T)values.GetValue(Shared.Next(values.Length));
        }

        /// <summary>
        /// 从枚举类型中随机选取一个已定义的值（排除组合标志等未命名值）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NextDefinedEnum<T>() where T : struct
        {
            Array allValues = Enum.GetValues(typeof(T));
            List<T> defined = new List<T>();
            foreach (T val in allValues)
            {
                if (Enum.IsDefined(typeof(T), val))
                    defined.Add(val);
            }

            if (defined.Count == 0)
                throw new InvalidOperationException(string.Format("Enum type '{0}' has no defined values.",
                    typeof(T).Name));
            return defined[Shared.Next(defined.Count)];
        }

        #endregion

        #region Guid

        /// <summary>
        /// 生成新的随机 Guid。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid NextGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// 生成指定格式的 Guid 字符串。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NextGuidString(string format = "D")
        {
            return Guid.NewGuid().ToString(format);
        }

        #endregion

        #region DateTime

        /// <summary>
        /// 生成 [min, max) 范围内的随机时间。
        /// </summary>
        public static DateTime NextDateTime(DateTime? min = null, DateTime? max = null)
        {
            DateTime minValue = min ?? DateTime.MinValue;
            DateTime maxValue = max ?? DateTime.MaxValue;
            AssertKit.AssertFalse(minValue >= maxValue, "minValue >= maxValue", "min must be less than max");

            long rangeTicks = (maxValue - minValue).Ticks;
            if (rangeTicks <= 0) return minValue;

            long randomTicks = NextLong(0, rangeTicks);
            return minValue.AddTicks(randomTicks);
        }

        /// <summary>
        /// 生成 [min, max) 范围内的随机 DateTimeOffset。
        /// </summary>
        public static DateTimeOffset NextDateTimeOffset(DateTimeOffset? min = null, DateTimeOffset? max = null)
        {
            DateTimeOffset minValue = min ?? DateTimeOffset.MinValue;
            DateTimeOffset maxValue = max ?? DateTimeOffset.MaxValue;
            long rangeTicks = (maxValue - minValue).Ticks;
            if (rangeTicks <= 0) return minValue;

            long randomTicks = NextLong(0, rangeTicks);
            return minValue.AddTicks(randomTicks);
        }

        /// <summary>
        /// 生成随机的 TimeSpan。
        /// </summary>
        public static TimeSpan NextTimeSpan(TimeSpan? min = null, TimeSpan? max = null)
        {
            TimeSpan minValue = min ?? TimeSpan.MinValue;
            TimeSpan maxValue = max ?? TimeSpan.MaxValue;
            long rangeTicks = (maxValue - minValue).Ticks;
            if (rangeTicks <= 0) return minValue;

            long randomTicks = NextLong(0, rangeTicks);
            return minValue.Add(TimeSpan.FromTicks(randomTicks));
        }

        #endregion

        #region 集合

        /// <summary>
        /// 从数组中随机选取一个元素。空集合返回 default(T)。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NextItem<T>(T[] array)
        {
            if (array == null || array.Length == 0) return default(T);
            return array[Shared.Next(array.Length)];
        }

        /// <summary>
        /// 从 IList 中随机选取一个元素。空列表返回 default(T)。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NextItem<T>(IList<T> list)
        {
            if (list == null || list.Count == 0) return default(T);
            return list[Shared.Next(list.Count)];
        }

        /// <summary>
        /// 从集合中随机选取一个元素。空集合返回 default(T)。
        /// 注意：对于非索引集合，此方法会枚举并缓存。
        /// </summary>
        public static T NextItem<T>(IEnumerable<T> source)
        {
            IList<T> list = source as IList<T>;
            if (list != null)
                return NextItem(list);
            T[] array = source as T[];
            if (array != null)
                return NextItem(array);

            // 对纯 IEnumerable 做一次枚举缓存
            List<T> snapshot = source == null ? null : source.ToList();
            if (snapshot == null || snapshot.Count == 0) return default(T);
            return snapshot[Shared.Next(snapshot.Count)];
        }

        /// <summary>
        /// 从集合中随机选取 count 个不重复的元素（采样）。
        /// 若 count ≥ 集合大小，则返回打乱顺序后的全集。
        /// </summary>
        public static List<T> NextSample<T>(IList<T> source, int count)
        {
            AssertKit.AssertNotNull(source, nameof(source));
            AssertKit.AssertPositive(count, nameof(count));
            if (count == 0) return new List<T>();

            int n = source.Count;
            if (count >= n)
            {
                List<T> result = new List<T>(source);
                ShuffleInPlace(result);
                return result;
            }

            // 使用 Fisher-Yates 部分洗牌：只洗前 count 个
            int[] indices = Enumerable.Range(0, n).ToArray();
            for (int i = 0; i < count; i++)
            {
                int j = Shared.Next(i, n);
                int tmp = indices[i];
                indices[i] = indices[j];
                indices[j] = tmp;
            }

            List<T> sample = new List<T>(count);
            for (int i = 0; i < count; i++)
                sample.Add(source[indices[i]]);
            return sample;
        }

        /// <summary>
        /// 对列表进行 Fisher-Yates 原地洗牌。
        /// </summary>
        public static void ShuffleInPlace<T>(IList<T> list)
        {
            AssertKit.AssertNotNull(list, nameof(list));
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Shared.Next(i + 1);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        /// <summary>
        /// 返回一个打乱顺序后的新集合（不修改原集合）。
        /// </summary>
        public static List<T> Shuffle<T>(IEnumerable<T> source)
        {
            List<T> list = source == null ? new List<T>() : source.ToList();
            ShuffleInPlace(list);
            return list;
        }

        /// <summary>
        /// 根据权重从候选项中随机选取一个元素。
        /// </summary>
        /// <param name="items">候选项。</param>
        /// <param name="weightSelector">权重提取函数。权重必须 ≥ 0。</param>
        /// <returns>被选中的元素。</returns>
        public static T NextWeightedItem<T>(IList<T> items, Func<T, double> weightSelector)
        {
            AssertKit.AssertNotEmpty(items, nameof(items), "items must not be null or empty");
            AssertKit.AssertNotNull(weightSelector, nameof(weightSelector));

            double totalWeight = 0.0;
            double[] cumulative = new double[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                double w = weightSelector(items[i]);
                if (w < 0) throw new ArgumentException(string.Format("Weight for item at index {0} is negative.", i));
                totalWeight += w;
                cumulative[i] = totalWeight;
            }

            if (totalWeight <= 0)
                throw new InvalidOperationException("Total weight must be greater than zero.");

            double roll = Shared.NextDouble() * totalWeight;
            int index = Array.BinarySearch(cumulative, roll);
            if (index < 0) index = ~index;
            return items[Math.Min(index, items.Count - 1)];
        }

        #endregion

        #region 图像

#if !NET8_0_OR_GREATER
        /// <summary>
        /// 生成随机颜色（含透明度）。
        /// </summary>
        public static Color NextColor(bool withAlpha = true)
        {
            byte a = withAlpha ? (byte)Shared.Next(256) : (byte)255;
            byte r = (byte)Shared.Next(256);
            byte g = (byte)Shared.Next(256);
            byte b = (byte)Shared.Next(256);
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// 生成随机纯色（不透明）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color NextSolidColor()
        {
            return Color.FromArgb(255, (byte)Shared.Next(256), (byte)Shared.Next(256), (byte)Shared.Next(256));
        }

        /// <summary>
        /// 生成随机验证码风格图片的字节数组。
        /// </summary>
        /// <param name="width">宽度（px）</param>
        /// <param name="height">高度（px）</param>
        /// <param name="format">输出图片格式</param>
        /// <param name="elementCount">干扰元素数量</param>
        /// <param name="backgroundColor">背景色（null 则为随机色）</param>
        public static byte[] NextImage(int width, int height, ImageFormat format,
            int elementCount = 10, Color? backgroundColor = null)
        {
            AssertKit.AssertGreater(width, 0, nameof(width));
            AssertKit.AssertGreater(height, 0, nameof(width));
            AssertKit.AssertGreater(elementCount, 0, nameof(elementCount));

            using (Bitmap image = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // 背景
                graphics.Clear(backgroundColor ?? NextColor(withAlpha: false));

                // 干扰元素工厂
                Action<Graphics>[] painters = new Action<Graphics>[]
                {
                    g =>
                    {
                        int n = Shared.Next(3, 10);
                        PointF[] pts = new PointF[n];
                        for (int i = 0; i < n; i++)
                            pts[i] = new PointF(NextFloat(1, width), NextFloat(1, height));
                        using (Pen pen = RandomPen())
                            g.DrawClosedCurve(pen, pts);
                    },
                    g =>
                    {
                        int n = Shared.Next(3, 10);
                        PointF[] pts = new PointF[n];
                        for (int i = 0; i < n; i++)
                            pts[i] = new PointF(NextFloat(1, width), NextFloat(1, height));
                        using (Pen pen = RandomPen())
                            g.DrawPolygon(pen, pts);
                    },
                    g =>
                    {
                        float startAngle = NextFloat(1, 359);
                        using (Pen pen = RandomPen())
                            g.DrawArc(pen,
                                NextFloat(1, width), NextFloat(1, height),
                                NextFloat(1, width / 2f + 1), NextFloat(1, height / 2f + 1),
                                startAngle, NextFloat(startAngle, 360));
                    },
                    g =>
                    {
                        using (Pen pen = RandomPen())
                            g.DrawLine(pen,
                                NextFloat(1, width), NextFloat(1, height),
                                NextFloat(1, width), NextFloat(1, height));
                    },
                    g =>
                    {
                        using (Pen pen = RandomPen())
                            g.DrawEllipse(pen,
                                NextFloat(1, width), NextFloat(1, height),
                                NextFloat(1, width / 2f), NextFloat(1, height / 2f));
                    },
                    g =>
                    {
                        using (Pen pen = RandomPen())
                            g.DrawRectangle(pen,
                                NextFloat(1, width), NextFloat(1, height),
                                NextFloat(1, width / 2f), NextFloat(1, height / 2f));
                    },
                };

                for (int i = 0; i < elementCount; i++)
                    painters[Shared.Next(painters.Length)](graphics);

                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, format);
                    return stream.ToArray();
                }
            }
        }

        private static Pen RandomPen()
        {
            return new Pen(NextColor(), NextFloat(1f, 5f));
        }
#endif
#endregion

        #region 模型

        /// <summary>
        /// 构造 T 类型的完全随机对象（使用反射策略）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NextModel<T>()
        {
            return (T)NextModel(typeof(T));
        }

        /// <summary>
        /// 构造指定 Type 的完全随机对象（使用反射策略）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object NextModel(Type type)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            RandomGenerateContext context = RandSeedProxy.Instance.CreateContext();
            return RandSeedProxy.Instance.GetPolicy(type).Generate(type, context);
        }

        /// <summary>
        /// 构造 T 类型的完全随机对象（使用自定义上下文）。
        /// </summary>
        public static T NextModel<T>(RandomGenerateContext context)
        {
            AssertKit.AssertNotNull(context, nameof(context));
            return (T)RandSeedProxy.Instance.GetPolicy(typeof(T)).Generate(typeof(T), context);
        }

        /// <summary>
        /// 构造指定 Type 的完全随机对象（使用自定义上下文）。
        /// </summary>
        public static object NextModel(Type type, RandomGenerateContext context)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            AssertKit.AssertNotNull(context, nameof(context));
            return RandSeedProxy.Instance.GetPolicy(type).Generate(type, context);
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 按概率返回 true。等价于 <c>NextBool(probability)</c>。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Roll(double probability)
        {
            return NextBool(probability);
        }

        /// <summary>
        /// 从多个候选项中等概率随机选取一个。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Pick<T>(params T[] items)
        {
            AssertKit.AssertNotEmpty(items, nameof(items));
            return items[Shared.Next(items.Length)];
        }

        /// <summary>
        /// 生成指定长度的随机字节数组。
        /// </summary>
        public static byte[] NextBytes(int length)
        {
            AssertKit.AssertPositive(length, nameof(length));
            byte[] bytes = new byte[length];
            Shared.NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// 用随机字节填充目标数组。
        /// </summary>
        public static void FillBytes(byte[] buffer)
        {
            AssertKit.AssertNotNull(buffer, nameof(buffer));
            Shared.NextBytes(buffer);
        }

        #endregion

        #region 概率分布

        /// <summary>
        /// 正态分布（Box-Muller 变换）随机数。
        /// </summary>
        /// <param name="mean">均值 μ</param>
        /// <param name="stdDev">标准差 σ</param>
        public static double NextGaussian(double mean = 0.0, double stdDev = 1.0)
        {
            // Box-Muller 变换
            double u1 = 1.0 - NextDouble(); // (0, 1]
            double u2 = 1.0 - NextDouble();
            double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * standardNormal;
        }

        /// <summary>
        /// 指数分布随机数。
        /// </summary>
        /// <param name="rate">率参数 λ（必须 &gt; 0）</param>
        public static double NextExponential(double rate = 1.0)
        {
            AssertKit.AssertPositive(rate, nameof(rate));
            return -Math.Log(1.0 - NextDouble()) / rate;
        }

        /// <summary>
        /// 泊松分布随机整数。
        /// </summary>
        /// <param name="lambda">均值 λ（必须 &gt; 0）</param>
        public static int NextPoisson(double lambda)
        {
            AssertKit.AssertPositive(lambda, nameof(lambda));

            // Knuth 算法
            double l = Math.Exp(-lambda);
            int k = 0;
            double p = 1.0;
            do
            {
                k++;
                p *= NextDouble();
            } while (p > l);

            return k - 1;
        }

        #endregion

        #region 加权与分层采样

        /// <summary>
        /// 从加权集合中选取 count 个不重复的元素。
        /// 使用 Efraimidis-Spirakis A-ES 算法，时间复杂度 O(n log n)。
        /// </summary>
        public static List<T> NextWeightedSample<T>(IList<T> items, Func<T, double> weightSelector, int count)
        {
            AssertKit.AssertNotEmpty(items, nameof(items));
            AssertKit.AssertNotNull(weightSelector, nameof(weightSelector));
            AssertKit.AssertPositive(count, nameof(count));
            if (count == 0)
                return new List<T>();

            int n = items.Count;
            if (count >= n)
            {
                List<T> all = new List<T>(items);
                ShuffleInPlace(all);
                return all;
            }

            // A-ES 算法：为每个元素分配 key = random^(1/weight)，取 key 最大的前 count 个
            List<KeyValuePair<int, double>> entries = new List<KeyValuePair<int, double>>(n);
            for (int i = 0; i < n; i++)
            {
                double w = weightSelector(items[i]);
                if (w < 0) throw new ArgumentException(string.Format("Weight for item at index {0} is negative.", i));
                if (w > 0)
                {
                    double key = Math.Pow(NextDouble(), 1.0 / w);
                    entries.Add(new KeyValuePair<int, double>(i, key));
                }
            }

            return entries
                .OrderByDescending(e => e.Value)
                .Take(count)
                .Select(e => items[e.Key])
                .ToList();
        }

        /// <summary>
        /// 分层采样：将 [min, max) 分成 strata 层，每层随机取一个值。
        /// 适用于蒙特卡洛方差缩减场景。
        /// </summary>
        public static double[] NextStratifiedSample(double min, double max, int strata)
        {
            AssertKit.AssertGreater(max, min, nameof(max), "min must be less than max");
            AssertKit.AssertGreater(strata, 0, nameof(strata));

            double[] result = new double[strata];
            double interval = (max - min) / strata;
            for (int i = 0; i < strata; i++)
            {
                double low = min + i * interval;
                result[i] = NextDouble(low, low + interval);
            }

            return result;
        }

        #endregion

        #region 国际化字符

        /// <summary>
        /// 生成随机中文字符（Unicode 基本汉字区 U+4E00-U+9FFF）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char NextChineseChar()
        {
            return (char)NextInt(0x4E00, 0x9FFF + 1);
        }

        /// <summary>
        /// 生成随机中文句子。
        /// </summary>
        /// <param name="length">字数</param>
        public static string NextChineseText(int length)
        {
            AssertKit.AssertPositive(length, nameof(length));
            if (length == 0) return string.Empty;

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = NextChineseChar();
            return new string(chars);
        }

        /// <summary>
        /// 生成随机日文平假名。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char NextHiraganaChar()
        {
            return (char)NextInt(0x3041, 0x3096 + 1);
        }

        /// <summary>
        /// 生成随机日文片假名。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char NextKatakanaChar()
        {
            return (char)NextInt(0x30A1, 0x30FA + 1);
        }

        /// <summary>
        /// 生成随机韩文字符（谚文音节 U+AC00-U+D7A3）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char NextKoreanChar()
        {
            return (char)NextInt(0xAC00, 0xD7A3 + 1);
        }

        /// <summary>
        /// 生成随机 Emoji（常用范围）。
        /// </summary>
        public static string NextEmoji()
        {
            string[] emojis = new[]
            {
                "\U0001F600", "\U0001F601", "\U0001F602", "\U0001F603",
                "\U0001F604", "\U0001F605", "\U0001F606", "\U0001F607",
                "\U0001F609", "\U0001F60A", "\U0001F60B", "\U0001F60D",
                "\U0001F60E", "\U0001F60F", "\U0001F610", "\U0001F611",
                "\U0001F634", "\U0001F635", "\U0001F637", "\U0001F638",
                "\U0001F914", "\U0001F917", "\U0001F92D", "\U0001F92F",
                "\u2764\uFE0F", "\u2705", "\u274C", "\u26A1",
                "\u2600\uFE0F", "\u2614", "\u2B50", "\uD83D\uDD25",
            };
            return NextItem(emojis);
        }

        #endregion

        #region 数据模拟

        /// <summary>
        /// 生成符合特定格式的随机手机号（中国大陆）。
        /// </summary>
        public static string NextPhoneNumberCN()
        {
            string[] prefixes = new[]
            {
                "130", "131", "132", "133", "134", "135", "136", "137", "138", "139",
                "150", "151", "152", "153", "155", "156", "157", "158", "159",
                "180", "181", "182", "183", "184", "185", "186", "187", "188", "189",
                "170", "171", "172", "173", "174", "175", "176", "177", "178", "179",
                "190", "191", "192", "193", "195", "196", "197", "198", "199"
            };
            string prefix = NextItem(prefixes);
            return prefix + NextString(8, ConstCharPools.Digits);
        }

        /// <summary>
        /// 生成随机邮箱地址。
        /// </summary>
        public static string NextEmail()
        {
            string[] domains = new[]
                { "gmail.com", "outlook.com", "qq.com", "163.com", "126.com", "sina.com", "foxmail.com" };
            int nameLength = NextInt(5, 13);
            string name = NextString(nameLength, ConstCharPools.LowerLetters + ConstCharPools.Digits);
            string domain = NextItem(domains);
            return string.Format("{0}@{1}", name, domain);
        }

        /// <summary>
        /// 生成随机 IPv4 地址。
        /// </summary>
        public static string NextIPv4()
        {
            return string.Format("{0}.{1}.{2}.{3}", NextInt(1, 256), NextInt(0, 256), NextInt(0, 256), NextInt(1, 255));
        }

        /// <summary>
        /// 生成随机 URL。
        /// </summary>
        public static string NextUrl()
        {
            string[] schemes = new[] { "https", "http" };
            string[] domains = new[] { "example.com", "test.org", "demo.io", "mysite.cn", "api.dev" };
            string[] paths = new[] { "api/v1", "users", "products", "orders", "images", "docs", "" };

            string scheme = NextItem(schemes);
            string domain = NextItem(domains);
            string path = NextItem(paths);
            string id = NextString(6, ConstCharPools.LowerLetters + ConstCharPools.Digits);

            return string.IsNullOrEmpty(path)
                ? string.Format("{0}://{1}/{2}", scheme, domain, id)
                : string.Format("{0}://{1}/{2}/{3}", scheme, domain, path, id);
        }

        /// <summary>
        /// 生成随机中文姓名。
        /// </summary>
        public static string NextChineseName()
        {
            string[] surnames = new[]
            {
                "王", "李", "张", "刘", "陈", "杨", "赵", "黄", "周", "吴",
                "徐", "孙", "胡", "朱", "高", "林", "何", "郭", "马", "罗",
                "梁", "宋", "郑", "谢", "韩", "唐", "冯", "于", "董", "萧",
                "程", "曹", "袁", "邓", "许", "傅", "沈", "曾", "彭", "吕",
            };
            string surname = NextItem(surnames);
            int nameLen = NextInt(1, 3); // 1-2 个名
            string givenName = NextChineseText(nameLen);
            return surname + givenName;
        }

        /// <summary>
        /// 生成随机身份证号（符合校验规则）。
        /// </summary>
        public static string NextChineseIdCard()
        {
            // 生成地区码（6位）
            string[] areaCodes = new[] { "110101", "310101", "440103", "330102", "320102", "510104" };
            string area = NextItem(areaCodes);

            // 生成出生日期（8位）
            DateTime birthDate = NextDateTime(
                new DateTime(1960, 1, 1),
                new DateTime(2010, 12, 31));
            string birth = birthDate.ToString("yyyyMMdd");

            // 顺序码（3位）
            string sequence = NextString(3, ConstCharPools.Digits);

            // 前17位
            string id17 = area + birth + sequence;

            // 计算校验位
            int[] weights = new[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            char[] checkCodes = new[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

            int sum = 0;
            for (int i = 0; i < 17; i++)
                sum += (id17[i] - '0') * weights[i];
            char checkCode = checkCodes[sum % 11];

            return id17 + checkCode;
        }

        /// <summary>
        /// 生成指定长度的 Lorem Ipsum 风格占位文本。
        /// </summary>
        public static string NextLoremIpsum(int wordCount)
        {
            string[] words = new[]
            {
                "lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
                "adipiscing", "elit", "sed", "do", "eiusmod", "tempor",
                "incididunt", "ut", "labore", "et", "dolore", "magna",
                "aliqua", "enim", "ad", "minim", "veniam", "quis",
                "nostrud", "exercitation", "ullamco", "laboris", "nisi",
                "aliquip", "ex", "ea", "commodo", "consequat", "duis",
                "aute", "irure", "reprehenderit", "voluptate", "velit",
                "esse", "cillum", "fugiat", "nulla", "pariatur",
            };

            if (wordCount <= 0) return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < wordCount; i++)
            {
                if (i > 0) sb.Append(' ');
                string word = NextItem(words);
                if (i == 0)
                    sb.Append(char.ToUpper(word[0])).Append(word.Substring(1));
                else
                    sb.Append(word);
            }

            sb.Append('.');
            return sb.ToString();
        }

        #endregion

        #region 序列生成

        /// <summary>
        /// 生成无限随机序列（惰性求值）。
        /// 使用 <c>.Take(n)</c> 取前 n 个元素。
        /// </summary>
        public static IEnumerable<T> NextSequence<T>(Func<T> generator)
        {
            AssertKit.AssertNotNull(generator, nameof(generator));
            while (true) yield return generator();
        }

        /// <summary>
        /// 生成 count 个随机整数。
        /// </summary>
        public static IEnumerable<int> NextInts(int count, int min = 0, int max = int.MaxValue)
        {
            for (int i = 0; i < count; i++)
                yield return NextInt(min, max);
        }

        /// <summary>
        /// 生成 count 个随机双精度浮点数。
        /// </summary>
        public static IEnumerable<double> NextDoubles(int count, double min = 0.0, double max = 1.0)
        {
            for (int i = 0; i < count; i++)
                yield return NextDouble(min, max);
        }

        /// <summary>
        /// 生成 count 个随机字符串。
        /// </summary>
        public static IEnumerable<string> NextStrings(int count, int length, string pool = ConstCharPools.AlphaNumeric)
        {
            for (int i = 0; i < count; i++)
                yield return NextString(length, pool);
        }

        #endregion

        #region 位运算与标记

        /// <summary>
        /// 生成随机的 [Flags] 枚举组合值。
        /// </summary>
        /// <param name="maxFlags">最多同时设置几个标记位。</param>
        public static T NextFlags<T>(int? maxFlags = null) where T : struct
        {
            Array values = Enum.GetValues(typeof(T));
            List<long> definedValues = new List<long>();
            foreach (T val in values)
            {
                long v = Convert.ToInt64(val);
                // 仅 2 的幂（单标志位）
                if (v != 0 && (v & (v - 1)) == 0)
                    definedValues.Add(v);
            }

            if (definedValues.Count == 0)
                return default(T);

            int flagCount = maxFlags.HasValue
                ? Math.Min(maxFlags.Value, definedValues.Count)
                : NextInt(1, definedValues.Count + 1);

            List<long> selected = NextSample(definedValues, flagCount);
            long combined = 0L;
            foreach (long f in selected)
                combined |= f;
            return (T)Enum.ToObject(typeof(T), combined);
        }

        #endregion

        #region 批量生成

        /// <summary>
        /// 批量生成 count 个 T 类型的随机模型。
        /// </summary>
        public static List<T> NextModels<T>(int count)
        {
            AssertKit.AssertPositive(count, nameof(count));
            if (count == 0) return new List<T>();

            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
                result.Add(NextModel<T>());
            return result;
        }

        /// <summary>
        /// 批量生成 count 个指定类型的随机模型。
        /// </summary>
        public static List<object> NextModels(Type type, int count)
        {
            AssertKit.AssertNotNull(type, nameof(type));
            AssertKit.AssertPositive(count, nameof(count));
            if (count == 0) return new List<object>();

            List<object> result = new List<object>(count);
            for (int i = 0; i < count; i++)
                result.Add(NextModel(type));
            return result;
        }

        #endregion

        #region 构建器模式

        /// <summary>
        /// 创建随机模型构建器，支持声明式属性覆盖和生成策略定制。
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <returns>构建器实例</returns>
        public static RandomModelBuilder<T> BuildModel<T>()
        {
            return new RandomModelBuilder<T>();
        }

        #endregion
    }
}