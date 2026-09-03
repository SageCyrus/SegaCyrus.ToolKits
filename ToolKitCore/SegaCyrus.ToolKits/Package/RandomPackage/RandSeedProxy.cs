using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SegaCyrus.ToolKits.Kit;
using SegaCyrus.ToolKits.Model.Random.Attributes;
using SegaCyrus.ToolKits.Model.Random.Model;

namespace SegaCyrus.ToolKits.Package.RandomPackage
{
    /// <summary>
    /// 随机种子策略工厂
    /// </summary>
    public class RandSeedProxy
    {
        #region 单例

        private static readonly Lazy<RandSeedProxy> _instance =
            new Lazy<RandSeedProxy>(() => new RandSeedProxy(), true);

        /// <summary>
        /// 实例
        /// </summary>
        public static RandSeedProxy Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region 分层字典

        /// <summary>
        /// 精确类型 → 策略（O(1) 命中）
        /// </summary>
        private readonly ConcurrentDictionary<Type, BaseRandSeed> _exact;

        /// <summary>
        /// 注册的继承锚点 → 策略。查找时沿 key.BaseType 链匹配
        /// </summary>
        private readonly Dictionary<Type, BaseRandSeed> _inheritance;

        /// <summary>
        /// 接口 Type → 策略
        /// </summary>
        private readonly Dictionary<Type, BaseRandSeed> _interfaces;

        /// <summary>
        /// 泛型定义 Type → 策略
        /// </summary>
        private readonly Dictionary<Type, BaseRandSeed> _genericDefs;

        /// <summary>
        /// 兜底策略（typeof(object)）
        /// </summary>
        private BaseRandSeed _fallback;

        /// <summary>
        /// 查找缓存：Type → 策略
        /// </summary>
        private readonly ConcurrentDictionary<Type, BaseRandSeed> _cache;

        private readonly object _lock = new object();

        #endregion

        #region 构造

        private RandSeedProxy()
        {
            _exact = new ConcurrentDictionary<Type, BaseRandSeed>();
            _inheritance = new Dictionary<Type, BaseRandSeed>();
            _interfaces = new Dictionary<Type, BaseRandSeed>();
            _genericDefs = new Dictionary<Type, BaseRandSeed>();
            _cache = new ConcurrentDictionary<Type, BaseRandSeed>();

            AutoDiscover();
        }

        #endregion

        #region 公共 API

        /// <summary>
        /// 根据 Type 获取对应的随机种子策略（O(1) 查找）。
        /// </summary>
        public BaseRandSeed GetPolicy(Type type)
        {
            AssertKit.AssertNotNull(type, nameof(type));

            return _cache.GetOrAdd(type, ResolvePolicy);
        }

        /// <summary>
        /// 为指定类型注册自定义策略。
        /// </summary>
        public void Register(Type typeDefine, BaseRandSeed seed)
        {
            AssertKit.AssertNotNull(typeDefine, nameof(typeDefine));
            AssertKit.AssertNotNull(seed, nameof(seed));

            lock (_lock)
            {
                RegisterInternal(typeDefine, seed);
                _cache.Clear();
            }
        }

        /// <summary>
        /// 创建生成上下文。
        /// </summary>
        public RandomGenerateContext CreateContext(RandomContext randomContext = null,
            RandomConfigAttribute[] configAttrs = null)
        {
            RandomGenerateContext context = new RandomGenerateContext(randomContext, this);
            context.SetConfigAttrs(configAttrs);
            return context;
        }

        #endregion

        #region 内部注册

        private void RegisterInternal(Type typeDefine, BaseRandSeed seed)
        {
            if (typeDefine == typeof(object))
            {
                _fallback = seed;
                return;
            }

            if (typeDefine.IsGenericTypeDefinition)
            {
                _genericDefs[typeDefine] = seed;
            }
            else if (typeDefine.IsInterface)
            {
                _interfaces[typeDefine] = seed;
            }
            else
            {
                _inheritance[typeDefine] = seed;
            }

            // 同时加入精确匹配（允许精确类型也命中）
            _exact[typeDefine] = seed;
        }

        #endregion

        #region 匹配引擎

        private BaseRandSeed ResolvePolicy(Type key)
        {
            // 1. 精确匹配
            BaseRandSeed result;
            if (_exact.TryGetValue(key, out result))
                return result;

            // 2. 继承链匹配（沿 BaseType 向上查找）
            Type baseType = key.BaseType;
            while (baseType != null)
            {
                if (_inheritance.TryGetValue(baseType, out result))
                    return result;
                baseType = baseType.BaseType;
            }

            // 3. 接口匹配
            Type[] interfaces = key.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (_interfaces.TryGetValue(interfaces[i], out result))
                    return result;
            }

            // 4. 泛型定义匹配
            if (key.IsGenericType)
            {
                Type genericDef = key.GetGenericTypeDefinition();
                if (_genericDefs.TryGetValue(genericDef, out result))
                    return result;
            }

            // 5. 兜底
            if (_fallback != null)
                return _fallback;

            throw new InvalidOperationException(
                string.Format("No random seed strategy found for type '{0}'.", key.FullName));
        }

        #endregion

        #region 自动发现

        private void AutoDiscover()
        {
            List<Model.Common.KVModel<Type, RandomSeedRegisterAttribute>> discovered =
                ReflectKit.GetMarkAttrAndInhertClassTypes<BaseRandSeed, RandomSeedRegisterAttribute>(true);

            // 按策略类分组，每个类只实例化一次
            Dictionary<Type, BaseRandSeed> instanceCache = new Dictionary<Type, BaseRandSeed>();

            foreach (Model.Common.KVModel<Type, RandomSeedRegisterAttribute> kvp in discovered)
            {
                Type seedType = kvp.Key;
                RandomSeedRegisterAttribute attr = kvp.Value;

                BaseRandSeed instance;
                if (!instanceCache.TryGetValue(seedType, out instance))
                {
                    instance = (BaseRandSeed)ReflectKit.CreateInstance(seedType);
                    instanceCache[seedType] = instance;
                }

                RegisterInternal(attr.TypeDefine, instance);
            }
        }

        #endregion
    }
}